using UnityEngine;
using System;
using HMLFramwork.CommonMgr;

namespace HMLFramwork.AV
{
    /// <summary>
    /// 音、视频功能管理基类
    /// </summary>
    public class AVFuncMgrBase : MgrBase<int, AVFuncBase>
    {
        [HideInInspector]
        public ActionState MyFuncState = ActionState.DEFAULT;
        protected override void Awake()
        {
            base.Awake();
        }

        /// <summary>
        /// 开启功能，只能同时播放一个音频
        /// </summary>
        /// <param name="ep"></param>
        /// <param name="callback"></param>
        public void SinglePlay(EventPara ep, Action callback = null)
        {
            if (ep.Paras.Length == 2)
            {
                if (ep[0].GetType().Equals(typeof(int)) && ep[1].GetType().Equals(typeof(int)))
                {
                    SinglePlay((int)ep[0], (int)ep[1], callback);
                }
                else Debug.Log("警告：传入的参数有误...");
            }
            else if (ep.Paras.Length == 1)
            {
                if (ep[0].GetType().Equals(typeof(int)))
                {
                    SinglePlay((int)ep[0], callback);
                }
                else Debug.Log("警告：传入的参数有误...");
            }
        }

        /// <summary>
        /// 开启功能，只能同时播放一个音频（int funcOrder,int AVIndex,Action callback = null）
        /// </summary>
        /// <param name="funcOrder">功能顺序</param>
        /// <param name="AVIndex">音视频的下标</param>
        /// <param name="callback">回调</param>
        public void SinglePlay(int funcOrder, int AVIndex, Action callback = null)
        {
            //如果有功能正在执行，则停止当前的功能；
            if (cur_Target != null) cur_Target.Stop();
            //然后开启新的功能
            targets.TryGetValue(funcOrder, out cur_Target);
            if (cur_Target) cur_Target.Excute(AVIndex, callback);
            else Debug.Log("警告：无法获取功能组件...");
        }

        /// <summary>
        /// 开启功能，只能同时播放一个音频（int funcOrder, Action callback = null）
        /// </summary>
        /// <param name="funcOrder">功能顺序</param>
        /// <param name="callback">回调</param>
        public void SinglePlay(int funcOrder, Action callback = null)
        {
            // 如果有功能正在执行，则停止当前的功能；
            if (cur_Target != null) cur_Target.Stop();
            //然后开启新的功能
            targets.TryGetValue(funcOrder, out cur_Target);
            if (cur_Target) cur_Target.Excute(callback);
            else Debug.Log("警告：无法获取功能组件...");
        }
        /// <summary>
        /// 可以同时播放多个音频
        /// </summary>
        /// <param name="funcOrder"></param>
        /// <param name="callback"></param>
        public void MultiplePlay(int funcOrder, Action callback = null)
        {
            AVFuncBase _currentFunc;
            targets.TryGetValue(funcOrder, out _currentFunc);
            if (_currentFunc) _currentFunc.Excute(callback);
            else Debug.Log("警告：无法获取功能组件...");
        }

        /// <summary>
        /// 可以同时播放多个音频
        /// </summary>
        /// <param name="funcOrder"></param>
        /// <param name="callback"></param>
        public void MultiplePlay(int funcOrder, int AVIndex, Action callback = null)
        {
            AVFuncBase _currentFunc;
            targets.TryGetValue(funcOrder, out _currentFunc);
            if (_currentFunc) _currentFunc.Excute(AVIndex, callback);
            else Debug.Log("警告：无法获取功能组件...");
        }

        /// <summary>
        /// 可以同时播放多个音频
        /// </summary>
        public void MultiplePlay(EventPara ep, Action callback = null)
        {
            if (ep.Paras.Length == 2)
            {
                if (ep[0].GetType().Equals(typeof(int)) && ep[1].GetType().Equals(typeof(int)))
                {
                    MultiplePlay((int)ep[0], (int)ep[1], callback);
                }
                else Debug.Log("警告：传入的参数有误...");
            }
            else if (ep.Paras.Length == 1)
            {
                if (ep[0].GetType().Equals(typeof(int)))
                {
                    MultiplePlay((int)ep[0], callback);
                }
                else Debug.Log("警告：传入的参数有误...");
            }
        }
        /// <summary>
        /// 停止播放视频
        /// </summary>
        public virtual void Stop()
        {
            foreach (var item in targets)
            {
                item.Value.Stop();
            }
            MyFuncState = ActionState.DONE;
        }

        /// <summary>
        /// 暂停
        /// </summary>
        public virtual void Pause()
        {
            foreach (var item in targets)
            {
                if (item.Value.MyFuncState == ActionState.DOING)
                    item.Value.Pause();
            }
            MyFuncState = ActionState.PAUSE;
        }

        /// <summary>
        /// 暂停后重新开始
        /// </summary>
        public virtual void UnPause()
        {
            foreach (var item in targets)
            {
                if (item.Value.MyFuncState == ActionState.PAUSE)
                    item.Value.UnPause();
            }
            MyFuncState = ActionState.DOING;
        }
        /// <summary>
        /// 全部静音
        /// </summary>
        public virtual void Mute()
        {
            foreach (var item in targets)
            {
                item.Value.Mute();
            }
        }

        /// <summary>
        /// 除了指定的，其他静音
        /// </summary>
        public virtual void MuteExceptTheFunc(int funcIndex)
        {
            if (targets != null && targets.Count > 0)
            {
                for (int i = 0; i < targets.Count; i++)
                {
                    if (i == funcIndex) continue;
                    else targets[i].Mute();
                }
            }
            else Debug.Log("警告：无功能组件");
        }

        /// <summary>
        /// 除了指定的，其他静音
        /// </summary>
        public virtual void MuteExceptTheFunc(params object[] funcIndexs)
        {
            if (targets != null && targets.Count > 0 && funcIndexs.Length < targets.Count)
            {
                for (int i = 0; i < targets.Count; i++)
                {
                    for (int j = 0; j < funcIndexs.Length; j++)
                    {
                        if (i == (int)funcIndexs[j]) continue;
                        else targets[i].Mute();
                    }
                }
            }
            else Debug.Log("警告：传入的条件不符合要求...");
        }
        /// <summary>
        /// 恢复声音状态
        /// </summary>
        public virtual void RecoverVoice()
        {
            foreach (var item in targets)
            {
                item.Value.RecoverVoice();
            }
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
        }

        /// <summary>
        /// 所有的字段必须初始化（引用型字段必须赋空值）
        /// </summary>
        public override void Init()
        {
            base.Init();
            foreach (var item in targets)
            {
                item.Value.Init();
            }
            cur_Target = null;
        }

    }
}
