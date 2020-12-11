using UnityEngine;

public class DontDestroyGO<T> : MonoBehaviour where T: MonoBehaviour
{
    protected static GameObject dontDestroyGO;

    public static T Ins;
    //创建挂载常存的游戏对象
    internal static void CreateDontDestroyGO()
    {
        if (dontDestroyGO == null) dontDestroyGO = GameObject.Find("DontDestroyGO");
        if (dontDestroyGO == null)
        {
            dontDestroyGO = new GameObject("DontDestroyGO");
            DontDestroyOnLoad(dontDestroyGO);
        }
        if (Ins) return;
        Ins = dontDestroyGO.GetComponent<T>();
        if (Ins == null) Ins = dontDestroyGO.AddComponent<T>();
    }
}
