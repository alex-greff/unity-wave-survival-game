/*Alex Greff
19/01/2016
Weapon
The abstract class handling the fundementals of the weapons
*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Weapon : MonoBehaviour {
    protected Transform shooter = null;
    protected Vector2 offset = Vector2.zero;

    protected ParticleSystem ps;
    private GameObject particleGo;

    [SerializeField] private AudioClip fireSound;

    [SerializeField] protected GameObject particlePrefab;

    protected bool alive = false;

    //Properties
    public Transform Shooter { //The shooter of the weapon
        get {
            return shooter;
        }
    }

    public AudioClip FireSound {
        get {
            return fireSound;
        }
        set {
            fireSound = value;
        }
    }

    [SerializeField] protected float range = 5;
    [SerializeField] protected float cooldown = 0.5f;
    protected float animationCooldown = 0.1f;
    [SerializeField] protected float damageAmount = 2;
    protected int spawnAmount = 1;
    

    public virtual float Range {
        get {
            return range;
        }
    }

    public virtual int SpawnAmount { //The amount of this weapon type to be spawned in a pool
        get {
            return spawnAmount;
        }
    }

    public virtual float DamageAmount { //The amount of damage the weapon deals
        get {
            return damageAmount;
        }
    }

    public virtual float Cooldown {
        get {
            return cooldown;
        }
    }

    public virtual float AnimationCooldown {
        get {
            return animationCooldown;
        }
    }

    

    protected virtual void Awake () {
        //ps = GetComponentInChildren<ParticleSystem>();
        if (particlePrefab)
            SimplePool.Preload(particlePrefab, 5);
    }
    
    //Spawn methods
    public virtual void Spawn(Transform shooter) {
        Spawn(shooter, Vector3.zero, Quaternion.identity);
    }
    public virtual void Spawn (Transform shooter, Vector3 pos, Quaternion rot) {
        transform.position = new Vector3(pos.x + offset.x, pos.y + offset.y, 0);
        transform.rotation = rot;

        alive = true; //Set alive flag

        this.shooter = shooter; //Set the shooter

        gameObject.SetActive(true); //Set to active

        if (particlePrefab) {

            particleGo = SimplePool.Spawn(particlePrefab, pos, rot);

            ps = particleGo.GetComponent<ParticleSystem>(); ;
            particleGo.GetComponent<FollowScript>().target = transform;
            

            if (ps.isStopped)
                ps.Play();

            ps.enableEmission = true;
            //ps.Play();
        }

        if (fireSound != null) {
            SoundManager.Instance.PlayAudio(fireSound);
        }
    }

    public virtual void Despawn() {
        if (particlePrefab) {
            if (particleGo == null) return;

            alive = false; //Set alive flag

            ps.enableEmission = false;
            //ps.Stop();

            particleGo.GetComponent<FollowScript>().target = null;

            StartCoroutine(delayedParticleDespawn(particleGo));
        }
    }

    private IEnumerator delayedParticleDespawn (GameObject go) {

        yield return new WaitForSeconds(2f);

        SimplePool.Despawn(go);
    }

    public virtual void SetOffset() {
        offset = Vector3.zero;
    }

    public virtual void SetOffset(Vector2 offset) {
        this.offset = offset;
    }

    protected void OnTriggerEnter2D(Collider2D other) {
        hitObject(other);
    }

    protected virtual void hitObject (Collider2D other) {
        if (isHitable(other) == false) return; //If it can hit the object

        GameObject hit = other.gameObject;

        if (hit.GetComponent<Player>()) { //If it hits a player
            Player player = hit.GetComponent<Player>();
            player.damage(DamageAmount); //Damage the player
        }

        if (hit.GetComponent<Enemy>()) { //If its an enemy
            Enemy enemy = hit.GetComponent<Enemy>();

            enemy.damage(DamageAmount, shooter); //Damage the enemy
        }
    }

    protected bool isHitable (Collider2D other) {
        if (other.gameObject.layer == 10) //Weapon layer
            return false;

        if (shooter)
            if (shooter.gameObject.layer == 12 && other.gameObject.layer == 12) //Enemy layer
                return false; //Enemies can't friendly fire

        if (other.transform == Shooter) return false; //Don't do anything if it collides with the shooter

        if (other.transform.name == "Trigger Area") return false; //Ignore the trigger area around the player

        return true;
    }
}
