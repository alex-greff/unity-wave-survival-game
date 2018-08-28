/*Alex Greff
19/01/2016
OnlineCanvasComponents
A canvas that only shows it's components if the player is in online mode
*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OnlineCanvasComponents : ToggleCanvas {
    private static OnlineCanvasComponents current;

    private const string USERNAME_TEXT_NAME = "Username Text";
    private Text usernameText;

    //Getters and setters
    public static OnlineCanvasComponents Current {
        get {
            return current;
        }
    }

	protected override void Start() {
        current = this;
        base.Start();

        usernameText = transform.Find(USERNAME_TEXT_NAME).GetComponent<Text>(); //Get reference

        usernameText.text = MultiSceneVariables.Instance.Username; //Set the username to the text

        if (MultiSceneVariables.Instance.Online == false) //If not online
            HideCanvas(); //Hide the canvas
    }
}
