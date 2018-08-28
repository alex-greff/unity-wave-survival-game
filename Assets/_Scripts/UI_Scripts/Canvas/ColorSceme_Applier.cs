/*Alex Greff
19/01/2016
ColorSchemeApplier
Used to apply a certain color scheme to all children of a canvas
*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ColorSceme_Applier : MonoBehaviour {
    private Selectable[] selectableObjects;

    //User input
    public Color normalColor;
    public Color highlightedColor;
    public Color pressedColor;
    public Color disabledColor;
    [Range (1,5)] public float colorMultiplier = 1;
    public float fadeDuration = 0.3f;


    void Awake () {
        //Get reference to all selectable objects
        selectableObjects = GetComponentsInChildren<Selectable>();
    }
	
	void Start () {
        ApplyColors(); //Apply the colors on start
	}

    private void ApplyColors () {
        foreach (Selectable selectable in selectableObjects) {
            if (selectable.GetComponent<Ignore_script>() == null) { //If it doesn't have the ignore flag
                ColorBlock cb = selectable.colors; //Make a copy of the colors
                
                //Set the custom colors
                cb.normalColor = normalColor;
                cb.highlightedColor = highlightedColor;
                cb.pressedColor = pressedColor;
                cb.disabledColor = disabledColor;

                cb.colorMultiplier = colorMultiplier;
                cb.fadeDuration = fadeDuration;

                selectable.colors = cb; //Re-apply the colors
            }
        }
    }
}
