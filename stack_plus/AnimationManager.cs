using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationManager : MonoBehaviour
{
    public Animator cutAnimator;

    public void CutOn()
    {
        cutAnimator.SetBool("Cut", true);
    }

    public void CutOff()
    {
        cutAnimator.SetBool("Cut", false);
    }

}
