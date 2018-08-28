/*Alex Greff
19/01/2016
UIManager
Manages the player's HUD
*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour {
    [SerializeField] private Player player;
    [SerializeField] private SmoothSlider healthBar;
    [SerializeField] private Notification healthText;
    [SerializeField] private Notification moneyText;

    [SerializeField] private float updateTime = 0.3f;

    //Constant references to the name of component gameobjects
    private const string HEALTH_BAR_NAME = "Health Bar";
    private const string HEALTH_TEXT_NAME = "Health Text";
    private const string MONEY_TEXT_NAME = "Money Text";
    
    void Awake () {
        //If variables are left null then attempt to search for them automatically
        if (player == null)
            player = GameObject.FindObjectOfType<Player>();

        if (healthBar == null)
            healthBar = GameObject.Find(HEALTH_BAR_NAME).GetComponent<SmoothSlider>();

        if (healthText == null) 
            healthText = GameObject.Find(HEALTH_TEXT_NAME).GetComponent<Notification>();

        if (moneyText == null)
            moneyText = GameObject.Find(MONEY_TEXT_NAME).GetComponent<Notification>();
    }

	void Start () {
        //Initialize the texts
        healthText.gameObject.GetComponent<Text>().text = player.InitialHealth.ToString();
        moneyText.gameObject.GetComponent<Text>().text = "$ " + player.Money.ToString();
	}

    void OnEnable () {
        //Attach to events
        Player.OnHealthChange += UpdateHealth;
        Player.OnRespawn += UpdateHealth;
        Player.OnDeath += UpdateHealth;
        Player.OnMoneyChange += UpdateMoney;
    }

    private void UpdateHealth() { //Updates the player's health bar
        healthText.StartAnimation(player.Health.ToString());

        healthBar.StartAnimation(player.Health / player.InitialHealth, updateTime);
    }

    private void UpdateMoney () { //Update the money bar
        moneyText.StartAnimation("$ " + player.Money.ToString());
    }

    void OnDisable () {
        //Un-attach events
        Player.OnHealthChange -= UpdateHealth;
        Player.OnRespawn -= UpdateHealth;
        Player.OnDeath -= UpdateHealth;
        Player.OnMoneyChange -= UpdateMoney;
    }
}
