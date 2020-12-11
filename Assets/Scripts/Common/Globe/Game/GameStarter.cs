using HMLFramwork;
using UnityEngine;

public class GameStarter : MonoBehaviour {

	// Use this for initialization
	void Start () {
        GlobeFuncMgr.Ins.Start();
        DontDestroyOnLoad(this);
    }
	
	// Update is called once per frame
	void Update () {
        GlobeFuncMgr.Ins.Update();
    }
}
