using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum UIState
{
    TitleUI,
    GameUI,
    ScoreUI,
}

public class UIManager : MonoSingleton<UIManager>
{
    UIState state = UIState.TitleUI;

    Transform titleUITrans = null;
    Button titleBtn = null;

    public Transform gameUITrans = null;
    public Text gameScoreText = null;
    public Text gameComboText = null;

    public Transform scoreUITrans = null;
    public Text scoreText = null;
    public Text comboText = null;
    public Button restartBtn = null;
    public Button exitBtn = null;


    private void Start()
    {
        titleUITrans = this.transform.Find("IntroPanel");
        if(titleUITrans == null)
        {
            Debug.Log("IntroPanel is not founded");
            return;
        }

        Transform tempTrans = null;
        tempTrans = titleUITrans.Find("Panel/Button");
        titleBtn = tempTrans?.GetComponent<Button>();

        if(titleBtn == null)
        {
            Debug.Log("title btn is not founded");
            return; 
        }

        // title 버튼을 찾은 경우
        titleBtn.onClick.AddListener(OnClickStartBtnAtTitle);
        //람다 이름없는 메소드, 호출 불가, 델리게이터를 사용해서 저장해줘야함.
        //titleBtn.onClick.AddListener(() =>
        //{
        //    OnClickStartBtnAtTitle();
        //} );

        restartBtn.onClick.AddListener(OnClickRestartBtn);
        exitBtn.onClick.AddListener(delegate { OnClickQuitBtn(); });

        ChangeState(UIState.TitleUI);
    }
    
    void Update()
    {
        
    }
    //state를 변경할때 changeState 이용.
    public void ChangeState(UIState nextState)
    {
        DisableState(state);
        state = nextState;
        EnableState(state);
    }

    void DisableState(UIState curState)
    {
        switch (curState)
        {
            case UIState.TitleUI:
                titleUITrans.gameObject.SetActive(false);
                break;
            case UIState.GameUI:
                gameUITrans.gameObject.SetActive(false);
                break;
            case UIState.ScoreUI:
                scoreUITrans.gameObject.SetActive(false);
                break;
        }
    }

    void EnableState(UIState curState)
    {
        switch (curState)
        {
            case UIState.TitleUI:
                titleUITrans.gameObject.SetActive(true);
                break;
            case UIState.GameUI:
                gameUITrans.gameObject.SetActive(true);
                StackManager.Instance.Restart();
                SetScore(0, 0);
                break;
            case UIState.ScoreUI:
                scoreUITrans.gameObject.SetActive(true);
                break;
        }
    }

    public void OnClickStartBtnAtTitle()
    {
        ChangeState(UIState.GameUI);
        // stackMgr.Restart();
        // Title UI Off
        Debug.Log("on clicked start btn");
    }

    public void SetScore(int score, int combo)
    {
        gameScoreText.text = score.ToString();
        if (combo != 0)
        {
            gameComboText.gameObject.SetActive(true);
            gameComboText.text = combo.ToString();
        }
        else
        {
            gameComboText.gameObject.SetActive(false);
        }
    }
    
    public void OnClickRestartBtn()
    {
        ChangeState(UIState.GameUI);
        Debug.Log("Restart");

    }

    public void OnClickQuitBtn()
    {
        Application.Quit();
    }

    public void SetScorePanel(int score, int combo)
    {
        scoreText.text = score + ""; //자동형변환 더하기에서 큰 타입을 따라감.
        comboText.text = combo.ToString(); //내장되어있는 메소드
    }

}
