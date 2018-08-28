using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class LoginManager : MonoBehaviour {
    private static LoginManager instance;

    public static LoginManager Instance {
        get {
            return instance;
        }
    }

    private InputField usernameField;
    private InputField passwordField;
    private Text usernameStatus;
    private LoadingCanvas lc;

    private string LAST_USERNAME = "";
    private string LAST_PASSWORD = "";

    private const string key = "117TMHR@Con";

    private string LOCAL_DATA_ROOT_PATH;
    private string REGISTERED_USERS_FILE_PATH;
    private string WEB_DATA_ROOT_PATH;
    private string CURRENT_CREDENTIALS_FILE_PATH;
    private string REGISTER_URL_LINK;

    private Dictionary<string, string> users = new Dictionary<string, string>();
    private Dictionary<string, string> originalUsernames = new Dictionary<string, string>();

    private Selectable canvasSelectables;

    void Awake () {
        if (instance == null) {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else {
            Destroy(gameObject);
        }
    }

	void Start () { 
	}

    void OnEnable () {
        SceneManager.sceneLoaded += OnLevelFinishedLoading;
    }

    void OnDisable () {
        SceneManager.sceneLoaded -= OnLevelFinishedLoading;
    }

    void OnLevelFinishedLoading (Scene scene, LoadSceneMode mode) {
        if (scene.name == "login") {

            lc = GameObject.FindObjectOfType<LoadingCanvas>(); //Find the loading canvas

            usernameField = GameObject.Find("Username Input").GetComponent<InputField>();
            passwordField = GameObject.Find("Password Input").GetComponent<InputField>();
            usernameStatus = GameObject.Find("Username Status").GetComponent<Text>();

            //LOCAL_DATA_ROOT_PATH = Application.persistentDataPath.ToString() + "/_data/";
            LOCAL_DATA_ROOT_PATH = Accessories.LOCAL_DATA_ROOT_PATH;
            REGISTERED_USERS_FILE_PATH = Accessories.REGISTERED_USERS_FILE_PATH;
            WEB_DATA_ROOT_PATH = Accessories.WEB_DATA_ROOT_PATH;
            CURRENT_CREDENTIALS_FILE_PATH = Accessories.CURRENT_CREDENTIALS_FILE_PATH;
            REGISTER_URL_LINK = Accessories.REGISTER_URL_LINK;

            StartCoroutine(initialize()); //Attempt to download a list of the registered users and write it to a local file        
        }
    }
    

    private IEnumerator initialize () {
        ftp ftpClient = new ftp(Accessories.FTP_ADDRESS(key), Accessories.FTP_USERNAME(key), Accessories.FTP_PASSWORD(key));

        if (Directory.Exists(LOCAL_DATA_ROOT_PATH) == false) { //If the data folder doesn't exist 
            Directory.CreateDirectory(LOCAL_DATA_ROOT_PATH); //Create it
        }

        if (File.Exists(LOCAL_DATA_ROOT_PATH + REGISTERED_USERS_FILE_PATH)) { //If the registered users file is still there
            File.Delete(LOCAL_DATA_ROOT_PATH + REGISTERED_USERS_FILE_PATH); //Remove it
        }

        //Download the files
        CoroutineWithData cd = new CoroutineWithData(this, ftpClient.downloadAsync(WEB_DATA_ROOT_PATH + REGISTERED_USERS_FILE_PATH, LOCAL_DATA_ROOT_PATH + REGISTERED_USERS_FILE_PATH));

        yield return cd.coroutine;

        //Once everything has been downloaded (or not)

        if (cd.result.ToString().Equals(ftp.sucessMessage)) { //If the ftp file download succeeded

            SplitRegisteredUserFile(); //Attempt to 

            CheckForCurrentCredentials();

            MultiSceneVariables.Instance.Online = true;

            LoginCanvasManager.current.EnableCanvas(); //Disable the canvas
            //OfflineCanvasManager.current.DisableCanvas(); //Enable the offline option
        }
        else if (cd.result.ToString().Equals(ftp.failMessage)) { //If the ftp file download failed
            print("Running fail");

            NotificationManager.Instance.ShowNotification(NotificationType.TOP, "Unable to Connect to server", Mathf.Infinity, Color.red);

            MultiSceneVariables.Instance.Online = false;

            LoginCanvasManager.current.DisableCanvas(); //Disable the canvas
            //OfflineCanvasManager.current.EnableCanvas(); //Enable the offline option
        }
    }

    private void SplitRegisteredUserFile () {
        string[] lines = File.ReadAllLines(LOCAL_DATA_ROOT_PATH + "registeredUsers.txt"); //Get all lines in the file

        if (users.Count > 0) //If the dictionary is not empty
            users.Clear(); //Clear it

        if (originalUsernames.Count > 0)
            originalUsernames.Clear();

        if (lines != null) {
            foreach(string l in lines) { //Iterate through each line in the file
                //print(l);
                string[] split = l.Split("~".ToCharArray()); //Split it into username and password

                users.Add(split[0].ToLower(), split[1]); //Add to dictionary
                originalUsernames.Add(split[0].ToLower(), split[0]);
                //print("Username:" + split[0].ToLower() + "-Password:" + split[1]); 
            }
        }
    }
	
    private void CheckForCurrentCredentials () {
        if (File.Exists(LOCAL_DATA_ROOT_PATH + "currentCredentials.txt") == false) return; //If the file doesn't exist

        string contents = File.ReadAllText(LOCAL_DATA_ROOT_PATH + "currentCredentials.txt");
        //print(contents);
        if (contents.Equals("") == false && contents != null) { //If the file exists and contents are not empty
            string[] split = contents.Split("~".ToCharArray()); //Split it into username and password

            string username = split[0];
            string password = split[1];

            if (CheckCredentials(username, password)) { //If everything checks out
                //SceneManager.LoadScene("menu", LoadSceneMode.Single);

                //SceneLoadManager.instance.LoadScene("menu"); //Load the menu scene
                StartCoroutine(exitLoginScreen("menu", 1, username, password));
            }
        }
    }

    private bool CheckCredentials (string username, string password) { //Check if 
        string p = "";

        if (users == null) return false; //If there aren't any usernames in the dictionary

        if (users.TryGetValue(username.ToLower(), out p)) {
            if (password.Equals(p)) {
                
                return true;
            }
        }

        return false;
    }

    private IEnumerator exitLoginScreen (string scene, float duration, string username, string password) {
        BlackScreenCanvas.Instance.FadeIn(duration);
        yield return new WaitForSeconds (duration);

        NotificationManager.Instance.CancelAllNotifications();

        if (MultiSceneVariables.Instance.Online == true) { //If loading the game in online mode
            MultiSceneVariables.Instance.Username = username; //Store the username
            SetCurrentCredentials(username, password); 
        }
        else { //If loading the game in offline mode
            //TODO: some offline specific stuff here
        }

        if (File.Exists(LOCAL_DATA_ROOT_PATH + REGISTERED_USERS_FILE_PATH) == true) { //If the registered users file is still around
            File.Delete(LOCAL_DATA_ROOT_PATH + REGISTERED_USERS_FILE_PATH); //Delete the file
        }

        StopAllCoroutines();
        SceneManager.LoadScene(scene);
    }

    private IEnumerator exitAndLogout (string scene, float duration) {
        BlackScreenCanvas.Instance.FadeIn(duration);
        yield return new WaitForSeconds (duration);

        NotificationManager.Instance.CancelAllNotifications();

        if (File.Exists(LOCAL_DATA_ROOT_PATH + REGISTERED_USERS_FILE_PATH) == true) { //If the registered users file is still around
            File.Delete(LOCAL_DATA_ROOT_PATH + REGISTERED_USERS_FILE_PATH); //Delete the file
        }

        if (File.Exists(LOCAL_DATA_ROOT_PATH + CURRENT_CREDENTIALS_FILE_PATH) == true) { //If the credentials file exists
            File.Delete(LOCAL_DATA_ROOT_PATH + CURRENT_CREDENTIALS_FILE_PATH); //Delete the file
        }

        SceneManager.LoadScene(scene);
    }

    private void SetCurrentCredentials (string username, string password) {
        string u = "";
        originalUsernames.TryGetValue(username.ToLower(), out u); //Get the original username (case sensitive)

        File.WriteAllText(LOCAL_DATA_ROOT_PATH + "currentCredentials.txt", u + "~" + password);
    }

    //USER METHODS
    public void DisplayCheckUsername (string username) {

        if (users.ContainsKey(username.ToLower())) {
            //print("Valid Username");
            usernameStatus.color = Color.green;
            usernameStatus.text = "Valid Username";
        }
        else {
            //print("Invalid Username");
            usernameStatus.color = Color.red;
            usernameStatus.text = "Invalid Username";
        }
    }

    public void SubmitCredentials () { //Called when player presses the submit button
        string username = usernameField.text;
        string password = passwordField.text;

        if (CheckCredentials(username, password)) {
            //NotificationManager.instance.ShowNotification(NotificationType.LARGE, "Correct Credentials", 5, Color.green);
            //SceneManager.LoadScene("menu", LoadSceneMode.Single);

            //SceneLoadManager.instance.LoadScene("menu"); //Load the menu scene
            StartCoroutine(exitLoginScreen("menu", 1, username, password));
        }
        else {
            NotificationManager.Instance.ShowNotification(NotificationType.LARGE, "Incorrect Credentials", 5, Color.red);
        }
    }

    public void PlayOffline () {
        StartCoroutine(exitLoginScreen("menu", 1, "", ""));
    }

    public void Register () {
        Application.OpenURL(REGISTER_URL_LINK);
    }

    public void Logout () {
        StartCoroutine(exitAndLogout("login", 1));
    }
}
