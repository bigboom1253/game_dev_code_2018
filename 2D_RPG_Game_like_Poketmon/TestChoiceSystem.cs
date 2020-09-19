using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class TestChoiceSystem : MonoBehaviour
{
    [SerializeField]
    public Choice choice;

    public Dialogue dialogue1; // 선택지에 따라 다른 대화창이 뜨도록 설정.

    private OrderManager theOrder;
    private ChoiceManager theChoice;

    public bool flag;

    // Start is called before the first frame update
    void Start()
    {
        theOrder = FindObjectOfType<OrderManager>();
        theChoice = FindObjectOfType<ChoiceManager>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(!flag)
        {
            StartCoroutine(ACoroutine());
        }
    }
    
    IEnumerator ACoroutine()
    {
        flag = true;
        theOrder.NotMove();
        theChoice.ShowChoice(choice);
        yield return new WaitUntil(() => !theChoice.choiceIng);

        theOrder.Move();
        Debug.Log(theChoice.GetResult());
    }
}
