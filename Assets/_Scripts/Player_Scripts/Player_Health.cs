using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PlayerComponent {
    public class Player_Health : MonoBehaviour {
        private Player p;
        
        [SerializeField] //Show in inspector even though it's a private variable (keeps encapsulation intact)
        private float initialHealth = 10;

        private const float MAX_HEALTH = 50;

        private float topHealth = 10; //The highest the health has been

        [ShowOnly] [SerializeField] //Show in inspector as read-only
        private float health = 0;


        [SerializeField] private AudioClip hitSound;

        void OnEnable () {
            //Attach events
            Player.OnRespawn += Player_OnRespawn;
            Player.OnDeath += Player_OnDeath;
        }

        private void Player_OnDeath() {
            health = 0;
        }

        private void Player_OnRespawn() {
            health = initialHealth;
            topHealth = health;
        }

        void OnDisable () {
            //De-attach events
            Player.OnRespawn -= Player_OnRespawn;
            Player.OnDeath -= Player_OnDeath;
        }

        void Awake () {
            p = GetComponent<Player>();
        }

	    void Start () {
            health = initialHealth;
            topHealth = health;
	    }
    
        public void damage (float amt) {

            amt = Mathf.Abs(amt); //Remove any negatives

            health -= amt; //Remove the amount to the player health

            SoundManager.Instance.PlayAudio(hitSound);
            ScoreTextManager.Instance.SpawnText(transform.position, Color.red, "-" + amt);

            checkForDeath();
        }

        public void heal (float amt) {

            amt = Mathf.Abs(amt); //Remove any negatives

            health += amt; //Add the amount to the player health

            if (health > topHealth)
                topHealth = health;

            if (health > MAX_HEALTH) //If the health goes over the max health
                health = MAX_HEALTH; //Keep it at the max health
        }

        private void checkForDeath () { //Checks if player is dead
            if (health <= 0) {
                //Call the death event
                health = 0;
                kill();
            }
        }

        public void kill() {
            p.kill(); //Kill the player  
        }

        //Getters and setters
        public float Health {
            get {
                return health;
            }
        }

        public float TopHealth {
            get {
                return topHealth;
            }
        }

        public float MaxHealth {
            get {
                return MAX_HEALTH;
            }
        }
    }
}
