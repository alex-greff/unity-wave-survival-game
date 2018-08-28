/*Alex Greff
19/01/2016
Player_Currency
Handles the player's money
*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace PlayerComponent {
    public class Player_Currency : MonoBehaviour {
        //Show the money in the inspector but only as readonly 
        [SerializeField] [ShowOnly] private float money = 0;

	    void Start () {
            money = 0; //Initialize money
	    }

        //Getters and setters
        public float Money {
            get {
                return money;
            }
        }
	
	    public void increase (float amount) {
            money += Mathf.Abs(amount);
        }

        public void decrease (float amount) {
            money -= Mathf.Abs(amount);

            if (money < 0)
                money = 0;
        }
    }
}
