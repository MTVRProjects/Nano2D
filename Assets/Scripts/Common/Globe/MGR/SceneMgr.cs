using HMLFramwork;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace HMLFramwork
{
    public class SceneMgr : HMLFramwork.Singleton.SingleInstance<SceneMgr>
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Start()
        {
            EventCenter.Ins.Add(EventSign.LOAD_SCENE, LoadScene);
        }

        /// <summary>
        /// 加载场景
        /// </summary>
        /// <param name="ep"></param>
        static void LoadScene(EventPara ep)
        {
            if (ep.Paras.Length == 1)
            {
                if (ep[0].GetType().Equals(typeof(string)))
                {
                    try
                    {
                        SceneManager.LoadScene((string)ep[0]);
                    }
                    catch
                    {
                        Debug.Log("警告：加载场景失败...");
                    }
                }
                else if (ep[0].GetType().Equals(typeof(int)))
                {
                    try
                    {
                        SceneManager.LoadScene((int)ep[0]);
                    }
                    catch
                    {
                        Debug.Log("警告：加载场景失败...");
                    }
                }
            }
            else if (ep.Paras.Length == 2)
            {
                if ((ep[0].GetType().Equals(typeof(string)) || ep[0].GetType().Equals(typeof(int))) && ep[1].GetType().Equals(typeof(AsynOrSync)))
                {
                    if (ep[0].GetType().Equals(typeof(string)))
                    {
                        if ((AsynOrSync)ep[1] == AsynOrSync.SYNC)
                        {
                            SceneManager.LoadScene((string)ep[0]);
                        }
                        else if ((AsynOrSync)ep[1] == AsynOrSync.ASYN)
                        {
                            SceneManager.LoadSceneAsync((string)ep[0]);
                        }
                        else Debug.Log("警告：传参有误...");
                    }
                    else if (ep[0].GetType().Equals(typeof(int)))
                    {
                        if ((AsynOrSync)ep[1] == AsynOrSync.SYNC)
                        {
                            SceneManager.LoadScene((int)ep[0]);
                        }
                        else if ((AsynOrSync)ep[1] == AsynOrSync.ASYN)
                        {
                            SceneManager.LoadSceneAsync((int)ep[0]);
                        }
                        else Debug.Log("警告：传参有误...");
                    }
                }
            }
        }
    }
}
