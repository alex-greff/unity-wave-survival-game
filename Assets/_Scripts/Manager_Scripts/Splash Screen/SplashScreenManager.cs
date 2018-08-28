using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SplashScreenManager : MonoBehaviour {
    public float duration = 5f;
    private const string NEXT_SCENE_NAME_SCHOOL_VERSION = "login";
    private const string NEXT_SCENE_NAME_CONSUMER_VERSION = "menu";

    public string nextLevel = "login";

	void Start () {
        StartCoroutine(nextSceneTimer(duration));
        StartCoroutine(companyTextTimer());
	}
	
    private IEnumerator companyTextTimer () {
        yield return new WaitForSeconds(1f);
        //NotificationManager.instance.ShowNotification(NotificationType.LARGE, "<Company Name>", 3f, Color.magenta);
    }

	private IEnumerator nextSceneTimer (float duration) {

        yield return new WaitForSeconds(duration);

        SceneLoadManager.Instance.LoadScene(nextLevel);

/*#if UNITY_EDITOR || UNITY_STANDALONE

        SceneLoadManager.Instance.LoadScene(NEXT_SCENE_NAME_SCHOOL_VERSION);

#else //If it's probably not something that supports c# ftp (android / iOS) then just skip the login screen and load the menu

        SceneLoadManager.Instance.LoadScene(NEXT_SCENE_NAME_CONSUMER_VERSION);

#endif*/
    }
}
