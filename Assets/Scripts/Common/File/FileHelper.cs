using UnityEngine;
using HMLFramwork.Singleton;
using System.Collections.Generic;
using System.IO;
using HMLFramwork.Helpers;
using System.Text;

namespace HMLFramwork
{
    /// <summary>
    /// 文件工具
    /// </summary>
    public class FileHelper : SingleInstance<FileHelper>
    {

        /// <summary>
        /// 获取某类文件（适用于编辑器模式）：fileType—文件类型 ， loadPath—Resources加载路径
        /// </summary>
        /// <typeparam name="T">文件的对象类型（针对Unity而言）</typeparam>
        /// <param name="fileType">文件类型</param>
        /// <param name="loadPath">Resources加载路径</param>
        /// <returns></returns>
        public List<T> Get_Res_Files<T>(string fileType, string loadPath) where T : Object
        {
            List<T> fileList = new List<T>();
            string path = StringHelper.Package(Application.dataPath, "/", "Resources/", loadPath);
            if (Directory.Exists(path))
            {
                //获取文件信息
                DirectoryInfo direction = new DirectoryInfo(path);

                FileInfo[] files = direction.GetFiles(fileType, SearchOption.TopDirectoryOnly);

                for (int i = 0; i < files.Length; i++)
                {
                    //过滤掉临时文件
                    if (files[i].Name.EndsWith(".meta")) continue;
                    //获取不带扩展名的文件名
                    string name = Path.GetFileNameWithoutExtension(files[i].ToString());

                    string resourcesLoadPath = StringHelper.Package(loadPath, "/", name);

                    // FileInfo.Name是返回带扩展名的名字 
                    fileList.Add(Resources.Load(resourcesLoadPath, typeof(T)) as T);
                }
            }
            return fileList;
        }

        /// <summary>
        /// 获取某个文件：fileType—文件类型 ，fileName—文件名字， loadPath—Resources加载路径
        /// </summary>
        /// <typeparam name="T">文件的对象类型（针对Unity而言）</typeparam>
        /// <param name="fileType">文件类型</param>
        /// <param name="fileName">文件名字</param>
        /// <param name="loadPath">Resources加载路径</param>
        /// <returns></returns>
        public T Get_Res_File<T>(string fileType, string fileName, string loadPath) where T : Object
        {
            T file = null;
            string path = StringHelper.Package(Application.dataPath, "/", "Resources/", loadPath);
            if (Directory.Exists(path))
            {
                //获取文件信息
                DirectoryInfo direction = new DirectoryInfo(path);

                FileInfo[] files = direction.GetFiles(fileType, SearchOption.TopDirectoryOnly);

                for (int i = 0; i < files.Length; i++)
                {
                    if (!files[i].Name.StartsWith(fileName)) continue;
                    //过滤掉临时文件
                    if (files[i].Name.EndsWith(".meta")) continue;
                    //获取不带扩展名的文件名
                    string name = Path.GetFileNameWithoutExtension(files[i].ToString());

                    if (fileName.Equals(name))
                    {
                        string resourcesLoadPath = StringHelper.Package(loadPath, "/", name);

                        // FileInfo.Name是返回带扩展名的名字 
                        file = Resources.Load(resourcesLoadPath, typeof(T)) as T;
                    }
                }
            }
            return file;
        }
        /// <summary>
        /// 存储文件
        /// </summary>
        /// <param name="strContent">需要储存的内容</param>
        /// <param name="filePath">文件存储路径（完整路径）</param>
        /// <param name="isCover">是否覆盖原文件</param>
        /// <param name="isNewLine">是否换行</param>
        /// <param name="encoding">编码格式</param>
        public static void Save(string strContent, string filePath, bool isCover = true, bool isNewLine = true, Encoding encoding = null)
        {
            bool isExistFile = File.Exists(filePath);
            FileStream fs = null;
            //当需要覆盖文件，或者文件不存在时
            if (!isExistFile || isCover)
            {
                //首先判断文件夹是否存在，不存在则创建文件夹
                if (!Directory.Exists(filePath)) Directory.CreateDirectory(new FileInfo(filePath).Directory.FullName);
                fs = new FileStream(filePath, FileMode.Create);
                byte[] bts = (encoding == null ? Encoding.UTF8 : encoding).GetBytes(strContent + "\r\n");
                fs.Write(bts, 0, bts.Length);
            }
            else //当文件存在时，且不需要覆盖
            {
                fs = new FileStream(filePath, FileMode.Append);
                StreamWriter sw = new StreamWriter(fs, encoding == null ? Encoding.UTF8 : encoding);
                if (isNewLine) sw.WriteLine(strContent);
                else sw.Write(strContent);
                sw.Flush();
                sw.Close();
            }
            if (fs != null) fs.Close();

        }
        /// <summary>
        /// 存储文件
        /// </summary>
        /// <param name="strContent">需要储存的内容</param>
        /// <param name="folderPath">文件存储路径</param>
        /// <param name="fileName">文件名字</param>
        /// <param name="fileType">文件类型（文件扩展名）</param>
        /// <param name="isCover">是否覆盖原文件</param>
        /// <param name="isNewLine">是否换行</param>
        public static void Save(string strContent, string folderPath, string fileName, string fileType, bool isCover = true, bool isNewLine = true)
        {
            string filePath = Path.Combine(folderPath, fileName + fileType);
            Save(strContent, filePath, isCover, isNewLine);
        }

        /// <summary>
        /// 读取json文件内的字符串
        /// </summary>
        /// <param name="filePath">json文件路径</param>
        /// <returns></returns>
        public static string GetStrFromFile(string filePath, Encoding encoding = null)
        {
            string _txtContent = string.Empty;
            if (File.Exists(filePath))
            {
                if (encoding != null) _txtContent = File.ReadAllText(filePath);
                else _txtContent = File.ReadAllText(filePath,encoding);
            }
            return _txtContent;
        }

        
    }
}

