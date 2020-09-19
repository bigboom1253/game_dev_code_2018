using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FloatingText : MonoBehaviour
{
    public float moveSpeed;
    public float destroyTime;

    public Text text;

    private Vector3 vector;
    private Vector3 scale;

    // Update is called once per frame
    void Update()
    {
        //movespeed만큼 위로 이동.
        vector.Set(text.transform.position.x, text.transform.position.y + (moveSpeed * Time.deltaTime), text.transform.position.z);
        scale.Set(1, 1, 1);
        text.transform.localScale = scale;
        text.transform.position = vector;

        destroyTime -= Time.deltaTime;

        if (destroyTime <= 0)
            Destroy(this.gameObject);
    }
}
