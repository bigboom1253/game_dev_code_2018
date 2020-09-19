using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class Title : MonoBehaviour
{
    public bool preventExec;
    public bool titleActivated;
    public int selectedTitleMenu; //선택된 메뉴 1)새로 시작 2)이어하기 3)종료
    public GameObject[] selectedTitlePanel;
    public OrderManager theOrder;
    public GameObject go_title;
    public FadeManager theFade;
    public SaveNLoad theSaveNLoad;
    WaitForSeconds waitTime = new WaitForSeconds(1f);
    
    // Start is called before the first frame update
    void Start()
    {
        theOrder = FindObjectOfType<OrderManager>();
        theFade = FindObjectOfType<FadeManager>();
        theSaveNLoad = FindObjectOfType<SaveNLoad>();
        selectedTitleMenu = 0;
        titleActivated = true;
      
    }

    // Update is called once per frame
    void Update()
    {
        if (titleActivated)
        {
            theOrder.NotMove();
            SelectedMenu(selectedTitlePanel); // 타이틀 메뉴 패널 투명도 전부 0으로 설정 
            
            //첫번째 패널 투명도 0.5로 설정
            Color color = selectedTitlePanel[selectedTitleMenu].GetComponent<Image>().color;
            color.a = 0.5f;
            selectedTitlePanel[selectedTitleMenu].GetComponent<Image>().color = color;

            ControllPanel(selectedTitlePanel); //화살키로 패널 고르기 

            if (Input.GetKeyDown(KeyCode.Z))
            {
                switch (selectedTitleMenu)
                {
                    case 0: //새로 시작하기
                        StartCoroutine(TitleToGame(selectedTitleMenu));
                        break;

                    case 1: //로드하기
                        StartCoroutine(TitleToGame(selectedTitleMenu));
                        break;

                    case 2: //종료
                        Application.Quit();
                        break;
                }

            }

        }
    }

    IEnumerator TitleToGame(int _selectedTitle)
    {
        titleActivated = false;
        theFade.FadeOut();
        yield return waitTime;
        switch (_selectedTitle)
        {
            case 0: //새로시작
                theSaveNLoad.CallNewStart();
                break;
            case 1: //이어하기
                theSaveNLoad.CallLoad();
                break;
        }
        theFade.FadeIn();
        go_title.SetActive(false);
        yield return waitTime;
        theOrder.Move();
    }




    public void ControllPanel(GameObject[] _selectPanel)
    {
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            if (selectedTitleMenu < _selectPanel.Length - 1)
                selectedTitleMenu++;
            else
                selectedTitleMenu = 0;
            SelectedMenu(_selectPanel);
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            if (selectedTitleMenu > 0)
                selectedTitleMenu--;
            else
                selectedTitleMenu = _selectPanel.Length - 1;
            SelectedMenu(_selectPanel);
        }
    }

    public void SelectedMenu(GameObject[] _selectedPanel)
    {
        Color color = _selectedPanel[selectedTitleMenu].GetComponent<Image>().color;
        color.a = 0f;
        for (int i = 0; i < _selectedPanel.Length; i++)
        {
            _selectedPanel[i].GetComponent<Image>().color = color;
        }
        StartCoroutine(SelectedTabEffectCoroutine(_selectedPanel));
    } //선택된 탭을 제외하고 다른 모든 탭의 컬러 알파값을 0으로 조정

    IEnumerator SelectedTabEffectCoroutine(GameObject[] _selectedPanel)// 선택된 탭 알파값 1;
    {

        Color color = _selectedPanel[0].GetComponent<Image>().color;
        color.a = 0.5f;
        _selectedPanel[selectedTitleMenu].GetComponent<Image>().color = color;
        yield return new WaitForSeconds(0.03f);
    }


}
