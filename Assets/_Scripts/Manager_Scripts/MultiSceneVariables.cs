/*Alex Greff
19/01/2016
MutliSceneVariables
Provides a place for variables to be preserved between scenes
*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultiSceneVariables : MonoBehaviour {
    private static MultiSceneVariables instance;

    

    [SerializeField] [ShowOnly] private bool online = false;

    [SerializeField] [ShowOnly] private string username = null;

    //Getters and setters
    public static MultiSceneVariables Instance {
        get {
            return instance;
        }
    }

    public bool Online {
        get {
            return online;
        }
        set {
            online = value;
        }
    }

    public string Username {
        get {
            return username;
        }
        set {
            username = value;
        }
    }
    
	void Awake () {
        if (instance == null) { //If there is no instance already
            instance = this; //Claim instance
            DontDestroyOnLoad(gameObject); 
        }
        else {
            Destroy(gameObject); //Just destroy the gameobject
        }
    }
}
