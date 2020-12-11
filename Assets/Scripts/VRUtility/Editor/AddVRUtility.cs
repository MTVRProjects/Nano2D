//================================================
//描述 ： 
//作者 ：
//创建时间 ：2020/11/04 10:12:03  
//版本： 
//================================================
using MiaoTech.VR;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace MiaoTech.VR
{
    public class AddVRUtility : MonoBehaviour
    {
        const string commonTitle = "MiaoTech/VR/";
        [MenuItem(commonTitle + "添加传送工具")]
        static void addTeleport()
        {
            var go = new GameObject("Teleport");
            go.transform.parent = createVRUtilityRoot().transform;
            go.AddComponent<Teleport>();
        }


        static GameObject createVRUtilityRoot()
        {
            return new GameObject("VRUtilityRoot");
        }
    }
}
