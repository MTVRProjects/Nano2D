//================================================
//描述 ： Json通用工具
//作者 ：HML
//创建时间 ：2018/07/20 17:21:27  
//版本： 1.0
//================================================
using System.IO;
using System.Text;
using UnityEngine;

namespace HMLFramwork
{
    /// <summary>
    /// json工具
    /// </summary>
    public class JsonHelper
    {
        /// <summary>
        /// 获取Json字符串某节点的值（最多到两层）
        /// </summary>
        /// <param name="jsonStr">Json文件字符串</param>
        /// <param name="key">需要获取的value值对应的key值</param>
        /// <returns>返回所需的Value值</returns>
        public static string getValue(string jsonStr, string key)
        {
            // {"key1":"value1","key2":{"key3":"value3","key4":"value4"}}
            // {"key1":value1,"key2":{"key3":value3,"key4":value4}}
            string result = string.Empty;
            if (!jsonStr.Contains(key)) return result;
            if (!string.IsNullOrEmpty(jsonStr))
            {
                int keyLen = key.Length;//指定key的起始下标
                int keyIndex = jsonStr.IndexOf(key);//key 的长度
                //截取key后面的冒号后的一位字符串，只有三种情况：1是引号“ " ”，2是“ { ”，3直接是value值的第一个元素
                string iden_Str = jsonStr.Substring(keyIndex + keyLen + 2, 1);

                int start = keyIndex + keyLen + 2;

                if (iden_Str.Equals("{"))
                {
                    int end = jsonStr.IndexOf("}", keyIndex);
                    result = jsonStr.Substring(start, end - start + 1);
                    result = result.Trim(new char[] { ' ' }); //删除引号或空格
                }
                else if (iden_Str.Equals("["))
                {
                    int end = jsonStr.IndexOf("]", keyIndex);
                    result = jsonStr.Substring(start, end - start + 1);
                    result = result.Trim(new char[] { ' ' }); //删除引号或空格
                }
                else
                {
                    int end = jsonStr.IndexOf(',', start);

                    if (end == -1) end = jsonStr.IndexOf('}', start);
                    //截取带双引号的value值
                    result = jsonStr.Substring(start, end - start);
                    result = result.Trim(new char[] { '"', ' ', '\'', '}' }); //删除引号或空格
                }
            }
            return result;
        }

        /// <summary>
        /// 将对象转成json保存到本地文件
        /// </summary>
        public static void Save(object obj, string path, string fileName)
        {
            FileHelper.Save(toJson(obj), path, fileName, ".json", true, true);
        }

        /// <summary>
        /// 对象转json字符串
        /// </summary>
        public static string toJson(object value)
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(value);
        }

        /// <summary>
        /// json转对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="json"></param>
        /// <returns></returns>
        public static T toObject<T>(string json)
        {
            return Newtonsoft.Json.JsonConvert.DeserializeObject<T>(json);
        }
    }
}
