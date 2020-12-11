using System;
using System.Net;

namespace HMLFramwork
{
    /// <summary>
    /// 当前操作系统信息
    /// </summary>
    public class SystemHelper
    {
        /// <summary>
        /// 操作系统主版本号（Win7---6，Win10---10）
        /// </summary>
        public static int Major { get { return Environment.OSVersion.Version.Major; } }

        /// <summary>
        /// 操作系统副版本号（Win7---1，Win8---2，Win8.1---3，Win10---0）
        /// </summary>
        public static int Minor { get { return Environment.OSVersion.Version.Minor; } }
        static CurrentSystem _currentSystem = CurrentSystem.Default;
        /// <summary>
        /// 当前的操作系统
        /// </summary>
        public static CurrentSystem MyCurrentSystem
        {
            get
            {
                if (_currentSystem == CurrentSystem.Default)
                {
                    if (Major == 6 && Minor == 1) _currentSystem = CurrentSystem.WIN7;
                    else if(Major == 6 && Minor == 0) _currentSystem = CurrentSystem.WIN10;
                }
                return _currentSystem;
            }
        }

        static string ip = string.Empty;
        /// <summary>
        /// IP地址
        /// </summary>
        public static string IP { get { if (String.IsNullOrEmpty(ip)) ip = GetLocalIPAddress(); return ip; } }
        /// <summary>
        /// 获取本机IP地址
        /// </summary>
        /// <returns></returns>
        static String GetLocalIPAddress()
        {
            String str;
            String result = "";
            String hostName = Dns.GetHostName();
            IPAddress[] myIP = Dns.GetHostAddresses(hostName);
            foreach (IPAddress address in myIP)
            {
                str = address.ToString();
                for (int i = 0; i < str.Length; i++)
                {
                    if (str[i] >= '0' && str[i] <= '9' || str[i] == '.') result = str;
                }
            }
            return result;
        }

    }
}

