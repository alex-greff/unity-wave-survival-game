//Update the A* mesh to include this enemy as an obstacle
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

//Component requirements
[RequireComponent(typeof (Rigidbody2D))]
[RequireComponent(typeof (Seeker))]
public class Fighter : Enemy {
    private Seeker seeker; //The seeker script reference

    private Path path;
    
    private float updateRate = 2f;
    private ForceMode2D fMode = ForceMode2D.Force;
    [HideInInspector]
    private bool pathIsEnded = false;
    private float nextWaypointDistance = 3;
    private int currentWaypoint = 0;
    private IEnumerator updatePath;

    protected override void Awake() {
        base.Awake();

        seeker = GetComponent<Seeker>(); //Get reference to seeker script
    }

    protected override void Start () {
        base.Start();

        baseSpeed = 5;
        baseTurnSpeed = 4;
    }	

    protected override void OnEnable () {
        base.OnEnable();

        updatePath = UpdatePath();
        StartCoroutine(updatePath); //Start the update path loop
    }
    protected override void OnDisable () {
        base.OnDisable();

        StopCoroutine(updatePath); //Stop the update path loop
    }
    
    protected override void Update () {
        base.Update();

        FireAtTarget(); //Fires whenever the player comes in range
    }
    

    protected override void FixedUpdate () {
        if (PauseManager.Instance)
            if (PauseManager.Instance.Paused) return;

        base.FixedUpdate();

        FollowTarget(); 
    }


    //EVIDENCE: RECURSION (of a kind)
    private IEnumerator UpdatePath () {
        yield return new WaitUntil(() => target != null); //Wait until the target exists
        yield return new WaitUntil(() => PauseManager.Instance.Paused == false); //Wait until the game isn't paused

        if (target != null) { //If there is a target
            seeker.StartPath(transform.position, target.position, OnPathComplete); //Start a path to the target
        }

        yield return new WaitForSeconds (1f / updateRate); //Wait for update rate time

        StartCoroutine(UpdatePath()); //Recall the function
    }

    private void OnPathComplete (Path p) {
        if (!p.error) { //If no errors were thrown
            path = p; //Set path
            currentWaypoint = 0; //Reset current waypoint
        }
    }

    private void FollowTarget () { //Follows the player
        if (PauseManager.Instance)
            if (PauseManager.Instance.Paused) return; //Check for pause

        if (target == null) //Check if target exists
           return;

        if (path == null) //Check if path exists
            return;

        if (currentWaypoint >= path.vectorPath.Count) { //If there are more waypoints to traverse
            if (pathIsEnded) //If path has ended
                return;

            pathIsEnded = true;
            return;
        }
        pathIsEnded = false;

        //Direction to the next waypoint
        Vector3 dir = (path.vectorPath[currentWaypoint] - transform.position).normalized;
        dir *= Speed * 10000 * Time.fixedDeltaTime; //Use fixed b/c it's in fixedUpdate

        //Move the AI
        rb.AddForce(dir, fMode);

        //Calculate if enemy has reached the next waypoint
        float dist = Vector3.Distance(transform.position, path.vectorPath[currentWaypoint]);
        if (dist < nextWaypointDistance) {
            currentWaypoint++;
            return;
        }
    }

    protected override void SetIdle(bool state) { //Sets idle state
        base.SetIdle(state);

        if (state == true) {
            if (idleMovement == null) { //If there is no idle movement running
                idleMovement = IdleMovement();
                StartCoroutine(idleMovement); //Start one
            }
        }
        else {
            if (idleMovement != null)
                StopCoroutine(idleMovement); //Stop idle movement coroutine
        }
    }
}
