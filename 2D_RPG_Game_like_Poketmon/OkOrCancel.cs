using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public delegate void onClickBtn();

public class OkOrCancel : MonoBehaviour
{
    private AudioManager theAudio;
    public string key_sound;
    public string enter_sound;
    public string cancel_sound;

    public GameObject ok_Panel;
    public GameObject cancel_Panel;

    public Text ok_Text;
    public Text cancel_Text;

    public bool activated;
    private bool keyInput;
    private bool result = true;

    onClickBtn ok;
    onClickBtn cancel;

    void Start()
    {
        theAudio = FindObjectOfType<AudioManager>();

    }

    public void Selected()
    {
        theAudio.Play(key_sound);
        result = !result;

        if (result)
        {
            ok_Panel.gameObject.SetActive(false);
            cancel_Panel.gameObject.SetActive(true);
        }
        else
        {
            ok_Panel.gameObject.SetActive(true);
            cancel_Panel.gameObject.SetActive(false);
        }
    }

    public void ShowTwoChoice(string _okText, string _cancelText, onClickBtn okEvent, onClickBtn cancelEvent) // 초이스 
    {
        ok = okEvent;
        cancel = cancelEvent;

        activated = true;
        result = true;
        ok_Text.text = _okText;
        cancel_Text.text = _cancelText;

        ok_Panel.gameObject.SetActive(false); //보여짐
        cancel_Panel.gameObject.SetActive(true); // 안보여짐

        StartCoroutine(ShowTwoChoiceCoroutine());
    }

    public bool GetResult()
    {
        return result;
    }


    IEnumerator ShowTwoChoiceCoroutine() //유예시간을 통한 중복키 처리 방지
    {
        yield return new WaitForSeconds(0.01f);
        keyInput = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (keyInput)
        {
            if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                Selected();
            }
            else if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                Selected();
            }
            else if (Input.GetKeyDown(KeyCode.Z))
            {
                theAudio.Play(enter_sound);
                keyInput = false;
                activated = false;
                ok();
            }
            else if (Input.GetKeyDown(KeyCode.X))
            {
                theAudio.Play(cancel_sound);
                keyInput = false;
                activated = false;
                result = false;
                cancel();
            }
        }
    }
}
