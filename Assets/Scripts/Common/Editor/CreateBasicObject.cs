//================================================
//描述 ： 在场景中创建基本的游戏对象
//作者 ：HML
//创建时间 ：2019/04/08 16:24:13  
//版本： 1.0
//================================================
using UnityEngine;
using UnityEditor;

namespace HMLFramwork
{
    public class CreateBasicObject : MonoBehaviour
    {

        [MenuItem("HML/创建游戏启动器", false, 10)]
        public static void createGameStarter()
        {
            if (GameObject.Find("GameStarter"))
            {
                Debug.Log("提醒：场景中已存在游戏启动器...");
                return;
            }
            GameObject go = new GameObject("GameStarter");
            go.AddComponent<GameStarter>();
        }
    }
}