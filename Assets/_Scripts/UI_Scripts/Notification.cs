/*Alex Greff
19/01/2016
Notification
Notifications that are displayed to the user
*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Notification : StretchFade {
    private Text txt;
    protected Color initialColor;

	protected override void Awake () {
        //Get text component
        txt = GetComponent<Text>();

        if (txt == null) //If it doesn't find any text component then check its children
            txt = GetComponentInChildren<Text>(); //Find text component in children

        base.Awake();
    }
    protected override void Start() {
        initialColor = txt.color; //Get initial color

        base.Start();
    }
    
    public void StartAnimation(string message, float duration, Color color, Vector3 position) {
        txt.color = color; //Set text color
        txt.text = message; //Set text message

        StartAnimation(duration, position); //Start the animation
    }

    public void StartAnimation (string message) { //Keeps the notification open forever
        StartAnimation(message, Mathf.Infinity, initialColor, initialPos);
    }

    public void StartAnimation (string message, float duration) { //Opens the notification
        StartAnimation(message, duration, initialColor, initialPos);
    }

    public void StartAnimation (string message, float duration, Vector3 position) {
        StartAnimation(message, duration, initialColor, position);
    } 

    public void StartAnimation (string message, float duration, Color color) {
        StartAnimation(message, duration, color, initialPos);
    }
}
