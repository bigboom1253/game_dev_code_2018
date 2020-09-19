using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestScript1 : MonoBehaviour
{
    BGMManager BGM;
    public int playMusicTrack;

    // Start is called before the first frame update
    void Start()
    {
        BGM = FindObjectOfType<BGMManager>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        BGM.Play(playMusicTrack);
        BGM.SetVolumn(0.1f);
        //활성화를 꺼줌.
        this.gameObject.SetActive(false);
    }
}
