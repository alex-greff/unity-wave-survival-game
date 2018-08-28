/*Alex Greff
19/01/2016
PauseManager
Handles pausing the game
*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PauseManager : MenuGUIManager {

    private static PauseManager instance;

    public static PauseManager Instance {
        get {
            return instance;
        }
    }

    private bool menuOpen = false;

    private float init_duration = 1;

    private bool isPaused = false;

    public delegate void PauseEvent();
    public static event PauseEvent OnPause;

    public delegate void UnpauseEvent();
    public static event UnpauseEvent OnUnpause;

    private float timeSetBack = 0;
    
    public float TimeSetBack {
        get {
            return timeSetBack;
        }
    }

    protected override void Awake () {
        base.Awake();

        instance = this;
    }

    void Update () {
        if (isPaused) //If paused
            timeSetBack += Time.deltaTime; //Add time to the time setback

        if (Input.GetKeyDown(KeyCode.Escape)) {
            if (menuOpen) //If the pause menu is open
                CloseMenu(0.5f); //Close it
        }
    }

    void OnEnable () {
        OnPause += PauseManager_OnPause;
        OnUnpause += PauseManager_OnUnpause;
    }

    private void PauseManager_OnPause() {
        isPaused = true;
    }

    private void PauseManager_OnUnpause() {
        isPaused = false;
    }

    void OnDisable () {
        OnPause -= PauseManager_OnPause;
        OnUnpause -= PauseManager_OnUnpause;
    }

	public override void OpenMenu (float duration) {
        menuOpen = true;
        base.OpenMenu(duration); //Open pause menu

        Pause(); //Pause game
    }

    public override void CloseMenu (float duration) {
        menuOpen = false;
        base.CloseMenu(duration); //Close pause menu

        if (ShopManager.Instance != null)
            if (ShopManager.Instance.ShopOpen == false) //If the shop isn't open
                UnPause(); //Unpause game
    }


    public void Pause () {
        OnPause();
    }

    public void UnPause () {
        OnUnpause();
    }

    public bool Paused {
        get {
            return isPaused;
        }
        set {
            if (value != isPaused) { //If the state will change the pause state
                isPaused = value; //Change the pause flag

                if (value == true) //If it's pausing
                    OnPause(); //Call pause event
                else //If not
                    OnUnpause(); //Call unpause event
            }
        }
    }

    
}
