/*Alex Greff
19/01/2016
ToggleCanvas
Used to enable and disable canvases
*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ToggleCanvas : MonoBehaviour {
    [SerializeField] private bool DisableOnStart = false;

    private GameObject[] children;

    private Selectable[] selectables;

	protected virtual void Start () {
        //Get all selectables
        selectables = GetComponentsInChildren<Selectable>();

        children = new GameObject[transform.childCount];
        for (int i = 0; i < transform.childCount; i++) {
            children[i] = transform.GetChild(i).gameObject;
        }
             

        if (DisableOnStart)
            DisableCanvas();
	}

    //Enable / disable all interactable UI components
	public void EnableCanvas () {
        foreach (Selectable s in selectables) 
            s.interactable = true;
    }

    public void DisableCanvas () {
        foreach (Selectable s in selectables) 
            s.interactable = false;
    }

    //Show / hide all gameobjects in the canvas
    public void ShowCanvas () {
        foreach (GameObject go in children)
            go.SetActive(true);
    }

    public void HideCanvas () {
        foreach (GameObject go in children)
            go.SetActive(false);
    }
}
