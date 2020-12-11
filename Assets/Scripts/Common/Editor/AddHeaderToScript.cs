//================================================
//描 述 ： 给脚本添加抬头
//作 者 ：HML
//创建时间 ：2018/05/22 11:49:15  
//版 本： 1.0
// ================================================
using UnityEngine;
using System.IO;

public class AddHeaderToScript : UnityEditor.AssetModificationProcessor
{
    private static string annotationStr =
          "//================================================\r\n"
           + "//描述 ： \r\n"
           + "//作者 ：\r\n"
           + "//创建时间 ：#CreatTime#  \r\n"
           + "//版本： \r\n"
           + "//================================================\r\n";

    //方法必须为static
    private static void OnWillCreateAsset(string path)
    {
        //排除“.meta”的文件
        path = path.Replace(".meta", "");
        //如果是cs脚本，则进行添加注释处理
        if (path.EndsWith(".cs"))
        {
            string ori_txt = File.ReadAllText(path);
            if ((!ori_txt.Contains("//================================================")) && (!ori_txt.Contains("描述")) && (!ori_txt.Contains("作者")))
            {
                //读取cs脚本的内容并添加到annotationStr后面
                string content = annotationStr+ ori_txt;
                //把#CreateTime#替换成具体创建的时间
                content = content.Replace("#CreatTime#",
                    System.DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"));
                //把内容重新写入脚本
                File.WriteAllText(path, content);
                content = "";
            }
        }
        Debug.Log(path);
    }
}
