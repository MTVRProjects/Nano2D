//================================================
//描述 ： 时间工具
//作者 ：HML
//创建时间 ：2019/04/30 11:25:28  
//版本： 1.0
//================================================
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HMLFramwork
{
    public class TimeHelper
    {

        /// <summary>
        /// 时间戳转时间      
        /// </summary>
        /// <param name="timeStamp">时间戳字符串</param>
        /// <param name="type">时间戳的类型：传过来的时间戳是秒，还是毫秒</param>
        /// <returns></returns>
        public static DateTime TimeStampToDateTime(string timeStamp, TimeSpanType type = TimeSpanType.SEC)
        {
            DateTime dtStart = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
            long lTime = 0;
            switch (type)
            {
                case TimeSpanType.SEC:
                    lTime = long.Parse(timeStamp + "0000") * 1000;
                    break;
                case TimeSpanType.MS:
                    lTime = long.Parse(timeStamp + "0000");
                    break;
            }
            TimeSpan toNow = new TimeSpan(lTime);
            return dtStart.Add(toNow);
        }
        /// <summary>
        /// 获取时间戳
        /// </summary>
        /// <returns></returns>
        public static string getTimeStamp(System.DateTime time)
        {
            long ts = convertDateTimeToInt(time);
            return ts.ToString();
        }
        /// <summary>  
        /// 将c# DateTime时间格式转换为Unix时间戳格式  
        /// </summary>  
        /// <param name="time">时间</param>  
        /// <returns>long</returns>  
        public static long convertDateTimeToInt(System.DateTime time)
        {
            System.DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1, 0, 0, 0, 0));
            long t = (time.Ticks - startTime.Ticks) / 10000;   //除10000调整为13位      
            return t;
        }



    }
}

