/*Alex Greff
19/01/2016
LoadingCanvas
Creates a loading screen when needed
*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadingCanvas : MonoBehaviour {
    private static LoadingCanvas instance;

    public static LoadingCanvas Instance {
        get {
            return instance;
        }
    }

    private const string BLOCKING_PANEL_NAME = "Blocking Panel";
    private const float INIT_FADE_DURATION = 0.5f;

    public Color[] colors;

    [SerializeField] private Image blockingPanel;

    [SerializeField] private ColorFader text;
    [SerializeField] private ColorFader image;


    void Awake () {
        if (instance == null) {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else {
            Destroy(gameObject);
        }

        //Get references to components
        blockingPanel = transform.Find(BLOCKING_PANEL_NAME).GetComponent<Image>(); ;

        text = transform.Find("Text").GetComponent<ColorFader>();

        image = transform.Find("Black Screen").GetComponent<ColorFader>();
        
    }

	void Start () {
        //Set loading screen off by default
        image.SetInitialColor(new Color(0, 0, 0, 0.8f));
        LoadEnd(0); 
	}

    public void LoadStart () {
        LoadStart(INIT_FADE_DURATION);
    }

    public void LoadStart (float fade_duration) { //Brings up the loading screen
        blockingPanel.raycastTarget = true;

        image.FadeIn(fade_duration);

        text.SetColorCycle(colors, 2f);
    }

    public void LoadEnd () { 
        LoadEnd(INIT_FADE_DURATION);
    }

    public void LoadEnd (float fade_duration) { //Removes the loading screen
        blockingPanel.raycastTarget = false;

        image.FadeOut(fade_duration);

        text.FadeOut(fade_duration);
    }
}
