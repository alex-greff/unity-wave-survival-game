/*Alex Greff
19/01/2016
StretchFade
A neat animation effect
*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StretchFade : MonoBehaviour {
    public enum FadeAnimation {
        VERTICAL, HORIZONTAL
    }

    [SerializeField]
    private bool showOnStart = false;

    [SerializeField]
    private bool usePauseFlag = false;

    private RectTransform rt;

    [SerializeField]
    private FadeAnimation rotationAxis = FadeAnimation.VERTICAL;

    [SerializeField] private float START_ROT = 90;
    [SerializeField] private float START_SCALE = 1;

    private Quaternion startRot = Quaternion.identity;
    private Quaternion endRot = Quaternion.identity;

    private Vector3 startScale = Vector3.one;
    private Vector3 endScale = Vector3.one;

    private float t = Mathf.Infinity;
    private float duration = 2;
    private float timeStamp = 0;
    private float fade_duration = 0.3f;

    private bool firstTime = false;
    private bool shouldEnd = false;

    protected Vector3 initialPos;

    protected bool running = false;

    public bool isRunning {
        get {
            return running;
        }
    }

    protected virtual void Awake () {
        rt = GetComponent<RectTransform>();    
    }
	
	protected virtual void Start () {
        initialPos = rt.position;

        if (showOnStart)
            gameObject.SetActive(true);
        else
            gameObject.SetActive(false);
	}
	
	protected virtual void Update () {
        if (t <= 1) { //If the animation is runnign
            rt.rotation = Quaternion.Lerp(startRot, endRot, t); //Lerp rotation
            rt.localScale = Vector3.Lerp(startScale, endScale, t); //Lerp scale
            t += Time.deltaTime / fade_duration; //Increment t
        }
        if (t > 1 && shouldEnd) { //If the animation should end
            gameObject.SetActive(false); //Set active to false
            shouldEnd = false; //Set should end flag
        }

        if (usePauseFlag) { //If using the modified pause time
            if (PauseManager.Instance)
                if (PauseManager.Instance.Paused) return; //Check for pause

            if (Accessories.time - timeStamp > duration && firstTime) {
                EndAnimation(); //End the animation
                firstTime = false; 
            }
        }
        else { //If just using regular time
            if (Time.time - timeStamp > duration && firstTime) {
                EndAnimation(); //End the animation
                firstTime = false;
            }
        }
    }

    //Starts the animation
    public void StartAnimation(float duration, Vector3 position) {
        //Initialize values
        rt.position = position;

        this.duration = duration;

        if (usePauseFlag)
            timeStamp = Accessories.time;
        else
            timeStamp = Time.time;

        if (rotationAxis == FadeAnimation.VERTICAL) {
            startRot = Quaternion.Euler(new Vector3(START_ROT, 0, 0));
            startScale = new Vector3(START_SCALE, 1, 1);
        }
        else if (rotationAxis == FadeAnimation.HORIZONTAL) {
            startRot = Quaternion.Euler(new Vector3(0, START_ROT, 0));
            startScale = new Vector3(0, START_SCALE, 1);
        }

        endRot = Quaternion.identity;
        endScale = Vector3.one;

        rt.rotation = startRot;
        rt.localScale = startScale;

        //Set flags
        firstTime = true;
        shouldEnd = false;

        gameObject.SetActive(true); //Show gameobject

        t = 0; //Reset timer variable

        running = true; //Set running flag
    }

    public void StartAnimation () { //Keeps the notification open forever
        StartAnimation(Mathf.Infinity, initialPos);
    }

    public void StartAnimation (float duration) { //Opens the notification
        StartAnimation(duration, initialPos);
    }

    public void EndAnimation () { //Ends the notification with an animation
        startRot = rt.rotation;
        startScale = rt.localScale;

        endRot = Quaternion.Euler(new Vector3(START_ROT, 0, 0)); //Get the end rotation
        endScale = Vector3.one;

        shouldEnd = true;

        t = 0;

        running = false;
    }

    public void CancelAnimation () { //End the notification without an animation
        t = Mathf.Infinity;
        firstTime = false;
        shouldEnd = false;

        gameObject.SetActive(false);

        running = false;
    }
}
