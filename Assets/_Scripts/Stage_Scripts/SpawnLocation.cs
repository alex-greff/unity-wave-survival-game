/*Alex Greff
19/01/2016
SpawnLocation
Handles the random spawn locations
*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnLocation : MonoBehaviour {
    private Transform location;
    [SerializeField] [ShowOnly] private bool available = true;

    [SerializeField] private EnemyType enemyType;

    private float resetDelay = 2f; //How long untill the spawn location resets to being available

    void Start () {
        location = transform;
        available = true;

        if (enemyType == EnemyType.TURRET) { 
            resetDelay = Mathf.Infinity; //No reset delay if it's a turret
        }
    }

    public Vector3 Use () {
        if (!available) return Vector3.zero; //If it's not available just return the origin point

        available = false; //Set the spawn point to unavailable
         
        StartCoroutine(resetTimer()); //Start reset timer

        return location.position;
    }

    public Vector3 Position { 
        get {
            if (!available) return Vector3.zero;

            return location.position;
        }
    }

    private IEnumerator resetTimer () {
        if (resetDelay == Mathf.Infinity) yield return null; //If it's never going to reset then just stop the coroutine

        yield return new WaitForSeconds(resetDelay); //Wait
        available = true; //Set available to true
    }

    //Getter and setters
    public bool Availabe {
        get {
            return available;
        }
        set {
            available = value;
        }
    }

    public EnemyType EnemyType {
        get {
            return enemyType;
        }
    }
}
