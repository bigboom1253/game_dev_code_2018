using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TransferMapOtherScene : MonoBehaviour
{
    public string transferMapName; //이동할 맵의 이름. 유니티내에서 설정

    public Transform target;
    public BoxCollider2D targetBound; //이동할 바운드 설정
    private CameraManager theCamera;
    private PlayerManager thePlayer;

   
    void Start()
    {
     
        theCamera = FindObjectOfType<CameraManager>();
        thePlayer = FindObjectOfType<PlayerManager>();
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        theCamera.SetBound(targetBound);
        if (collision.gameObject.name == "Player")
        { 
            thePlayer.currentMapName = transferMapName;
            //scene 전환
             SceneManager.LoadScene(transferMapName);
           
        }
    }


}
