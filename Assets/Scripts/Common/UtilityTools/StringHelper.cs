using System;
using System.Collections.Generic;
using System.Text;

namespace HMLFramwork.Helpers
{
    /// <summary>
    /// 字符串工具
    /// </summary>
    public class StringHelper
    {
        /// <summary>
        /// 将多个字符串封装成一个字符串
        /// </summary>
        /// <param name="args">传入的字符串参数</param>
        /// <returns>string</returns>
        public static string Package(params object[] args)
        {
            if (args != null || args.Length > 0)
            {
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < args.Length; i++)
                {
                    sb.Append(args[i]);
                }
                return sb.ToString();
            }
            else return null;
        }

        /// <summary>
        /// 通过指定字符分割字符串
        /// </summary>
        /// <param name="value">需要分割的字符串</param>
        /// <param name="arg">指定的字符</param>
        /// <returns>string[]</returns>
        public static string[] Split(string value, char arg)
        {
            if (value != null && value != "")
            {
                try {
                    string[] strArr = value.Split(arg);
                    return strArr;
                }
                catch { return null; }
            }
            else return null;
        }
        /// <summary>
        /// 获取随机字符串（包含数字和字母）
        /// </summary>
        /// <param name="length">长度</param>
        /// <returns></returns>
        public static string getRandomStr(int length)
        {
            string str = "";
            int rep = 0;
            long num2 = DateTime.Now.Ticks + rep;
            rep++;
            Random random = new Random(((int)(((ulong)num2) & 0xffffffffL)) | ((int)(num2 >> rep)));
            for (int i = 0; i < length; i++)
            {
                char ch;
                int num = random.Next();
                if ((num & 1) == 0)//为偶数
                {
                    ch = (char)(0x30 + ((ushort)(num % 10)));
                }
                else
                {
                    ch = (char)(0x41 + ((ushort)(num % 0x1a)));
                }
                str = str + ch.ToString();
            }
            return str;
        }

        /// <summary>
        /// 切割字符串获得切割后的字符
        /// </summary>
        /// <param name="targetStr"></param>
        /// <param name="cutter"></param>
        /// <returns></returns>
        public static List<string> getStrList(string targetStr,char cutter)
        {
            string[] str_arr = targetStr.Split(cutter);
            List<string> str_list = new List<string>();
            if (str_arr.Length>0)
            {
                for (int i = 0; i < str_arr.Length; i++)
                {
                    if (str_arr[i].Trim().Length>0)
                    {
                        str_list.Add(str_arr[i]);
                    }
                }
            }
            return str_list;
        }

    }
}

