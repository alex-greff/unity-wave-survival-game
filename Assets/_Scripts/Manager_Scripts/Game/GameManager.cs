/*Alex Greff
19/01/2016
GameManager
Handles the game statistics
*/
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {
    private static GameManager instance;

    public static GameManager Instance {
        get {
            return instance;
        }
    }

    private static float startTime = 0; //The time that the game was started at
    private static int enemiesKilled = 0;
    private static int timesRevived = 0;
    private static int wavesCompleted = 0;
    private static int wavesAmount = 0;
    private static int healthBought = 0;
    private static float score = 0;

    private Player player;

    //Getters and setter

    public static float TimePlayed {
        get {
            return Mathf.RoundToInt(Accessories.time - startTime);
        }
    }

    public static int EnemiesKilled {
        get {
            return enemiesKilled;
        }
    }

    public static int TimesRevived {
        get {
            return timesRevived;
        }
    }

    public static int WavesCompleted {
        get {
            return wavesCompleted;
        }
    }

    public static int HealthBought {
        get {
            return healthBought;
        }
    }

    public static int WavesAmount {
        get {
            return wavesAmount;
        }
        set {
            wavesAmount = value;
        }
    }

    public static float Score {
        get {
            //Round the score to an int and subtract a tenth of the match time from it
            return Mathf.RoundToInt(Mathf.Clamp(score - (TimePlayed/10), 0, Mathf.Infinity)); 
        }
    }

    void Awake() {
        FindPlayer();
    }

    void Start () {
        instance = this;

        //Initialize the values
        startTime = Accessories.time; //Set the start timestamp
        enemiesKilled = 0;
        timesRevived = 0;
        wavesCompleted = 0;
        wavesAmount = 0;
        healthBought = 0;
        score = 0;
    }

    private void FindPlayer () {
        player = GameObject.FindObjectOfType<Player>(); //Find player
    }

    public static void killedEnemy () {
        enemiesKilled++; //Increment enemeis killed
        score += 5 + waveAmountBonus(10); //Update score
    }

    public static void revived () {
        timesRevived++;
        score -= 10 + waveAmountBonus(50); //Update score
    }

    public static void healthPurchased (int amt) {
        amt = (int) Mathf.Clamp(amt, 0, Mathf.Infinity);

        healthBought += amt;

        score -= (1 * amt) + waveAmountBonus(2 * amt); //Update score
    }

    public static void completedWave () {
        wavesCompleted++;
        score += 10 + waveAmountBonus(50); //Update score
    }

    private static float waveAmountBonus (float max) { //Gives score bonus based on how far the player is
        return max * (wavesCompleted/wavesAmount);
    }
    
    public void Fire() { //Make player fire
        if (player == null) //If the player isn't found
            FindPlayer(); //Try to find the player
         
        if (player == null) //If it's still not found
            return; //Don't fire

        player.fire(); 
    }

    private Vector2 gravityDirection = Vector2.down;

    public void ToggleGravity() { //Toggles the player's gravity
        gravityDirection *= -1;
        player.changeGravity(gravityDirection);
    }

    public void Jump () { //Make player jump
        player.jump();
    }

    public void StartChildCoroutine(IEnumerator coroutineMethod) {
        StartCoroutine(coroutineMethod);
    }

    public void StopChildCoroutine (IEnumerator coroutineMethod) {
        StopCoroutine(coroutineMethod);
    }
}
