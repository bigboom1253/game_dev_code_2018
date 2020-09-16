using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScorePoint : MonoBehaviour
{
    UIManager uiMgr = null;
    void Start()
    {
        //uiMgr = UIManager.Getinstance();
        uiMgr = UIManager.Instance;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        uiMgr.PointUP(1);
    }


}
