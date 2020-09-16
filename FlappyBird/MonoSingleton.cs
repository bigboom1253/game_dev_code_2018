using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonoSingleton<ManagerType> : MonoBehaviour where ManagerType : MonoBehaviour
{
    private static ManagerType instance = null;
    private static bool bShutDown = false;

    private void OnApplicationQuit()
    {
        //앱이 종료될 때
        bShutDown = true;
    }

    public static ManagerType Instance
    {
        get
        {
            if(bShutDown == true)
                return null;
            

            if(instance == null)
            {
                instance = GameObject.FindObjectOfType<ManagerType>();

                if(instance == null)
                {
                    //씬에 ManagerType이 존재 하지 않습니다.
                    GameObject go = new GameObject(); //create Empty
                    go.name = typeof(ManagerType).ToString();
                    instance = go.AddComponent<ManagerType>();
                }
            }

        
            return instance;
        }
    }

}
