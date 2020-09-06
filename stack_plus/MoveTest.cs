using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 이 컴포넌트는 Rigidbody가 필수
[RequireComponent(typeof(Rigidbody))]      
public class MoveTest : MonoBehaviour
{
    Vector3 moveDir = Vector3.zero;
    Rigidbody rigid = null;
    public float moveSpeed = 10f;
    public float jumpForce = 100f;
    public bool isGround = false;

    void Start()
    {
        rigid = this.GetComponent<Rigidbody>();
    }

    
    void Update()
    {
        float x = Input.GetAxis("Horizontal");
        float y = 0;                // Input.GetAxis("Vertical");
        if(x != 0 || y != 0)        // x가 0이 아니고 (또는) y가 0이 아니면
            Debug.Log("X : " + x + " Y : " + y);

        moveDir = new Vector3(x, y);
        // moveDir.Nomalize(); -> moveDir = moveDir.normalized;
        // NomalVector 방향벡터 노말벡터 법선벡터
        // 크기가 1인 벡터 -> 방향만을 의미한다.

        // #1
        //Vector3 pos = this.transform.position;
        //pos += moveDir.normalized * Time.deltaTime;
        //this.transform.position = pos;

        // #2
        //this.transform.Translate(moveDir.normalized * Time.deltaTime);

        // #3
        // Raycaset
        RaycastHit hit;
        // out 메서드 내부에서 한번의 대입은 필수
        // ref 참조의 형태로 변환
        // Ray ray = new Ray()
        if (Physics.Raycast(transform.position, Vector3.down,
                                                    out hit, 0.7f))
            isGround = true;
        else
            isGround = false;

        rigid.useGravity = !isGround;

        moveDir.Normalize();
        if (Input.GetKeyDown(KeyCode.Space))
            moveDir += Vector3.up * jumpForce;

        rigid.AddForce(moveDir * moveSpeed);
    }
}
