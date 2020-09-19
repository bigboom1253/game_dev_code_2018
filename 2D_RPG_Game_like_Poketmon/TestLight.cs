using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestLight : MonoBehaviour
{
    public GameObject go;

    private bool flag = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!flag && Input.GetKey(KeyCode.Z))
        {
            flag = true;
            go.SetActive(true);
        }
        else if(flag && Input.GetKey(KeyCode.Z)) //rihgt off
        {
            go.SetActive(false);
            flag = false;
        }

    }
}
