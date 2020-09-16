using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoSingleton<UIManager>
{

    public int point = 0;
    public Text scoreText = null;

    //private static UIManager instance = null;

    //public static UIManager Instance
    //{
    //    get
    //    {
    //        if (instance == null)
    //        {
    //            //최초 접근
    //            // 탐색 있으면 쓰고 없으면 만들고
    //            instance = GameObject.FindObjectOfType<UIManager>();
    //            if (instance != null)
    //            {
    //                return instance;
    //            }
    //            else
    //            {
    //                //찾아봤는데 없다.
    //                GameObject go = new GameObject();
    //                go.name = "UIManager";
    //                instance = go.AddComponent<UIManager>();

    //            }
    //        }
    //        return instance;
    //    }
    //}


        //자식을 찾아오는 방법 (transform으로 서로 연결되기 때문에 transform을 기준으로)
    //ver1
    //  Singleton Pattern
    // 쉬운 접근, 단일 객체 유지
    //private static UIManager instance = null;
    //public static UIManager GetInstance()
    //{
    //    return instance;
    //}

    //private void Awake()
    //{
    //    instance = this;
    //}  

    //ver2
    //private static UIManager instance = null;

    //public static UIManager Getinstance()
    //{
    //    if(instance == null)
    //    {
    //        //최초 접근
    //        // 탐색 있으면 쓰고 없으면 만들고
    //        instance = GameObject.FindObjectOfType<UIManager>();
    //        if(instance != null)
    //        {
    //            return instance;
    //        }
    //        else
    //        {
    //            //찾아봤는데 없다.
    //            GameObject go = new GameObject();
    //            instance = go.AddComponent<UIManager>();

    //        }
    //    }

    //    return instance;
    //}

    //private void awake()
    //{
    //    instance = this;
    //}
    void Start()
    {

        //transform.getChild(번호) 0~
        //transform.Find(경로)
        Transform t = this.transform.Find("Text");
        if(t == null)
        {
            Debug.Log("Text Not Founded");
            return;
        }
        scoreText = t.GetComponent<Text>();

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public int PointUP(int point)
    {
        this.point += point;
        scoreText.text = "SCORE : " + this.point;
        Debug.Log("point UP");
        return 0;
    }
}
