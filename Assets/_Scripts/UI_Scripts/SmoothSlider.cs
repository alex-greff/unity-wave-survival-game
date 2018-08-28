/*Alex Greff
19/01/2016
SmoothSlider
Smoothly slides the slider components
*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SmoothSlider : MonoBehaviour {
    private Image img;

    private float duration = 0.3f;

    private float t = Mathf.Infinity;
    private float end_percent = 1;
    private float start_percent = 1;


    void Awake () {
        img = GetComponent<Image>();
    }
	
	void Update () {
		if (t <= 1) {
            float amt = Mathf.Lerp(start_percent, end_percent, t); //Determine the raw movement amount
            amt = Mathf.Round(amt * 100) / 100; //Round to nearest hundredth

            if (amt <= 0.02f)
                amt = 0;

            img.fillAmount = amt; //Apply amount
            t += Time.deltaTime / duration; //Decrement time position of lerp
        }
	}

    public void StartAnimation (float endPercent, float duration) {
        if (duration <= 0) { //If duration is less than or equal to zero
            img.fillAmount = endPercent; //Instantly set the fill amount to the end percent
            return; //Don't go any further
        }

        this.duration = duration;

        end_percent = endPercent; //Get the target percent amount
        start_percent = img.fillAmount; //Get a snapshot of the current percent amount

        t = 0; //Start lerp
    }
}
