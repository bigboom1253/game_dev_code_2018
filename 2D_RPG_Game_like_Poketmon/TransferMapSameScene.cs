using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TransferMapSameScene : MonoBehaviour
{

    public Transform target;
    public BoxCollider2D targetBound;

    private CameraManager theCamera;
    private PlayerManager thePlayer;
    private OrderManager theOrder;
    private FadeManager theFade;
    private WaitForSeconds waitTime = new WaitForSeconds(1f);
    void Start()
    {

        theCamera = FindObjectOfType<CameraManager>();
        thePlayer = FindObjectOfType<PlayerManager>();
        theFade = FindObjectOfType<FadeManager>();
        theOrder = FindObjectOfType<OrderManager>();
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        
        if (collision.gameObject.name == "Player")
        {
            StartCoroutine(TransferCoroutine());
        }
    }

    IEnumerator TransferCoroutine()
    {
        theOrder.NotMove();
        theFade.FadeOut();
        yield return waitTime;
        theCamera.SetBound(targetBound);
        theCamera.transform.position = new Vector3(target.transform.position.x,
                                                    target.transform.position.y,
                                                    theCamera.transform.position.z);
        thePlayer.transform.position = target.transform.position;
       //콜라이더 조정
        theFade.FadeIn();
        yield return waitTime;
        theOrder.Move();
    }


}
