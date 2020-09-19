using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestDialogue : MonoBehaviour
{
    [SerializeField]
    public Dialogue dialogue;

    private DialogueManager theDM;
    private OrderManager theOrder;
    private PlayerManager thePlayer;
    private NPCManager theNPC;
    private int tempFrequency;
    public bool flag;
    Vector3 temp;

    // Start is called before the first frame update
    void Start()
    {
        theOrder = FindObjectOfType<OrderManager>();
        theDM = FindObjectOfType<DialogueManager>();
        thePlayer = FindObjectOfType<PlayerManager>();
        theNPC = GetComponent<NPCManager>();
    }

    //대화 큐 늘어나는거.
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "Player")
        {
            flag = true;
            StartCoroutine(EventCoroutine());
        }
    }

    IEnumerator EventCoroutine()
    {
        theOrder.NotMove();

        theOrder.PreLoadCharacter();

        theDM.ShowDialogue(dialogue);

        yield return new WaitUntil(() => !theDM.talking); //(조건)대화가 끝나면 이동시키겠다. 

        theOrder.Move();

    }


}
