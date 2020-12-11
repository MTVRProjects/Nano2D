using UnityEngine;
using System.Collections.Generic;
using System;

namespace HMLFramwork
{
    public class TimerTask
    {
        /// <summary>
        /// 延时时间
        /// </summary>
        public float delayed_time;
        /// <summary>
        /// 计时器，计时达到延时时间就执行事件
        /// </summary>
        public float timer;
        /// <summary>
        /// 是否重复执行
        /// </summary>
        public bool isRepeat;
        /// <summary>
        /// 事件当前状态：是暂停，还是继续
        /// </summary>
        public bool isPause;
        /// <summary>
        /// 事件
        /// </summary>
        public Action func;

        public TimerTask(float delayed_time, bool isRepeat, Action func)
        {
            this.delayed_time = delayed_time;
            timer = 0;
            this.isRepeat = isRepeat;
            this.isPause = false;
            this.func = func;
        }
    }
    /// <summary>
    /// 定时任务控制
    /// </summary>
    public class TimerEvent : DontDestroyGO<TimerEvent>
    {
        static List<TimerTask> timerTasks = new List<TimerTask>();

        void OnEnable()
        {
            Debug.Log("提醒：开启定时器...");
            EventCenter.Ins.Add(EventSign.ClearTimerEvent, Clear);
            EventCenter.Ins.Add(EventSign.IsPuaseTimerEvent, isPause);
        }

        TimerTask taskTemp;
        void FixedUpdate()
        {
            if (pause) return;
            if (timerTasks.Count < 1) return;

            for (int i = 0; i < timerTasks.Count; i++)
            {
                taskTemp = timerTasks[i];
                if (!taskTemp.isPause) taskTemp.timer += Time.fixedDeltaTime;
                if (taskTemp.timer >= taskTemp.delayed_time)
                {
                    taskTemp.func.Invoke();
                    if (!taskTemp.isRepeat)
                    {
                        timerTasks.RemoveAt(i);
                        --i;
                        if (timerTasks.Count == 0) break;
                    }
                    else
                    {
                        taskTemp.timer = 0;
                    }
                }
            }

        }

        /// <summary>
        /// 添加固定更新定时事件
        /// </summary>
        /// <param name="time">定时时长</param>
        /// <param name="callback">回调事件</param>
        /// <param name="isrepeat">是否重复(不传入，则默认为不重复执行)</param>
        public static void Add(float delayedTime, Action func, bool isrepeat = false)
        {
            CreateDontDestroyGO();
            if (func != null)
            {
                bool isContain = false;
                for (int i = 0; i < timerTasks.Count; i++)
                {
                    if (timerTasks[i].func.Equals(func))
                    {
                        isContain = true;
                        break;
                    }
                }
                if (!isContain) timerTasks.Add(new TimerTask(delayedTime, isrepeat, func));
            }
        }
        /// <summary>
        /// 是否暂停计时器
        /// </summary>
        static bool pause = false;
        /// <summary>
        /// 是否暂停计时器
        /// </summary>
        static void isPause(EventPara ep)
        {
            if (ep != null && ep.Paras.Length == 1 && ep[0].GetType().Equals(typeof(bool)))
            {
                pause = (bool)ep[0];
            }
        }

        /// <summary>
        /// 暂停延时事件的计时
        /// </summary>
        /// <param name="callback"></param>
        public static void Pause(Action func)
        {
            if (func != null)
            {
                for (int i = 0; i < timerTasks.Count; i++)
                {
                    var taskTemp = timerTasks[i];
                    if (taskTemp.func.Equals(func))
                    {
                        taskTemp.isPause = true;
                    }
                }
            }

        }

        /// <summary>
        /// 结束事件的计时暂停状态
        /// </summary>
        /// <param name="callback"></param>
        public static void UnPause(Action func)
        {
            if (func != null)
            {
                for (int i = 0; i < timerTasks.Count; i++)
                {
                    var taskTemp = timerTasks[i];
                    if (taskTemp.func.Equals(func))
                    {
                        taskTemp.isPause = false;
                    }
                }
            }
        }

        /// <summary>
        /// 移除指定事件
        /// </summary>
        /// <param name="callback"></param>
        public static void Remove(Action func)
        {
            if (func != null)
            {
                for (int i = 0; i < timerTasks.Count; i++)
                {
                    var taskTemp = timerTasks[i];
                    if (taskTemp.func.Equals(func))
                    {
                        timerTasks.Remove(taskTemp);
                    }
                }
            }
        }
        /// <summary>
        /// 清空定时任务
        /// </summary>
        public static void Clear()
        {
            timerTasks.Clear();
        }
    }
}
