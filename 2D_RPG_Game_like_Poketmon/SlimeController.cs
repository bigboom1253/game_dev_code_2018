using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlimeController : MovingObject
{
    public float attackDelay; // 공격 유예.

    public float inter_MoveWaitTime; // 대기 시간.
    private float current_interMWT;

    public string atkSound;

    private Vector2 playerPos; // 플레이어의 좌표값.

    private int random_int;
    private string direction;

    // Use this for initialization
    void Start()
    {
        queue = new Queue<string>();
        current_interMWT = inter_MoveWaitTime;
    }

    // Update is called once per frame
    void Update()
    {

        current_interMWT -= Time.deltaTime;

        if (current_interMWT <= 0)
        {
            current_interMWT = inter_MoveWaitTime;

            if (NearPlayer())
            {
                Flip();
                return;
            }
            else
            {
                animator.SetBool("Attack", false);
            }

            RandomDirection();

            if (base.CheckColision())
            {
                //queue.Clear();
                return;
            }

            base.Move(direction);
        }

    }

    private void Flip()
    {
        Vector3 flip = transform.localScale;
        if (playerPos.x > this.transform.position.x)
            flip.x = -2.5f;
        else
            flip.x = 2.5f;
        this.transform.localScale = flip;
        animator.SetBool("Attack", true);
        StartCoroutine(WaitCoroutine());
    }

    IEnumerator WaitCoroutine()
    {
        yield return new WaitForSeconds(attackDelay); //애니메이션 공격모션쯤까지 쉼
        AudioManager.instance.Play(atkSound);
        if (NearPlayer())
        {
            PlayerStat.instance.Hit(GetComponent<EnemyStat>().atk);
        }
    }

    private bool NearPlayer()
    {
        playerPos = PlayerManager.instance.transform.position;
        //좌우 거리 파악
        if (Mathf.Abs(Mathf.Abs(playerPos.x) - Mathf.Abs(this.transform.position.x)) <= speed * walkCount * 1.2f)
        {
            if (Mathf.Abs(Mathf.Abs(playerPos.y) - Mathf.Abs(this.transform.position.y)) <= speed * walkCount * 0.5f)
            {
                return true;
            }
        }
        //상하 거리 파악
        if (Mathf.Abs(Mathf.Abs(playerPos.y) - Mathf.Abs(this.transform.position.y)) <= speed * walkCount * 1.2f)
        {
            if (Mathf.Abs(Mathf.Abs(playerPos.x) - Mathf.Abs(this.transform.position.x)) <= speed * walkCount * 0.5f)
            {
                return true;
            }
        }

        return false;
    }

    private void RandomDirection()
    {
        vector.Set(0, 0, vector.z);
        random_int = Random.Range(0, 4);
        switch (random_int)
        {
            case 0:
                vector.y = 1f;
                direction = "UP";
                break;
            case 1:
                vector.y = -1f;
                direction = "DOWN";
                break;
            case 2:
                vector.x = 1f;
                direction = "RIGHT";
                break;
            case 3:
                vector.x = -1f;
                direction = "LEFT";
                break;
        }
    }

}
