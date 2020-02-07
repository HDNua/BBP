using System.Collections;
using System.Collections.Generic;
using UnityEngine;



/// <summary>
/// 디버거 개체입니다.
/// </summary>
public class Debugger : MonoBehaviour
{
    public AnimationClip[] animationClips;

    // Use this for initialization
    void Start()
    {
        foreach (AnimationClip clip in animationClips)
        {
            //Debug.Log(clip.length);
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
