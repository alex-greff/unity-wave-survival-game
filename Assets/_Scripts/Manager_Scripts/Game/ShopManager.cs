/*Alex Greff
19/01/2016
ShopManager
Handles the player buying new items
*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopManager : MenuGUIManager {
    private static ShopManager instance = null;

    public static ShopManager Instance {
        get {
            return instance;
        }
    }

    Player p;

    [SerializeField] private bool useCheats = false;

    [SerializeField] private Notification moneyText;
    [SerializeField] private Notification healthText;
    [SerializeField] private List<ShopCategory> categories = new List<ShopCategory>();

    private ShopItem[] shopItems;

    private int currentIndex = 0;

    private bool shopOpen = false;
    private bool shopOpening = false;

    private ShopItem currentWeaponItem;

    private Notification shopNotification;

    private const string SHOP_NOTIFICATION_NAME = "Shop Notification";

    public bool isUsingCheats {
        get {
            return useCheats;
        }
    }

    public ShopItem CurrentWeapon {
        get {
            return currentWeaponItem;
        }
        set {
            currentWeaponItem = value;
        }
    }

    public bool ShopOpen {
        get {
            return shopOpen;
        }
    }

    public bool ShopOpening {
        get {
            return shopOpening;
        }
        set {
            shopOpening = value;
        }
    }

    protected override void Awake () {
        base.Awake();

        instance = this;
    }

    protected override void Start() {
        base.Start();

        p = Player.Instance;

        categories.AddRange(GetComponentsInChildren<ShopCategory>()); //Get all the shop categories

        foreach (ShopCategory sc in categories)
            sc.Close(0); //Close all the shop categories

        shopItems = GetComponentsInChildren<ShopItem>(); //Get all the shop items

        shopNotification = transform.Find(SHOP_NOTIFICATION_NAME).GetComponent<Notification>(); //Find the local shop notification
    }

    void Update () {
        if (Input.GetKeyDown(KeyCode.Escape)) {
            if (ShopOpen) //If the shop is open
                CloseMenu(0.5f); //Close it
        }
    }

    public override void OpenMenu(float duration) {
        base.OpenMenu(duration); //Open shop menu

        shopOpen = true;

        PauseManager.Instance.Pause(); //Pause the game

        LoadCategory(currentIndex); //Load the current category

        //Update text
        UpdateMoneyText(); 
        UpdateHealthText();
    }

    public override void CloseMenu(float duration) {
        base.CloseMenu(duration); //Close shop menu

        shopOpen = false;

        PauseManager.Instance.UnPause(); //Unpause game

        ShopBlock.instance.CloseMenu(); //Close menu on the shopblock

        foreach (ShopCategory sc in categories)
            sc.Close(duration); //Close the shopcategories
    }

    public void NextOne () {
        IncrementOne(1);
    }

    public void BackOne () {
        IncrementOne(-1);
    }

    public void IncrementOne (int direction) { //Increment/decrements one to the current index 
        direction = (int) Mathf.Sign(direction);

        currentIndex += direction;
        if (currentIndex >= categories.Count)
            currentIndex = 0;
        else if (currentIndex < 0) {
            currentIndex = categories.Count - 1;
        }

        LoadCategory(currentIndex);
    }

    private void LoadCategory (int index) { //Loads a category up
        for (int i = 0; i < categories.Count; i++) {
            if (i == index)
                categories[i].Open(0.3f);
            else 
                categories[i].Close(0.3f);
        }
    }

    public void UpdateMoneyText () { 
        moneyText.StartAnimation("$" + Player.Instance.Money);
    }

    public void UpdateHealthText () {
        healthText.StartAnimation(Player.Instance.Health + "");
    }


    //Purchase methods

    public void BuyOrEnableWeapon (ShopItem item) {
        Player p = Player.Instance;

        int cost = item.Cost;

        if (item.Active) return;

        if (item.Bought) { //If the item is bought
            if (item.Type == ShopItemType.WEAPON) { //If the shop item was a weapon
                //Activate the weapon
                item.Active = true; 

                p.changeWeapon(item.Reward as GameObject); //Change the player's weapon

                currentWeaponItem.Active = false; //Disable the old current weapon
                currentWeaponItem.UpdateItem();
                currentWeaponItem = item; //Set the current weapon to the new one
                currentWeaponItem.UpdateItem();

                shopNotification.StartAnimation("Item equipped!", 2f, Color.cyan);
            }
        }
        else {
            if (canBuy(cost) || useCheats == true) { //If the item can be bought
                if (item.Type == ShopItemType.WEAPON) { //If the shop item was a weapon
                    if (!useCheats) {
                        p.decreaseMoney(cost); //Remove money
                        UpdateMoneyText(); //Update the money text
                    }

                    item.Bought = true;
                    item.Active = true;

                    //Activate and equip the weapon
                    p.changeWeapon(item.Reward as GameObject); //Change the player's weapon

                    currentWeaponItem.Active = false;
                    currentWeaponItem.UpdateItem();
                    currentWeaponItem = item;
                    currentWeaponItem.UpdateItem();

                    shopNotification.StartAnimation("Item bought!", 2f, Color.green);
                }
                else if (item.Type == ShopItemType.HEALTH) {
                    int healAmt = (int)item.Reward;

                    if (canHeal(healAmt) == false) //If the player's health would go over the max health
                        shopNotification.StartAnimation("Unable to buy health", 2f, Color.red);
                    else {
                        if (!useCheats) {
                            p.decreaseMoney(cost); //Remove money
                            UpdateMoneyText(); //Update the money text
                        }

                        p.heal(healAmt); //Add health to the player
                        shopNotification.StartAnimation("Health bought!", 2f, Color.green);
                        GameManager.healthPurchased(healAmt); //Update heal amount tracker
                        UpdateHealthText();
                    }
                }

                
            }
            else
                shopNotification.StartAnimation("Unable to buy item", 3f, Color.red);
        }
    }

    private bool canBuy (int cost) { //Checks if the player can buy item
        if ((p.Money - cost) >= 0)
            return true;

        return false;
    }

    private bool canHeal (int amount) {
        if ((p.Health + amount) > p.MaxHealth)
            return false;

        return true;
    }
}
