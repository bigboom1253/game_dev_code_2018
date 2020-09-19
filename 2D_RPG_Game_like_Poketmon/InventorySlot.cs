using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour
{

    //public Image icon;
    public Text itemName_Text;
    public Text itemCount_Text;
    public Text itemPrice_Text;
    public bool sellListActivated;
    public GameObject selected_Item;
    private Shopping theShopping;

    private void Start()
    {
        theShopping = FindObjectOfType<Shopping>();
    }

    public void AddItem(Item _item)
    {
        itemName_Text.text = _item.itemName;

        if (itemPrice_Text != null)
        {
            if (sellListActivated)
                itemPrice_Text.text = _item.sellPrice.ToString();
            else
                itemPrice_Text.text = _item.buyPrice.ToString();
        }
        // icon.sprite = _item.itemIcon;
        if (Item.ItemType.Use == _item.itemType)
        {
            if (_item.itemCount > 0)
                itemCount_Text.text = "x " + _item.itemCount.ToString();
            else
                itemCount_Text.text = "";
        }
    }

    public void RemoveItem()
    {
        itemName_Text.text = "";
        itemCount_Text.text = "";
        //icon.sprite = null;

    }

}
