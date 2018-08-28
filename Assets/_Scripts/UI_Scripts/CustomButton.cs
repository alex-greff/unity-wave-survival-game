/*Alex Greff
19/01/2016
CustomButton
A custom button script that handles the unique animations
*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

//Implements these interfaces to get access to pointer events
public class CustomButton : MonoBehaviour, IPointerDownHandler, IPointerEnterHandler, IPointerUpHandler, IPointerExitHandler, IPointerClickHandler { 
    public Color normalTextColor;
    public Color[] highlightedTextColors;
    public Color pressedTextColor;

    private ColorFader text;

    private Button button;

    private ColorFader thisColorFader;

    private ColorFaderArray lines;

    void Awake () {
        //Get references
        Image[] images = GetComponentsInChildren<Image>();

        text = GetComponentInChildren<Text>().gameObject.GetComponent<ColorFader>();
        button = GetComponent<Button>();

        thisColorFader = GetComponent<ColorFader>();
        lines = GetComponent<ColorFaderArray>();
    }
	
	void Start () {
        FadeOutLines(0); //Fade out the lines instantly

        if (text)
            text.SetColor(normalTextColor, 0); //Initialize text color
	}

    public void OnPointerClick( PointerEventData data ) {
		//When the button is clicked on
	}

    public void OnPointerDown( PointerEventData data ) {
        if (button.interactable == false) return;

        FadeInLines(0.5f); //Fade in the lines

        if (text)
            text.SetColor(pressedTextColor, 0.5f); //Set the color
	}

    public void OnPointerUp( PointerEventData data ) {
        if (button.interactable == false) return;

        FadeOutLines(0.3f); //Fade out the lines

        if (text)
            text.SetColor(normalTextColor, 0.3f); //Set the color
	}

    public void OnPointerEnter( PointerEventData data ) {
		if (button.interactable == false) return;

        FadeInLines(0.5f); //Fade in the lines

        if (text)
            text.SetColorCycle(highlightedTextColors, 0.5f); //Set the color
	}

	public void OnPointerExit( PointerEventData data ) {
        if (button.interactable == false) return;

        FadeOutLines(0.3f); //Fade out the lines

        if (text)
            text.SetColor(normalTextColor, 0.3f);
	}

    //Handles the line fading
    public void FadeInLines (float duration) {
        lines.FadeIn(duration);
    }

    public void FadeOutLines (float duration) {
        lines.FadeOut(duration);
    }

    public void FadeIn (float duration) { //Fade in

        lines.FadeIn(duration);

        if (text)
            text.FadeIn(duration);

        thisColorFader.FadeIn(duration);
    }

    public void FadeOut (float duration) { //Fade out
        lines.FadeOut(duration);

        text.Cancel(); //Cancel any animations in the text

        if (text)
            text.FadeOut(duration);

        thisColorFader.FadeOut(duration);
    }

    public void SetInteractable (bool interactable) { //Set interactability
        button.interactable = interactable;
        if (text)
            text.Cancel();
    }

    public void SetText (string text) { //Change the text of the button
        if (this.text)
            this.text.gameObject.GetComponent<Text>().text = text;
    }
}
