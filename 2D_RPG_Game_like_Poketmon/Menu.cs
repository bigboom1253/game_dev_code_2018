using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class Menu : MonoBehaviour
{
    //TODO:fix 이벤트 도중에는 메뉴창 못띄우게
    public static Menu instance;

    private void Awake()
    {
        if (instance == null)
        {
            DontDestroyOnLoad(this.gameObject);
            instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }
    }
    public GameObject go_menu;
    public GameObject go_equipment;
    public GameObject go_inventory;
    public GameObject go_useMagic;
    public GameObject go_save;
    public GameObject go_load;
    public GameObject go_exit;
    public GameObject go_title;

    public GameObject go_EquipmentList;
    public Transform tf; //슬롯의 부모객체

    public SaveNLoad theSaveNLoad;
    public Inventory theInven;
    public AudioManager theAudio;
    public OrderManager theOrder;
    public Equipment theEquip;
    private Shopping theShopping;
    public Title theTitle;
    public FadeManager theFade;

    public bool menuActivated; //메뉴 활성화시 true;
    public int selectedMenu; //선택된 메뉴
    public GameObject[] selectedMenuPanel;
    public GameObject[] selectedEquipmentPanel;
    public int selectedItem; //선택된 아이템.
    public int selectedItemIndex; // 실제로 선택된 아이템을 찾아주는 인덱스
    public string call_sound;
    public string key_sound;
    public string enter_sound;
    public string cancel_sound;
    public string open_sound;
    public string beep_sound;

    private int page;//페이지
    private int slotCount;//활성화된 슬롯의 개수
    private const int MAX_SLOTS_COUNT = 5; //최대 슬롯개수

    public int prev_selectedItem; //직전에 선택된 아이템 패널 저장
    //넣어놓고, esc를 눌렀을 때, 메뉴 팝 ->
    //빈스택에, esc를 눌렀을 때, 메뉴 푸시 ->
    public Stack<GameObject> menuStack;
    
    private bool EquipmentListActivated;
    private bool activated = false;
    private bool EquipmentMenuActivated;
    public bool preventExec; //중복실행 제한.
    public InventorySlot[] slots; // 인벤도리 슬롯들 
    private WaitForSeconds waitTime = new WaitForSeconds(0.03f);
    public List<Item> inventoryItemList; //플레이어가 소지한 아이템 리스트.
    public List<Item> inventoryTabList; //조건에 맞는 아이템을 정렬한 리스트

    public void Exit() //종료 
    {
        selectedMenu = 0;
        go_menu.SetActive(false);
        menuActivated = false;
        activated = false;

        StartCoroutine(GameToTitle());

    }
    IEnumerator GameToTitle()
    {
        theFade.FadeOut();
        yield return new WaitForSeconds(1f);
        go_title.SetActive(true);
        theTitle.selectedTitleMenu = 0;
        theTitle.titleActivated = true;
        theFade.FadeIn();
        yield return new WaitForSeconds(1f);
    }

    // Start is called before the first frame update
    void Start()
    {
        theShopping = FindObjectOfType<Shopping>();
        theEquip = FindObjectOfType<Equipment>(); //equipment 를 따로 빼서 만들어줘야함
        theSaveNLoad = FindObjectOfType<SaveNLoad>();
        theAudio = FindObjectOfType<AudioManager>();
        theOrder = FindObjectOfType<OrderManager>();
        theInven = FindObjectOfType<Inventory>();
        theTitle = FindObjectOfType<Title>();
        theFade = FindObjectOfType<FadeManager>();
        inventoryTabList = new List<Item>(); //정렬한 아이템들 리스트
        slots = tf.GetComponentsInChildren<InventorySlot>(); //각각 인벤토리 슬롯의 아이템 정보 삽입
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && !theShopping.shopActivated && !theShopping.shoppingListActivated)
        {
                
            if (!activated)
            {
                activated = true;
                theAudio.Play(call_sound);
                theOrder.NotMove();
                go_menu.SetActive(true);
                go_equipment.SetActive(false);
                selectedMenu = 0;
                menuActivated = true;
                ShowMenu();
                preventExec = true;
                
            }
        }

        if (activated)
        {
            if (menuActivated)
            {
                ControllPanel(selectedMenuPanel);

                if (Input.GetKeyDown(KeyCode.Z) && !preventExec)
                {
                    theAudio.Play(enter_sound);
                    Color color = selectedMenuPanel[selectedMenu].GetComponent<Image>().color;
                    color.a = 1f;
                    selectedMenuPanel[selectedMenu].GetComponent<Image>().color = color;
                    preventExec = true;
                    menuActivated = false;

                    switch (selectedMenu)
                    {
                        case 0: //장비창
                            selectedMenu = 0;
                            EquipmentMenuActivated = true;
                            go_equipment.SetActive(true);
                            selectedEquipmentPanel[selectedMenu].GetComponent<Image>().color = color;
                            break;

                        case 1: //인벤토리

                            theInven.activated = true;
                            theAudio.Play(open_sound);
                            theInven.go.SetActive(true);
                            theInven.selectedTab = 0;
                            theInven.tabActivated = true;
                            theInven.itemActivated = false;
                            theInven.ShowTab();

                            break;
                        case 2: //마법사용
                            break;
                        case 3: // 세이브
                            theSaveNLoad.CallSave();
                            menuActivated = true;
                            break;
                        case 4: // 로드
                            theSaveNLoad.CallLoad();
                            menuActivated = true;
                            break;
                        case 5: // 종료
                            Exit();        
                            break;
                    }
                }
                // preventExec: esc누를때 중복실행 방지
                else if (Input.GetKeyDown(KeyCode.Escape) && !preventExec)
                {
                    theAudio.Play(cancel_sound);
                    StopAllCoroutines();
                    selectedMenu = 0;
                    go_menu.SetActive(false);
                    menuActivated = false;
                    activated = false;

                    theOrder.Move();
                }
                else if(Input.GetKeyUp(KeyCode.Z))
                {
                    preventExec = false;
                }
            } //탭 활성화시 키입력 처리
            else if (EquipmentMenuActivated)
            {
                ControllPanel(selectedEquipmentPanel);
                if (Input.GetKeyDown(KeyCode.Z))
                {
                    theAudio.Play(enter_sound);

                    Color color = selectedEquipmentPanel[selectedMenu].GetComponent<Image>().color;
                    color.a = 1f;
                    selectedEquipmentPanel[selectedMenu].GetComponent<Image>().color = color;

                    menuActivated = false;
                    EquipmentListActivated = true;
                    EquipmentMenuActivated = false;
                    go_equipment.SetActive(false);
                    go_EquipmentList.SetActive(true);
                    prev_selectedItem = selectedItem;
                    selectedItem = 0;
                    ShowEquipment();
                    SelectedItem();

                }
                else if (Input.GetKeyDown(KeyCode.Escape))
                {
                    //칼라 초기화 추가
                    theAudio.Play(cancel_sound);
                    selectedMenu = 0; //장비 인덱스
                    SelectedMenu(selectedEquipmentPanel); // 뒤로 갔을때, 장비패널 칼라 0으로 초기화
                    go_equipment.SetActive(false);
                    EquipmentMenuActivated = false;
                    menuActivated = true;

                }

            }
            else if (EquipmentListActivated)
            {
                
                if (Input.GetKeyDown(KeyCode.DownArrow))
                {
                    if (selectedItem + 1 > slotCount)
                    {
                        if(page < (inventoryTabList.Count - 1) / MAX_SLOTS_COUNT)
                            page++;
                        else
                            page = 0;

                        RemoveSlot();
                        ShowPage();
                        selectedItem = -1; //밑에과정으로 넘어가게
                    }
                    if (selectedItem < inventoryTabList.Count - 1)
                        selectedItem ++;
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
                else if (Input.GetKeyDown(KeyCode.Z))
                {
                    //장비장착
                    selectedItemIndex = (page * 5) + selectedItem;
                    theEquip.EquipItem(inventoryTabList[selectedItemIndex]);
                    theInven.inventoryItemList.Remove(inventoryTabList[selectedItemIndex]); //장착후 인벤토리에서 제거
                    go_equipment.SetActive(true);
                    selectedItem = prev_selectedItem;
                    go_EquipmentList.SetActive(false);
                    EquipmentListActivated = false;
                    EquipmentMenuActivated = true;
                    theAudio.Play(beep_sound);

                }
                else if (Input.GetKeyDown(KeyCode.Escape))
                {
                    Debug.Log("esc 눌림");
                    theAudio.Play(cancel_sound);
                    StopAllCoroutines();
                    selectedItem = 0;
                    go_equipment.SetActive(true);
                    go_EquipmentList.SetActive(false);
                    EquipmentListActivated = false;
                    EquipmentMenuActivated = true;
                }
            }
           
            
        }
        if (Input.GetKeyUp(KeyCode.Escape)) //중복 실행 방지
            preventExec = false;
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

    public void ShowEquipment()//선택된 종류의 장비 리스트를 보여줌.
    {
        RemoveSlot();
        inventoryTabList.Clear();
        selectedItem = 0;
        page = 0;

        switch (selectedMenu)
        {
            case 0: //무기
                for (int i = 0; i < theInven.inventoryItemList.Count; i++)
                {
                    if (Item.ItemType.Equip == theInven.inventoryItemList[i].itemType 
                        && Item.EquipType.WEAPON == theInven.inventoryItemList[i].equipType)
                        inventoryTabList.Add(theInven.inventoryItemList[i]);
                }
                break;
            case 1: //갑옷
                for (int i = 0; i < theInven.inventoryItemList.Count; i++)
                {
                    if (Item.ItemType.Equip == theInven.inventoryItemList[i].itemType && Item.EquipType.ARMOR == theInven.inventoryItemList[i].equipType)
                        inventoryTabList.Add(theInven.inventoryItemList[i]);
                }
                break;
            case 2: //방패
                for (int i = 0; i < theInven.inventoryItemList.Count; i++)
                {
                    if (Item.ItemType.Equip == theInven.inventoryItemList[i].itemType && Item.EquipType.SHILED == theInven.inventoryItemList[i].equipType)
                        inventoryTabList.Add(theInven.inventoryItemList[i]);
                }
                break;
            case 3: //장신구
                for (int i = 0; i < theInven.inventoryItemList.Count; i++)
                {
                    if (Item.ItemType.Equip == theInven.inventoryItemList[i].itemType && Item.EquipType.ACCESSARY == theInven.inventoryItemList[i].equipType)
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


    public void ShowMenu()
    {
        SelectedMenu(selectedMenuPanel);
    } //탭 활성화

    public void SelectedMenu(GameObject[] _selectedPanel)
    {
        Color color = _selectedPanel[selectedMenu].GetComponent<Image>().color;
        color.a = 0f;
        for (int i = 0; i < _selectedPanel.Length; i++)
        {
            _selectedPanel[i].GetComponent<Image>().color = color;
        }
        StartCoroutine(SelectedTabEffectCoroutine(_selectedPanel));
    } //선택된 탭을 제외하고 다른 모든 탭의 컬러 알파값을 0으로 조정

    IEnumerator SelectedTabEffectCoroutine(GameObject[] _selectedPanel)// 선택된 탭 알파값 1;
    {

            Color color = _selectedPanel[0].GetComponent<Image>().color;
            color.a = 1f;
            _selectedPanel[selectedMenu].GetComponent<Image>().color = color;
            yield return new WaitForSeconds(0.03f); 
    }
    IEnumerator WaitCoroutine()
    {
        yield return waitTime;
    }


    public void RemoveSlot()
    {
        for (int i = 0; i < slots.Length; i++)
        {
            slots[i].RemoveItem();
            slots[i].gameObject.SetActive(false);
        }
    } //인벤토리 슬롯 초기화

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
        while (EquipmentListActivated)
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
}
