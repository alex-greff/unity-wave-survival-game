/*Alex Greff
19/01/2016
Floating Block
The block that the player can move
*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof(Rigidbody2D))]
[RequireComponent (typeof(SpriteRenderer))]
public class FloatingBlock : MonoBehaviour {
    private Rigidbody2D rb;
    private SpriteRenderer sr;

    [SerializeField] private Color activeColor;

    [SerializeField] private float resetDelayTime = 2f; //The amount of time before it resets itself
    [SerializeField] private float speed = 2000f;
    [SerializeField] private LayerMask collidableObject = 0;

    private List<GameObject> touchingGameobjects = new List<GameObject>();

    private Vector3 originalPos;
    private Color originalColor;

    private bool playerTouching = false;

    void Awake () {
        //Get references
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
    }
	
	void Start () {
        //Initialize values
        originalPos = transform.position;
        originalColor = sr.color;
	}
	
	void Update () {
        if (playerTouching == false) { //If the player isn't touching it
            if (Vector3.Distance(transform.position, originalPos) < 0.1f)
                return; 

            Vector3 dir = (originalPos - transform.position).normalized;
            dir *= speed * Time.deltaTime;

            rb.AddForce(dir, ForceMode2D.Impulse);
        }
	}

    void OnTriggerEnter2D(Collider2D col) {
        if (collidableObject == (collidableObject | (1 << col.gameObject.layer)))  { //If the colider's tag is in the layer list
            playerTouching = true; 
            sr.color = activeColor; //Change the color to the active color
        }
    }

    void OnTriggerExit2D(Collider2D col) {
        if (collidableObject == (collidableObject | (1 << col.gameObject.layer))) { //If the colider's tag is in the layer list
            playerTouching = false;
            sr.color = originalColor; //Reset color to original color
        }
    }
}
