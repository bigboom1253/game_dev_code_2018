using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class itemInfo
{
    public int no;
    public string name;
}
public enum test
{
    tt,
}

public class StaticTest
{
    public static itemInfo[] itemInfos = {
        new itemInfo{ no = 1, name = "검", },

    };

    public static Dictionary<test, itemInfo> dicInfo =
    new Dictionary<test, itemInfo>{
        {test.tt , new itemInfo{ } },


    };




}
