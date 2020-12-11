using HMLFramwork.Singleton;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnAccessibleMsgMgr : MonoSingleton<UnAccessibleMsgMgr>
{
    Animator animator;
    int ani_id;

    GameObject child;
    void Start()
    {
        animator = GetComponentInChildren<Animator>();
        child = transform.GetChild(0).gameObject;
        child.SetActive(false);
    }

    bool panel_state = false;
    public void Open(bool open)
    {
        if (panel_state == open) return;
        child.SetActive(open);
        panel_state = open;
        animator.speed = open ? 1 : -1;
        animator.SetTrigger("Play");
    }
}
