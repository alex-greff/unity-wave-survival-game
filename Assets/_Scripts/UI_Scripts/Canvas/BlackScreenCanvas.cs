/*Alex Greff
19/01/2016
BlackSceenCanvas
Used to fade the screen out to black
*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class BlackScreenCanvas : MonoBehaviour {
    private static BlackScreenCanvas instance;

    public static BlackScreenCanvas Instance {
        get {
            return instance;
        }
    }

    public static bool fadeOnStart = true;

    private ColorFader blackBackground;

    void Awake () {
        if (instance == null) {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else {
            Destroy(gameObject);
        }

        //Get reference to black background
        blackBackground = GetComponentInChildren<ColorFader>();
    }

    void OnEnable () {
        //Attach events
        SceneManager.sceneLoaded += OnLevelFinishedLoading;
    }

    void OnDisable () {
        //Un-attach events
        SceneManager.sceneLoaded -= OnLevelFinishedLoading;
    }

    void OnLevelFinishedLoading (Scene scene, LoadSceneMode mode) {
        if (fadeOnStart) { //If it's fading on start
            blackBackground.SetColor(Color.black, 0); //Set screen to black

            FadeOut(2f); //Fade it out (fade in the screen)
        }
        else { //If not...
            blackBackground.SetColor(Color.clear, 0); //Just default it to clear
        }
    }

    public void FadeIn(float duration) {
        blackBackground.FadeIn(duration); //Start fade in
    }

    public void FadeOut(float duration) {
        blackBackground.FadeOut(duration); //Start fade out
    }
}
