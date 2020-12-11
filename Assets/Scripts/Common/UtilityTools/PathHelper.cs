//================================================
//描述 ： 
//作者 ：
//创建时间 ：2020/05/10 22:18:10  
//版本： 
//================================================
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace HMLFramwork.Helpers
{
    public class PathHelper
    {
        /// <summary>
        /// PC、安卓、IOS可读写
        /// </summary>
        public static string persistentDataPath
        {
            get { return Application.persistentDataPath; }
        }
        /// <summary>
        /// 获取运行程序当前路径
        /// </summary>
        public static string getCurrentDirectory { get { return System.Environment.CurrentDirectory; } }
        /// <summary>
        /// 获取程序文档存储路径
        /// </summary>
        public static string appDataPathInDocuments
        {
            get
            {
                string[] strArr = getCurrentDirectory.Split('/');
                string appName = strArr[strArr.Length - 1];
                return System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments) + "/" + appName;
            }
        }
        /// <summary>
        /// 获取某个路径（没有则创建）
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string getThePath(string path)
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
        /// 获取PC上的设置文件夹
        /// </summary>
        public static string getWinSettingFolder
        {
            get
            {
                return getThePath(appDataPathInDocuments + "/_settingData");
            }
        }
        /// <summary>
        /// 获取各平台的数据存储路径
        /// </summary>
        /// <returns></returns>
        public static string getDataPath()
        {
            if (Application.platform == RuntimePlatform.IPhonePlayer)
            {
                string path = Application.dataPath.Substring(0, Application.dataPath.Length - 5);
                path = path.Substring(0, path.LastIndexOf('/'));
                return path + "/Documents";
            }
            else if (Application.platform == RuntimePlatform.Android)
            {
                return Application.persistentDataPath + "/";
            }
            else
            {
                return Application.dataPath;
            }
        }
        /// <summary>
        /// 获取当前运行的平台
        /// </summary>
        /// <returns></returns>
        public static string getRuntimePlatform()
        {
            string platform = "";
            if (Application.platform == RuntimePlatform.WindowsPlayer || Application.platform == RuntimePlatform.WindowsEditor)
            {
                platform = "win";
            }
            else if (Application.platform == RuntimePlatform.Android)
            {
                platform = "apk";
            }
            else if (Application.platform == RuntimePlatform.IPhonePlayer)
            {
                platform = "ios";
            }
            else if (Application.platform == RuntimePlatform.OSXPlayer || Application.platform == RuntimePlatform.OSXEditor)
            {
                platform = "OSX";
            }
            return platform;
        }
        /// <summary>
        /// 获取指定文件夹下指定文件类型的所有路径
        /// </summary>
        /// <param name="fileType"></param>
        /// <param name="folderPath"></param>
        /// <param name="withOutExtension"></param>
        /// <returns></returns>
        public static List<string> getPathsOfFiles(string fileType, string folderPath, bool withOutExtension)
        {
            List<string> filesPath = new List<string>();
            DirectoryInfo direction = new DirectoryInfo(folderPath);
            FileInfo[] files = direction.GetFiles(fileType, SearchOption.TopDirectoryOnly);
            int fileCount = files.Length;
            if (fileCount > 0)
            {
                for (int i = 0; i < fileCount; i++)
                {
                    //过滤掉临时文件
                    if (files[i].Name.EndsWith(".meta")) continue;
                    string _path = "";
                    //获取不带扩展名的文件路径
                    if (withOutExtension)
                    {
                        _path = Path.GetFullPath(files[i].ToString());
                        //去掉文件后缀名
                        _path = _path.Remove(_path.Length - fileType.Length + 1);
                    }
                    else _path = Path.GetFullPath(files[i].ToString());
                    if (!filesPath.Contains(_path)) filesPath.Add(_path);
                }
            }

            return filesPath;

        }
        /// <summary>
        /// 各平台的流资源文件夹
        /// </summary>
        public static string streamingAssetsPath
        {
            get
            {
#if UNITY_EDITOR || UNITY_STANDALONE
                return Application.dataPath + "/StreamingAssets";
#elif UNITY_IPHONE
                return Application.dataPath +"/Raw"+"/my.xml";
#elif UNITY_ANDROID
                return "jar:file://" + Application.dataPath + "!/assets/";
#endif
            }
        }
        /// <summary>
        /// 设置文件存储路径
        /// </summary>
        public static string getSettingFilePath { get { return getThePath(Path.Combine(streamingAssetsPath, "settingfiles")); } }
        public static string getSceneSettingPath { get { return getThePath(Path.Combine(streamingAssetsPath, "settingfiles/scene_settings")); } }
        public static string getAssetsBundlePath { get { return getThePath(Path.Combine(streamingAssetsPath, "assetsbundles")); } }
        public static string getUI_ABResPath { get { return getThePath(Path.Combine(getAssetsBundlePath, "ui_res")); } }
        public static string getUI_ImageResPath { get { return getThePath(Path.Combine(getUI_ABResPath, "images")); } }

        public static string getVersionPath { get { return getThePath(Path.Combine(getSettingFilePath, "version")); } }

        /// <summary>
        /// 获取ab包加载路径
        /// </summary>
        public static string getStreamingAssetsPath_AB
        {
            get
            {
#if UNITY_EDITOR || UNITY_STANDALONE
                return Application.dataPath + "/StreamingAssets";
#elif UNITY_IPHONE
                return Application.dataPath +"/Raw"+"/my.xml";
#elif UNITY_ANDROID
                return Application.dataPath + "!/assets/";
#endif
            }
        }

        /// <summary>
        /// 合并路径
        /// </summary>
        /// <param name="paths1"></param>
        /// <param name="isRevers">是否倒序，即paths2在前，path1在后</param>
        /// <param name="paths2"></param>
        /// <returns></returns>
        public static string Combine(bool isRevers, string[] paths1, params string[] paths2)
        {
            string result1, result2 = "";

            result1 = Combine(paths1);
            result2 = Combine(paths2);

            if (isRevers) return Path.Combine(result2, result1);
            else return Path.Combine(result1, result2);

        }

        /// <summary>
        /// 合并路径
        /// </summary>
        /// <param name="paths"></param>
        /// <returns></returns>
        public static string Combine(params string[] paths)
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

}
