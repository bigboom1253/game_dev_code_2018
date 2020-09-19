using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D; //구버젼 기능 적용

public class MovingObject : MonoBehaviour
{
    public string characterName;

    public float speed = 0.05f;
    public int walkCount = 20;
    protected int currentWalkCount; //한칸이동      


    private bool notCoroutine = false;
    protected Vector3 vector;

    public Queue<string> queue;
    public BoxCollider2D boxCollider;
    public LayerMask layerMask; //통과불가 레이어마스크
    public Animator animator;
    public int dialogueResetFlag;

    public void Move(string _dir, int _frequency = 5) //npc move(큐 방식)
    {

        queue.Enqueue(_dir);
        if(!notCoroutine)
        {
            notCoroutine = true;
            StartCoroutine(MoveCoroutine(_dir, _frequency));
        }
        
    }

    IEnumerator MoveCoroutine(string _dir, int _frequency)
    {
        while(queue.Count != 0)
        {

            switch (_frequency) //무브 코루틴 빈도
            {
                case 1:
                    yield return new WaitForSeconds(4f);
                    break;
                case 2:
                    yield return new WaitForSeconds(3f);
                    break;
                case 3:
                    yield return new WaitForSeconds(2f);
                    break;
                case 4:
                    yield return new WaitForSeconds(1f);
                    break;
                case 5:
                    break;
            }
            if(GetComponentInChildren<NPCManager>() != null)
                yield return new WaitUntil(() => GetComponentInChildren<NPCManager>().npc.npcCanMove == true);

            string direction = queue.Dequeue(); //string 형식의 큐에서 하나 뽑아옴.
       
            vector.Set(0, 0, vector.z);

            switch (direction)
            {
                case "UP":
                    vector.y = 1.0f;
                    break;
                case "DOWN":
                    vector.y = -1.0f;
                    break;
                case "RIGHT":
                    vector.x = 1.0f;
                    break;
                case "LEFT":
                    vector.x = -1.0f;
                    break;


            }
            //animator float 변수 파라미터에 삽입
            animator.SetFloat("DirX", vector.x);
            animator.SetFloat("DirY", vector.y);
            
            //항상 colisionFlag(통과가능한지)확인 후 animation
            while(true)
            {
                bool checkColisionFlag = CheckColision();
                if (checkColisionFlag)
                {
                    animator.SetBool("Walking", false);
                    yield return new WaitForSeconds(1f);
                }
                else
                {
                    break;
                }
            }

            animator.SetBool("Walking", true);

            //방향벡터로 콜라이더 크기 키웠다가, 오프셋 유지. 이동하면 크기줄이고 오프셋 제로. 방식
            //TODO 물어보기(수정전)
            //boxCollider.offset = new Vector2
            //  (vector.x * 0.7f * speed * walkCount, vector.y * 0.7f * speed * walkCount);

            while (currentWalkCount < walkCount) 
            {

                if (vector.x != 0)
                {
                    transform.Translate(vector.x * speed, 0, 0);
                }
                else if (vector.y != 0)
                {
                    transform.Translate(0, vector.y * speed, 0);
                }
                currentWalkCount++;

                //if (currentWalkCount == walkCount * 0.5f + 2) // 9일떄 콜라이더가 원래 자리로 돌아옴                   
                //    boxCollider.offset = Vector2.zero;

                yield return new WaitForSeconds(0.01f);
            }
            currentWalkCount = 0;

            if (_frequency != 5)
                animator.SetBool("Walking", false);
          
        }

        animator.SetBool("Walking", false);
        notCoroutine = false;
    }

    protected bool CheckColision()
    {
        RaycastHit2D hit;

        //hit = null
        //hit = 방해물

        Vector2 start = new Vector2(transform.position.x +vector.x * speed * walkCount,
            transform.position.y + vector.y * speed * walkCount);  //캐릭터 현재위치 값
        Vector2 end = start +
            new Vector2(vector.x * speed, vector.y * speed); //캐릭터가 이동하고자하는 위치값

        boxCollider.enabled = false;
        hit = Physics2D.Linecast(start, end, layerMask);
        boxCollider.enabled = true;

        if (hit.transform != null)
            return true;
        return false;
    }
}
