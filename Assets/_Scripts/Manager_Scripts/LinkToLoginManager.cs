/*Alex Greff
19/01/2016
LinkToLoginManager
Provides a link for UI buttons to access static classes
*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LinkToLoginManager : MonoBehaviour {
    public void Logout () {
        LoginManager.Instance.Logout();
    }

    public void DisplayCheckUsername(string username) {
        LoginManager.Instance.DisplayCheckUsername(username);
    }

    public void SubmitCredentials () {
        LoginManager.Instance.SubmitCredentials();
    }

    public void PlayOffline () {
        LoginManager.Instance.PlayOffline();
    }

    public void Register () {
        LoginManager.Instance.Register();
    }
}
