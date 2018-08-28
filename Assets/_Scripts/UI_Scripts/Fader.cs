using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class Fader : MonoBehaviour {
    [SerializeField] protected float initialDuration = 0.3f;
    protected float duration;

    protected float t = Mathf.Infinity;

    private bool running = false;
	
    //Getters and setter
    public bool Running {
        get { return running; }
    }

    protected virtual void Awake () {
        //On awake
    }

    protected virtual void Start () {
        //On start (called after awake)
    }

	protected virtual void Update () {
        if (t <= 1) { //If the t hasn't reach one yet
            UpdateEvent(); //Call update event
        }
        if (t > 1 && running) { //If its greater than one
            EndEvent(); //End the fade
        }
	}

    protected virtual void UpdateEvent () {
        t += Time.deltaTime / duration; //Increment the time
    }
    protected virtual void EndEvent () {
        running = false; //Set running to false
    }
    protected virtual void CancelEvent () { //Cancels the fade
        t = Mathf.Infinity;
        running = false;
    }

    protected virtual void StartAnim (float duration) { //Starts the fade
        this.duration = duration;

        t = 0;

        running = true;
    }

    public virtual void Cancel () { //Cancles the fade (public version)
        CancelEvent();
    }
}
