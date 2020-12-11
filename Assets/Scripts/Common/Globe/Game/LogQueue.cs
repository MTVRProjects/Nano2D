//================================================
//描述 ： 可在非Unity主线程中打印日志
//作者 ：HML
//创建时间 ：2018/07/14 15:31:40  
//版本： 1.0
//================================================
using System.Collections.Generic;
using UnityEngine;

namespace HMLFramwork.Log
{
    /// <summary>
    /// 日志队列
    /// </summary>
    public static class LogQueue
    {

        private static Queue<object> _logQueue = new Queue<object>();

        public static void Add(object log)
        {
            if (log == null) log = "日志内容为空...";
            _logQueue.Enqueue(log);
        }

        /// <summary>
        /// 需要加入到Update函数中
        /// </summary>
        public static void OutputLog()
        {
            if (_logQueue.Count > 0)
            {
                Debug.Log(_logQueue.Dequeue());
            }
        }
    }


}
