/*Alex Greff
19/01/2016
SceneLoadManager
Handles loading scenes
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoadManager : MonoBehaviour {
    private static SceneLoadManager instance;

    private static float duration = 1f; //The duration of the screen fade out

    private static bool loadingScene = false; //Loading scene flag

    //Getter and setters
    public static SceneLoadManager Instance {
        get {
            return instance;
        }
    }

    public static bool LoadingScene {
        get {
            return loadingScene;
        }
    }

    void Awake () {
        if (instance == null) {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else {
            Destroy(gameObject);
        }
    }

    public void ReloadScene () {
        LoadScene(SceneManager.GetActiveScene().name, duration);
    }

    public void SaveAndReloadScene () {
        SaveAndLoadScene(SceneManager.GetActiveScene().name, duration);
    }

	public void LoadScene (string name) {
        LoadScene(name, duration);
    }
    
    public void LoadScene (string name, float fade_duration) {
        if (PauseManager.Instance != null)
            if (PauseManager.Instance.Paused) //If paused
                PauseManager.Instance.CloseMenu(fade_duration / 1.5f); //Close the pause screen

        StartCoroutine(timer(name, fade_duration)); //Start the fadeout timer
    }

    private IEnumerator timer (string scene, float duration) {
        loadingScene = true;
        BlackScreenCanvas.Instance.FadeIn(duration); //Fade the black screen
        yield return new WaitForSeconds (duration); //Wait for the duration that it fades in

        loadingScene = false;
        NotificationManager.Instance.CancelAllNotifications(); //Cancel any notifications
        StopAllCoroutines(); //Stop any coroutines that are running (there shouldn't be any coroutines running between scenes)
        SceneManager.LoadScene(scene); //Load the scene
    }

    public void SaveAndLoadScene (string name) {
        SaveAndLoadScene(name, duration);
    }

    public void SaveAndLoadScene (string name, float fade_duration) {
        if (PauseManager.Instance != null)
            if (PauseManager.Instance.Paused) //If paused
                PauseManager.Instance.CloseMenu(fade_duration / 1.5f); //Close the pause screen

        StartCoroutine(timerWithSave(name, fade_duration));
    }

    private IEnumerator timerWithSave (string scene, float duration) {
        loadingScene = true;
        BlackScreenCanvas.Instance.FadeIn(duration); //Fade the black screen in
        yield return new WaitForSeconds (duration); //Wait

        bool usingCheats = false;
        if (ShopManager.Instance != null)
            usingCheats = ShopManager.Instance.isUsingCheats;

        //Save the game stats to Mr Computers
        if (MultiSceneVariables.Instance.Online && usingCheats == false) { //If the player is online and is not using cheats

            string key = "117TMHR@Con"; //The password for accessing the ftp login credentials
            string username = MultiSceneVariables.Instance.Username; //Get the username

            //File paths
            string DATA_FILE_PATH = "users/" + username + "_data.txt";
            string LOCAL_DATA_ROOT_PATH = Accessories.LOCAL_DATA_ROOT_PATH;
            string WEB_DATA_ROOT_PATH = Accessories.WEB_DATA_ROOT_PATH;

            if (Directory.Exists(LOCAL_DATA_ROOT_PATH + "users") == false) //Check if the user's folder exists
                Directory.CreateDirectory(LOCAL_DATA_ROOT_PATH + "users"); //If not, create it

            ftp ftpClient = new ftp(Accessories.FTP_ADDRESS(key), Accessories.FTP_USERNAME(key), Accessories.FTP_PASSWORD(key)); //Make instance of ftp class

            //Download the files
            CoroutineWithData cd = new CoroutineWithData(this, ftpClient.downloadAsync(WEB_DATA_ROOT_PATH + DATA_FILE_PATH, LOCAL_DATA_ROOT_PATH + DATA_FILE_PATH));

            yield return cd.coroutine; //Wait until it's completed

            if (cd.result.ToString().Equals(ftp.sucessMessage)) { //If the ftp file download succeeded
                if (File.Exists(LOCAL_DATA_ROOT_PATH + DATA_FILE_PATH)) { //If the file exists
                    DateTime localTime = DateTime.Now;

                    //Construct entry text
                    string entry = 
                        localTime.ToString("g") + "," + 
                        username + "," + 
                        GameManager.Score + "," +
                        GameManager.TimePlayed + "," + 
                        GameManager.WavesCompleted + "," + 
                        GameManager.EnemiesKilled + "," +
                        GameManager.HealthBought + "," + 
                        GameManager.TimesRevived + "\n";

                    File.AppendAllText(LOCAL_DATA_ROOT_PATH + DATA_FILE_PATH, entry); //Record entry locally to a current list of the user's games

                    //Upload the data file
                    CoroutineWithData cd2 = new CoroutineWithData(this, ftpClient.uploadAsync(WEB_DATA_ROOT_PATH + DATA_FILE_PATH, LOCAL_DATA_ROOT_PATH + DATA_FILE_PATH));
                }
            }
        }

        loadingScene = false;
        NotificationManager.Instance.CancelAllNotifications(); //Cancel any notifications
        StopAllCoroutines(); //Stop any coroutines that are running (there shouldn't be any coroutines running between scenes)
        SceneManager.LoadScene(scene); //Load the scene
    }
}
