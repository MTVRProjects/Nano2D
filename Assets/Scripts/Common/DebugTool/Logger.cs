using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using HMLFramwork;
using System;
using HMLFramwork.Helpers;

namespace HMLFramwork.Log
{
    public class Logger : DontDestroyGO<Logger>
    {

        static bool _enableLog = false;
        public static bool enableLog { set { if (value) CreateDontDestroyGO(); _enableLog = value; Debug.unityLogger.logEnabled = value; } get { return _enableLog; } }

        static Dictionary<string, Color> msgDic = new Dictionary<string, Color>();
        void Start()
        {
            if (!enableLog) return;
            //在这里做一个Log的监听  
            Application.logMessageReceived += getLog;
            Debug.Log("启动Log");
        }

        List<string> msgsTemp;
        /// <summary>
        /// 获取Log信息
        /// </summary>
        /// <param name="logString"></param>
        /// <param name="stackTrace"></param>
        /// <param name="type"></param>
        void getLog(string logString, string stackTrace, LogType type)
        {
            string msg = StringHelper.Package("<", DateTime.Now.ToString("HH:mm:ss"), ">", type.ToString(), "：", logString);
            switch (type)
            {
                case LogType.Error: msgDic.Add(msg, Color.red); break;
                case LogType.Assert: break;
                case LogType.Warning: msgDic.Add(msg, Color.yellow); break;
                case LogType.Log: msgDic.Add(msg, Color.white); break;
                case LogType.Exception: msgDic.Add(msg, Color.green); break;
            }
            msgsTemp = new List<string>(msgDic.Keys);
            if (Application.isPlaying)
            {
                if (msgsTemp.Count > 30) msgDic.Remove(msgsTemp[0]);
            }
        }

        List<string> msgs;

        void OnGUI()
        {
            if (!enableLog) return;

            msgs = new List<string>(msgDic.Keys);
            GUIStyle gs = new GUIStyle();
            gs.fontSize = 20;
            gs.normal.background = null;
            for (int i = 0, imax = msgDic.Count; i < imax; ++i)
            {
                gs.normal.textColor = msgDic[msgs[i]];
                GUILayout.Label(msgs[i], gs);
            }
            if (GUI.Button(new Rect(Screen.width - 80, Screen.height - 40, 60, 30), "Close")) enableLogFunc(false);
            if (GUI.Button(new Rect(Screen.width - 200, Screen.height - 40, 60, 30), "Clear")) msgDic.Clear();
        }

        public void Init()
        {
            msgDic.Clear();
        }

        void enableLogFunc(bool _enableLog)
        {
            Debug.unityLogger.logEnabled = _enableLog;
            CreateDontDestroyGO();
            if (_enableLog)
            {
                enableLog = _enableLog;
            }
            else
            {
                GameObject.DestroyImmediate(this);
            }
        }

        void OnDestory()
        {
            if (!enableLog) return;
            Application.logMessageReceived -= getLog;
        }
        void OnApplicationQuit()
        {
            if (!enableLog) return;
            Debug.Log("提醒：程序退出...");
        }
    }
}

