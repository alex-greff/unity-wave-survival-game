/*Alex Greff
19/01/2016
Lazer
The lazer weapon type
*/
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof(BoxCollider2D))]
[RequireComponent (typeof(LineRenderer))]
[RequireComponent (typeof(FollowScript))]
public class Lazer : Weapon {
    private BoxCollider2D bc;
    private LineRenderer lr;
    private FollowScript fs;

    private const float HITBOX_SCALE_FACTOR = 0.9f; //The percentage amount the hitbox is shrunk by relative to the lazer

    //Timer variables
    private float weaponTimeStamp = 0;
    private float baseWeaponDuration = 0.1f;
    [SerializeField] private float weaponDuration = 1;

    public float Duration {
        get {
            return weaponDuration * baseWeaponDuration;
        }
        set {
            weaponDuration = value;
            animationCooldown = Duration;
        }
    }

    public override float AnimationCooldown {
        get {
            return Duration;
        }
    }

    //Variables that can be set by the user


    //Implement abstract properties
    /*public override float Range {
        get { return 5; }
    }

    public override float Cooldown {
        get { return 0.5f; }
    }

    public override float AnimationCooldown {
        get { return 0.1f; }
    }

    public override int SpawnAmount {
        get { return 1; }
    }

    public override float DamageAmount {
        get { return 2;}

    }*/

    protected override void Awake () {
        base.Awake();
        //Get references
        bc = GetComponent<BoxCollider2D>();
        lr = GetComponent<LineRenderer>();
        fs = GetComponent<FollowScript>();
    }

    void Start () {
        spawnAmount = 1;
        animationCooldown = 0.1f;

        Duration = weaponDuration;
    }

    void Update () {
        if (PauseManager.Instance) 
            if (PauseManager.Instance.Paused) return; //Check pause

        if (Accessories.time - weaponTimeStamp > Duration) { //If the weapon timer has run out
            Despawn(); //Despawn itself
        }
    }

    //Spawn methods
    public override void Spawn(Transform shooter) {
        Spawn(shooter, Vector3.zero, Quaternion.identity);
    }

    public override void Spawn (Transform shooter, Vector3 pos, Quaternion rot) {
        base.Spawn(shooter, pos, rot);

        lr.SetPosition(0, Vector3.zero); //Set the begining point
        lr.SetPosition(1, new Vector3((Mathf.Sign(shooter.localScale.x) * Range), 0, 0)); //Set the end point

        //Encompass the lazer with the box collider
        bc.size = new Vector2 (Range * HITBOX_SCALE_FACTOR, bc.size.y);
        bc.offset = new Vector2 ((Mathf.Sign(shooter.localScale.x) * Range * HITBOX_SCALE_FACTOR)/2, 0);

        fs.target = shooter; //Set the target that it stays relative to

        weaponTimeStamp = Accessories.time; //Start the despawn timer
    }

    public override void Despawn() {
        base.Despawn();

        SimplePool.Despawn(gameObject); //Despawn itself from the pool manager
        gameObject.SetActive(false);
    }

    public override void SetOffset() { //Removes the offset
        base.SetOffset();
        fs.offset = Vector2.zero;
    }
    public override void SetOffset(Vector2 offset) { //Add the given offset to the follow script
        base.SetOffset(offset);
        fs.offset = offset;
    }

    //Properties

    public LineRenderer Lr { //Returns the line renderer
        get {
            return lr;
        }
    }
}
