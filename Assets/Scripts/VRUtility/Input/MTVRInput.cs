//================================================
//描述 ： 
//作者 ：
//创建时间 ：2020/11/18 16:38:00  
//版本： 
//================================================
using HMLFramwork.Res;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
#if OVR

#elif VIVE

#elif WVR
using wvr;

#elif HVR
    
#elif PVR
//支持Unity2017.4到Unity 2019.3.6。
using Pvr_UnitySDKAPI;

#endif

namespace MiaoTech.VR
{
    public enum HandType
    {
        Left,

        Right,

        Single
    }
    /// <summary>
    /// 按钮类型
    /// </summary>
    public enum ButtonType
    {
        Trigger,

        Pad,

        Grab,

        Menu
    }

    public class MTVRInput
    {
        #region Singlon

        static MTVRInput _ins;
        public static MTVRInput Ins
        {
            get
            {
                if (_ins == null) _ins = new MTVRInput();
                return _ins;
            }
        }

        #endregion


#if OVR
        OVRCameraRig ovr_cam = null;
        public OVRCameraRig OVRCam
        {
            get
            {
                if (ovr_cam == null)
                {
                    ovr_cam = GameObject.FindObjectOfType<OVRCameraRig>();
                }
                return ovr_cam;
            }
        }

        IEnumerator OVR_Shake(float frequency, float amplitude, float duration, OVRInput.Controller controller)
        {
            OVRInput.SetControllerVibration(frequency, amplitude, controller);
            yield return new WaitForSeconds(duration);
            OVRInput.SetControllerVibration(0, 0, controller);
        }
#elif VIVE

#elif WVR

        public (WaveVR_Controller.EDeviceType, WaveVR_Controller.EDeviceType) getLeftAndRight()
        {
            WaveVR_Controller.EDeviceType device_type_left, device_type_right;
            if (WaveVR_Controller.IsLeftHanded)
            {
                device_type_left = WaveVR_Controller.EDeviceType.Dominant;
                device_type_right = WaveVR_Controller.EDeviceType.NonDominant;
            }
            else
            {
                device_type_left = WaveVR_Controller.EDeviceType.NonDominant;
                device_type_right = WaveVR_Controller.EDeviceType.Dominant;
            }
            return (device_type_left, device_type_right);
        }

        public WaveVR_Controller.Device getDevice(HandType handType)
        {
            (WaveVR_Controller.EDeviceType, WaveVR_Controller.EDeviceType) left_and_right = getLeftAndRight();
            switch (handType)
            {
                case HandType.Left:
                    return WaveVR_Controller.Input(left_and_right.Item1);
                case HandType.Right:
                    return WaveVR_Controller.Input(left_and_right.Item2);
                case HandType.Single:
                    return WaveVR_Controller.Input(left_and_right.Item1);
                default: return null;
            }

        }
#elif HVR
    
#elif PVR


#endif

        Transform vr_cam_root = null;
        public Transform VRCamRoot
        {
            get
            {
                if (vr_cam_root == null)
                {
#if OVR
                    vr_cam_root = GameObject.FindObjectOfType<OVRPlayerController>().transform;
                    if (vr_cam_root == null) vr_cam_root = GameObject.FindObjectOfType<OVRCameraRig>().transform;
#elif VIVE

#elif WVR
                    vr_cam_root = GameObject.FindObjectOfType<WaveVR_Render>().transform.parent;
#elif HVR
    
#elif PVR
                    vr_cam_root = GameObject.FindObjectOfType<Pvr_UnitySDKHeadTrack>().transform.parent;
#elif EDITOR_TEST && UNITY_EDITOR
                    vr_cam_root = VRCam;
#endif
                }
                return vr_cam_root;
            }
        }

        Transform vr_cam = null;
        public Transform VRCam
        {
            get
            {
                if (vr_cam_root == null)
                {
#if OVR
                    vr_cam = GameObject.FindObjectOfType<OVRScreenFade>().transform;
#elif VIVE

#elif WVR
                    vr_cam = GameObject.FindObjectOfType<WaveVR_Render>().transform;
#elif HVR
    
#elif PVR
                    vr_cam = GameObject.FindObjectOfType<Pvr_UnitySDKHeadTrack>().transform;
#elif EDITOR_TEST && UNITY_EDITOR
                    var cam = GameObject.Find("VRTestCam");
                    if (cam == null)
                    {
                        vr_cam = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/VRUtility/VRUtility/Res/Prefabs/VRTestCam.prefab").transform;
                    }
                    vr_cam = cam.transform;
#endif
                }
                return vr_cam;
            }
        }

        GameObject left_hand = null;
        public GameObject LeftHand
        {
            get
            {
                if (left_hand == null)
                {
#if OVR
                    left_hand = OVRCam.transform.Find("TrackingSpace/LeftHandAnchor").gameObject;
#elif VIVE

#elif WVR
                    var device_type = WaveVR_Controller.IsLeftHanded ? WaveVR_Controller.EDeviceType.Dominant : WaveVR_Controller.EDeviceType.NonDominant;
                    left_hand = WaveVR_EventSystemControllerProvider.Instance.GetControllerModel(device_type);
#elif HVR
    
#elif PVR
                    left_hand = GameObject.FindObjectOfType<Pvr_ControllerManager>().transform.GetChild(0).gameObject;


#elif EDITOR_TEST
                    left_hand = VRCamRoot.Find("Body/LeftHand").gameObject;
#endif
                }
                return left_hand;
            }
        }

        GameObject right_hand = null;
        public GameObject RightHand
        {
            get
            {
                if (right_hand == null)
                {
#if OVR
                    right_hand = OVRCam.transform.Find("TrackingSpace/RightHandAnchor").gameObject;
#elif VIVE

#elif WVR
                    var device_type = WaveVR_Controller.IsLeftHanded ? WaveVR_Controller.EDeviceType.NonDominant : WaveVR_Controller.EDeviceType.Dominant;
                    right_hand = WaveVR_EventSystemControllerProvider.Instance.GetControllerModel(device_type);
#elif HVR
    
#elif PVR
                    right_hand = GameObject.FindObjectOfType<Pvr_ControllerManager>().transform.GetChild(1).gameObject;

#elif EDITOR_TEST
                    right_hand = VRCamRoot.Find("Body/RightHand").gameObject;
#endif
                }

                return right_hand;
            }
        }

        /// <summary>
        /// 按住手柄按钮
        /// </summary>
        /// <param name="handType">手柄类型：左、右手柄</param>
        /// <param name="buttonType">按钮类型</param>
        /// <returns></returns>
        public static bool GetPress(HandType handType, ButtonType buttonType)
        {
            bool press = false;
#if OVR
            switch (handType)
            {
                case HandType.Left:
                    switch (buttonType)
                    {
                        case ButtonType.Trigger:
                            press = OVRInput.Get(OVRInput.Button.PrimaryIndexTrigger, OVRInput.Controller.All);
                            break;
                        case ButtonType.Pad:
                            press = OVRInput.Get(OVRInput.Button.PrimaryTouchpad, OVRInput.Controller.All);
                            break;
                        case ButtonType.Grab:
                            press = OVRInput.Get(OVRInput.Button.PrimaryHandTrigger, OVRInput.Controller.All);
                            break;
                        case ButtonType.Menu:
                            press = OVRInput.Get(OVRInput.Button.Start);
                            break;
                    }
                    break;
                case HandType.Right:
                    switch (buttonType)
                    {
                        case ButtonType.Trigger:
                            press = OVRInput.Get(OVRInput.Button.SecondaryIndexTrigger, OVRInput.Controller.All);
                            break;
                        case ButtonType.Pad:
                            press = OVRInput.Get(OVRInput.Button.SecondaryTouchpad, OVRInput.Controller.All);
                            break;
                        case ButtonType.Grab:
                            press = OVRInput.Get(OVRInput.Button.SecondaryHandTrigger, OVRInput.Controller.All);
                            break;
                        case ButtonType.Menu:
                            press = OVRInput.Get(OVRInput.Button.Start);
                            break;
                    }
                    break;

            }
#elif VIVE

#elif WVR
            switch (buttonType)
            {
                case ButtonType.Trigger:
                    press = getDevice(handType).GetPress(WVR_InputId.WVR_InputId_Alias1_Trigger);
                    break;
                case ButtonType.Pad:
                    press = getDevice(handType).GetPress(WVR_InputId.WVR_InputId_Alias1_Touchpad);
                    break;
                case ButtonType.Grab:
                    press = getDevice(handType).GetPress(WVR_InputId.WVR_InputId_Alias1_Grip);
                    break;
                case ButtonType.Menu:
                    press = getDevice(handType).GetPress(WVR_InputId.WVR_InputId_Alias1_Menu);
                    break;
            }
#elif HVR

#elif PVR
            switch (buttonType)
            {
                case ButtonType.Trigger:
                    press = Pvr_UnitySDKAPI.Controller.UPvr_GetKey((int)handType, Pvr_KeyCode.TRIGGER);
                    break;
                case ButtonType.Pad:
                    press = Pvr_UnitySDKAPI.Controller.UPvr_GetKey((int)handType, Pvr_KeyCode.TOUCHPAD);
                    break;
                case ButtonType.Grab:
                    press = Pvr_UnitySDKAPI.Controller.UPvr_GetKey((int)handType, (int)handType == 0 ? Pvr_KeyCode.Left : Pvr_KeyCode.Right);
                    break;
                case ButtonType.Menu:
                    press = Pvr_UnitySDKAPI.Controller.UPvr_GetKey((int)handType, Pvr_KeyCode.HOME);
                    break;
            }

#endif

#if UNITY_EDITOR && EDITOR_TEST
            switch (buttonType)
            {
                case ButtonType.Trigger:
                    press = Input.GetKey(KeyCode.T);
                    break;
                case ButtonType.Pad:
                    press = Input.GetKey(KeyCode.P);
                    break;
                case ButtonType.Grab:
                    press = Input.GetKey(KeyCode.G);
                    break;
                case ButtonType.Menu:
                    press = Input.GetKey(KeyCode.M);
                    break;
            }
#endif
            return press;
        }

        /// <summary>
        /// 按下后松开手柄按钮
        /// </summary>
        /// <param name="handType">手柄类型：左、右手柄</param>
        /// <param name="buttonType">按钮类型</param>
        /// <returns></returns>
        public static bool GetPressUp(HandType handType, ButtonType buttonType)
        {
            bool press_up = false;
#if OVR
            switch (handType)
            {
                case HandType.Left:
                    switch (buttonType)
                    {
                        case ButtonType.Trigger:
                            press_up = OVRInput.GetUp(OVRInput.Button.PrimaryIndexTrigger, OVRInput.Controller.All);
                            break;
                        case ButtonType.Pad:
                            press_up = OVRInput.GetUp(OVRInput.Button.PrimaryTouchpad, OVRInput.Controller.All);
                            break;
                        case ButtonType.Grab:
                            press_up = OVRInput.GetUp(OVRInput.Button.PrimaryHandTrigger, OVRInput.Controller.All);
                            break;
                        case ButtonType.Menu:
                            press_up = OVRInput.GetUp(OVRInput.Button.Start);
                            break;
                    }
                    break;
                case HandType.Right:
                    switch (buttonType)
                    {
                        case ButtonType.Trigger:
                            press_up = OVRInput.GetUp(OVRInput.Button.SecondaryIndexTrigger, OVRInput.Controller.All);
                            break;
                        case ButtonType.Pad:
                            press_up = OVRInput.GetUp(OVRInput.Button.SecondaryTouchpad, OVRInput.Controller.All);
                            break;
                        case ButtonType.Grab:
                            press_up = OVRInput.GetUp(OVRInput.Button.SecondaryHandTrigger, OVRInput.Controller.All);
                            break;
                        case ButtonType.Menu:
                            press_up = OVRInput.GetUp(OVRInput.Button.Start);
                            break;
                    }
                    break;

            }

#elif VIVE

#elif WVR
            switch (buttonType)
            {
                case ButtonType.Trigger:
                    press_up = getDevice(handType).GetPressUp(WVR_InputId.WVR_InputId_Alias1_Trigger);
                    break;
                case ButtonType.Pad:
                    press_up = getDevice(handType).GetPressUp(WVR_InputId.WVR_InputId_Alias1_Touchpad);
                    break;
                case ButtonType.Grab:
                    press_up = getDevice(handType).GetPressUp(WVR_InputId.WVR_InputId_Alias1_Grip);
                    break;
                case ButtonType.Menu:
                    press_up = getDevice(handType).GetPressUp(WVR_InputId.WVR_InputId_Alias1_Menu);
                    break;
            }
#elif HVR

#elif PVR
            switch (buttonType)
            {
                case ButtonType.Trigger:
                    press_up = Pvr_UnitySDKAPI.Controller.UPvr_GetKeyUp((int)handType, Pvr_KeyCode.TRIGGER);
                    break;
                case ButtonType.Pad:
                    press_up = Pvr_UnitySDKAPI.Controller.UPvr_GetKeyUp((int)handType, Pvr_KeyCode.TOUCHPAD);
                    break;
                case ButtonType.Grab:
                    press_up = Pvr_UnitySDKAPI.Controller.UPvr_GetKeyUp((int)handType, (int)handType == 0 ? Pvr_KeyCode.Left : Pvr_KeyCode.Right);
                    break;
                case ButtonType.Menu:
                    press_up = Pvr_UnitySDKAPI.Controller.UPvr_GetKeyUp((int)handType, Pvr_KeyCode.HOME);
                    break;
            }
#endif

#if UNITY_EDITOR && EDITOR_TEST
            switch (buttonType)
            {
                case ButtonType.Trigger:
                    press_up = Input.GetKeyUp(KeyCode.T);
                    break;
                case ButtonType.Pad:
                    press_up = Input.GetKeyUp(KeyCode.P);
                    break;
                case ButtonType.Grab:
                    press_up = Input.GetKeyUp(KeyCode.G);
                    break;
                case ButtonType.Menu:
                    press_up = Input.GetKeyUp(KeyCode.M);
                    break;
            }
#endif
            return press_up;
        }

        /// <summary>
        /// 按下手柄按钮
        /// </summary>
        /// <param name="handType">手柄类型：左、右手柄</param>
        /// <param name="buttonType">按钮类型</param>
        /// <returns></returns>
        public static bool GetPressDown(HandType handType, ButtonType buttonType)
        {
            bool press_down = false;
#if OVR
            switch (handType)
            {
                case HandType.Left:
                    switch (buttonType)
                    {
                        case ButtonType.Trigger:
                            press_down = OVRInput.GetUp(OVRInput.Button.PrimaryIndexTrigger, OVRInput.Controller.All);
                            break;
                        case ButtonType.Pad:
                            press_down = OVRInput.GetUp(OVRInput.Button.PrimaryTouchpad, OVRInput.Controller.All);
                            break;
                        case ButtonType.Grab:
                            press_down = OVRInput.GetUp(OVRInput.Button.PrimaryHandTrigger, OVRInput.Controller.All);
                            break;
                        case ButtonType.Menu:
                            press_down = OVRInput.GetUp(OVRInput.Button.Start);
                            break;
                    }
                    break;
                case HandType.Right:
                    switch (buttonType)
                    {
                        case ButtonType.Trigger:
                            press_down = OVRInput.GetUp(OVRInput.Button.SecondaryIndexTrigger, OVRInput.Controller.All);
                            break;
                        case ButtonType.Pad:
                            press_down = OVRInput.GetUp(OVRInput.Button.SecondaryTouchpad, OVRInput.Controller.All);
                            break;
                        case ButtonType.Grab:
                            press_down = OVRInput.GetUp(OVRInput.Button.SecondaryHandTrigger, OVRInput.Controller.All);
                            break;
                        case ButtonType.Menu:
                            press_down = OVRInput.GetUp(OVRInput.Button.Start);
                            break;
                    }
                    break;
            }

#elif VIVE

#elif WVR
            switch (buttonType)
            {
                case ButtonType.Trigger:
                    press_down = getDevice(handType).GetPressDown(WVR_InputId.WVR_InputId_Alias1_Trigger);
                    break;
                case ButtonType.Pad:
                    press_down = getDevice(handType).GetPressDown(WVR_InputId.WVR_InputId_Alias1_Touchpad);
                    break;
                case ButtonType.Grab:
                    press_down = getDevice(handType).GetPressDown(WVR_InputId.WVR_InputId_Alias1_Grip);
                    break;
                case ButtonType.Menu:
                    press_down = getDevice(handType).GetPressDown(WVR_InputId.WVR_InputId_Alias1_Menu);
                    break;
            }
#elif HVR

#elif PVR
            switch (buttonType)
            {
                case ButtonType.Trigger:
                    press_down = Pvr_UnitySDKAPI.Controller.UPvr_GetKeyDown((int)handType, Pvr_KeyCode.TRIGGER);
                    break;
                case ButtonType.Pad:
                    press_down = Pvr_UnitySDKAPI.Controller.UPvr_GetKeyDown((int)handType, Pvr_KeyCode.TOUCHPAD);
                    break;
                case ButtonType.Grab:
                    press_down = Pvr_UnitySDKAPI.Controller.UPvr_GetKeyDown((int)handType, (int)handType == 0 ? Pvr_KeyCode.Left : Pvr_KeyCode.Right);
                    break;
                case ButtonType.Menu:
                    press_down = Pvr_UnitySDKAPI.Controller.UPvr_GetKeyDown((int)handType, Pvr_KeyCode.HOME);
                    break;
            }
#endif

#if UNITY_EDITOR && EDITOR_TEST
            switch (buttonType)
            {
                case ButtonType.Trigger:
                    press_down = Input.GetKeyDown(KeyCode.T);
                    break;
                case ButtonType.Pad:
                    press_down = Input.GetKeyDown(KeyCode.P);
                    break;
                case ButtonType.Grab:
                    press_down = Input.GetKeyDown(KeyCode.G);
                    break;
                case ButtonType.Menu:
                    press_down = Input.GetKeyDown(KeyCode.M);
                    break;
            }
#endif
            return press_down;
        }

        /// <summary>
        /// 获取TouchPad触摸坐标
        /// </summary>
        /// <param name="handType"></param>
        /// <returns></returns>
        public Vector2 GetTouchPadPos(HandType handType)
        {

            Vector2 v2_temp = Vector2.zero;
#if OVR

#elif VIVE

#elif WVR
            (WaveVR_Controller.EDeviceType, WaveVR_Controller.EDeviceType) left_and_right = getLeftAndRight();
            switch (handType)
            {
                case HandType.Left:
                    v2_temp = WaveVR_Controller.Input(left_and_right.Item1).GetAxis(WaveVR_ButtonList.EButtons.Touchpad);
                    break;
                case HandType.Right:
                    v2_temp = WaveVR_Controller.Input(left_and_right.Item2).GetAxis(WaveVR_ButtonList.EButtons.Touchpad);
                    break;
                case HandType.Single:
                    v2_temp = WaveVR_Controller.Input(left_and_right.Item1).GetAxis(WaveVR_ButtonList.EButtons.Touchpad);
                    break;
                default:
                    break;
            }

#elif HVR
    
#elif PVR
            v2_temp = Pvr_UnitySDKAPI.Controller.UPvr_GetTouchPadPosition((int)handType);
#endif
            return v2_temp;
        }

        /// <summary>
        /// 手柄震动
        /// </summary>
        /// <param name="handType">震动的手柄</param>
        /// <param name="shakeTime">震动时间</param>
        /// <param name="frequency">震动频率</param>
        /// <param name="amplitude">振动幅度</param>
        public void HandShake(HandType handType, float shakeTime, int frequency, float amplitude)
        {
#if OVR
            switch (handType)
            {
                case HandType.Left:
                    OVRCam.StartCoroutine(OVR_Shake(frequency, amplitude, shakeTime, OVRInput.Controller.LTouch));
                    break;
                case HandType.Right:
                    OVRCam.StartCoroutine(OVR_Shake(frequency, amplitude, shakeTime, OVRInput.Controller.RTouch));
                    break;
            }

#elif VIVE

#elif WVR
            (WaveVR_Controller.EDeviceType, WaveVR_Controller.EDeviceType) left_and_right = getLeftAndRight();
            switch (handType)
            {
                case HandType.Left:
                    WaveVR_Controller.Input(left_and_right.Item1).TriggerHapticPulse();
                    break;
                case HandType.Right:
                    WaveVR_Controller.Input(left_and_right.Item2).TriggerHapticPulse();
                    break;
                case HandType.Single:
                    WaveVR_Controller.Input(left_and_right.Item1).TriggerHapticPulse();
                    break;
                default:
                    break;
            }

#elif HVR
            //没有手柄震动
#elif PVR
            //没有手柄震动
#endif

#if UNITY_EDITOR
            Debug.Log(string.Format("{0}手柄震动，持续时间为：{1}，频率为：{2}，幅度为：{3}...", handType, shakeTime, frequency, amplitude));
#endif
        }
    }
}
