/*Alex Greff
19/01/2016
ShopBlock
The block that the player can stand on to load the shop up
*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopBlock : MonoBehaviour {
    public static ShopBlock instance;

    private GameObject worldSpaceCanvas;

    private Vector2 offset = new Vector2(0, 1f);

    private SmoothSlider slider;
    private ColorFader sliderColorFader;

    private float contactTime = 0;
    private float contactMinTime = 1f;
    private float noContactMaxTime = 1f;
    private float colorChangeTime = 0.2f;

    private bool inContact = false;
    private bool openMenu = false;

    private IEnumerator resetTimer;

    private float percent = 0;

    void Awake () {
        instance = this;
    }

	void Start () {
        //Get the world-space canvas
        worldSpaceCanvas = GameObject.FindGameObjectWithTag("WorldSpaceCanvas");

        //Instantiate the loading slider
        GameObject slider = Instantiate(Resources.Load("Prefabs/Stage/SlidingInfoBar", typeof(GameObject)), (Vector2) transform.position + offset, Quaternion.identity, worldSpaceCanvas.transform) as GameObject;
        this.slider = slider.GetComponent<SmoothSlider>();
        sliderColorFader = slider.GetComponent<ColorFader>();

        this.slider.StartAnimation(0, 0); //Set the progress slider to 0%

        StartCoroutine(updateTimer(colorChangeTime)); //Start the update timer
	}

    void Update () {
        if (inContact && PauseManager.Instance.Paused == false && Player.Instance.IsAlive == true && GameOverManager.Instance.GameOver == false) {
            contactTime += Time.deltaTime; //Increase the contact time
            percent = contactTime / contactMinTime; //Recalculate the percentage done
        }

        if (contactTime >= contactMinTime && inContact) { //If the player has been in contact long enough
            inContact = false;
            openMenu = true;
            contactTime = contactMinTime;
            percent = 1;

            sliderColorFader.SetColor(Color.green, 0.5f); //Change color to green

            if (resetTimer != null)
                StopCoroutine(resetTimer); //Stop the reset timer if it's running

            ShopManager.Instance.OpenMenu(1f); //Open the shop menu
        }
    }
	
	void OnTriggerEnter2D(Collider2D col) {
        if (col.gameObject.tag == "Player") {
            OpenMenu(); 
        }
    }

    void OnTriggerExit2D (Collider2D col) {
        if (col.gameObject.tag == "Player") {
            CloseMenu();
        }
    }

    public void OpenMenu() {
        if (resetTimer != null)
            StopCoroutine(resetTimer); //Stop any prexisting reset timers running

        inContact = true;
        ShopManager.Instance.ShopOpening = true; //Set shop opening flag to true

        sliderColorFader.ResetColor(colorChangeTime); //Reset the color
    }

    public void CloseMenu () {
        if (openMenu) { //If the player is in the menu
            openMenu = false;
            contactTime = 0;
            percent = 0;

            ShopManager.Instance.ShopOpening = false; //Set shop opening flag to false
        }

        inContact = false;
            
        if (resetTimer != null)
        StopCoroutine(resetTimer); //Stop any prexisting reset timers running

        resetTimer = resetDelay(noContactMaxTime); //Initialize the reset timer
        StartCoroutine(resetTimer); //Start the reset timer
    }

    private IEnumerator resetDelay (float delay) {
        yield return new WaitForSeconds (delay);

        sliderColorFader.SetColor(Color.red, colorChangeTime); //Set color to red

        ShopManager.Instance.ShopOpening = false; //Set shop opening flag to false
        contactTime = 0; //Reset the contact time
        percent = 0;
    }

    private IEnumerator updateTimer (float updateTime) {
        while (true) {
            slider.StartAnimation(percent, updateTime); //Update the slider with the percentage value
            yield return new WaitForSeconds(updateTime); //Wait
        }
    }
}
