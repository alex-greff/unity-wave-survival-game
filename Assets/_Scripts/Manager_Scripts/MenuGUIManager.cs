/*Alex Greff
19/01/2016
MenuGUIManager
The base class for menus
*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuGUIManager : MonoBehaviour {
    //The button used to enter the UI
    [Header("Components with the keywords 'blocking' and 'backdrop' are required.")]
    [SerializeField] protected CustomButton entryButton;

    //The buttons in the UI menu
    protected List<CustomButton> menuButtons = new List<CustomButton>();

    //The other components (images / text) in the UI menu
    protected List<ColorFader> otherComponents = new List<ColorFader>();

    //The image component used to fade the background UI and game elements out
    protected ColorFader backdrop; 

    //The panel used to block inputs to other items with a lower order layer
    private Image blockingPanel; 

    protected IEnumerator timedFunction;

    public bool defaultOpen = false;

    protected CustomButton ignoredButton; //A button that is ignored when changes are applied

    protected virtual void Awake () {
        CustomButton[] allButtons = GetComponentsInChildren<CustomButton>();

        foreach (CustomButton cb in allButtons) {
            if (cb != entryButton) //If the Custom button isn't the entry button
                if (cb.GetComponentInParent<ShopCategory>() == null) //If it's not part of a shop category
                    menuButtons.Add(cb); //Add it to the menu buttons list
                else //If it is
                    if (GetComponent<ShopItem>() != null) //If the parent is a shop item
                        menuButtons.Add(cb); //Add it to the menu buttons list
        }

        ColorFader[] allColorFaders = GetComponentsInChildren<ColorFader>();

        foreach (ColorFader cf in allColorFaders) {
            if (cf.transform.name.ToLower().Contains("backdrop")) { //If it's the backdrop
                backdrop = cf;
            }

            if (cf != backdrop) {//If the color fader script isn't the backdrop's 
                if (cf.GetComponentInParent<CustomButton>() == null) //If it doesn't belong to a custom button
                    if (cf.GetComponentInParent<ShopCategory>() == null)
                        otherComponents.Add(cf); //Add it to the other components list
                    else
                        if (GetComponent<ShopItem>() != null) //If the parent is a shop item
                            otherComponents.Add(cf); //Add it to the other components list
            }
        }

        //Find the blocking panel
        Image[] allImages = GetComponentsInChildren<Image>();
        foreach (Image img in allImages) {
            if (img.transform.name.ToLower().Contains("blocking")) //If the transform name includes "blocking"
                blockingPanel = img; //Assume it's the blocking panel
        }
    }

	protected virtual void Start () {
        if (defaultOpen)
            OpenMenu(0); //Open menu if defaulted to open
        else
            CloseMenu(0); //Close menu if the menu is defaulted to closed
	}

    public virtual void OpenMenu (float duration) {
        //Set up a new instance the timed delayed function
        if (timedFunction != null)
            StopCoroutine(timedFunction);

        timedFunction = setComponentsActive(true, 0);
        StartCoroutine(timedFunction);

        if (entryButton) //If there is an entry button
            entryButton.SetInteractable(false); //Disable the entry button

        if (blockingPanel)
            blockingPanel.enabled = true; //Enable the blocking panel

        //Fade in the buttons
        foreach (CustomButton mb in menuButtons) {
            if (mb != ignoredButton) {
                mb.SetInteractable(true);
            }
            mb.FadeIn(duration);
        }

        if (backdrop)
            backdrop.FadeIn(duration); //Fade in the backdrop

        //Fade in the other UI components
        foreach (ColorFader other in otherComponents) {
            other.Cancel();
            other.FadeIn(duration);
        }
    }

    public virtual void CloseMenu (float duration) {
        if (entryButton) //If there is an entry button
            entryButton.SetInteractable(true); //Renable entry button

        if (blockingPanel)
            blockingPanel.enabled = false; //Disable blocking panel

        //Fade out menu buttons
        foreach (CustomButton mb in menuButtons) {
            if (mb != ignoredButton) {
                mb.SetInteractable(false);
            }
            mb.FadeOut(duration);
        }

        if (backdrop)
            backdrop.FadeOut(duration); //Fade out the backdrop

        //Fade out the other UI components
        foreach (ColorFader other in otherComponents) {
            other.Cancel();
            other.FadeOut(duration);
        }

        //Set up a new instance the timed delayed function
        if (timedFunction != null)
            StopCoroutine(timedFunction);

        timedFunction = setComponentsActive(false, duration);
        StartCoroutine(timedFunction);
    }

    private IEnumerator setComponentsActive (bool state, float delay) {
        yield return new WaitForSeconds(delay); //Wait for a certain amount of time

        foreach (CustomButton mb in menuButtons) {
            mb.gameObject.GetComponent<Image>().enabled = state; //Enable the image of each menu button

            if (state == true)
                mb.FadeOutLines(0); //Make sure their effect lines are initialized 
        }

        foreach (ColorFader other in otherComponents) {
            other.gameObject.GetComponent<MaskableGraphic>().enabled = state; //Enable/disable any graphical (text or image) component
        }
    }
}
