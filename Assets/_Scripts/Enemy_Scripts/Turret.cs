/*Alex Greff
19/01/2016
Turret
The turret enemy
*/
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class Turret : Enemy
{
    private SpriteRenderer gunPart;


    protected override void OnEnable()
    {
        base.OnEnable();

        UpdateAStarPath();
    }

    protected override void OnDisable()
    {
        base.OnDisable();

        UpdateAStarPath();
    }

    private void UpdateAStarPath()
    { //Updates the A* graph of the moveable areas for the AI around the turrets
        var guo = new GraphUpdateObject(GetComponent<Collider2D>().bounds);
        guo.updatePhysics = true;
        try
        {
            AstarPath.active.UpdateGraphs(guo);
        }
        catch (Exception e) { }
    }

    protected override void Awake()
    {
        base.Awake();
    }

    protected override void Start()
    {
        base.Start();

        baseSpeed = 0;
        baseTurnSpeed = 3;
    }

    protected override void Update()
    {
        if (PauseManager.Instance)
            if (PauseManager.Instance.Paused) return; //Check for pause

        base.Update();

        CheckForTarget(weapon.Range * 1.2f); //Will fire at the player when he/she comes in range of about 1.2 times the distance of its projectile
    }
}
