using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

public class DeleteUnusedData : EditorWindow
{
    [MenuItem("HML/DeleteUnusedFile")]
    static void excute()
    {
        Rect wr = new Rect(0, 0, 600, 300);
        DeleteUnusedData window = (DeleteUnusedData)GetWindowWithRect(typeof(DeleteUnusedData), wr, true, "删除无用文件");
        window.Show();
    }

    string file_contains_info;
    string file_path;

    List<string> files = new List<string>();
    List<string> infos;
    int delete_count = 0;
    private void OnGUI()
    {
        //绘制贴图选择框
        file_contains_info = EditorGUILayout.TextField("0011,0018,0019,0020,0021,0022,0025,0042,0063,0077,0084,0085,0087,0092,0123,0124,0126,0127,0134,0136,0138,0146,0147,0151,0160,0161,0162,0177,0184,0185,0192,0194,0214,0221,0222,0223,0225,0226,0227,0253");

        if (GUILayout.Button("请选择文件所在路径 ", GUILayout.Width(200), GUILayout.Height(30)))
        {
            file_path = EditorUtility.OpenFolderPanel("请选择文件所在路径", Application.dataPath, "StreamingAssets");
        }

        //绘制按钮且编辑点击按钮执行功能
        if (GUILayout.Button("删除 ", GUILayout.Width(200), GUILayout.Height(30)))
        {
            delete_count = 0;
            infos = new List<string>(file_contains_info.Split(','));
           
            GetDirector(file_path,files);
            for (int i = 0; i < files.Count; i++)
            {
                string file_name = Path.GetFileNameWithoutExtension(files[i]);
                bool usefull = false;
                for (int j = 0; j < infos.Count; j++)
                {
                    if (file_name.Contains(infos[j]))
                    {
                        usefull = true;
                    }
                }
                if (!usefull)
                {
                    Debug.Log("删除文件为：" + file_name);
                    File.Delete(files[i]);
                }
            }
            
        }
    }


    public void GetDirector(string dirs,List<string> fileFullNames)
    {
        //绑定到指定的文件夹目录
        DirectoryInfo dir = new DirectoryInfo(dirs);
        //检索表示当前目录的文件和子目录
        FileSystemInfo[] fsinfos = dir.GetFileSystemInfos();
        //遍历检索的文件和子目录
        foreach (FileSystemInfo fsinfo in fsinfos)
        {
            //判断是否为空文件夹　　
            if (fsinfo is DirectoryInfo)
            {
                //递归调用
                GetDirector(fsinfo.FullName, fileFullNames);
            }
            else
            {
                //将得到的文件全路径放入到集合中
                fileFullNames.Add(fsinfo.FullName);
            }
        }
    }
}
