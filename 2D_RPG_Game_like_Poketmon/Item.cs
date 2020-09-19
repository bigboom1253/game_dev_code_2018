using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Item
{
    public int itemID; // 아이템의 고유 ID값 중복 불가능.
    public string itemName; // 아이템의 이름. 중복가능.(여러개)
    public string itemDescription; //아이템 설명
    public int itemCount; //소지 개수
    public Sprite itemIcon; //아이템의 아이콘
    public ItemType itemType;
    public EquipType equipType;
    public int buyPrice;
    public int sellPrice;

    public enum EquipType
    {
        NONEQUIP,
        WEAPON,
        ARMOR,
        SHILED,
        ACCESSARY,
    }


    public enum ItemType
    {
        Use,
        Equip,
        Quest,
        ETC,

    }

    public int atk;
    public int def;
    public int avd; //회피율
    public int hit; //명중률
    public int str;
    public int dex;
    //등등 추가

    //TODO: 아이템 살때가격, 팔때 가격 추가
    public Item(int _itemID, string _itemName, string _itemDes,
        ItemType _itemType, int _buyPrice, int _sellPrice, EquipType _equipType = EquipType.NONEQUIP, int _atk = 0, int _def = 0, int _itemCount = 1)
    {
        itemID = _itemID;
        itemName = _itemName;
        itemDescription = _itemDes;
        itemType = _itemType;
        itemCount = _itemCount;
        equipType = _equipType;
        buyPrice = _buyPrice;
        sellPrice = _sellPrice;
        //itemIcon = Resources.Load("ItemIcon/" + _itemID.ToString(), typeof(Sprite)) as Sprite; 아이콘 이미지 불러오기.

        atk = _atk;
        def = _def;

    }


    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
