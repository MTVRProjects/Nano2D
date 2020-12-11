using UnityEngine;
using UnityEngine.SceneManagement;
using HMLFramwork;
using HMLFramwork.Log;

public class GlobeFuncMgr : HMLFramwork.Singleton.SingleInstance<GlobeFuncMgr>
{

    public void Start()
    {
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
    }

}

