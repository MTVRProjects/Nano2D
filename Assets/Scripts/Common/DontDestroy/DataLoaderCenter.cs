using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

namespace HMLFramwork.Res
{
    /// <summary>
    /// 数据结构：数据加载任务
    /// </summary>
    public class LoadTask
    {
        /// <summary>
        /// 数据所在路径或地址
        /// </summary>
        public string path_or_Uri;
        /// <summary>
        /// 数据加载完后的回调
        /// </summary>
        public Action<UnityWebRequest> done_callback;

        /// <summary>
        /// 数据加载失败的回调
        /// </summary>
        public Action fail_callback = null;
        public LoadTask() { }

        public LoadTask(string path_or_Uri, Action<UnityWebRequest> done_callback, Action fail_callback = null)
        {
            this.path_or_Uri = path_or_Uri;
            this.done_callback = done_callback;
            this.fail_callback = fail_callback;
        }
    }

    /// <summary>
    /// 数据加载代理，使用UnityWebRequest进行加载
    /// </summary>
    public class DataLoaderCenter : DontDestroyGO<DataLoaderCenter>
    {

        /// <summary>
        /// 数据加载队列
        /// </summary>
        static Queue<LoadTask> loadDataTask = new Queue<LoadTask>();

        /// <summary>
        /// 添加数据加载任务
        /// </summary>
        /// <param name="path_or_Uri">数据加载路径</param>
        /// <param name="callback">数据加载完后的回调</param>
        public static void Add(string path_or_Uri, Action<UnityWebRequest> done_callback, Action fail_callback = null)
        {
            CreateDontDestroyGO();
            if (!string.IsNullOrEmpty(path_or_Uri))
            {
                loadDataTask.Enqueue(new LoadTask(path_or_Uri, done_callback, fail_callback));
            }
        }

        /// <summary>
        /// 添加数据加载任务
        /// </summary>
        /// <param name="path_or_Uri">数据加载路径</param>
        /// <param name="callback">数据加载完后的回调</param>
        /// <param name="encoding">解析加载完数据后需要用的编码</param>
        public static void Add(string path_or_Uri, Action<string> done_callback, Action fail_callback = null, Encoding encoding = null)
        {
            CreateDontDestroyGO();
            if (!string.IsNullOrEmpty(path_or_Uri))
            {
                Add(path_or_Uri, (request) =>
                {
                    string text = encoding == null ? Encoding.UTF8.GetString(request.downloadHandler.data) : encoding.GetString(request.downloadHandler.data);
                    done_callback(text);
                }, fail_callback);
            }
        }

        LoadTask task_temp;
        // Update is called once per frame
        void Update()
        {
            if (loadDataTask.Count > 0)
            {
                task_temp = loadDataTask.Dequeue();
                excuteLoadTask(task_temp);
            }
        }

        /// <summary>
        /// 执行数据加载任务
        /// </summary>
        /// <param name="task_temp"></param>
        void excuteLoadTask(LoadTask task_temp)
        {
            if (task_temp.done_callback.GetType().Equals(typeof(Action<UnityWebRequest>)))
            {
                StartCoroutine(loadData(task_temp.path_or_Uri, (Action<UnityWebRequest>)task_temp.done_callback,task_temp.fail_callback));
            }
        }

        /// <summary>
        /// 使用协程来等待数据加载完毕
        /// </summary>
        /// <param name="path_or_Uri"></param>
        /// <param name="callback"></param>
        /// <returns></returns>
        IEnumerator loadData(string path_or_Uri, Action<UnityWebRequest> callback, Action fail_callback)
        {
            UnityWebRequest request = UnityWebRequest.Get(path_or_Uri);
            yield return request.SendWebRequest();
            if (request.isHttpError || request.isNetworkError)
            {
                if (fail_callback != null)
                {
                    fail_callback.Invoke();
                }
                Debug.Log(request.error);
            }
            else
            {
                callback(request);
            }
            request.Dispose();
        }
    }
}
