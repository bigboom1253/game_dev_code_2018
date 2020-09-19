using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemPickUp : MonoBehaviour
{
    public int itemID;
    public int _count;
    public string pickUpsound;
    public Dialogue dialogue_1;


    private DialogueManager theDM;
    private OrderManager TheOrder;
    private PlayerManager thePlayer;
    private FadeManager theFade;

    public bool flag; // 이벤트가 한번만 실행되도록.

    void Start()
    {
        theDM = FindObjectOfType<DialogueManager>();
        TheOrder = FindObjectOfType<OrderManager>();
        thePlayer = FindObjectOfType<PlayerManager>();
        theFade = FindObjectOfType<FadeManager>();
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        if(thePlayer.dialogueResetFlag > 10 && Input.GetKeyDown(KeyCode.Z))
        {
            //인벤토리에 추가
            thePlayer.dialogueResetFlag = 0;
            AudioManager.instance.Play(pickUpsound);
            Inventory.instance.GetAnItem(itemID, _count);
            StartCoroutine(EventCoroutine());
            
            //Destroy(this.gameObject);

        }
    }

    IEnumerator EventCoroutine()
    {
        TheOrder.NotMove();
        TheOrder.PreLoadCharacter();

        theDM.ShowDialogue(dialogue_1);
        //기다렸다가 이 이후부터 재실행
        yield return new WaitUntil(() => !theDM.talking); //(조건)대화가 끝나면 이동시키겠다. 
        TheOrder.Move();

        Destroy(this.gameObject);

    }
}
