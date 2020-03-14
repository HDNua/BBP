using System.Collections;
using System.Collections.Generic;
using UnityEngine;



/// <summary>
/// 
/// </summary>
public class EffectRinshanLightning : EffectScript
{
    /// <summary>
    /// 
    /// </summary>
    public AudioClip[] _soundEffectClips;

    /// <summary>
    /// 
    /// </summary>
    protected override void Start()
    {
        base.Start();

        /* 
        foreach (AudioClip clip in _soundEffectClips)
        {
            AttachSound(clip);
        }

        // 
        _audioSource.Play();
        */

        PlayEffectSound(_soundEffectClips[0]);
    }
}
