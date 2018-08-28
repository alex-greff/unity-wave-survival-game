/*Alex Greff
19/01/2016
Pellet
The pellet weapon type
*/
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof(CircleCollider2D))]
public class Pellet : Weapon {
    private CircleCollider2D cc;    

    private Vector2 startPos = Vector2.zero;

    private Vector3 direction = Vector3.zero;
    private int initPlayerScaleX = 1;

    protected float baseSpeed = 20f;
    [SerializeField] protected float speed = 1;

    [SerializeField] private bool destroyOnHit = true;

    public float Speed {
        get {
            return baseSpeed * speed;
        }
        set {
            speed = value;
        }
    }

    //Implement abstract properties
    /*public override float Range {
        get { return 30; }
    }

    public override float AnimationCooldown {
        get { return 0.1f; }
    }

    public override float Cooldown {
        get { return 0.2f; }
    }

    public override float DamageAmount {
        get { return 1; }
    }

    public override int SpawnAmount {
        get { return 5; }
    }*/


    protected override void Awake () {
        base.Awake();

        cc = GetComponent<CircleCollider2D>();
    }

    void Start () {
        animationCooldown = 0.1f;
        spawnAmount = 5;
        baseSpeed = 20;
    }
	
	void Update () {
        if (PauseManager.Instance)
            if (PauseManager.Instance.Paused) return; //Check pause

        if (alive) { //If the pellet is alive
            transform.Translate(initPlayerScaleX * direction * Speed * Time.deltaTime); //Move forward
        }

		if (Mathf.Abs(Vector2.Distance(startPos, transform.position)) >= Range) { //If it goes beyond its range
            Despawn(); //Despawn it
        }
	}

    public override void Despawn() {
        base.Despawn();

        alive = false; //Set alive flag
        SimplePool.Despawn(gameObject); //Despawn itself from the pool manager
        gameObject.SetActive(false); //Set inactive 
    }

    //Spawn methods
    public override void Spawn(Transform shooter) {
        Spawn(shooter, Vector3.zero, Quaternion.identity);
    }

    public override void Spawn(Transform shooter, Vector3 pos, Quaternion rot) {
        base.Spawn(shooter, pos, Quaternion.identity);
        
        //Initialize variables
        startPos = transform.position; 

        initPlayerScaleX = (int) Mathf.Sign(shooter.localScale.x);
        direction = shooter.transform.right;

        alive = true; //Set alive flag
    }

    protected override void hitObject(Collider2D other) { //When it hit an object
        if (!isHitable(other)) return; //If it can hit

        base.hitObject(other);

        if (destroyOnHit || other.tag != "Enemy")
            Despawn(); //Despawn
    }
}
