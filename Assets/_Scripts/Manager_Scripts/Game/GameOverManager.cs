/*Alex Greff
19/01/2016
GameOverManager
Handles the game over screen
*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameOverManager : MenuGUIManager {
    private static GameOverManager instance;
    public static GameOverManager Instance {
        get {
            return instance;
        }
    }

    private bool gameOver = false;

    public delegate void GameOverEvent();
    public static event GameOverEvent OnGameOver;

    private const string SCORE_TEXT_NAME = "Score Text";
    private Text scoreText;

    public bool GameOver {
        get {
            return gameOver;
        }
        set {
            gameOver = value;
        }
    }

    protected override void Awake() {
        base.Awake();
        instance = this;
    }

    protected override void Start() {
        base.Start();

        scoreText = transform.Find(SCORE_TEXT_NAME).GetComponent<Text>(); //Get score text
    }

    void OnEnable () {
        OnGameOver += GameOverManager_OnGameOver;
        Player.OnDeath += Player_OnDeath;
    }

    private void Player_OnDeath() {
        StartCoroutine(deathDelay());
    }

    private IEnumerator deathDelay () { //The delay before the death scren is opened
        gameOver = true;
        NotificationManager.Instance.ShowNotification(NotificationType.LARGE, "You died!", 2, Color.red);
        yield return new WaitForSeconds(2);
        OpenMenu(0.5f);
    }

    private void GameOverManager_OnGameOver() {
        //When the game is over
    }

    void OnDisable () {
        OnGameOver -= GameOverManager_OnGameOver;
        Player.OnDeath -= Player_OnDeath;
    }

    public override void OpenMenu(float duration) {
        base.OpenMenu(duration); //Open menu

        scoreText.text = "Score: " + GameManager.Score; //Update score text

        gameOver = true; //Set game over to true

        OnGameOver(); //Call game over event

        if (PauseManager.Instance)
            PauseManager.Instance.Pause(); //Freeze time
    }

    public override void CloseMenu(float duration) {
        base.CloseMenu(duration); //Close menu

        if (PauseManager.Instance)
            PauseManager.Instance.UnPause(); //Unfreeze time
    }
}
