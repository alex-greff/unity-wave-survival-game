/*Alex Greff
19/01/2016
ShopItem
A shop item that the player can purchase
*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public enum ShopItemType {
    WEAPON, HEALTH
}   

public class ShopItem : MenuGUIManager {
    public bool defaultWeapon = false;

    [SerializeField] private ShopItemType type;

    [SerializeField] private GameObject weaponPrefab;

    [SerializeField] private int healAmount = 0;


    [SerializeField] private int cost = 0;

    private CustomButton button;

    private bool isBought = false;

    private bool isActive = false;

    private const string DESCRIPTION_TEXT_NAME = "Item Description";
    private const string TITLE_TEXT_NAME = "Item Name";

    private Text descriptionText;
    private Text titleText;

    //Getters and setters
    public ShopItemType Type {
        get {
            return type;
        }
    }

    public int Cost {
        get {
            return cost;
        }
    }

    public object Reward { //Returns the award
        get {
            if (type == ShopItemType.WEAPON)
                return weaponPrefab;
            else if (type == ShopItemType.HEALTH)
                return healAmount;

            return null;
        }
    }

    public bool Bought { //Returns if it's been bought already
        get {
            return isBought;
        }
        set {
            isBought = value;
        }
    }

    public bool Active { //Returns if the item is even active at the moment
        get {
            return isActive;
        }
        set {
            isActive = value;
        }
    }

    protected override void Awake() {
        base.Awake();

        descriptionText = transform.Find(DESCRIPTION_TEXT_NAME).GetComponent<Text>();
        titleText = transform.Find(TITLE_TEXT_NAME).GetComponent<Text>();
    }

    protected override void Start() {
        base.Start();

        button = GetComponentInChildren<CustomButton>(); //Get the buy button

        if (defaultWeapon) { //If is the default weapon
            isBought = true;
            isActive = true;

            if (type == ShopItemType.WEAPON) { //If it's a weapon
                ShopManager.Instance.CurrentWeapon = this; //Set the last weapon to this, the default weapon
                Player.Instance.changeWeapon(weaponPrefab); //Set the player's initial weapon to this
            }
        }

        //Construct description text
        string txt = "";
        if (type == ShopItemType.WEAPON) {
            Weapon wp = weaponPrefab.GetComponent<Weapon>();

            txt += "Weapon type: ";
            if (wp is Lazer)
                txt += "Lazer\n";
            else if (wp is Pellet) 
                txt += "Projectile\n";

            txt +=  "\nDamage: " + wp.DamageAmount;
            txt += "\nCooldown: " + wp.Cooldown;
            txt += "\nRange: " + wp.Range;

            if (wp is Lazer) {
                Lazer lz = (Lazer)wp;
                txt += "\nDuration: " + lz.Duration;
            }
            else if (wp is Pellet) {
                Pellet pl = (Pellet)wp;
                txt += "\nSpeed: " + pl.Speed;
            }
        }
        else if (type == ShopItemType.HEALTH) {
            txt = "Heals " + healAmount + " health";
        }

        descriptionText.text = txt;

        UpdateItem(); //Update the item GUI
    }

    public override void CloseMenu(float duration) {
        base.CloseMenu(duration);
    }

    public override void OpenMenu(float duration) {
        base.OpenMenu(duration);
    }

    public void UpdateItem () { //Updates the GUI of the item
        if (Active) { //If its active
            button.SetText("Active"); 
            button.SetInteractable(false);
            ignoredButton = button; //Setup the ignored button
        }
        else { //If not
            if (Bought) //If its been bought already 
                button.SetText("Use"); 
            else //If not
                button.SetText("$" + cost);
            
            button.SetInteractable(true);
            ignoredButton = null; //Set the ignored button in the base class to null
        }
    }
}