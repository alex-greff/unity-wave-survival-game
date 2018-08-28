/*Alex Greff
19/01/2016
Player
The main player class
*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayerComponent;

//This class deals with all the user related things by referencing the other player classes and handling their public methods
//It acts as a kind of proxy for the important methods
public class Player : MonoBehaviour {
    private static Player instance = null;

    public static Player Instance {
        get {
            return instance;
        }
    }

    //References to components
    private Player p;
    private Rigidbody2D rb;
    private ConstantForce2D cf;
    private SpriteRenderer sr;
    private CapsuleCollider2D cc;
    private Animator anim;

    private Player_Health ph;
    private Player_Fire pf;
    private Player_Movement pm;
    private Player_Jump pj;
    private Player_Currency pc;

    private CapsuleCollider2D deadCollider;

    [SerializeField] private AudioClip deathSound;

    //Events
    public delegate void DeathEvent();
    public static event DeathEvent OnDeath;

    public delegate void RespawnEvent();
    public static event RespawnEvent OnRespawn;

    public delegate void HealthChangeEvent();
    public static event HealthChangeEvent OnHealthChange;

    public delegate void MoneyChangeEvent();
    public static event MoneyChangeEvent OnMoneyChange;

    private bool isAlive = true;

    private bool isGrounded = true;
    private bool hasJump = true;
    private bool hasDoubleJump = true;
    private bool canFire = true;
    private bool doubleSwipe = false;

    private bool isPaused = false;

    private bool onEmptyScreen = true;

    private Touch movementTouch;

    private Vector2 oldRbVelocity = Vector2.zero;

    void Awake () {
        //Get references
        p = GetComponent<Player>();
        rb = GetComponent<Rigidbody2D>();
        cf = GetComponent<ConstantForce2D>();
        sr = GetComponent<SpriteRenderer>();
        cc = GetComponent<CapsuleCollider2D>();
        anim = GetComponent<Animator>();

        ph = GetComponent<Player_Health>();
        pf = GetComponent<Player_Fire>();
        pm = GetComponent<Player_Movement>();
        pj = GetComponent<Player_Jump>();
        pc = GetComponent<Player_Currency>();

        instance = this; //Set instance to this player

        //Get the collider for the player's dead state
        deadCollider = transform.Find("Dead Collider").GetComponent<CapsuleCollider2D>();
        deadCollider.enabled = false; //Set inactive
    }

    void OnEnable () {
        //Attach to events
        OnDeath += Player_OnDeath;
        OnRespawn += Player_OnRespawn;
        OnHealthChange += Player_OnHealthChange;
        OnMoneyChange += Player_OnMoneyChange;

        PauseManager.OnPause += PauseManager_OnPause;
        PauseManager.OnUnpause += PauseManager_OnUnpause;
    }

    //Event methods
    private void PauseManager_OnPause() {
        isPaused = true;

        oldRbVelocity = rb.velocity; //Save the velocity

        rb.isKinematic = true;
        cf.enabled = false;
        rb.velocity = Vector2.zero; //Stop the velocity of the rigidbody
    }

    private void PauseManager_OnUnpause() {
        isPaused = false;

        rb.isKinematic = false;
        cf.enabled = true;
        rb.velocity = oldRbVelocity; //Reapply the velocity of what the player was going at before the pause
    }

    private void Player_OnMoneyChange() {
        //When the money changes
    }

    private void Player_OnHealthChange() {
        //When the player's health changes
    }

    private void Player_OnRespawn() { //When the player respawns
        anim.SetBool("dead", false);
        //TODO: respawn at a spawn point

        //Give the player his/her jumps back
        hasJump = true;
        hasDoubleJump = true;

        isAlive = true;

        cc.enabled = true; //Disable capsule collider
        deadCollider.enabled = false;

        ResetPosition();

        GameManager.revived(); //Increase one to the amount of times the player has been revied
    } 

    public void ResetPosition () {
        if (isAlive == false) return;
        //Reset position
        transform.position = Vector3.zero;
        transform.rotation = Quaternion.identity;

        rb.velocity = Vector2.zero; //Reset velocity
    }

    private void Player_OnDeath() {
        anim.SetBool("dead", true);

        isAlive = false;

        cc.enabled = false;
        deadCollider.enabled = true;

        SoundManager.Instance.PlayAudio(deathSound);
    }

    void OnDisable () {
        //Un-attach events
        OnDeath -= Player_OnDeath;
        OnRespawn -= Player_OnRespawn;
        OnHealthChange -= Player_OnHealthChange;
        OnMoneyChange -= Player_OnMoneyChange;

        PauseManager.OnPause -= PauseManager_OnPause;
        PauseManager.OnUnpause -= PauseManager_OnUnpause;
    }

    public void heal (float amt) { //Heal player
        ph.heal(amt);
        if (OnHealthChange != null)
            OnHealthChange(); //Call health change event
    }

    public void damage (float amt) { //Damage player
        ph.damage(amt);
        if (OnHealthChange != null)
            OnHealthChange(); //Call health change event
    }

    public void fire () { //Fire bullet
        pf.Fire();
    }

    public void changeGravity (Vector2 dir) { //Change the gravity
        pm.changeGravity(dir);
    }

    public void jump () { //Make player jump
        pj.Jump();
    }

    public void changeWeapon (GameObject prefab) { //Change the weapon the player is using
        pf.UpdateWeapon(prefab);
    }

    public void kill () { //Kill the player
        if (OnDeath != null) 
            OnDeath(); //Call the death event
    }

    public void respawn () { //Respawn the player
        if (OnRespawn != null)
            OnRespawn();
    }

    public void increaseMoney (float amount) { //Increase the player's money
        pc.increase(amount);

        if (OnMoneyChange != null) 
            OnMoneyChange();
    }

    public void decreaseMoney (float amount) { //Decrease the player's money
        pc.decrease(amount);

        if (OnMoneyChange != null)
            OnMoneyChange();
    }

    //Properties (getters and setters)
    public float Money { 
        get {
            return pc.Money;
        }
    }
    
    public bool IsAlive {
        get {
            return isAlive;
        }
    }

    public Touch MovementTouch {
        get {
            return movementTouch;
        }
        set {
            movementTouch = value;
        }
    }

    public bool IsGrounded {
        get {
            return isGrounded;
        }
        set {
            isGrounded = value;
        }
    }

    public bool HasJump {
        get {
            return hasJump;
        }
        set {
            hasJump = value;
        }
    }

    public bool HasDoubleJump {
        get {
            return hasDoubleJump;
        }
        set {
            hasDoubleJump = value;
        }
    }

    public bool CanFire {
        get {
            return canFire;
        }
        set {
            canFire = value;
        }
    }

    public bool DoubleSwipe {
        get {
            return doubleSwipe;
        }
        set {
            doubleSwipe = value;
        }
    }

    public bool OnEmptyScreen {
        get {
            return onEmptyScreen;
        }
        set {
            onEmptyScreen = value;
        }
    }

    public float Health {
        get {
            return ph.Health;
        }
    }

    public float InitialHealth {
        get {
            return ph.TopHealth;
        }
    }

    public bool IsPaused {
        get {
            return isPaused;
        }
    }

    public float MaxHealth {
        get {
            return ph.MaxHealth;
        }
    }
}
