using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Dialogue
{
    [TextArea(1,2)] //영역 두줄로 넓힘
    public string[] sentences;
    public Sprite[] sprites;
    public Sprite[] dialogueWindows;
}
