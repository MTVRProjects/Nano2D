using System;
using System.Collections.Generic;
using UnityEngine;

namespace HMLFramwork.Helpers
{
    /// <summary>
    /// 音频通用工具
    /// </summary>
    public class AudioUtility : DontDestroyGO<AudioUtility>
    {
        /// <summary>
        /// 事件结构
        /// </summary>
        public struct FuncBody
        {
            /// <summary>
            /// 事件类型
            /// </summary>
            public AS_State _eventType;
            /// <summary>
            /// 事件
            /// </summary>
            public Action _func;
            public FuncBody(AS_State et, Action func)
            {
                _eventType = et;
                _func = func;
            }
        }
        /// <summary>
        /// 音频状态
        /// </summary>
        public enum AS_State
        {
            ///默认
            Default,
            /// <summary>
            /// 播放中
            /// </summary>
            Playing,
            /// <summary>
            /// 暂停
            /// </summary>
            Pause,
            /// <summary>
            /// 取消暂停
            /// </summary>
            UnPause,
            /// <summary>
            /// 静音
            /// </summary>
            Mute,
            /// <summary>
            /// 取消静音
            /// </summary>
            UnMute,
            /// <summary>
            /// 停止
            /// </summary>
            Stop,
            /// <summary>
            /// 播放完成
            /// </summary>
            Done
        }

        /// <summary>
        /// 音频组件列表
        /// </summary>
        private static List<AudioSource> _asList = new List<AudioSource>();
        /// <summary>
        /// 事件状态列表
        /// </summary>
        private static List<AS_State> _asStateList = new List<AS_State>();
        /// <summary>
        /// 事件体列表
        /// </summary>
        private static List<FuncBody> _funcBodyList = new List<FuncBody>();

        /// <summary>
        /// 播放音频（返回音频组件）
        /// </summary>
        /// <param name="clip">需要被播放的音频</param>
        public static AudioSource Play(GameObject obj, AudioClip clip)
        {
            AudioSource source = null;
            if (obj)
            {
                source = obj.GetComponent<AudioSource>();
                if (source == null) source = obj.AddComponent<AudioSource>();
                if (!source.enabled) source.enabled = true;
                source.Stop();
                source.volume = 1f;
                if (clip == null) return source;
                source.clip = clip;
                source.Play();
            }
            else Debug.Log("警告：对象未赋值！");
            return source;
        }
        /// <summary>
        /// 播放音频（返回音频组件）
        /// </summary>
        public static AudioSource Play(AudioSource m_as, AudioClip clip)
        {
            if (m_as != null)
            {
                if (clip != null)
                {
                    m_as.Stop();
                    m_as.volume = 1.0f;
                    m_as.clip = clip;
                    m_as.Play();
                }
            }
            else Debug.Log("警告：对象未赋值！");
            return m_as;
        }

        /// <summary>
        /// 播放音频；第一个参数：游戏对象 / AudioSourse；第二个：音频文件；第三个：音量 / 是否循环播放
        /// </summary>
        /// <param name="erg">第一个参数：游戏对象；第二个：音频文件；第三个：音量/是否循环</param>
        public static AudioSource Play(params object[] parameters)
        {
            AudioSource source = null;
            if (parameters != null)
            {
                if (parameters[0].GetType().Equals(typeof(GameObject)) || parameters[0].GetType().Equals(typeof(AudioSource)))
                {
                    if (parameters[0].GetType().Equals(typeof(GameObject)))
                    {
                        source = ((GameObject)parameters[0]).GetComponent<AudioSource>();
                        if (source == null) source = ((GameObject)parameters[0]).AddComponent<AudioSource>();
                    }
                    else source = (AudioSource)parameters[0];

                    if ((AudioClip)parameters[1] == null) return null;

                    if ((AudioClip)parameters[1])
                    {
                        source.clip = (AudioClip)parameters[1];
                    }
                    if (parameters[2] != null)
                    {
                        if (parameters[2].GetType().Equals(typeof(float))) source.volume = (float)parameters[2];
                        else if (parameters[2].GetType().Equals(typeof(bool))) source.loop = (bool)parameters[2];
                    }
                    source.Play();
                }
            }
            else Debug.Log("警告：对象未赋值！");
            return source;
        }

        /// <summary>
        /// 播放音频
        /// </summary>
        /// <param name="go">在指定对象上添加音频组件并播放</param>
        /// <param name="clip">需要播放的音频</param>
        /// <param name="callback">回调函数</param>
        /// <param name="et">回调函数的类型</param>
        public static AudioSource Play(GameObject go, AudioClip clip, Action callback = null, AS_State et = AS_State.Done)
        {
            CreateDontDestroyGO();
            AudioSource _as = Play(go, clip);

            if (callback != null)
            {
                FuncBody funcBody = new FuncBody(et, callback);
                if (!_asList.Contains(_as))
                {
                    _asList.Add(_as);
                    _asStateList.Add(AS_State.Playing);
                    _funcBodyList.Add(funcBody);
                }
                else
                {
                    Debug.Log("已包含改音频组件...");
                    excuteEvent(go,AS_State.Playing);
                }
            }
            return _as;
        }

        public static AudioSource Play(AudioSource _as, AudioClip clip, Action callback = null, AS_State et = AS_State.Done)
        {
            CreateDontDestroyGO();
            if (_as) Play(_as, clip);
            if (callback != null)
            {
                FuncBody funcBody = new FuncBody(et, callback);
                if (!_asList.Contains(_as))
                {
                    _asList.Add(_as);
                    _asStateList.Add(AS_State.Playing);
                    _funcBodyList.Add(funcBody);
                }
                else
                {
                    Debug.Log("已包含改音频组件...");
                    excuteEvent(_as,AS_State.Playing);
                }
            }
            return _as;
        }

        /// <summary>
        /// 更新音频组件状态
        /// </summary>
        void Update()
        {
            if (_asList.Count > 0)
            {
                int count = _asList.Count;
                for (int i = 0; i < count; i++)
                {
                    if (!_asList[i].isPlaying && _asStateList[i] != AS_State.Pause) 
                    {
                        if (_funcBodyList[i]._eventType == AS_State.Done)
                        {
                            _funcBodyList[i]._func();
                        }
                        _asList.RemoveAt(i);
                        _funcBodyList.RemoveAt(i);
                        _asStateList.RemoveAt(i);
                        --i;--count;
                        if (_asList.Count == 0) break;
                    }
                }
            }
        }

        /// <summary>
        /// 执行相应的事件
        /// </summary>
        /// <param name="index"></param>
        /// <param name="state"></param>
        static void excuteEvent(int index, AS_State state)
        {
            //更新音频组件的状态
            _asStateList[index] = state;
            //执行对应状态的回调函数
            if (_funcBodyList[index]._eventType == state)
                _funcBodyList[index]._func();
        }
        /// <summary>
        /// 执行相应的事件
        /// </summary>
        /// <param name="index"></param>
        /// <param name="state"></param>
        static void excuteEvent(GameObject go, AS_State state)
        {
            if (go && go.GetComponent<AudioSource>())
            {
                AudioSource _as = go.GetComponent<AudioSource>();
                excuteEvent(_as,state);
            }
        }

        /// <summary>
        /// 执行相应的事件
        /// </summary>
        /// <param name="index"></param>
        /// <param name="state"></param>
        static void excuteEvent(AudioSource _as, AS_State state)
        {
            if (_as)
            {
                for (int i = 0; i < _asList.Count; i++)
                {
                    if (_as.Equals(_asList[i]))
                    {
                        //更新音频组件的状态
                        _asStateList[i] = state;
                        //执行对应状态的回调函数
                        if (_funcBodyList[i]._eventType == state)
                            _funcBodyList[i]._func();
                    }
                }
            }
        }

        /// <summary>
        /// 操作所有的音频组件（暂停、取消暂停、停止播放）
        /// </summary>
        /// <param name="state"></param>
        public static void OperateAll(AS_State state)
        {
            if (_asList.Count > 0)
            {
                //1.先操作
                for (int i = 0; i < _asList.Count; i++)
                {
                    switch (state)
                    {
                        case AS_State.Pause:
                            _asList[i].Pause();
                            break;
                        case AS_State.UnPause:
                            _asList[i].UnPause();
                            break;
                        case AS_State.Stop:
                            _asList[i].Stop();
                            break;
                        case AS_State.Mute:
                            _asList[i].mute = true;
                            break;
                        case AS_State.UnMute:
                            _asList[i].mute = false;
                            break;
                    }
                    //2.操作完后执行对应回调
                    excuteEvent(i,state);
                }
                //3.如果执行的操作是停止，则清楚所有音频组件和对应回调
                if (state == AS_State.Stop)
                {
                    _asList.Clear();
                    _funcBodyList.Clear();
                }
            }
        }

        /// <summary>
        /// 暂停所有的音频播放
        /// </summary>
        public static void PauseAll()
        {
            if (_asList.Count > 0)
            {
                for (int i = 0; i < _asList.Count; i++)
                {
                    _asList[i].Pause();
                    excuteEvent(i, AS_State.Pause);
                }
            }
        }

        /// <summary>
        /// 所有音频取消暂停状态
        /// </summary>
        public static void UnPauseAll()
        {
            if (_asList.Count > 0)
            {
                for (int i = 0; i < _asList.Count; i++)
                {
                    _asList[i].UnPause();
                    excuteEvent(i, AS_State.UnPause);
                }
            }
        }
        /// <summary>
        /// 停止播放所有的音频
        /// </summary>
        public static void StopAll()
        {
            if (_asList.Count > 0)
            {
                for (int i = 0; i < _asList.Count; i++)
                {
                    _asList[i].Stop();
                    excuteEvent(i, AS_State.Stop);
                    _asList.Clear();
                    _funcBodyList.Clear();
                    _asStateList.Clear();
                }
            }
        }

        /// <summary>
        /// 全部静音
        /// </summary>
        public static void MuteAll()
        {
            if (_asList.Count > 0)
            {
                for (int i = 0; i < _asList.Count; i++)
                {
                    _asList[i].mute = true;
                    excuteEvent(i, AS_State.Mute);
                }
            }
        }

        /// <summary>
        /// 取消所有的静音状态
        /// </summary>
        public static void UnMuteAll()
        {
            if (_asList.Count > 0)
            {
                for (int i = 0; i < _asList.Count; i++)
                {
                    _asList[i].mute = false;
                    excuteEvent(i, AS_State.UnMute);
                }
            }
        }

        /// <summary>
        /// 暂停
        /// </summary>
        public static AudioSource Pause(GameObject go)
        {
            AudioSource source = null;
            if (go && go.GetComponent<AudioSource>())
            {
                try
                {
                    source = go.GetComponent<AudioSource>();
                    source.Pause();
                    excuteEvent(go, AS_State.Pause);
                }
                catch
                {
                    Debug.Log("提醒：暂停失败...");
                }
            }
            else
            {
                Debug.Log("警告：对象未赋值，或没有音频组件...");
            }
            return source;
        }
        /// <summary>
        /// 取消暂停
        /// </summary>
        /// <param name="go"></param>
        public static AudioSource UnPause(GameObject go)
        {
            AudioSource source = null;
            if (go && go.GetComponent<AudioSource>())
            {
                try
                {
                    source = go.GetComponent<AudioSource>();
                    source.UnPause();
                    excuteEvent(go, AS_State.UnPause);
                }
                catch
                {
                    Debug.Log("提醒：取消暂停失败...");
                }
            }
            else
            {
                Debug.Log("警告：对象未赋值，或没有音频组件...");
            }
            return source;
        }
        /// <summary>
        /// 停止播放
        /// </summary>
        /// <param name="go"></param>
        public static AudioSource Stop(GameObject go,bool isEnable = true)
        {
            AudioSource source = null;
            if (go && go.GetComponent<AudioSource>())
            {
                try
                {
                    source = go.GetComponent<AudioSource>();
                    source.Stop();
                    source.enabled = isEnable;
                    excuteEvent(go, AS_State.Stop);
                }
                catch
                {
                    Debug.Log("提醒：取消暂停失败...");
                }
            }
            else
            {
                Debug.Log("警告：对象未赋值，或没有音频组件...");
            }
            return source;
        }

        /// <summary>
        /// 静音
        /// </summary>
        /// <param name="go"></param>
        public static AudioSource Mute(GameObject go)
        {
            AudioSource source = null;
            if (go && go.GetComponent<AudioSource>())
            {
                try
                {
                    source = go.GetComponent<AudioSource>();
                    source.mute = true;
                    excuteEvent(go, AS_State.Mute);
                }
                catch
                {
                    Debug.Log("提醒：静音失败...");
                }
            }
            else
            {
                Debug.Log("警告：对象未赋值，或没有音频组件...");
            }
            return source;
        }
        /// <summary>
        /// 取消静音
        /// </summary>
        /// <param name="go"></param>
        public static AudioSource UnMute(GameObject go)
        {
            AudioSource source = null;
            if (go && go.GetComponent<AudioSource>())
            {
                try
                {
                     source = go.GetComponent<AudioSource>();
                    source.mute = false;
                    excuteEvent(go, AS_State.UnMute);
                }
                catch
                {
                    Debug.Log("提醒：静音失败...");
                }
            }
            else
            {
                Debug.Log("警告：对象未赋值，或没有音频组件...");
            }
            return source;
        }
        
    }
}

