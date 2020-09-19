using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Inventory : MonoBehaviour
{
    public static Inventory instance;

    private Menu theMenu;
    private DatabaseManager theDatabase;
    private OrderManager theOrder;
    private AudioManager theAudio;
    private OkOrCancel theOOC;
    private Equipment theEquip;

    public string key_sound;
    public string enter_sound;
    public string cancel_sound;
    public string open_sound;
    public string beep_sound;

    public InventorySlot[] slots; // 인벤도리 슬롯들 

    public List<Item> inventoryItemList; //플레이어가 소지한 아이템 리스트.
    public List<Item> inventoryTabList;

    public Text description_Text; //부연설명
    public string[] tabDescription; //탭 부연설명

    public Transform tf; //슬롯의 부모객체

    public GameObject go; //인벤토리 활성화 비활성화
    public GameObject[] selectedTabImages;
    public GameObject go_OOC; //OOC 활성화 비활성화
    public GameObject prefab_Floating_Text;
    public GameObject go_Gold_Text;

    public int selectedItem; //선택된 아이템 패널
    public int selectedTab; //선택된 탭
    public int selectedItemIndex; // 실제로 선택된 아이템을 찾아주는 인덱스

    public int gold;
    public int page;//페이지
    public int slotCount;//활성화된 슬롯의 개수
    private const int MAX_SLOTS_COUNT = 10; //최대 슬롯개수

    public bool activated; //인벤토리 활성화시 true;
    public bool tabActivated; //탭 활성화시 true;
    public bool itemActivated; //아이템 활성화시 true;
    public bool stopKeyInput; //키입력 제한(소비할 때 확인 문장때, 키입력 방지)
    public bool preventExec; //중복실행 제한.

    private WaitForSeconds waitTime = new WaitForSeconds(0.01f);


    // Start is called before the first frame update
    void Start()
    {
        instance = this;
        theMenu = FindObjectOfType<Menu>();
        theAudio = FindObjectOfType<AudioManager>();
        theOrder = FindObjectOfType<OrderManager>();
        theDatabase = FindObjectOfType<DatabaseManager>();
        theOOC = FindObjectOfType<OkOrCancel>();
        theEquip = FindObjectOfType<Equipment>();

        inventoryItemList = new List<Item>();
        inventoryTabList = new List<Item>();
        slots = tf.GetComponentsInChildren<InventorySlot>(); //각각 인벤토리 슬롯의 아이템 정보 삽입
        //테스트용
        inventoryItemList.Add(new Item(10001, "빨간 포션", "체력을 50 회복한다.", Item.ItemType.Use, 50, 10));
        inventoryItemList.Add(new Item(10002, "파란 포션", "마력을 50 회복한다.", Item.ItemType.Use, 50, 10));

        inventoryItemList.Add(new Item(20001, "소형검", "공격력을 5 증가시킨다.", Item.ItemType.Equip, 200, 40, Item.EquipType.WEAPON, 5));
        inventoryItemList.Add(new Item(20002, "대형검", "공격력을 10 증가시킨다.", Item.ItemType.Equip, 300, 40, Item.EquipType.WEAPON, 10));
        inventoryItemList.Add(new Item(20003, "소나기검", "공격력을 100 증가시킨다.", Item.ItemType.Equip, 400, 40, Item.EquipType.WEAPON, 100));
        inventoryItemList.Add(new Item(20003, "철검", "공격력을 20 증가시킨다.", Item.ItemType.Equip, 200, 40, Item.EquipType.WEAPON, 20));
        inventoryItemList.Add(new Item(20001, "소형검", "공격력을 5 증가시킨다.", Item.ItemType.Equip, 200, 40, Item.EquipType.WEAPON, 5));
        inventoryItemList.Add(new Item(20002, "대형검", "공격력을 10 증가시킨다.", Item.ItemType.Equip, 200, 40, Item.EquipType.WEAPON, 10));
        inventoryItemList.Add(new Item(20003, "소나기검", "공격력을 100 증가시킨다.", Item.ItemType.Equip, 200, 40, Item.EquipType.WEAPON, 100));
        inventoryItemList.Add(new Item(20003, "철검", "공격력을 20 증가시킨다.", Item.ItemType.Equip, 200, 40, Item.EquipType.WEAPON, 20));


        inventoryItemList.Add(new Item(20101, "천옷", "방어력을 5 증가시킨다.", Item.ItemType.Equip, 200, 40, Item.EquipType.ARMOR, 0, 5));
        inventoryItemList.Add(new Item(20102, "탐험가의 옷", "방어력을 10 증가시킨다.", Item.ItemType.Equip, 200, 40, Item.EquipType.ARMOR, 0, 10));
        inventoryItemList.Add(new Item(20103, "철 갑옷", "방어력을 30 증가시킨다.", Item.ItemType.Equip, 200, 40, Item.EquipType.ARMOR, 0, 30));
        inventoryItemList.Add(new Item(20103, "황금 갑옷", "방어력을 50 증가시킨다.", Item.ItemType.Equip, 200, 40, Item.EquipType.ARMOR, 0, 50));

        inventoryItemList.Add(new Item(20201, "청동방패", "방어력을 5 증가시킨다.", Item.ItemType.Equip, 200, 40, Item.EquipType.SHILED, 0, 5));
        inventoryItemList.Add(new Item(20202, "철방패", "방어력을 10 증가시킨다.", Item.ItemType.Equip, 200, 40, Item.EquipType.SHILED, 0, 10));

        inventoryItemList.Add(new Item(20303, "재해의부적", "방어력을 5 증가시킨다.", Item.ItemType.Equip, 200, 40, Item.EquipType.ACCESSARY, 0, 5));
        inventoryItemList.Add(new Item(20304, "오프너의반지", "공격력을 10 증가시킨다.", Item.ItemType.Equip, 200, 40, Item.EquipType.ACCESSARY, 10));

        gold = 1000;

    }
    // Update is called once per frame
    void Update()
    {
        ShowGold();

        if (activated)
        {
            if (tabActivated)
            {
                if (Input.GetKeyDown(KeyCode.RightArrow))
                {
                    if (selectedTab < selectedTabImages.Length - 1)
                        selectedTab++;
                    else
                        selectedTab = 0;
                    theAudio.Play(key_sound);
                    SelectedTab();
                }
                else if (Input.GetKeyDown(KeyCode.LeftArrow))
                {
                    if (selectedTab > 0)
                        selectedTab--;
                    else
                        selectedTab = selectedTabImages.Length - 1;
                    theAudio.Play(key_sound);
                    SelectedTab();
                }

                //TODO:FIX 중복 실행 방지
                else if (Input.GetKeyDown(KeyCode.Z) && !theMenu.preventExec)
                {
                    theAudio.Play(enter_sound);
                    Color color = selectedTabImages[selectedTab].GetComponent<Image>().color;
                    color.a = 0.25f;
                    selectedTabImages[selectedTab].GetComponent<Image>().color = color;
                    itemActivated = true;
                    tabActivated = false;
                    preventExec = true;
                    ShowItem();
                }
                else if (Input.GetKeyDown(KeyCode.Escape))
                {
                    activated = !activated;
                    theAudio.Play(cancel_sound);
                    StopAllCoroutines();
                    go.SetActive(false);
                    tabActivated = false;
                    itemActivated = false;
                    theMenu.menuActivated = true;

                }
                if (Input.GetKeyUp(KeyCode.Z)) //중복 실행 방지
                    theMenu.preventExec = false;
            } //탭 활성화시 키입력 처리

            else if (!theOOC.activated && itemActivated && !tabActivated)
            {
                if (inventoryTabList.Count > 0)
                {
                    if (Input.GetKeyDown(KeyCode.DownArrow))
                    {


                        if (selectedItem + 2 > slotCount)//
                        {
                            if (page < (inventoryTabList.Count - 1) / MAX_SLOTS_COUNT)
                                page++;
                            else
                            {
                                page = 0;
                            }

                            RemoveSlot();
                            ShowPage();
                            selectedItem = -2;

                        }
                        if (selectedItem < slotCount - 1)
                        {
                            selectedItem += 2;
                        }
                        else
                        {
                            selectedItem %= 2;
                        }
                        theAudio.Play(key_sound);
                        SelectedItem();
                    }
                    else if (Input.GetKeyDown(KeyCode.UpArrow))
                    {
                        if (selectedItem - 2 < 0)//
                        {
                            if (page != 0)
                                page--;
                            else
                            {
                                page = (inventoryTabList.Count - 1) / MAX_SLOTS_COUNT;
                            }

                            RemoveSlot();
                            ShowPage();
                        }

                        if (selectedItem > 1)
                        {
                            selectedItem -= 2;
                        }
                        else
                        {
                            selectedItem = slotCount - selectedItem;
                        }
                        theAudio.Play(key_sound);
                        SelectedItem();
                    }
                    else if (Input.GetKeyDown(KeyCode.RightArrow))
                    {
                        if (selectedItem + 1 > slotCount)
                        {
                            if (page < (inventoryTabList.Count - 1) / MAX_SLOTS_COUNT)
                            {
                                page++;
                            }
                            else
                            {
                                page = 0;
                            }

                            RemoveSlot();
                            ShowPage();
                            selectedItem = -1;
                        }
                        if (selectedItem < slotCount)
                        {
                            selectedItem++;
                        }
                        else
                        {
                            selectedItem = 0;

                        }
                        theAudio.Play(key_sound);
                        SelectedItem();
                    }
                    else if (Input.GetKeyDown(KeyCode.LeftArrow))
                    {
                        if (selectedItem - 1 < 0)//
                        {
                            if (page != 0)
                            {
                                page--;
                            }
                            else
                            {
                                page = (inventoryTabList.Count - 1) / MAX_SLOTS_COUNT;
                            }

                            RemoveSlot();
                            ShowPage();
                        }

                        if (selectedItem > 0)
                        {
                            selectedItem--;
                        }
                        else
                        {
                            selectedItem = slotCount;
                        }
                        theAudio.Play(key_sound);
                        SelectedItem();
                    }
                    else if (Input.GetKeyDown(KeyCode.Z) && !preventExec)
                    {
                        Debug.Log("z 눌림");
                        if (selectedTab == 0)//소모품
                        {
                            selectedItemIndex = (page * 10) + selectedItem;
                            go_OOC.SetActive(true);
                            theOOC.ShowTwoChoice("사용", "취소", Ok, Cancel);
                            //아이템을 정말 사용할건지 확인 질의 호출
                        }
                        else if (selectedTab == 1)
                        {
                            //장비장착
                            selectedItemIndex = (page * 10) + selectedItem;
                            go_OOC.SetActive(true);
                            theOOC.ShowTwoChoice("장착", "취소", Ok, Cancel);
                        }
                        else
                        {
                            theAudio.Play(beep_sound);
                        }
                    }
                    else if (Input.GetKeyDown(KeyCode.Escape))
                    {
                        Debug.Log("x 눌림");
                        theAudio.Play(cancel_sound);
                        StopAllCoroutines();
                        itemActivated = false;
                        tabActivated = true;
                        ShowTab();
                    }
                }
                else if (Input.GetKeyDown(KeyCode.Escape))
                {
                    Debug.Log("x 눌림");
                    theAudio.Play(cancel_sound);
                    StopAllCoroutines();
                    itemActivated = false;
                    tabActivated = true;
                    ShowTab();
                }


            } //아이템 활성화시 키입력 처리

            if (Input.GetKeyUp(KeyCode.Z)) //중복 실행 방지
                preventExec = false;

        }
    }

    private void ShowGold()
    {
        go_Gold_Text.GetComponent<Text>().text = gold.ToString();
    }

    public List<Item> SaveItem()
    {
        return inventoryItemList;
    }

    public void LoadItem(List<Item> _itemList)
    {
        inventoryItemList = _itemList;
    }


    public void EquipToInventory(Item _item)
    {
        inventoryItemList.Add(_item);
    }
    public void GetAnItem(int _itemID, int _count = 1)
    {
        for (int i = 0; i < theDatabase.itemList.Count; i++) //데이터베이스 아이템 검색
        {
            if (_itemID == theDatabase.itemList[i].itemID)// 데이터베이스에 아이템 발견
            {
                var clone = Instantiate(prefab_Floating_Text, PlayerManager.instance.transform.position, Quaternion.Euler(Vector3.zero));
                clone.GetComponent<FloatingText>().text.text = theDatabase.itemList[i].itemName + " " + _count + "개 획득 + ";
                clone.transform.SetParent(this.transform);

                for (int j = 0; j < inventoryItemList.Count; j++) // 소지품에 같은 아이템이 있는지 검색
                {
                    if (inventoryItemList[j].itemID == _itemID)// 소지품에 같은 아이템이 있음 
                    {
                        if (inventoryItemList[j].itemType == Item.ItemType.Use) //소모품이면 -> 개수 증가 
                        {
                            inventoryItemList[j].itemCount += _count;
                        }
                        else // 소모품이 아니면 -> 아이템 추가
                        {
                            inventoryItemList.Add(theDatabase.itemList[i]);
                        }
                        return;
                    }
                }

                inventoryItemList.Add(theDatabase.itemList[i]); // 소지품에 같은 아이템이 없음 -> 아이템 추가
                inventoryItemList[inventoryItemList.Count - 1].itemCount = _count;
                return;
            }
        }
        Debug.LogError("데이터베이스에 해당 ID값을 가진 아이템이 존재하지 않습니다.");
    }

    public void RemoveSlot()
    {
        for (int i = 0; i < slots.Length; i++)
        {
            slots[i].RemoveItem();
            slots[i].gameObject.SetActive(false);
        }
    } //인벤토리 슬롯 초기화

    public void ShowTab()
    {
        RemoveSlot();
        SelectedTab();
    } //탭 활성화

    public void SelectedTab()
    {
        Color color = selectedTabImages[selectedTab].GetComponent<Image>().color;
        color.a = 0f;
        for (int i = 0; i < selectedTabImages.Length; i++)
        {
            selectedTabImages[i].GetComponent<Image>().color = color;
        }

        description_Text.text = tabDescription[selectedTab];
        StartCoroutine(SelectedTabEffectCoroutine());
    } //선택된 탭을 제외하고 다른 모든 탭의 컬러 알파값을 0으로 조정

    IEnumerator SelectedTabEffectCoroutine()// 선택된 탭 반짝임 효과
    {
        while (tabActivated)
        {
            //StopAllCoroutines();
            Color color = selectedTabImages[0].GetComponent<Image>().color;
            while (color.a < 0.5f)
            {
                color.a += 0.03f;
                selectedTabImages[selectedTab].GetComponent<Image>().color = color;
                yield return waitTime;
            }
            while (color.a > 0f)
            {
                color.a -= 0.03f;
                selectedTabImages[selectedTab].GetComponent<Image>().color = color;
                yield return waitTime;
            }

            yield return new WaitForSeconds(0.03f);
        }
    }

    public void ShowPage()
    {
        slotCount = -1;

        for (int i = page * MAX_SLOTS_COUNT; i < inventoryTabList.Count; i++)
        {
            slotCount = i - (page * MAX_SLOTS_COUNT); // 0 - 0* 10 = 0
            slots[slotCount].gameObject.SetActive(true);
            slots[slotCount].AddItem(inventoryTabList[i]);

            if (slotCount == MAX_SLOTS_COUNT - 1) //
                break;
        } //인벤토리 탭 리스트의 내용을. 인벤토리 슬롯에 추가
    }

    public void ShowItem()
    {
        inventoryTabList.Clear();
        RemoveSlot();
        selectedItem = 0;
        page = 0;

        switch (selectedTab)
        {
            case 0: //소모품
                for (int i = 0; i < inventoryItemList.Count; i++)
                {
                    if (Item.ItemType.Use == inventoryItemList[i].itemType)
                        inventoryTabList.Add(inventoryItemList[i]);
                }
                break;
            case 1: //장비
                for (int i = 0; i < inventoryItemList.Count; i++)
                {
                    if (Item.ItemType.Equip == inventoryItemList[i].itemType)
                        inventoryTabList.Add(inventoryItemList[i]);
                }
                break;
            case 2: //퀘스트용
                for (int i = 0; i < inventoryItemList.Count; i++)
                {
                    if (Item.ItemType.Quest == inventoryItemList[i].itemType)
                        inventoryTabList.Add(inventoryItemList[i]);
                }
                break;
            case 3: //기타
                for (int i = 0; i < inventoryItemList.Count; i++)
                {
                    if (Item.ItemType.ETC == inventoryItemList[i].itemType)
                        inventoryTabList.Add(inventoryItemList[i]);
                }
                break;
        } //탭에 따른 아이템 분류. 그것을 인벤토리 탭 리스트에 추가

        ShowPage();

        SelectedItem();

    }

    //아이템 활성화(inventoryTabList에 조건에 맞는 아이템들만 넣어주고, 인벤토리 슬롯에 출력)


    public void SelectedItem()//선택된 아이템을 제외하고 다른 모든 탭의 컬러 알파값을 0으로 조정
    {
        StopAllCoroutines();
        if (slotCount > -1)
        {
            Color color = slots[0].selected_Item.GetComponent<Image>().color;
            color.a = 0f;
            for (int i = 0; i < slotCount; i++)
                slots[i].selected_Item.GetComponent<Image>().color = color;

            description_Text.text = inventoryTabList[selectedItem].itemDescription;
            StartCoroutine(SelectedItemEffectCoroutine());
        }
        else
            description_Text.text = "해당 타입의 아이템을 소유하고 있지 않습니다.";
    }

    IEnumerator SelectedItemEffectCoroutine()
    {
        while (itemActivated)
        {
            //StopAllCoroutines();
            Color color = slots[0].GetComponent<Image>().color;
            while (color.a < 0.5f)
            {
                color.a += 0.03f;
                slots[selectedItem].selected_Item.GetComponent<Image>().color = color;
                yield return waitTime;
            }
            while (color.a > 0f)
            {
                color.a -= 0.03f;
                slots[selectedItem].selected_Item.GetComponent<Image>().color = color;
                yield return waitTime;
            }

            yield return new WaitForSeconds(0.03f);
        }
    } //선택된 아이템 반짝임 효과

    void Ok()
    {
        for (int i = 0; i < inventoryItemList.Count; i++) //소유 아이템 리스트에서
        {
            if (inventoryItemList[i].itemID == inventoryTabList[selectedItemIndex].itemID) // 같은 ID찾음
            {
                if (selectedTab == 0) //소모품탭
                {
                    theDatabase.UseItem(inventoryItemList[i].itemID);
                    if (inventoryItemList[i].itemCount > 1)
                    {
                        inventoryItemList[i].itemCount--;
                        go_OOC.SetActive(false);
                    }
                    else
                    {
                        inventoryItemList.RemoveAt(i);
                        go_OOC.SetActive(false);
                    }
                }
                else if (selectedTab == 1) //장비탭
                {
                    theEquip.EquipItem(inventoryItemList[i]);
                    go_OOC.SetActive(false);
                    inventoryItemList.RemoveAt(i);
                    ShowItem();
                    break;
                }

                //theAudio.Play()//아이템 먹는 소리 같은 것 추가
                ShowItem();
                break;
            }

        }
    }

    void Cancel()
    {
        stopKeyInput = false;
        go_OOC.SetActive(false);
    }

}
