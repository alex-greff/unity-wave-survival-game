/*Alex Greff
19/01/2016
LoadingText
Displays the loading text "dot dot dot" animation
*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent (typeof(Text))]
public class LoadingText : MonoBehaviour {
    private const float delay = 0.5f;

    private Text text;
    private IEnumerator animation_loop;

    private string initText;
    private int dots = 0;
    private const int MAX_DOTS = 3;

	void Awake () {
        //Get reference
        text = GetComponent<Text>();

        initText = text.text; //Get the initial text
    }
	
	void OnEnable () {
        animation_loop = loop();
        StartCoroutine(animation_loop); //Start the animation
    }

    void OnDisable () {
        StopCoroutine(animation_loop); //End the animations
    }

    private IEnumerator loop () {
        while(true) {
            dots++;
            dots = dots % (MAX_DOTS + 1); 

            string txt = initText;

            //Construct the dots
            for (int i = 0; i < dots; i++) {
                txt += ".";
            }

            text.text = txt; //Update text

            yield return new WaitForSeconds(delay); //Wait
        }
    }
}
