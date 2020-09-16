using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCam : MonoBehaviour
{
    public Transform target = null;
    private float offsetX = 0f;

    void Start()
    {
        if (target == null)
            return;

        offsetX = this.transform.position.x - target.position.x;    
    }

    void Update()
    {
        if (target == null)
            return;

        Vector3 pos = transform.position;

        pos.x = target.position.x + offsetX;
        transform.position = pos;
    }
}
