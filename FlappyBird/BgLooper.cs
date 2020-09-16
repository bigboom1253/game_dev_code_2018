using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BgLooper : MonoBehaviour
{
    int numBgCnt = 6;
    public float pipeMin = 0.5f;
    public float pipeMax = 0.5f;

    void Start()
    {
        // Transform -> class , transform -> field
        // GameObject -> class, gameobject -> field
        GameObject[] pipes =
        GameObject.FindGameObjectsWithTag("Pipe");

        for(int i = 0; i < pipes.Length; i++)
        {
            GameObject go = pipes[i];
            Vector3 pos = go.transform.position;
            pos.y = Random.Range(pipeMin, pipeMax);
            go.transform.position = pos;
        }

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("Triggerd : " + collision.name);

        float widthOfBgObject = ((BoxCollider2D)collision).size.x;

        Vector3 pos = collision.transform.position;
        pos.x += widthOfBgObject * numBgCnt;

        // 만약 파이프라면 Y값 랜덤
        if(collision.name.Contains("Pipe"))
        {
            pos.y = Random.Range(pipeMin,pipeMax);
        }

        collision.transform.position = pos;
    }

}
