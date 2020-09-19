using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TalkNPC : MonoBehaviour
{
    [SerializeField]
    public Dialogue dialogue;
    private DialogueManager theDM;
    private OrderManager theOrder;
    private PlayerManager thePlayer;
    private NPCManager theNPC;
    private Shopping theShopping;
    private int tempFrequency;
    public bool flag;
    [Tooltip("상점NPC는 체크필수")]
    public bool shopNPC;

    Vector3 temp;

    // Start is called before the first frame update
    void Start()
    {
        theShopping = FindObjectOfType<Shopping>();
        theOrder = FindObjectOfType<OrderManager>();
        theDM = FindObjectOfType<DialogueManager>();
        thePlayer = FindObjectOfType<PlayerManager>();
        theNPC = GetComponent<NPCManager>();
    }

    //대화 큐 늘어나는거.
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player" && !thePlayer.preventExec)//&& !shopNPC
        {
            flag = true;
            temp = (collision.transform.position - this.transform.position).normalized;
            StartCoroutine(EventCoroutine());
        }
        if(shopNPC)
        {
            theShopping.shopActivated = true;
        }
    }

    IEnumerator EventCoroutine()
    {
        theOrder.NotMove();

        theOrder.PreLoadCharacter();
        this.GetComponent<NPCManager>().npc.npcCanMove = false; //대화가 시작되면 멈춤
        //주인공을 바라봄
        theNPC.animator.SetFloat("DirX", temp.x);
        theNPC.animator.SetFloat("DirY", temp.y);


        theDM.ShowDialogue(dialogue);

        yield return new WaitUntil(() => !theDM.talking); //(조건)대화가 끝나면 이동시키겠다. 
        if (!shopNPC)
        {
            theOrder.Move();
        }
        this.GetComponent<NPCManager>().npc.npcCanMove = true;

    }


}
