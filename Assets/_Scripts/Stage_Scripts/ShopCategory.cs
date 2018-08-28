/*Alex Greff
19/01/2016
ShopCategory
The shop category
*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopCategory : MonoBehaviour {
    [SerializeField] private List<ShopItem> items = new List<ShopItem>(3);

    private const string TEXT_NAME = "Category Name";

    private ColorFader nameText;

    void Start () {
        nameText = transform.Find(TEXT_NAME).GetComponent<ColorFader>(); //Get the name text

        ShopItem[] temp = GetComponentsInChildren<ShopItem>(); //Get all shop items in the children

        nameText.FadeOut(0); //Hide the name text

        //Get the shop items
        items = new List<ShopItem>();

        foreach (ShopItem itm in temp)
            items.Add(itm);
    }

    public void Open (float duration) {
        nameText.FadeIn(duration); //Fade in text

        //Open each shop item
        foreach (ShopItem si in items)
            si.OpenMenu(duration);
    }

    public void Close (float duration) {
        nameText.FadeOut(duration); //Fade out text

        //Open each shop item
        foreach (ShopItem si in items) {
            si.CloseMenu(duration);
        }
    }

}