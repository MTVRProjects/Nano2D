using UnityEngine;
using System.Collections;
using HMLFramwork;
using System;
using HMLFramwork.Helpers;

/// <summary>
/// 音频功能基类
/// </summary>
public class AudioFuncBase : AVFuncBase
{
    public AudioClip[] Clips;

    private AudioSource myAudioSource;

    public override int MyOrder
    {
        get
        {
            if (myOrder == -1) myOrder = transform.GetSiblingIndex(); return myOrder;
        }
    }
    /// <summary>
    /// 播放音频
    /// </summary>
    /// <param name="clipIndex"></param>
    public override void Excute(int clipIndex, Action callback = null)
    {
        if (Clips != null && Clips.Length > 0)
        {
            if (clipIndex >= 0 && clipIndex < Clips.Length)
            {
                MyFuncState = ActionState.DOING;
                if (callback != null)
                {
                    callback += () => { MyFuncState = ActionState.DONE; };
                    myAudioSource = AudioUtility.Play(gameObject, Clips[clipIndex], callback);
                }
                else myAudioSource = AudioUtility.Play(gameObject, Clips[clipIndex], () => { MyFuncState = ActionState.DONE; });
            }
            else Debug.Log("警告：音频下标越界...");
        }
        else Debug.Log("警告：Clips没有赋值...");
    }

    public override void Excute(Action callback = null)
    {
        Excute(0, callback);
    }

    /// <summary>
    /// 重复播放
    /// </summary>
    /// <param name="clipIndex"></param>
    public override void RepeatPlay(int clipIndex)
    {
        if (Clips != null && Clips.Length > 0)
        {
            if (clipIndex >= 0 && clipIndex < Clips.Length)
            {
                MyFuncState = ActionState.DOING;
                myAudioSource = AudioUtility.Play(gameObject, Clips[clipIndex], true) ;
            }
            else Debug.Log("警告：音频下标越界...");
        }
        else Debug.Log("警告：Clips没有赋值...");
    }
    /// <summary>
    /// 暂停
    /// </summary>
    public override void Pause()
    {
        base.Pause();
        if (myAudioSource)
        {
            myAudioSource.Pause();
        }
    }

    /// <summary>
    /// 取消暂停
    /// </summary>
    public override void UnPause()
    {
        base.UnPause();
        if (myAudioSource)
        {
            myAudioSource.UnPause();
        }
    }

    /// <summary>
    /// 停止播放
    /// </summary>
    public override void Stop()
    {
        AudioUtility.Stop(gameObject, false);
        base.Stop();
    }

    public override void Stop(int clipIndex)
    {
        base.Stop();
    }

    /// <summary>
    /// 静音
    /// </summary>
    public override void Mute()
    {
        if (MyFuncState == ActionState.DONE) return;
        if (MyFuncState == ActionState.PAUSE) return;
        if (myAudioSource == null) myAudioSource = GetComponent<AudioSource>();
        if (myAudioSource != null) myAudioSource.mute = true;
        else Debug.Log("提醒：音频组件为空...");

        Debug.Log("提醒：开启静音...");
    }

    /// <summary>
    /// 恢复声音状态
    /// </summary>
    public override void RecoverVoice()
    {
        if (MyFuncState == ActionState.DONE) return;
        if (MyFuncState == ActionState.PAUSE) return;
        if (myAudioSource == null) myAudioSource = GetComponent<AudioSource>();
        if (myAudioSource != null) myAudioSource.mute = false;
        else Debug.Log("提醒：音频组件为空...");

        Debug.Log("提醒：关闭静音...");
    }

    public override void Init()
    {
        base.Init();
        myAudioSource = null;
    }
}
