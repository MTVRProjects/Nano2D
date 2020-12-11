using UnityEngine;
using System.Collections;
using System.Threading;
using System;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// 加入延迟时间的主线程任务
/// </summary>
public struct DelayedQueueItem
{
    public float time;
    public Action action;
}
public class Loom : DontDestroyGO<Loom> {
    /// <summary>
    /// 最大线程数，默认值为8
    /// </summary>
    public static int maxThreads = 8;
    /// <summary>
    /// 当前使用的线程数
    /// </summary>
    static int numThreads;

    /// <summary>
    /// 主线程任务链表
    /// </summary>
    private List<Action> _actions = new List<Action>();
    /// <summary>
    /// 封装好的主线程任务链表
    /// </summary>
    private List<DelayedQueueItem> _delayed = new List<DelayedQueueItem>();

    /// <summary>
    /// 缓存区：封装好的主线程任务链表
    /// </summary>
    List<DelayedQueueItem> _currentDelayed = new List<DelayedQueueItem>();

    /// <summary>
    /// 标志位：是否进行了初始化
    /// </summary>
    static bool isInit = false;
    /// <summary>
    /// 初始化
    /// </summary>
    static void Init()
    {
        if (!isInit)
        {
            if (!Application.isPlaying) return;
            CreateDontDestroyGO();
            isInit = true;
        }
    }
    public static void QueueOnMainThread(Action action)
    {
        QueueOnMainThread(action, 0f);
    }

    public static void QueueOnMainThread(Action action, float time)
    {

        if (time != 0)
        {
            lock (Ins._delayed)
            {
                Ins._delayed.Add(new DelayedQueueItem { time = Time.time + time, action = action });
            }
        }
        else
        {
            lock (Ins._actions)
            {
                Ins._actions.Add(action);
            }
        }
    }

    //异步线程
    public static Thread RunAsync(Action a)
    {
        Init();
        while (numThreads >= maxThreads)
        {
            Thread.Sleep(1);
        }
        //以原子操作的形式递增指定变量的值并存储结果，相当于 lock（obj）{numThreads++；}
        Interlocked.Increment(ref numThreads);
        //将子线程任务加入线程池中
        ThreadPool.QueueUserWorkItem(RunAction, a);
        return null;
    }

    private static void RunAction(object action)
    {
        try { ((Action)action)(); }
        catch { }
        finally {
            //以原子操作的形式递减指定变量的值并存储结果，相当于 lock（obj）{numThreads--；}
            Interlocked.Decrement(ref numThreads); 
        }
    }

    /// <summary>
    /// 缓冲区：主线程任务链表
    /// </summary>
    List<Action> _currentActions = new List<Action>();

    /// <summary>
    /// 主线程任务缓冲链表的任务数量
    /// </summary>
    int delayedCount = 0;
    // 每帧更新调用一次
    void Update()
    {
        lock (_actions)
        {
            _currentActions.Clear();
            _currentActions.AddRange(_actions);
            _actions.Clear();
        }

        for (int i = 0; i < _currentActions.Count; i++)
            _currentActions[i]();
        delayedCount = _currentDelayed.Count;
        lock (_delayed)
        {
            _currentDelayed.Clear();
            _currentDelayed.AddRange(_delayed.Where(d => d.time <= Time.time));
            for (int i = 0; i < delayedCount; i++)
                _delayed.Remove(_currentDelayed[i]);
        }
        for (int i = 0; i < delayedCount; i++)
            _currentDelayed[i].action();
    }

}