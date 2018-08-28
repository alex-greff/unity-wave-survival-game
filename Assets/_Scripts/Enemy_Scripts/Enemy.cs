/*Alex Greff
19/01/2016
Enemy
The enemy base class
*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Enemy : MonoBehaviour {

    protected SpriteRenderer[] srs;
    protected Rigidbody2D rb; //Rigidbody reference

    protected Transform target; //Target (usually player) reference

    public float initialHealth = 1; //Initial health

    protected bool isAlive = false;

    [ShowOnly] [SerializeField] //Show in inspector as read-only
    protected float health = 1; //Current health

    private float playerCheckRate = 2f; //The rate that changes in player state is set at

    //Firing timer
    private float fireTimeStamp = 0;
    private float fireRate = 2f;

    private float reward = 1f; //Reward amount for killing enemy (money)

    //Events
    public delegate void EnemySpawnDelegate();
    public static event EnemySpawnDelegate OnSpawn;

    public delegate void EnemyDeathDelegate();
    public static event EnemyDeathDelegate OnDeath;

    private Transform lastHitShooter; //The last object to shoot the enemy

    protected LayerMask layerMask = 1; //The ignored layers on the raycast

    public GameObject weaponPrefab; //The prefab of the weapon
    protected Weapon weapon; //The current weapon being fired

    //IEnumerator function references
    protected IEnumerator findClosestPlayer;
    protected IEnumerator idleLookRotation;
    protected IEnumerator idleMovement;

    //Idle state attributes
    protected bool idle = false;
    protected Quaternion idleRandomLookRot;
    protected float idleMovementTrust = 0;

    protected float baseTurnSpeed = 5;
    [SerializeField] protected float turnSpeed = 1;

    protected float baseSpeed = 5f;
    [SerializeField] protected float speed = 1f;

    [SerializeField] private AudioClip deathSound;
    [SerializeField] private AudioClip hitSound;

    //Getter and setters
    public virtual float TurnSpeed {
        get {
            return baseTurnSpeed * turnSpeed;
        }
        set {
            turnSpeed = value;
        }
    }

    public float Speed {
        get { return speed * baseTurnSpeed; }
        set { speed = value; }
    }

    public float Reward {
        get {
            return reward;
        }
        set {
            reward = value;
        }
    }

    public float Health {
        get;
    }

    public bool Alive {
        get {
            return isAlive;
        }
    }

    protected virtual void Awake () {
        rb = GetComponent<Rigidbody2D>(); //Get reference to Rigidbody
        srs = GetComponentsInChildren<SpriteRenderer>();
    }

    protected virtual void Start () {
        health = initialHealth; //Initialize health

        layerMask = ~((1 << 13) | (1 << 12) | (1 << 14)); //Invert the mask so the layers #12, 13, and 14 are ignored

        weapon = SimplePool.Spawn(weaponPrefab, transform.position, transform.rotation).GetComponent<Weapon>(); //Get the weapon to spawn
        weapon.Despawn(); //Make sure it's despawned after it's been referenced

        //Start the player finding loop
        findClosestPlayer = FindClosestPlayer();
        StartCoroutine(findClosestPlayer);
    }

    protected virtual void OnEnable () {
        //When the enemy is enabled
    }
    protected virtual void OnDisable () {
        if (findClosestPlayer != null) 
            StopCoroutine(findClosestPlayer); //Stop searching for the player
    }

    protected virtual void Update () {
        if (PauseManager.Instance)
            if (PauseManager.Instance.Paused) return;

        if (idle) { //If the enemy is idle
            transform.rotation = Quaternion.Lerp(transform.rotation, idleRandomLookRot, 0.5f * Time.deltaTime); //Rotate to the random direction
        }
    }

    protected virtual void FixedUpdate () {
        //Check if game is paused
        if (PauseManager.Instance)
            if (PauseManager.Instance.Paused) return;

        if (idle) { //If idle
            //Add a movement force that is determined by the idle loop
            rb.AddForce(transform.right * idleMovementTrust, ForceMode2D.Force); 
        }
    }


    //Spawn methods
    public void Spawn(int health, Vector3 position, Quaternion rotation) { //The basic spawn method - everything else calls it
        if (OnSpawn != null) 
            OnSpawn(); //Call the enemy spawn event

        this.health = health; //Set health
        isAlive = true;

        //Set position and rotation
        transform.position = position; 
        transform.rotation = rotation;

        gameObject.SetActive(true); //Set active
    }

    //Overload methods of spawn
    public void Spawn() {
        Spawn(1, Vector3.zero, Quaternion.identity);
    }
    public void Spawn(int health) {
        Spawn(health, Vector3.zero, Quaternion.identity);
    }
    public void Spawn(Vector3 position, Quaternion rotation) {
        Spawn(1, position, rotation);
    }
    public void Spawn(Transform[] spawnPoints) {
        Transform pos = getPos(spawnPoints); //Get a spawn point
        Spawn(1, pos.position, pos.rotation);
    }
    public void Spawn(int health, Transform[] spawnPoints) {
        Transform pos = getPos(spawnPoints); //Get a spawn point
        Spawn(health, pos.position, pos.rotation);
    }

    public virtual void SetColor (Color color) { //Sets the enemy's color
        foreach (SpriteRenderer sr in srs) 
            sr.color = color;
    }

    protected void CheckForTarget (float dist) { //Checks for target
        //If paused
        if (PauseManager.Instance)
            if (PauseManager.Instance.Paused) return; 

        if (target == null) return; //If target isn't set

        LookAtTarget(); //Rotate towards the target

        //Check if target is infront of enemy
        RaycastHit2D hit = Physics2D.Raycast(transform.position, transform.right, dist, layerMask); 

        //If the transform it hit doesn't exist for some reason
        if (hit.transform == null) return; //Don't go any further


        if (hit.transform.tag == "Player") { //If the transform it hit was the player
            Fire(); //Fire at it
        }
    }

    protected void FireAtTarget () { //Check for target with infinite range
        CheckForTarget(Mathf.Infinity);
    }

    protected void Fire() { //Fire weapon
        if (PauseManager.Instance)
            if (PauseManager.Instance.Paused) return; //Check if paused

        if (Accessories.time - fireTimeStamp > fireRate) { //If enemy can fire
            weapon = SimplePool.Spawn(weaponPrefab, transform.position, transform.rotation).GetComponent<Weapon>(); //Get the weapon to spawn
            weapon.Spawn(transform, transform.position, transform.rotation); //Spawn the weapon in

            fireTimeStamp = Accessories.time; //Reset the cooldown timer
        }
    }

    protected void LookAtTarget() { //Rotates to face the target
        if (PauseManager.Instance)
            if (PauseManager.Instance.Paused) return; //If paused
        if (target == null) return; //If target doesn't exist

        Vector3 vectorToTarget = target.position - transform.position; //Get vector to the target
        float angle = Mathf.Atan2(vectorToTarget.y, vectorToTarget.x) * Mathf.Rad2Deg; //Get the angle to turn
        Quaternion q = Quaternion.AngleAxis(angle, Vector3.forward); //Get the target rotation
        transform.rotation = Quaternion.Slerp(transform.rotation, q, Time.deltaTime * TurnSpeed); //Move the transform
    }
    
    protected IEnumerator FindClosestPlayer () { //Finds the closest player
        GameObject[] players = null; 

        while (true) { //Keep looking for player untill one is found
            players = GameObject.FindGameObjectsWithTag("Player"); //Find the players in the scene

            GameObject closestPlayer = null;
            float lastDistance = Mathf.Infinity;

            if (players != null) { //If there are players found
                foreach (GameObject p in players) { //Iterate through each player
                    float dist = Vector2.Distance(transform.position, p.transform.position); //Get distance to player

                    if (p.GetComponent<Player>().IsAlive) { //If the player is alive
                        if (dist < lastDistance) { //If the distance is less than the last distance
                            closestPlayer = p; //Set the current closest player to this one
                            lastDistance = dist; //Set the last distance to this current one
                        }
                    }
                }

            }

            if (closestPlayer != null) { //If there is a closest player
                target = closestPlayer.transform; //Set target to it
                SetIdle(false); //Stop the enemy from idling
            }
            else {
                target = null; //No target
                SetIdle(true); //Idle the enemy
            }

            yield return new WaitForSeconds (1f / playerCheckRate); //Wait until it should update again
        }
    }
    
    protected virtual void SetIdle (bool state) { //Sets the enemy to an idle behavior
        if (state == true) { //If setting to idle
            idle = true;

            if (idleLookRotation == null) { //If there is no idle movement running
                idleLookRotation = IdleRotation();
                StartCoroutine(idleLookRotation); //Start one
            }
        }
        else { //If not
            idle = false;
            
            if (idleLookRotation != null)
                StopCoroutine(idleLookRotation); //Stop the idle rotation

            idleLookRotation = null; //Remove instance of the idle movement eneumeration
        }
    }


    protected IEnumerator IdleRotation () {
        while (true) {
            int updateInterval = Random.Range(3, 10); //How long to wait before changing the substate of the idle enemy

            yield return new WaitForSeconds(updateInterval); //Wait for update interval

            //Get a random look rotation
            var randRot = transform.eulerAngles;
            randRot.z = Random.Range(20, 360);
            idleRandomLookRot = Quaternion.Euler(randRot);
        }
    }

    protected IEnumerator IdleMovement () {
        while (true) {
            int updateInterval = Random.Range(3, 10); //How long to wait before changing the substate of the idle enemy
            int movementDuration = Random.Range(2, 5); //How long the enemy will move forward for

            yield return new WaitForSeconds(updateInterval - movementDuration); //Wait

            idleMovementTrust = Random.Range(5, 30); //Move the enemy forward

            yield return new WaitForSeconds(movementDuration); //Wait

            idleMovementTrust = 0; //Stop moving forward
        }
    }

    private Transform getPos (Transform[] positions) { //Gets a random position
        int rnd = Random.Range(0, positions.Length + 1);

        return positions[rnd];
    }

    public void kill() { //Kills the enemy
        if (OnDeath != null) 
            OnDeath(); //Call the death event

        health = 0; //Set health to zero
        isAlive = false;

        if (lastHitShooter != null) {
            if (lastHitShooter.tag == "Player") { //If the player was the last one to hit them
                Player.Instance.increaseMoney(Reward); //Give the player the reward
                GameManager.killedEnemy(); //Add one to the kill count
            }
        }

        SoundManager.Instance.PlayAudio(deathSound);
           
        gameObject.SetActive(false); //Deactivate
    }

    public void heal(float amt) {
        amt = Mathf.Abs(amt); //Remove any negatives

        health += amt; //Add the amount to the player health
    }
    
    public void damage (float amt, Transform shooter) {
        lastHitShooter = shooter; //Set the last thing to hit it

        amt = Mathf.Abs(amt); //Remove any negatives

        health -= amt; //Remove the amount to the player health

        SoundManager.Instance.PlayAudio(hitSound);
        ScoreTextManager.Instance.SpawnText(transform.position, Color.red, "-" + amt); //Show minus health text effect

        checkForDeath(); //Check if the enemy died from that
    }

    public void damage(float amt) {
        damage(amt, null);
    }

    private void checkForDeath () { //Checks if player is dead
        if (health <= 0) {
            health = 0;
            kill(); //Call the death event
        }
    }
}
