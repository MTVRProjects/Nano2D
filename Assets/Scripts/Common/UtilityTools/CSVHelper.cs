//================================================
//描述 ：CSV文件操作
//作者 ：HML
//创建时间 ：2020/09/22 14:58:25  
//版本：1.0 
//================================================
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace HMLFramwork.Helpers
{
    public class CSVHelper
    {
        public static string toCSV<T>(List<List<T>> datas)
        {
            string str_temp = "";
            for (int i = 0, count = datas.Count; i < count; i++)
            {
                string row_str = "";
                List<T> row_list = datas[i];
                for (int j = 0; j < row_list.Count; j++)
                {
                    row_str += row_list[j].ToString() + ",";
                }
                row_str = row_str.Remove(row_str.Length - 1) + "\r\n";
                str_temp += row_str;
            }
            return str_temp;
        }

        public static void Save<T>(List<List<T>> datas, string whole_path, bool isCover = true, bool isNewLine = true, Encoding encoding = null)
        {
            FileHelper.Save(toCSV<T>(datas), whole_path, isCover, isNewLine, encoding);
        }
    }
}
