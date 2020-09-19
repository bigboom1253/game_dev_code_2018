using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestScript2 : MonoBehaviour
{
    public Dialogue dialogue_1;
    public Dialogue dialogue_2;

    private DialogueManager theDM;
    private OrderManager TheOrder;
    private PlayerManager thePlayer;
    private FadeManager theFade;

    public bool flag; // 이벤트가 한번만 실행되도록.

    // Start is called before the first frame update
    void Start()
    {
        theDM = FindObjectOfType<DialogueManager>();
        TheOrder = FindObjectOfType<OrderManager>();
        thePlayer = FindObjectOfType<PlayerManager>();
        theFade = FindObjectOfType<FadeManager>();
    }

    //표지판이나 이벤트 작동시
    private void OnTriggerStay2D(Collider2D collision)
    {
        
       // Debug.Log("flag가 true고 z키를 누르고있을떄,플레이어가 위를 바라보고있다면");
      
        if(thePlayer.dialogueResetFlag > 10 && Input.GetKey(KeyCode.Z) && thePlayer.animator.GetFloat("DirY") == 1f)
        {
            thePlayer.dialogueResetFlag = 0;
            StartCoroutine(EventCoroutine());
        }
    }

    IEnumerator EventCoroutine()
    {
        TheOrder.NotMove();
        TheOrder.PreLoadCharacter();

        theDM.ShowDialogue(dialogue_1);

        yield return new WaitUntil(() => !theDM.talking); //(조건)대화가 끝나면 이동시키겠다. 
        //TODO : fix (작동안함)
        TheOrder.Move("Player", "UP");
        TheOrder.Move("Player", "DOWN");
        TheOrder.Move("Player", "UP");
        TheOrder.Move("Player", "DOWN");

        yield return new WaitUntil(() => thePlayer.queue.Count == 0); //위에 move 큐가 전부 끝날떄까지 기다림.

        theFade.Flash();
        theDM.ShowDialogue(dialogue_2);

        TheOrder.Move();
    }
}
