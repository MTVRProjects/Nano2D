//================================================
//描述 ： 
//作者 ：
//创建时间 ：2020/11/18 09:44:25  
//版本： 
//================================================
using MiaoTech.VR;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace MiaoTech.VR
{
    public class MTPointerInput : BaseInput
    {
        public Camera eventCamera;
        public ButtonType clickButton = ButtonType.Trigger;
        public HandType handType = HandType.Right;

        protected override void Awake()
        {
            GetComponent<BaseInputModule>().inputOverride = this;
        }

        public override bool GetMouseButton(int button)
        {
            //return MTVRInput.Ins.GetPress(handType, clickButton);
            return OVRInput.Get(OVRInput.Button.PrimaryIndexTrigger, OVRInput.Controller.RTouch);
        }

        public override bool GetMouseButtonDown(int button)
        {
            return MTVRInput.GetPressDown(handType, clickButton);
            //return OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger, OVRInput.Controller.RTouch);
        }

        public override bool GetMouseButtonUp(int button)
        {
            //return MTVRInput.Ins.GetPressUp(handType, clickButton);
            return OVRInput.GetUp(OVRInput.Button.PrimaryIndexTrigger, OVRInput.Controller.RTouch);
        }

        public override Vector2 mousePosition
        {
            get
            {
                return new Vector2(eventCamera.pixelWidth / 2, eventCamera.pixelHeight / 2);
            }
        }
    }
}
