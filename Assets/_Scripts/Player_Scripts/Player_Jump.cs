using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PlayerComponent {
    public class Player_Jump : MonoBehaviour {
        private Player p;
        private Rigidbody2D rb;
        private ConstantForce2D cf;

        [SerializeField] private AudioClip jumpSound;

        [SerializeField]
        private LayerMask mask = -1;

        [SerializeField]
        private float jumpPower = 130;
        
        private bool jumping;

        //Jump timers
        private float jumpTimeStamp = 0;
        private float jumpRegainDelay = 0.2f;

        //Variables for calculating swipes
        private Vector2 initialPressPos = new Vector2(); //The beginning pos of the swipe
        private Vector2 endPressPos = new Vector2(); //The end pos of the swipe
        private Vector2 swipeVector = new Vector2(); //The pixel distance the touch as moved
        private Vector2 distanceVector = new Vector2(); //The percent of the screen that the touch has moved
        private Vector2 swipeDirection = new Vector2(); //The determined direction of the swipe
        private float beginingTime = 0;

        private float swipeThreshold = 2; //The screen percent that has to be travelled in order to register the swipe

        private bool firstJumpCall = false;

        void Awake () {
            //Get references
            p = GetComponent<Player>();
            rb = GetComponent<Rigidbody2D>();
            cf = GetComponent<ConstantForce2D>();
        }
	    
        void FixedUpdate() { //Physics tick updates
            if (jumping) {
                if (cf.force.y != 0)
                    rb.velocity = new Vector2(rb.velocity.x, 0); //Stop the y velocity of the player

                if (cf.force.x != 0)
                    rb.velocity = new Vector2(0, rb.velocity.y); //Stop the x velocity of the player

            
                rb.AddForce(transform.up * jumpPower, ForceMode2D.Impulse);
                jumping = false;
            }

            //Hit checkers (3 for more accuracy)
            bool hit1 = Physics2D.Raycast(transform.position, -transform.up, 0.9f, mask);
            bool hit2 = Physics2D.Raycast(transform.position + new Vector3(0.2f,0,0), -transform.up, 0.9f, mask);
            bool hit3 = Physics2D.Raycast(transform.position + new Vector3(-0.2f,0,0), -transform.up, 0.9f, mask);

            if(!hit1 && !hit2 && !hit3){ //If none of the raycasts are hitting something
                p.IsGrounded = false; //Then player is in an arial state
            }
            else { //Else player is grounded
                p.IsGrounded = true;
                if (Time.time - jumpTimeStamp > jumpRegainDelay) { //If player can get back jump
                    p.HasJump = true;
                    p.HasDoubleJump = true;
                    jumpTimeStamp = Time.time;
                }
            }
        }

	    void Update () {
            if (!p.IsAlive) return;

            KeyboardJump();
            TouchJump();
	    }

        private void TouchJump() {
            if (Input.touchCount >= 1) {
                Touch[] touches = Input.touches;
                Touch touch = Input.touches[0];

                foreach (Touch t in touches) { //Iterate through all touches
                    if (t.position.x > Screen.width/2) { //If a touch is on the right side
                        touch = t;
                        break;
                    }
                }

                if (touch.position.x > Screen.width / 2) { //If touch is on the right side of the screen
                    if (touch.phase == TouchPhase.Began) {
                        initialPressPos = touch.position;
                        endPressPos = touch.position;

                        firstJumpCall = true;
                    }
                    if (touch.phase == TouchPhase.Moved) {
                        if (initialPressPos == Vector2.zero)
                            return;

                        endPressPos = new Vector2(touch.position.x, touch.position.y);

                        swipeVector = new Vector2(endPressPos.x - initialPressPos.x, endPressPos.y - initialPressPos.y);

                        distanceVector = new Vector2(Mathf.Abs((swipeVector.x / Screen.width) * 100), Mathf.Abs((swipeVector.y / Screen.height) * 100));

                        if (Input.touchCount == 2) {
                            //If both swipes are moving in the same direction and the timing could be a double swipe
                            if (Accessories.ClosestDirection(touches[1].deltaPosition, DirectionRoundTypes.ALL) == Accessories.ClosestDirection(touches[0].deltaPosition, DirectionRoundTypes.ALL) && p.DoubleSwipe) {
                                return; //Don't attempt to jump
                            }
                        }


                        if (distanceVector.x > swipeThreshold || distanceVector.y > swipeThreshold) { //If the swipe is big enough...
                            Vector3 up = Accessories.ClosestDirection(transform.up, DirectionRoundTypes.ALL);
                            if (up == Vector3.up || up == Vector3.down) //If the player's gravity is on the vertical axis
                                swipeDirection = Accessories.ClosestDirection(swipeVector, DirectionRoundTypes.VERTICAL); //Determine the overall direction of the swipe
                            else //If the player's gravity is on the horizontal axis
                                swipeDirection = Accessories.ClosestDirection(swipeVector, DirectionRoundTypes.HORIZONTAL); //Determine the overall direction of the swipe
                            
                            if (swipeDirection.x == up.x && swipeDirection.y == up.y) {
                                if (firstJumpCall) {
                                    firstJumpCall = false;
                                    Jump(); //Try to jump using a specific jump (needed b/c it runs more than once)
                                }
                            }

                            //Reset the touch
                            initialPressPos = touch.position;
                            endPressPos = touch.position;
                        }
                    }

                    if (touch.phase == TouchPhase.Ended) {
                        //Reset variables
                        initialPressPos = Vector2.zero;
                        endPressPos = Vector2.zero;
                    }
                }
            }
        }

        private void KeyboardJump() {
            if (Input.GetKeyDown(KeyCode.Space)) {
                Jump();    
            }
        }

        public void Jump() { //Make the player jump
            if (!p.IsAlive || p.IsPaused) return;

            if (p.HasJump) { //If player can jump
                jumping = true;
                p.HasJump = false;

                PlayJumpSound();
            }
            else if (p.HasDoubleJump) { //If player can double jump
                jumping = true;
                p.HasDoubleJump = false;

                PlayJumpSound();
            }
        }

        private void PlayJumpSound () {
            SoundManager.Instance.PlayAudio(jumpSound);
        }
    }
}

