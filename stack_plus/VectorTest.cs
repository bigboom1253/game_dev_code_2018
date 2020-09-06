using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VectorTest : MonoBehaviour
{
    public GameObject A = null;
    public GameObject B = null;

    private Transform transA;
    private Transform transB;

    public float distance = 0f;
    public Vector3 nomalVector = Vector3.zero;      // 방향벡터


    void Start()
    {
        transA = A.transform;
        transB = B.transform;
    }

    // Update is called once per frame
    void Update()
    {
        // B - A -> A에서 B를 향하는 벡터
        Vector3 pos = transB.position - transA.position;
        distance = pos.magnitude;       // pos.sqrMagnitude
        nomalVector = pos.normalized;
        // nomalVector = pos / distance;

        if (distance > 0.1f)
        {
            transA.position += nomalVector * Time.deltaTime;    // 이동
            // transA.LookAt(transB.position);
            transA.LookAt(transA.position + nomalVector);
        }

    }

    private void OnDrawGizmos()
    {
        if (Application.isPlaying)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transA.position, transA.position + nomalVector * 2f);
        }
    }


}
