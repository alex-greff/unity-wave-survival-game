/*Alex Greff
19/01/2016
LinkToSceneManager
Provides a link for UI buttons to access static classes
*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LinkToSceneManager : MonoBehaviour {
    public void LoadScene (string name) {
        SceneLoadManager.Instance.LoadScene(name);
    }
    
    public void LoadScene (string name, float fade_duration) {
        SceneLoadManager.Instance.LoadScene(name, fade_duration);
    }

    public void ReloadScene () {
        SceneLoadManager.Instance.ReloadScene();
    }

    public void SaveAndReloadScene () {
        SceneLoadManager.Instance.SaveAndReloadScene();
    }

    public void SaveAndLoadScene (string name) {
        SceneLoadManager.Instance.SaveAndLoadScene(name);
    }
    public void SaveAndLoadScene (string name, float fade_duration) {
        SceneLoadManager.Instance.SaveAndLoadScene(name, fade_duration);
    }
    public void Exit () {
        Application.Quit();
    }
}
