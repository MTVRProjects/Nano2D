using UnityEditor;
using UnityEngine;
using System.IO;
using System;

public class CreateDefaultFolder : MonoBehaviour
{
    enum DefaultFolder
    {
        Scenes = 0, StreamingAssets, Scripts, Plugins, Resources, Editor, Test, ABResources,
        Resources_Prefabs, Resources_Materials, Resources_Models, Resources_Textures, StreamingAssets_Configs
    }
    [MenuItem("HML/创建默认文件夹")]
    static void createDefaultFolder()
    {
        string[] folderNames = Enum.GetNames(typeof(DefaultFolder));
        for (int i = 0; i < folderNames.Length; i++)
        {
            string folderTemp = folderNames[i];
            if (!folderTemp.Contains("_"))
                getThePath(Combine(System.Environment.CurrentDirectory, "Assets", folderTemp));
            else
                getThePath(Combine(true, folderTemp.Split('_'), System.Environment.CurrentDirectory, "Assets"));
        }
    }

    /// <summary>
    /// 获取某个路径（没有则创建）
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    static string getThePath(string path)
    {
        if (!string.IsNullOrEmpty(path))
        {
            if (!Directory.Exists(path))
            {
                DirectoryInfo dir = new DirectoryInfo(path);
                dir.Create();
                Debug.Log(string.Format("提醒：原路径({0})不存在，已创建。", path));
            }
        }
        else Debug.Log("警告：参数为空，请检查赋值...");
        return path;
    }

    /// <summary>
    /// 合并路径
    /// </summary>
    /// <param name="paths1"></param>
    /// <param name="isRevers">是否倒序，即paths2在前，path1在后</param>
    /// <param name="paths2"></param>
    /// <returns></returns>
    static string Combine(bool isRevers, string[] paths1, params string[] paths2)
    {
        string result1, result2 = "";

        result1 = Combine(paths1);
        result2 = Combine(paths2);

        if (isRevers) return Path.Combine(result2, result1);
        else return Path.Combine(result1, result2);

    }

    static string Combine(params string[] paths)
    {
        string result = "";
        if (paths != null && paths.Length > 1)
        {
            for (int i = 0; i < paths.Length; i++)
            {
                string pathTemp = paths[i];
                if (!string.IsNullOrEmpty(pathTemp))
                    result = Path.Combine(result, pathTemp);
            }
        }
        else Debug.Log("警告：需要合并的路径名不合规！");
        return result;
    }

}
