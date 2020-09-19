using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary; //직력화된걸 이진파일로 만드는 변환기
using UnityEngine.SceneManagement;

public class SaveNLoad : MonoBehaviour
{
    [System.Serializable] //데이터를 직력화 시켜줌. 컴퓨터 입장에서 읽고 쓰기 쉽게
    public class Data
    {
        public float playerX;
        public float playerY;
        public float playerZ;

        public int playerLv;
        public int playerHP;
        public int playerMP;

        public int playerCurrentHP;
        public int playerCurrentMP;
        public int playerCurrentEXP;


        public int playerATK;
        public int playerDEF;

        public int added_atk;
        public int added_def;
        public int added_hpr;
        public int added_mpr;

        public int gold;

        public List<int> playerItemInventory;
        public List<int> playerItemInventoryCount;
        public List<int> playerEquipItem;

        public string mapName;
        public string sceneName;

        public List<bool> swList;
        public List<string> swNameList;
        public List<string> varNameList;
        public List<float> varNumberList;

    }

    private PlayerManager thePlayer;
    private PlayerStat thePlayerStat;
    private DatabaseManager theDatabase;
    private Inventory theInven;
    private Equipment theEquip;

    public Data data;

    private Vector3 vector;

    public void CallSave()
    {

        theDatabase = FindObjectOfType<DatabaseManager>();
        thePlayer = FindObjectOfType<PlayerManager>();
        thePlayerStat = FindObjectOfType<PlayerStat>();
        theEquip = FindObjectOfType<Equipment>();
        theInven = FindObjectOfType<Inventory>();

        data.playerX = thePlayer.transform.position.x;
        data.playerY = thePlayer.transform.position.y;
        data.playerZ = thePlayer.transform.position.z;

        data.playerLv = thePlayerStat.character_Lv;
        data.playerHP = thePlayerStat.hp;
        data.playerMP = thePlayerStat.mp;
        data.playerCurrentHP = thePlayerStat.currentHP;
        data.playerCurrentMP = thePlayerStat.currentMP;
        data.playerCurrentEXP = thePlayerStat.currentExp;
        data.playerATK = thePlayerStat.atk;
        data.playerDEF = thePlayerStat.def;
        //data.added_atk = theEquip.added_atk;
        //data.added_def = theEquip.added_def;

        data.gold = theInven.gold;

        data.mapName = thePlayer.currentMapName;
        data.sceneName = thePlayer.currentSceneName;

        Debug.Log("기초 데이터 저장 성공");

        data.playerItemInventory.Clear();
        data.playerItemInventoryCount.Clear();
        data.playerEquipItem.Clear();

        for(int i= 0; i < theDatabase.var_name.Length; i++)
        {
            data.varNameList.Add(theDatabase.var_name[i]);
            data.varNumberList.Add(theDatabase.var[i]);

        }

        for (int i = 0; i < theDatabase.switch_name.Length; i++)
        {
            data.swNameList.Add(theDatabase.switch_name[i]);
            data.swList.Add(theDatabase.switches[i]);

        }

        List<Item> itemList = theInven.SaveItem();

        for (int i = 0; i < itemList.Count; i++)
        {
            Debug.Log("인벤토리의 아이템 저장 완료 : " + itemList[i].itemID);
            data.playerItemInventory.Add(itemList[i].itemID);
            data.playerItemInventoryCount.Add(itemList[i].itemCount);
        }

        //TODO:fix
        //for (int i = 0; i < theEquip.equipItemList.Length; i++)
        //{
        //    data.playerEquipItem.Add(theEquip.equipItemList[i].itemID);
        //    Debug.Log("장착된 아이템 저장 완료 : " + theEquip.equipItemList[i].itemID);
        //}

        BinaryFormatter bf = new BinaryFormatter(); //이진파일로 변환
        FileStream file = File.Create(Application.dataPath + "/SaveFile.dat"); //asset폴더 +/SaveFile.dat (파일형식은 아무렇게 가능)

        bf.Serialize(file, data); //파일을 기록하고 직렬화 후 이진파일로 변환
        file.Close();

        Debug.Log(Application.dataPath + "의 위치에 저장했습니다.");

    }
    //로드시 페이드인 페이드아웃 기능 추가


    public void CallLoad()
    {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Open(Application.dataPath + "/SaveFile.dat", FileMode.Open);

        if (file != null && file.Length > 0)
        {
            data = (Data)bf.Deserialize(file);

            theDatabase = FindObjectOfType<DatabaseManager>();
            thePlayer = FindObjectOfType<PlayerManager>();
            thePlayerStat = FindObjectOfType<PlayerStat>();
            theEquip = FindObjectOfType<Equipment>();
            theInven = FindObjectOfType<Inventory>();

            thePlayer.currentMapName = data.mapName;
            thePlayer.currentSceneName = data.sceneName;

            vector.Set(data.playerX, data.playerY, data.playerZ);
            thePlayer.transform.position = vector;
            Debug.Log("Test1");
            thePlayerStat.character_Lv = data.playerLv;
            thePlayerStat.hp = data.playerHP;
            thePlayerStat.mp = data.playerMP;
            thePlayerStat.currentHP = data.playerCurrentHP;
            thePlayerStat.currentMP = data.playerCurrentMP;
            thePlayerStat.currentExp = data.playerCurrentEXP;
            thePlayerStat.atk = data.playerATK;
            thePlayerStat.def = data.playerDEF;

            theInven.gold = data.gold;
            Debug.Log("Test2");

            //theEquip.added_atk = data.added_atk;
            //theEquip.added_def = data.added_def;


            theDatabase.var = data.varNumberList.ToArray();
            theDatabase.var_name = data.varNameList.ToArray();
            theDatabase.switches = data.swList.ToArray();
            theDatabase.switch_name = data.swNameList.ToArray();
            Debug.Log("Test3");
            //TODO:fix
            //for (int i = 0; i < theEquip.equipItemList.Length; i++)
            //{
            //    for (int x = 0; x < theDatabase.itemList.Count; x++)
            //    {
            //        if (data.playerEquipItem[i] == theDatabase.itemList[x].itemID)
            //        {
            //            theEquip.equipItemList[i] = theDatabase.itemList[x];
            //            Debug.Log("장착된 아이템을 로드했습니다 : " + theEquip.equipItemList[i].itemID);
            //            break;
            //        }
            //    }
            //}


            List<Item> itemList = new List<Item>();

            for (int i = 0; i < data.playerItemInventory.Count; i++)
            {
                for (int x = 0; x < theDatabase.itemList.Count; x++)
                {
                    if (data.playerItemInventory[i] == theDatabase.itemList[x].itemID)
                    {
                        itemList.Add(theDatabase.itemList[x]);
                        Debug.Log("인벤토리 아이템을 로드했습니다 : " + theDatabase.itemList[x].itemID);
                        break;
                    }
                }
            }
            Debug.Log("Test4");
            //for (int i = 0; i < data.playerItemInventoryCount.Count - 1; i++)
            //{
            //    itemList[i].itemCount = data.playerItemInventoryCount[i];
            //}

            Debug.Log("Test5");
            theInven.LoadItem(itemList);
            //theEquip.ShowTxT();

            GameManager theGM = FindObjectOfType<GameManager>();
            theGM.LoadStart();

            //SceneManager.LoadScene(data.sceneName);
        }
        else
        {
            Debug.Log("저장된 세이브 파일이 없습니다");
        }


        file.Close();
    }

    public void CallNewStart()
    {

        theDatabase = FindObjectOfType<DatabaseManager>();
        thePlayer = FindObjectOfType<PlayerManager>();
        thePlayerStat = FindObjectOfType<PlayerStat>();
        theEquip = FindObjectOfType<Equipment>();
        theInven = FindObjectOfType<Inventory>();

        thePlayer.currentMapName = "";
        thePlayer.currentSceneName = "";

        vector.Set(4.5f, -3.75f, 0f);
        thePlayer.transform.position = vector;
        Debug.Log("Test1");
        thePlayerStat.character_Lv = 1;
        thePlayerStat.hp = 50;
        thePlayerStat.mp = 50;
        thePlayerStat.currentHP = 50;
        thePlayerStat.currentMP = 50;
        thePlayerStat.currentExp = 0;
        thePlayerStat.atk = 10;
        thePlayerStat.def = 5;

        theInven.gold = 500;
        Debug.Log("Test2");

        //theEquip.added_atk = data.added_atk;
        //theEquip.added_def = data.added_def;


        //theDatabase.var = data.varNumberList.ToArray();
        //theDatabase.var_name = data.varNameList.ToArray();
        //theDatabase.switches = data.swList.ToArray();
        //.switch_name = data.swNameList.ToArray();
        //}


        List<Item> itemList = new List<Item>();

        theDatabase.StartItemSetting();
        GameManager theGM = FindObjectOfType<GameManager>();
            theGM.LoadStart();

   
    }
}