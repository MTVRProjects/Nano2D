//================================================//描述 ： //作者 ：//创建时间 ：2018/10/08 17:47:36  //版本： //================================================using System.Collections;using System.Collections.Generic;using UnityEngine;using HMLFramwork;using HMLFramwork.AV;
namespace HMLFramwork{    public class Test : MonoBehaviour    {        void Start()        {
        }        MusicPlayer mp;        MusicPlayer mp1;
        void Update()        {            if (Input.GetKeyDown(KeyCode.Space))            {                mp = new MusicPlayer("G:/Desk/TheFatRat - Unity.mp3");                mp.Play();
                //TimerEvent.Add(2,()=> {                //mp1 = new MusicPlayer("G:/Desk/111.mp3");                //mp1.Play();                //});            }
            if (Input.GetKeyDown(KeyCode.Escape))            {                mp.Stop();/* mp1.Stop();*/            }
        }    }}
