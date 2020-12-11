using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataPathHelper
{
    public static string Datas_path
    {
        get
        {
#if UNITY_EDITOR && MIAOTECH_FULL
            return "F:/SystemFolders/Desktop/ProjectsOfDoing/Nano2D/StreamingAssets/Datas";
#elif UNITY_STANDALONE_WIN
            return Application.streamingAssetsPath + "/Datas";
#elif UNITY_EDITOR && MIAOTECH_FREE
            return Application.streamingAssetsPath + "/FreeVersionDatas";
#elif UNITY_ANDROID && MIAOTECH_FULL
            return Application.persistentDataPath+"/Datas" ;
#elif UNITY_ANDROID && MIAOTECH_FREE
            return Application.streamingAssetsPath + "/FreeVersionDatas";
#endif
        }
    }
}
