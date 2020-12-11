//================================================
//描述 ： 生成场景配置文件,存储路径为流资源文件夹下的“settingfiles/scene_settings”
//作者 ：HML
//创建时间 ：2018/12/11 10:32:09  
//版本： 1.0
//================================================
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using HMLFramwork.Helpers;
using HMLFramwork.SCENE;

namespace HMLFramwork
{
    public class CreateSceneSettingFile : MonoBehaviour
    {
        [MenuItem("AB包工具/生成场景配置文件")]
        static void Create()
        {
            List<GameObject> gos = Selection.gameObjects.toList();
            List<GO_Data> _gos_data = new List<GO_Data>();
            for (int i = 0; i < gos.Count; i++)
            {
                Transform tran = gos[i].transform;
                GO_Data go_data = new GO_Data(tran.name, tran.position, tran.eulerAngles, tran.localScale);
                _gos_data.Add(go_data);
            }
            int buildIndex = gos[0].scene.buildIndex;
            string s_n = gos[0].scene.name;
            SceneData sd = new SceneData(buildIndex, s_n, _gos_data);
            JsonHelper.Save(sd, s_n + "_setting", Helpers.PathHelper.getSceneSettingPath);
            Debug.Log(string.Format("提示：场景名字：{0}，存储了场景内{1}个对象数据...", s_n, _gos_data.Count));

        }

    }



   

}



