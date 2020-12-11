using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace MiaoTech.VR
{
    public class SetVRPlatform : MonoBehaviour
    {
#if UNITY_EDITOR
        static VRP_PlayerPrefsData<string> VRP = new VRP_PlayerPrefsData<string>("VRP", "null");
        const string commonTitle = "MiaoTech/VR/";
        static string[] scriptingDefineSymbols = new string[] { "OVR", "VIVE", "PVR", "HVR", "WVR", "EDITOR_TEST" };
#endif


        [MenuItem(commonTitle + "获取当前VR平台预编译指令", false, 98)]
        static void getVRP()
        {
            Debug.Log("Standalone平台的VRP宏定义：" + PlayerSettings.GetScriptingDefineSymbolsForGroup(BuildTargetGroup.Standalone));
            Debug.Log("Android平台的VRP宏定义：" + PlayerSettings.GetScriptingDefineSymbolsForGroup(BuildTargetGroup.Android));
            Debug.Log("VRP的数据为：" + VRP.Data);
        }


        [MenuItem(commonTitle + "清空当前VR平台预编译指令", false, 99)]
        static void cleanVRP()
        {
            var conten_Standalone = PlayerSettings.GetScriptingDefineSymbolsForGroup(BuildTargetGroup.Standalone);
            var content_Android = PlayerSettings.GetScriptingDefineSymbolsForGroup(BuildTargetGroup.Android);

            for (int i = 0; i < scriptingDefineSymbols.Length; i++)
            {
                var str_temp = scriptingDefineSymbols[i] + ";";
                if (conten_Standalone.Contains(str_temp))
                {
                    conten_Standalone = conten_Standalone.Replace(str_temp, "");
                    Debug.Log("Test：" + conten_Standalone);
                }
                if (conten_Standalone.Contains(scriptingDefineSymbols[i]))
                {
                    conten_Standalone = conten_Standalone.Replace(scriptingDefineSymbols[i], "");
                }
                if (content_Android.Contains(str_temp))
                {
                    content_Android = content_Android.Replace(str_temp, "");
                }
                if (content_Android.Contains(scriptingDefineSymbols[i]))
                {
                    content_Android = content_Android.Replace(scriptingDefineSymbols[i], "");
                }

            }
            PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.Standalone, conten_Standalone);
            PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.Android, content_Android);
            VRP.Data = "Null";
        }

        const string VRPlatformTitle = commonTitle + "选择VR平台/";

        #region OVR
        [MenuItem(VRPlatformTitle + "OVR", false, 100)]
        static void ToggleVRPlatform_OVR()
        {

            var content_Standalone = PlayerSettings.GetScriptingDefineSymbolsForGroup(BuildTargetGroup.Standalone);
            var content_Android = PlayerSettings.GetScriptingDefineSymbolsForGroup(BuildTargetGroup.Android);
            if (!content_Standalone.Contains("OVR"))
            {
                //去掉旧的VRP宏定义
                if (content_Standalone.Contains(VRP.Data))
                {
                    content_Standalone = content_Standalone.Replace(VRP.Data, "");
                    if (content_Standalone.Contains(";;"))
                    {
                        content_Standalone = content_Standalone.Replace(";;", ";");
                    }
                }
                //加入新的VRP宏定义
                content_Standalone = content_Standalone + ";OVR";

            }
            if (!content_Android.Contains("OVR"))
            {
                if (content_Android.Contains(VRP.Data))
                {
                    content_Android = content_Android.Replace(VRP.Data, "");
                    if (content_Android.Contains(";;"))
                    {
                        content_Android = content_Android.Replace(";;", ";");
                    }
                }
                content_Android = content_Android + ";OVR";
            }
            PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.Standalone, content_Standalone);
            PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.Android, content_Android);

            VRP.Data = "OVR";

            Menu.SetChecked(VRPlatformTitle + "OVR", VRP.Data == "OVR");
        }


        [MenuItem(VRPlatformTitle + "OVR", true, 100)]
        static bool ToggleVRPlatform_OVR_Valid()
        {
            Menu.SetChecked(VRPlatformTitle + "OVR", VRP.Data == "OVR");
            return true;
        }

        #endregion

        #region VIVE
        [MenuItem(VRPlatformTitle + "VIVE", false, 101)]
        static void ToggleVRPlatform_VIVE()
        {
            var content_Standalone = PlayerSettings.GetScriptingDefineSymbolsForGroup(BuildTargetGroup.Standalone);
            if (!content_Standalone.Contains("VIVE"))
            {
                if (content_Standalone.Contains(VRP.Data))
                {
                    content_Standalone = content_Standalone.Replace(VRP.Data, "");
                    if (content_Standalone.Contains(";;"))
                    {
                        content_Standalone = content_Standalone.Replace(";;", ";");
                    }
                }
                content_Standalone = content_Standalone + ";VIVE";
            }
            PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.Standalone, content_Standalone);
            VRP.Data = "VIVE";
            Menu.SetChecked(VRPlatformTitle + "VIVE", VRP.Data == "VIVE");
        }


        [MenuItem(VRPlatformTitle + "VIVE", true, 101)]
        static bool ToggleVRPlatform_VIVE_Valid()
        {
            Menu.SetChecked(VRPlatformTitle + "VIVE", VRP.Data == "VIVE");
            return true;
        }
        #endregion

        #region PicoVR

        [MenuItem(VRPlatformTitle + "PVR", false, 102)]
        static void ToggleVRPlatform_PVR()
        {
            var content_Android = PlayerSettings.GetScriptingDefineSymbolsForGroup(BuildTargetGroup.Android);
            if (!content_Android.Contains("PVR"))
            {
                if (content_Android.Contains(VRP.Data))
                {
                    content_Android = content_Android.Replace(VRP.Data, "");
                    if (content_Android.Contains(";;"))
                    {
                        content_Android = content_Android.Replace(";;", ";");
                    }
                }
                content_Android = content_Android + ";PVR";
            }
            PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.Android, content_Android);
            VRP.Data = "PVR";
            Menu.SetChecked(VRPlatformTitle + "PVR", VRP.Data == "PVR");
        }


        [MenuItem(VRPlatformTitle + "PVR", true, 102)]
        static bool ToggleVRPlatform_PVR_Valid()
        {
            Menu.SetChecked(VRPlatformTitle + "PVR", VRP.Data == "PVR");
            return true;
        }

        #endregion

        #region WaveVR

        [MenuItem(VRPlatformTitle + "WVR", false, 103)]
        static void ToggleVRPlatform_WVR()
        {
            var content_Android = PlayerSettings.GetScriptingDefineSymbolsForGroup(BuildTargetGroup.Android);
            if (!content_Android.Contains("WVR"))
            {
                if (content_Android.Contains(VRP.Data))
                {
                    content_Android = content_Android.Replace(VRP.Data, "");
                    if (content_Android.Contains(";;"))
                    {
                        content_Android = content_Android.Replace(";;", ";");
                    }
                }
                content_Android = content_Android + ";WVR";
            }
            PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.Android, content_Android);
            VRP.Data = "WVR";
            Menu.SetChecked(VRPlatformTitle + "WVR", VRP.Data == "WVR");
        }


        [MenuItem(VRPlatformTitle + "WVR", true, 103)]
        static bool ToggleVRPlatform_WVR_Valid()
        {
            Menu.SetChecked(VRPlatformTitle + "WVR", VRP.Data == "WVR");
            return true;
        }

        #endregion

        #region HVR

        [MenuItem(VRPlatformTitle + "HVR", false, 104)]
        static void ToggleVRPlatform_HVR()
        {
            var content_Android = PlayerSettings.GetScriptingDefineSymbolsForGroup(BuildTargetGroup.Android);
            if (!content_Android.Contains("HVR"))
            {
                if (content_Android.Contains(VRP.Data))
                {
                    content_Android = content_Android.Replace(VRP.Data, "");
                    if (content_Android.Contains(";;"))
                    {
                        content_Android = content_Android.Replace(";;", ";");
                    }
                }
                content_Android = content_Android + ";HVR";
            }
            PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.Android, content_Android);
            VRP.Data = "HVR";
            Menu.SetChecked(VRPlatformTitle + "HVR", VRP.Data == "HVR");
        }


        [MenuItem(VRPlatformTitle + "HVR", true, 104)]
        static bool ToggleVRPlatform_HVR_Valid()
        {
            Menu.SetChecked(VRPlatformTitle + "HVR", VRP.Data == "HVR");
            return true;
        }

        #endregion


        #region EDITOR_TEST

        [MenuItem(VRPlatformTitle + "EDITOR_TEST", false, 105)]
        static void ToggleVRPlatform_EDITOR_TEST()
        {

            var content_Standalone = PlayerSettings.GetScriptingDefineSymbolsForGroup(BuildTargetGroup.Standalone);
            var content_Android = PlayerSettings.GetScriptingDefineSymbolsForGroup(BuildTargetGroup.Android);
            if (!content_Standalone.Contains("EDITOR_TEST"))
            {
                //去掉旧的VRP宏定义
                if (content_Standalone.Contains(VRP.Data))
                {
                    content_Standalone = content_Standalone.Replace(VRP.Data, "");
                    if (content_Standalone.Contains(";;"))
                    {
                        content_Standalone = content_Standalone.Replace(";;", ";");
                    }
                }
                //加入新的VRP宏定义
                content_Standalone = content_Standalone + ";EDITOR_TEST";

            }
            if (!content_Android.Contains("EDITOR_TEST"))
            {
                if (content_Android.Contains(VRP.Data))
                {
                    content_Android = content_Android.Replace(VRP.Data, "");
                    if (content_Android.Contains(";;"))
                    {
                        content_Android = content_Android.Replace(";;", ";");
                    }
                }
                content_Android = content_Android + ";EDITOR_TEST";
            }
            PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.Standalone, content_Standalone);
            PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.Android, content_Android);

            VRP.Data = "EDITOR_TEST";

            Menu.SetChecked(VRPlatformTitle + "EDITOR_TEST", VRP.Data == "EDITOR_TEST");
        }


        [MenuItem(VRPlatformTitle + "EDITOR_TEST", true, 100)]
        static bool ToggleVRPlatform_EDITOR_TEST_Valid()
        {
            Menu.SetChecked(VRPlatformTitle + "EDITOR_TEST", VRP.Data == "EDITOR_TEST");
            return true;
        }


        #endregion


    }
}
