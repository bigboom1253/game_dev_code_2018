using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Shopping : MonoBehaviour
{
    //todo
    public bool shopActivated = false; //TODO FIX:
    public bool shoppingListActivated;
    public bool preventExec; //중복실행 제한.
    public bool shopDialogueActivated = true; //상점 대화 온오프
    public int selectedMenu;
    public string key_sound;
    public bool itemPriceFlag;


    public InventorySlot[] slots;
    public int selectedItem; //선택된 아이템.
    public int selectedItemIndex; // 실제로 선택된 아이템을 찾아주는 인덱스

    private int page;//페이지
    private int slotCount;//활성화된 슬롯의 개수
    private const int MAX_SLOTS_COUNT = 5; //최대 슬롯개수

    public Transform tf; //슬롯의 부모객체

    private InventorySlot slot;
    private Inventory theInven;
    private OrderManager theOrder;
    private AudioManager theAudio;
    private DatabaseManager theDatabase;
    private OkOrCancel theOOC;

    public GameObject shopMenu;
    public GameObject[] selectedMenuPanel;
    public GameObject go_shopping_list;
    public GameObject[] go_shopping_list_panel;

    public List<Item> inventoryItemList; //플레이어가 소지한 아이템 리스트.
    public List<Item> inventoryTabList; //조건에 맞는 아이템을 정렬한 리스트
    [Tooltip("상점이 파는 아이템ID 리스트")]
    public List<int> itemIDList;//파는 아이템 ID 리스트
    public List<int> itemPriceList;//ID에 따른 가격
    private WaitForSeconds waitTime = new WaitForSeconds(0.03f);
    // Start is called before the first frame update
    void Start()
    {
        theOOC = FindObjectOfType<OkOrCancel>();
        theOrder = FindObjectOfType<OrderManager>();
        theAudio = FindObjectOfType<AudioManager>();
        theInven = FindObjectOfType<Inventory>();
        theDatabase = FindObjectOfType<DatabaseManager>();
        selectedMenu = 0;
        slots = tf.GetComponentsInChildren<InventorySlot>();
        slot = tf.GetComponentInChildren<InventorySlot>();
    }

    void Update()
    {
        if (shopActivated)
        {
            shopMenu.SetActive(true);
            SelectedTabEffect(selectedMenuPanel);// 처음 켜졌을 때, 삽니다에 이펙트가 가도록.
            theOrder.NotMove();
            ControllPanel(selectedMenuPanel);

            //preventExec = true;
            if (Input.GetKeyDown(KeyCode.Z) && !preventExec)
            {
                switch (selectedMenu)
                {
                    case 0:
                        Debug.Log("삽니다. 클릭");

                        shopActivated = false;
                        shoppingListActivated = true;

                        ShowShopItemList();
                        SelectedItem();
                        preventExec = true;


                        //삽니다에 이펙트 유지. 슬롯에 파는 아이템 적용, 
                        go_shopping_list.SetActive(true);

                        break;
                    case 1:
                        Debug.Log("팝니다. 클릭");
                        SellActivated(true);
                        shopActivated = false;
                        shoppingListActivated = true;
                        SelectedItem();
                        ShowShopItemList();
                        preventExec = true;

                        //인벤토리 리스트 삽입. 장비, 소모품 상관없이.
                        go_shopping_list.SetActive(true);
                        break;
                    default:
                        break;

                }
            }
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                itemPriceFlag = false;
                shopActivated = false;
                shopMenu.SetActive(false);
                theOrder.Move();
                shopDialogueActivated = false;
                selectedMenu = 0;
                SelectedMenu(selectedMenuPanel); // 다시 상점으로진입했을떄, 이펙트 0으로 초기화

            }

            if (Input.GetKeyUp(KeyCode.Escape))
            {
                preventExec = false;
            }

        }

        if (shoppingListActivated)
        {
            //StartCoroutine(SelectedTabEffectCoroutine(go_shopping_list_panel));
            if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                if (selectedItem + 1 > slotCount)
                {
                    if (page < (inventoryTabList.Count - 1) / MAX_SLOTS_COUNT)
                        page++;
                    else
                        page = 0;

                    RemoveSlot();
                    ShowPage();
                    selectedItem = -1; //밑에과정으로 넘어가게
                }
                if (selectedItem < inventoryTabList.Count - 1)
                    selectedItem++;
                else
                    selectedItem = 0;
                theAudio.Play(key_sound);
                SelectedItem();
            }
            else if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                if (selectedItem - 1 < 0)
                {
                    if (page != 0)
                    {
                        page--;
                        selectedItem = 5;
                    }
                    else
                    {
                        page = (inventoryTabList.Count - 1) / MAX_SLOTS_COUNT;
                        selectedItem = (inventoryTabList.Count - 1) % MAX_SLOTS_COUNT + 1;
                    }
                    RemoveSlot();
                    ShowPage();
                }

                if (selectedItem > 0)
                    selectedItem--;
                else
                    selectedItem = inventoryTabList.Count - 1;
                theAudio.Play(key_sound);
                SelectedItem();
            }
            else if (Input.GetKeyDown(KeyCode.Z) && !preventExec)
            {
                selectedItemIndex = (page * 5) + selectedItem;
                Debug.Log("z 눌림");
                switch (selectedMenu)
                {
                    case 0: //삽니다.
                        //확인질문
                        if (theInven.gold >= inventoryTabList[selectedItemIndex].buyPrice)
                        {
                            theInven.gold -= inventoryTabList[selectedItemIndex].buyPrice;
                            for (int i = 0; i < theDatabase.itemList.Count; i++) //데이터베이스 아이템 검색
                            {
                                if (inventoryTabList[selectedItemIndex].itemID == theDatabase.itemList[i].itemID)// 데이터베이스에 아이템 발견
                                {
                                    Debug.Log("1");
                                    for (int j = 0; j < theInven.inventoryItemList.Count; j++) // 소지품에 같은 아이템이 있는지 검색
                                    {
                                        if (theInven.inventoryItemList[j].itemID == inventoryTabList[selectedItemIndex].itemID)// 소지품에 같은 아이템이 있음 
                                        {
                                            Debug.Log("3");
                                            if (theInven.inventoryItemList[j].itemType == Item.ItemType.Use) //소모품이면 -> 개수 증가 
                                            {
                                                Debug.Log("소모품 구매");
                                                theInven.inventoryItemList[j].itemCount++; //한개만 사는걸로
                                            }
                                            else // 소모품이 아니면 -> 아이템 추가
                                            {
                                                theInven.inventoryItemList.Add(theDatabase.itemList[i]);
                                                Debug.Log("장비아이템 구매");
                                            }

                                            return;
                                        }


                                    }
                                    Debug.Log("4");
                                    theInven.inventoryItemList.Add(theDatabase.itemList[i]); // 소지품에 같은 아이템이 없음 -> 아이템 추가
                                    theInven.inventoryItemList[inventoryItemList.Count - 1].itemCount = 1; //한개만 사는걸로
                                    return;
                                }
                            }
                        }
                        else
                        {
                            Debug.Log("돈이 부족합니다.");
                        }
                        break;

                    case 1: //팝니다.
                        theInven.gold += inventoryTabList[selectedItemIndex].sellPrice;
                        for (int i = 0; i < theInven.inventoryItemList.Count; i++)
                        {
                            if (inventoryTabList[selectedItemIndex].itemID == theInven.inventoryItemList[i].itemID)
                                theInven.inventoryItemList.RemoveAt(i);
                        }
                        selectedMenu = 1;
                        SelectedItem();
                        ShowShopItemList();
                        break;
                }


            }
            else if (Input.GetKeyDown(KeyCode.Escape) && !preventExec)
            {
                Debug.Log("esc 눌림");
                SellActivated(false);
                selectedItem = 0;
                SelectedItem(); // 다시 상점으로진입했을떄, 이펙트 0으로 초기화
                shoppingListActivated = false;
                shopActivated = true;
                go_shopping_list.SetActive(false);
            }

            if (Input.GetKeyUp(KeyCode.Escape))
                preventExec = false;
            if (Input.GetKeyUp(KeyCode.Z))
                preventExec = false;
        }
    }


    public void ControllPanel(GameObject[] _selectPanel)
    {
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            if (selectedMenu < _selectPanel.Length - 1)
                selectedMenu++;
            else
                selectedMenu = 0;
            theAudio.Play(key_sound);
            SelectedMenu(_selectPanel);
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            if (selectedMenu > 0)
                selectedMenu--;
            else
                selectedMenu = _selectPanel.Length - 1;
            theAudio.Play(key_sound);
            SelectedMenu(_selectPanel);
        }
    }
    public void SelectedMenu(GameObject[] _selectedPanel)
    {
        Color color = _selectedPanel[selectedMenu].GetComponent<Image>().color;
        color.a = 0f;
        for (int i = 0; i < _selectedPanel.Length; i++)
        {
            _selectedPanel[i].GetComponent<Image>().color = color;
        }
        SelectedTabEffect(selectedMenuPanel);
    } //선택된 탭을 제외하고 다른 모든 탭의 컬러 알파값을 0으로 조정

    public void SelectedItem()//선택된 아이템을 제외하고 다른 모든 탭의 컬러 알파값을 0으로 조정
    {
        StopAllCoroutines();

        Color color = slots[0].selected_Item.GetComponent<Image>().color;
        color.a = 0f;
        for (int i = 0; i <= slotCount; i++)
        {
            slots[i].selected_Item.GetComponent<Image>().color = color;
        }
        StartCoroutine(SelectedItemEffectCoroutine());

    }
    IEnumerator SelectedItemEffectCoroutine()
    {
        while (shoppingListActivated)
        {
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
    }

    public void ShowShopItemList()//
    {
        RemoveSlot();
        inventoryTabList.Clear();
        selectedItem = 0;
        page = 0;

        switch (selectedMenu)
        {
            case 0: //삽니다.
                for (int i = 0; i < itemIDList.Count; i++)
                {
                    for (int j = 0; j < theDatabase.itemList.Count; j++) //데이터베이스 아이템 검색
                    {
                        if (itemIDList[i] == theDatabase.itemList[j].itemID) // 데이터베이스에 아이템 발견
                        {
                            inventoryTabList.Add(theDatabase.itemList[j]);
                        }
                    }
                }

                break;
            case 1: //팝니다.
                for (int i = 0; i < theInven.inventoryItemList.Count; i++)
                {
                    inventoryTabList.Add(theInven.inventoryItemList[i]);
                }
                break;
        } //탭에 따른 아이템 분류. 그것을 인벤토리 탭 리스트에 추가

        ShowPage();
    }

    private void ShowPage()
    {
        slotCount = -1;

        for (int i = page * MAX_SLOTS_COUNT; i < inventoryTabList.Count; i++)
        {
            slotCount = i - (page * MAX_SLOTS_COUNT);
            slots[slotCount].gameObject.SetActive(true);
            slots[slotCount].AddItem(inventoryTabList[i]);

            if (slotCount == MAX_SLOTS_COUNT - 1)
                break;
        } //인벤토리 탭 리스트의 내용을. 인벤토리 슬롯에 추가
    }

    public void RemoveSlot()
    {
        for (int i = 0; i < slots.Length; i++)
        {
            slots[i].RemoveItem();
            slots[i].gameObject.SetActive(false);
        }
    } //인벤토리 슬롯 초기화

    void SellActivated(bool _boolSet)
    {
        Debug.Log(slotCount);
        for (int i = 0; i < MAX_SLOTS_COUNT; i++)
        {
            slots[i].sellListActivated = _boolSet; //판매가격 적용되게
        }
    }
    void SelectedTabEffect(GameObject[] _selectedPanel)
    {
        Color color = _selectedPanel[0].GetComponent<Image>().color;
        color.a = 1f;
        _selectedPanel[selectedMenu].GetComponent<Image>().color = color;
    }
}
