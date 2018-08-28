/*Alex Greff
19/01/2016
Bounds Collider
The boundary manager for when the player falls out of the map
*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoundsCollider : MonoBehaviour {
    void OnTriggerExit2D(Collider2D other) {
        if (other.GetComponent<Player>()) { //If it's a player
            other.GetComponent<Player>().ResetPosition(); //Reset the player's position
        }
    }
}
