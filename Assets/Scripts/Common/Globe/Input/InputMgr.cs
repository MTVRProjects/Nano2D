using System;
using System.Collections.Generic;
using UnityEngine;
using HMLFramwork;

namespace HMLFramwork
{
    public class InputMgr
    {

        static InputMgr _instance;

        public static InputMgr Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new InputMgr();
                    UpdateCenter.Add(_instance.InputUpdate);
                }
                return _instance;
            }
        }

        /// <summary>
        /// 鼠标右键一直按下
        /// </summary>
        public event Action GetMouse_R = () => { };
        /// <summary>
        /// 鼠标左键一直按下
        /// </summary>
        public event Action GetMouse_L = () => { };
        /// <summary>
        /// 鼠标滚轮一直按下
        /// </summary>
        public event Action GetMouse_W = () => { };

        /// <summary>
        /// 鼠标右键按下时
        /// </summary>
        public event Action MouseDown_R = () => { };
        /// <summary>
        /// 鼠标左键按下时
        /// </summary>
        public event Action MouseDown_L = () => { };
        /// <summary>
        /// 鼠标滚轮按下时
        /// </summary>
        public event Action MouseDown_W = () => { };

        /// <summary>
        /// 鼠标右键抬起时
        /// </summary>
        public event Action MouseUP_R = () => { };
        /// <summary>
        /// 鼠标左键抬起时
        /// </summary>
        public event Action MouseUP_L = () => { };
        /// <summary>
        /// 鼠标滚轮抬起时
        /// </summary>
        public event Action MouseUP_W = () => { };

        //Dictionary<InputType, List<Action>> eventDic = new Dictionary<InputType, List<Action>>();
        public void BindEvent(InputType inputType, int mouseBnID, Delegate mouseFunc)
        {
            if (mouseFunc != null && mouseFunc.GetType().Equals(typeof(Action)))
            {
                Action func = (Action)mouseFunc;
                switch (inputType)
                {
                    case InputType.GetMouse:
                        switch (mouseBnID)
                        {
                            case 0: GetMouse_L += func; break;
                            case 1: GetMouse_R += func; break;
                            case 2: GetMouse_W += func; break;
                            default: break;
                        }
                        break;
                    case InputType.MouseDown:
                        switch (mouseBnID)
                        {
                            case 0: MouseDown_L += func; break;
                            case 1: MouseDown_R += func; break;
                            case 2: MouseDown_W += func; break;
                            default: break;
                        }
                        break;
                    case InputType.MouseUP:
                        switch (mouseBnID)
                        {
                            case 0: GetMouse_L += func; break;
                            case 1: GetMouse_R += func; break;
                            case 2: GetMouse_W += func; break;
                            default: break;
                        }
                        break;
                    case InputType.KeyDown:
                        switch (mouseBnID)
                        {
                            case 0: MouseUP_L += func; break;
                            case 1: MouseUP_R += func; break;
                            case 2: MouseUP_W += func; break;
                            default: break;
                        }
                        break;
                    case InputType.GetKey:

                        break;
                    case InputType.KeyUP:

                        break;
                    default:
                        break;
                }
            }

        }
        public void BindEvent(EventPara ep)
        {
            if (ep != null && ep.Paras.Length == 2 && ep[0].GetType().Equals(typeof(InputType)) && ep[1].GetType().Equals(typeof(int)) && ep[2].GetType().Equals(typeof(Delegate)))
            {
                InputType inputType = (InputType)ep[0];
                int mouseBnID = (int)ep[1];
                Delegate func = (Delegate)ep[2];
                BindEvent(inputType, mouseBnID, func);
            }
            else Debug.Log("警告：参数有误...");
        }

        public void Start()
        {
            EventCenter.Ins.Add(EventSign.BindMouseEvent, BindEvent);
        }

        public void InputUpdate()
        {
            if (Input.GetMouseButton(0)) GetMouse_L();
            if (Input.GetMouseButton(1)) GetMouse_R();
            if (Input.GetMouseButton(2)) GetMouse_W();

            if (Input.GetMouseButtonDown(0)) MouseDown_L();
            if (Input.GetMouseButtonDown(1)) MouseDown_R();
            if (Input.GetMouseButtonDown(2)) MouseDown_W();

            if (Input.GetMouseButtonUp(0)) MouseUP_L();
            if (Input.GetMouseButtonUp(1)) MouseUP_R();
            if (Input.GetMouseButtonUp(2)) MouseUP_W();
        }
    }
}