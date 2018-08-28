using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

namespace PlayerComponent {
    public class Player_Movement : MonoBehaviour {
        private Player p;
        private Rigidbody2D rb;
        private Animator anim;
        private ConstantForce2D cf;

        [SerializeField] private AudioClip gravitySwapSound;

        private ConstantForce2D localGravity;

        private const float GRAVITY_CONSTANT = 30f; 

        private float initScale;

        private float speedGround = 5f;
        private float speedAir = 4.5f;

        private Vector3 rotateTo;

        //For button movement
        private bool moving = false;
        private Vector2 movingDirection = Vector2.zero;

        private const string MANAGER_GAMEOBJECT_NAME = "_MANAGERS_";

        //For finger swipes
        private Vector2[] initialPressPos = new Vector2[2]; //The beginning pos of the swipe
        private Vector2[] endPressPos = new Vector2[2]; //The end pos of the swipe
        private Vector2[] swipeVector = new Vector2[2]; //The pixel distance the touch as moved
        private Vector2[] distanceVector = new Vector2[2]; //The percent of the screen that the touch has moved
        private Vector2[] swipeDirection = new Vector2[2]; //The determined direction of the swipe
        private float[] beginningTime = new float[2]; //The beginning times of the touches

        private float movementThreshold = 20f; //OLD MOVEMENT 4f

        private float swipeThreshold = 50f; //The pixels that has to be travelled in order to register the swipe //OLD 2f
        private float timeDifferenceThreshold = 0.5f; //500 milliseconds

        private float deltaPosThreshold = 3f; //The movement threshold needed to trigger a change in direction
        private float fingerStrayThreshold = 2f; //How far a finger can stray away from the last direction change point (in screen percent) before it's forced to recalculate

        private Vector2 moveDir = Vector2.zero; //The current direction of movement
        private Vector2 lastDeltaPos = Vector2.zero; //The last frame's change in touch vector
        private Vector2 lastCalculatedPos = Vector2.zero; //The last touch position that was calculated for movement direction

        private Vector2 greatestDeltaPos = Vector2.zero;

        private bool firstTimeSwipe = false;

        private Vector2 initialPosMovement = Vector2.zero;

        private bool firstTimeAudio = true;

        private Vector2 lastGravityDirection;

        void Awake () {
            //Get references
            p = GetComponent<Player>();
            rb = GetComponent<Rigidbody2D>();
            anim = GetComponent<Animator>();
            cf = GetComponent<ConstantForce2D>();

            localGravity = GetComponent<ConstantForce2D>();
        }

	    void Start () {
            rb.gravityScale = 0; //Disable default gravity
            changeGravity(Vector2.down); //Default gravity to down

            initScale = transform.localScale.y;
	    }
	
	    void Update () { //Frame updates
            anim.SetBool("grounded", p.IsGrounded); //Set grounded state

            if (!p.IsAlive) return;

            KeyboardMovement();
            KeyboardGravityFlip();

            TouchMovement();
            //TouchGravityFlip();

            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(rotateTo), 30 * Time.deltaTime);

            if (moving) {
                moveCharacter(movingDirection, true);
            }
	    }

        public void move (string dir) { //Used if being called by a button
            dir = dir.Trim().ToLower(); //Format the direction
            moving = true;

            if (dir == "left") 
                movingDirection = determineMovement(Vector2.left);
            else if (dir == "right")
                movingDirection = determineMovement(Vector2.right);
            else if (dir == "up")
                movingDirection = determineMovement(Vector2.up);
            else if (dir == "down")
                movingDirection = determineMovement(Vector2.down);
        }

        public void moveEnd() { //For button inputs
            anim.SetBool("walk", false);
            moving = false;
        }

        private void TouchMovement() {

            if (Input.touchCount >= 1) {
                Touch[] touches = Input.touches;
                Touch touch = Input.touches[0];

                foreach (Touch t in touches) { //Iterate through all touches
                    if (t.position.x  < Screen.width/2) { //If a touch is on the left side
                        touch = t; //Use that touch as the main one
                        break;
                    }
                }

                if (touch.position.x < Screen.width / 2) { //If touch is on the left side of the screen
                    p.MovementTouch = touch;
                    firstTimeSwipe = false;

                    if (touch.phase == TouchPhase.Began) {
                        //OLD MOVEMENT
                        initialPosMovement = touch.position;

                        initialPressPos[0] = touch.position;
                        endPressPos[0] = touch.position;

                        swipeDirection[0] = Vector2.zero;

                        UI_MovementManager.Instance.TouchInitialize(initialPressPos[0]);
                    }

                    if (touch.phase == TouchPhase.Moved) {
                        if (initialPosMovement == Vector2.zero) 
                            return;

                        

                        //NEW MOVEMENT
                        /*Vector2 deltaPos = touch.deltaPosition;

                        if (Vector2.Distance(deltaPos, Vector2.zero) > Vector2.Distance(greatestDeltaPos, Vector2.zero))
                            greatestDeltaPos = deltaPos;


                        Vector2 up = transform.up;

                        //if (deltaPos == Vector2.zero) //If there was no finger movement 
                        if (deltaPos == Vector2.zero) //If there was no finger movement 
                            return; //Do nothing (stop)

                        //If the change in position of the touch is greater than a threshold amount then calculate the movement direction
                        //This removes the hyper sensitivity to change in direction which could cause unintentional and erratic direction changes
                        //Funny fact: setting a really low delta pos threshold resulted in jitery fliping when firing and walking at once... which sucked
                        if (deltaPos.x > deltaPosThreshold || deltaPos.y > deltaPosThreshold) {
                            //Calculate the movement direction

                            if (up == Vector2.up || up == Vector2.down) 
                                moveDir = Accessories.ClosestDirection(deltaPos, DirectionRoundTypes.HORIZONTAL);
                            else if (up == Vector2.right || up == Vector2.left)
                                moveDir = Accessories.ClosestDirection(deltaPos, DirectionRoundTypes.VERTICAL);

                            moveCharacter(determineMovement(moveDir));

                            lastCalculatedPos = touch.position; //Record the last finger position that the player's direction was calculated

                            lastDeltaPos = deltaPos; //Update the last pos (this is only referenced again on the next frame)
                        }
                        //If the finger has strayed too far from the original turn point position
                        //Can be done by abusing the deltaPosThreshold mechanic by moving finger really slow 
                        //Which causes it directional change not to be calculated
                        //else if (Mathf.Abs(Vector2.Distance(lastCalculatedPos, touch.position)) > fingerStrayThreshold) {

                        else if (Mathf.Abs(lastCalculatedPos.x - touch.position.x) / Screen.width * 100 > fingerStrayThreshold || Mathf.Abs(lastCalculatedPos.y - touch.position.y) / Screen.height * 100 > fingerStrayThreshold) {
                            //Force a movement direction recalculation
                            if (up == Vector2.up || up == Vector2.down)
                                moveDir = Accessories.ClosestDirection(deltaPos, DirectionRoundTypes.HORIZONTAL);
                            else if (up == Vector2.right || up == Vector2.left)
                                moveDir = Accessories.ClosestDirection(deltaPos, DirectionRoundTypes.VERTICAL);

                            moveCharacter(determineMovement(moveDir));

                            lastCalculatedPos = touch.position; //Record the last finger position that the player's direction was calculated

                            lastDeltaPos = deltaPos; //Update the last pos (this is only referenced again on the next frame)
                        }
                        else { //If nothing is trigger to this point then continue moving in the last valid known direction
                            if (up == Vector2.up || up == Vector2.down) 
                                moveDir = Accessories.ClosestDirection(lastDeltaPos, DirectionRoundTypes.HORIZONTAL);
                            else if (up == Vector2.right || up == Vector2.left) 
                                moveDir = Accessories.ClosestDirection(lastDeltaPos, DirectionRoundTypes.VERTICAL);

                            moveCharacter(determineMovement(moveDir));
                        }
                        */

                        //OLD MOVEMENT
                        endPressPos[0] = touch.position;

                        swipeVector[0] = new Vector2(endPressPos[0].x - initialPressPos[0].x, endPressPos[0].y - initialPressPos[0].y);

                        //distanceVector[0] = new Vector2(Mathf.Abs((swipeVector[0].x / Screen.width) * 100), Mathf.Abs((swipeVector[0].y / Screen.height) * 100));
                        
                        Vector2 distanceVec = new Vector2(Mathf.Abs(swipeVector[0].x), Mathf.Abs(swipeVector[0].y));
                        //if (distanceVector[0].x > movementThreshold || distanceVector[0].y > movementThreshold) { //If the swipe is big enough...
                        if (distanceVec.x > movementThreshold || distanceVec.y > movementThreshold) {

                            //swipeDirection[0] = Accessories.ClosestDirection(swipeVector[0], DirectionRoundTypes.ALL); //Determine the overall direction of the swipe


                            //swipeDirection[0] = Accessories.ClosestDirection(swipeVector[0], DirectionRoundTypes.VERTICAL); //Determine the overall direction of the swipe
                            swipeDirection[0] = Accessories.ClosestDirection(swipeVector[0], DirectionRoundTypes.HORIZONTAL); //Determine the overall direction of the swipe

                            moveCharacter(determineMovement(swipeDirection[0]), true);
                        }
                        else //If the movement isn't enough 
                            swipeDirection[0] = Vector2.zero; //Set swipe direction to nothing

                        UI_MovementManager.Instance.TouchUpdate(endPressPos[0]);

                        
                    }

                    if (touch.phase == TouchPhase.Stationary) {
                        if (initialPosMovement == Vector2.zero) 
                            return;

                        //NEW MOVEMENT
                        /*Vector2 deltaPos = touch.deltaPosition;

                        if (Vector2.Distance(deltaPos, Vector2.zero) > Vector2.Distance(greatestDeltaPos, Vector2.zero))
                            greatestDeltaPos = deltaPos;

                        if (greatestDeltaPos.x > 0.5f || greatestDeltaPos.y > 0.5f)
                            moveCharacter(determineMovement(moveDir));
                        */

                        moveCharacter (determineMovement(swipeDirection[0]), true); //OLD MOVEMENT

                        UI_MovementManager.Instance.TouchUpdate(endPressPos[0]);
                    }

                    if (touch.phase == TouchPhase.Ended) {

                        //NEW MOVEMENT
                        //greatestDeltaPos = Vector2.zero; //Reset 


                        //OLD MOVEMENT
                        initialPosMovement = Vector2.zero;

                        initialPressPos[0] = Vector2.zero;
                        endPressPos[0] = Vector2.zero;

                        UI_MovementManager.Instance.TouchEnd();

                        anim.SetBool("walk", false);
                    }
                }
                else {
                    if (firstTimeSwipe == false) {
                        firstTimeSwipe = true;

                        initialPressPos[0] = Vector2.zero;
                        endPressPos[0] = Vector2.zero;

                        UI_MovementManager.Instance.TouchEnd();

                        anim.SetBool("walk", false);
                    }
                }
            }
        }


        private void TouchGravityFlip () {
            if (Input.touchCount > 0) {
                Touch touch1 = Input.touches[0];
                if (touch1.phase == TouchPhase.Began) {
                    beginningTime[0] = Time.time;

                    initialPressPos[0] = touch1.position;
                    endPressPos[0] = touch1.position;
                }
            }


            if (Input.touchCount == 2) {
                Touch[] touches = { Input.touches[0], Input.touches[1] };

                if (touches[1].phase == TouchPhase.Began) {

                    beginningTime[1] = Time.time;

                    initialPressPos[1] = touches[1].position;
                    endPressPos[1] = touches[1].position;

                    if (beginningTime[1] - beginningTime[0] < timeDifferenceThreshold)  //If start times are small enough
                        p.DoubleSwipe = true;
                    else //If not
                        p.DoubleSwipe = false;
                }

            
                if (!p.DoubleSwipe) //If the swipe doesn't count
                    return; //Stop from progressing any futher

                if (touches[0].phase == TouchPhase.Moved || touches[1].phase == TouchPhase.Moved) { //If one of the touches moves
                    for (int i = 0; i < 2; i++) {
                        endPressPos[i] = new Vector2(touches[i].position.x, touches[i].position.y);

                        swipeVector[i] = new Vector2(endPressPos[i].x - initialPressPos[i].x, endPressPos[i].y - initialPressPos[i].y);

                        Vector2 distanceVec = new Vector2(Mathf.Abs(swipeVector[0].x), Mathf.Abs(swipeVector[0].y));

                        if (distanceVec.x > swipeThreshold || distanceVec.y > swipeThreshold) 
                            swipeDirection[i] = Accessories.ClosestDirection(swipeVector[i], DirectionRoundTypes.VERTICAL); //Determine the overall direction of the swipe
                    }
                }

                if (touches[0].phase == TouchPhase.Ended || touches[1].phase == TouchPhase.Ended) { //If one of them ends
                    for (int i = 0; i < 2; i++) { //Reset the values
                        initialPressPos[i] = Vector2.zero;
                        endPressPos[i] = Vector2.zero;
                    }
                }
            }

            if (swipeDirection[0] == Vector2.zero || swipeDirection[1] == Vector2.zero)
                return;

            if (swipeDirection[0] == swipeDirection[1])
                changeGravity(swipeDirection[0]);
        }

        private void KeyboardMovement() {
            //MOVEMENT
            if (Input.GetKey(KeyCode.A)) {
                moveCharacter(determineMovement(Vector2.left), false);
                return;
            }
            if (Input.GetKey(KeyCode.D)) {
                moveCharacter(determineMovement(Vector2.right), false);
                return;
            }
            if (Input.GetKey(KeyCode.W)) {
                moveCharacter(determineMovement(Vector2.up), false);
                return;
            }
            if (Input.GetKey(KeyCode.S)) {
                moveCharacter(determineMovement(Vector2.down), false);
                return;
            }

            anim.SetBool("walk", false);    
        }

        private void KeyboardGravityFlip () {
            if (Input.GetKeyDown(KeyCode.W)) { //Up 
                changeGravity(Vector2.up);
            }
            if (Input.GetKeyDown(KeyCode.S)) { //Down
                changeGravity(Vector2.down);
            }
        }


        private Vector2 determineMovement(Vector2 dir) {
            //print(dir);
            //print(dir + " = " + Vector2.down);

            if (dir == Vector2.left) {
                Vector3 up = transform.up;
                if (up == Vector3.up) { //Normal orientation
                    return Vector2.left;
                }
                else if (up == Vector3.down) { //Upside down orientation
                    return Vector2.right;
                }
                else if (up == Vector3.right) { //Gravity to left
                    //Do nothing
                }
                else if (up == Vector3.left) { //Gravity to right
                    //Do nothing
                }
            }
            else if (dir == Vector2.right) {
                Vector3 up = transform.up;
                if (up == Vector3.up) { //Normal orientation
                    //moveCharacter(Vector3.right);
                    return Vector2.right;
                }
                else if (up == Vector3.down) { //Upside down orientation
                    //moveCharacter(Vector3.left);
                    return Vector2.left;
                }
                else if (up == Vector3.right) { //Gravity to left
                    //Do nothing
                }
                else if (up == Vector3.left) { //Gravity to right
                    //Do nothing
                }
            
            }
            else if (dir == Vector2.up) {
                Vector3 up = transform.up;

                if (up == Vector3.up) { //Normal orientation
                    //Do nothing
                }
                else if (up == Vector3.down) { //Upside down orientation
                    //Do nothing
                }
                else if (up == Vector3.right) { //Gravity to left
                    return Vector2.left;
                }
                else if (up == Vector3.left) { //Gravity to right
                    return Vector2.right;
                }
            }
            else if (dir == Vector2.down) {
                Vector3 up = transform.up;


                if (up == Vector3.up) { //Normal orientation
                    //Do nothing      
                }
                else if (up == Vector3.down) { //Upside down orientation
                    //Do nothing
                }
                else if (up == Vector3.right) { //Gravity to left
                    return Vector2.right;
                }
                else if (up == Vector3.left) { //Gravity to right
                    return Vector2.left;
                }
            }


            return Vector2.zero; //If nothing passes
        }

        private void moveCharacter(Vector2 dir, bool calculatePercent) {
            if (!p.IsAlive || p.IsPaused) return;

            //print("Dir in moveCharacter: " + dir);

            float percent = 1;

            if (calculatePercent == true) { //Get the speed that the player is moving at currently
                percent = UI_MovementManager.Instance.Percent;
                percent = percent * 1.5f;
                percent = Mathf.Clamp01(percent);
            }

            if (anim.GetCurrentAnimatorStateInfo(0).IsName("walk")) //If the player is walking
                anim.speed = percent; //Set the animation speed to the percent
            else //If not...
                anim.speed = 1; //Just leave it be

            if (dir == Vector2.zero || dir == Vector2.up || dir == Vector2.down) { 
                anim.SetBool("walk", false); //Set player to not walking
                return;
            }

            if (p.IsGrounded) { //If on the ground
                transform.Translate(dir * speedGround * percent * Time.deltaTime); //Move player by ground speed
            }
            else {
                transform.Translate(dir * speedAir * percent * Time.deltaTime); //Move player by air speed
            }

            transform.localScale = new Vector3(dir.x * initScale, initScale, 1); //Set x scale
            anim.SetBool("walk", true); //Set walking to true
        }

        public void changeGravity (Vector2 dir) {
            if (!p.IsAlive || p.IsPaused) return;
            if (lastGravityDirection != dir) {
                rotateTo = new Vector3(0, 0, 180);

                //Set local gravity
                localGravity.force = rb.mass * GRAVITY_CONSTANT * dir; 

                //Rotate the player to face direction of gravity
                if (dir == Vector2.up)
                    rotateTo = new Vector3(0, 0, 180);
                else if (dir == Vector2.down)
                    rotateTo = new Vector3(0, 0, 0);
                else if (dir == Vector2.left)
                    rotateTo = new Vector3(0, 0, 270);
                else if (dir == Vector2.right)
                    rotateTo = new Vector3(0, 0, 90);

                lastGravityDirection = dir;

                if (firstTimeAudio) {
                    firstTimeAudio = false;
                    return;
                }

                SoundManager.Instance.PlayAudio(gravitySwapSound);
            }
        }
    }
}

