using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Equipment : MonoBehaviour
{

    private OrderManager theOrder;
    private AudioManager theAudio;
    private PlayerStat thePlayerStat;
    private OkOrCancel theOOC;
    private Inventory theInven;

    public string key_sound;
    public string enter_sound;
    public string open_sound;
    public string close_sound;
    public string takeoff_sound;

    private const int WEAPON = 0, ARMOR = 1, SHILED = 2, ACCESSARY = 3;

    private const int ATK = 4, DEF = 5; // 장비에 추가된 스탯을 나타내는 텍스트
    public int added_atk = 0, added_def = 0; //장비로 추가된 스탯. 로드,세이브 할 때 분리해줘야함.

    public GameObject go_OOC;
    public Text[] text; // 스탯
    public Text[] kindofEquipment;
    //public Image[] img_slots; // 장비 슬롯 아이콘.
    // public GameObject go_selected_Slot_UI; // 선택된 장비 슬롯 UI.

    public Item[] equipItemList; // 장착된 장비 리스트.

    private int selectedSlot; // 선택된 장비 슬롯.

    public bool activated = false;
    private bool inputKey = true;

    // Use this for initialization
    void Start()
    {

        theOrder = FindObjectOfType<OrderManager>();
        theAudio = FindObjectOfType<AudioManager>();
        thePlayerStat = FindObjectOfType<PlayerStat>();
        theOOC = FindObjectOfType<OkOrCancel>();
        theInven = FindObjectOfType<Inventory>();
    }

    public void ShowEquipmentNameText(int _kindofEquipment, Item _item) //0~3 무기, 갑옷, 방패, 장신구
    {
        kindofEquipment[_kindofEquipment].text = _item.itemName;
    }


    public void ShowAddedStatText()
    {

        text[ATK].text = thePlayerStat.atk.ToString() + "(+" + added_atk + ")";

        text[DEF].text = thePlayerStat.def.ToString() + "(+" + added_def + ")";
    }

    public void EquipItem(Item _item)
    {
        string temp = _item.itemID.ToString();
        temp = temp.Substring(0, 3); //앞에서 세글자
        switch (temp)
        {
            case "200": // 무기
                EquipItemCheck(WEAPON, _item);
                break;
            case "201": // 갑옷
                EquipItemCheck(ARMOR, _item);
                break;
            case "202": // 방패
                EquipItemCheck(SHILED, _item);
                break;
            case "203": // 장신구
                EquipItemCheck(ACCESSARY, _item);
                break;
        }
    }
    //equipItemList[selectedSlot] = new Item(0, "", "", Item.ItemType.Equip); 으로 빈칸 채우기
    public void EquipItemCheck(int _count, Item _item)//아이템 장착
    {
        if (equipItemList[_count].itemID == 0) //장착된 장비가 없다면, 아이템 장착
        {
            equipItemList[_count] = _item;


            //인벤토리에서 삭제

        }
        else //장착된 장비가 있다면, 장착아이템을 인벤토리에 넣고, 아이템 장착
        {
            theInven.EquipToInventory(equipItemList[_count]);
            TakeOffEffect(equipItemList[_count]);
            equipItemList[_count] = _item;
        }
        EquipEffect(_item); //장착효과 적용
        ShowAddedStatText(); //추가 스탯 텍스트
        ShowEquipmentNameText(_count, _item); //장착 아이템 이름 적용
    }

    private void EquipEffect(Item _item)
    {
        thePlayerStat.atk += _item.atk;
        thePlayerStat.def += _item.def;

        added_atk += _item.atk;
        added_def += _item.def;

    }

    private void TakeOffEffect(Item _item)
    {
        thePlayerStat.atk -= _item.atk;
        thePlayerStat.def -= _item.def;

        added_atk -= _item.atk;
        added_def -= _item.def;


    }

    //IEnumerator OOCCoroutine(string _ok, string _cancel)
    //{
    //    //go_OOC.SetActive(true);
    //    theOOC.ShowTwoChoice(_ok, _cancel, ok, cancel);
    //    //yield return new WaitUntil(() => !theOOC.activated);
    //    //if (theOOC.GetResult())
    //    //{

    //    //}

    //    //go_OOC.SetActive(false);
    //}
    
    /*
    void ok()
    {
        theInven.EquipToInventory(equipItemList[selectedSlot]);
        TakeOffEffect(equipItemList[selectedSlot]);
        ShowAddedStatText(); //추가 스탯 텍스트 변경
        equipItemList[selectedSlot] = new Item(0, "", "", Item.ItemType.Equip, 0, 0);
        theAudio.Play(takeoff_sound);
    }
    void cancel()
    {
        inputKey = true;
    }
    */
}

