using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerBird : MonoBehaviour
{
    private Animator playerAnim = null;
    private Rigidbody2D rigid = null;

    Vector2 moveForce = Vector2.zero;
    public float flapForce = 100f;
    public float forwardSpeed = 1f;

    public bool isFlap = false;

    public bool godMode = false;
    bool isDead = false;
    float deadCoolTime = 0f;

    void Start()
    {
        rigid = this.GetComponent<Rigidbody2D>();
        playerAnim = this.GetComponentInChildren<Animator>();
    }

    
    void Update()
    {
        if (isDead)
        {
            // 재시작
            if (deadCoolTime <= 0)
            {
                if (Input.GetMouseButtonDown(0))
                    SceneManager.LoadScene(0);
            }
            else
            {
                deadCoolTime -= Time.deltaTime;
            }
        }
        else
        {
            if (Input.GetMouseButtonDown(0))
            {
                Debug.Log("Mouse Click");
                isFlap = true;
            }
        }
    }

    private void FixedUpdate()
    {
        if (isDead == true)
            return;

        moveForce = Vector2.right * forwardSpeed;

        if(isFlap == true)
        {
            moveForce += Vector2.up * flapForce;
            isFlap = false;
        }

        rigid.AddForce(moveForce);

        if(rigid.velocity.y > 0)
        {
            transform.rotation = Quaternion.identity;
        }
        else
        {
            float angle = Mathf.Lerp(0, -90, (-rigid.velocity.y / 3f) );
            transform.rotation = Quaternion.Euler(0, 0, angle);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (godMode == true)
            return;

        playerAnim.SetInteger("IsDead", 1);
        isDead = true;
        deadCoolTime = 1f;

    }




}
