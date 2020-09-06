using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyZone : MonoBehaviour
{

    private void OnCollisionEnter(Collision collision)
    {
        // Destroy(this);
        // Destroy(collision.collider);

        GameObject target = collision.collider.gameObject;
        // if(target.tag.Equals("Rubble"))
        if(target.name.Equals("Rubble"))
            Destroy(target);
    }

}
