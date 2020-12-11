using UnityEngine;
using System;

namespace HMLFramwork
{
    /// <summary>
    /// 音、视频功能基类
    /// </summary>
    public class AVFuncBase : FuncBase
    {
        protected int myOrder = -1;
        public virtual int MyOrder { get; private set; }

        /// <summary>
        /// 播放音频
        /// </summary>
        public virtual void Excute(int AVIndex, Action callback = null)
        {
            Excute(callback);
        }

        public virtual void Excute(Action callback = null)
        {
            base.Excute();
            //执行回调
            if (callback != null )
            {
                callback.Invoke();
            }
        }

        /// <summary>
        /// 重复播放
        /// </summary>
        /// <param name="clipIndex"></param>
        public virtual void RepeatPlay(int clipIndex) { }

        /// <summary>
        /// 暂停
        /// </summary>
        public virtual void Pause()
        {
            MyFuncState = ActionState.PAUSE;
        }

        /// <summary>
        /// 暂停后重新开始
        /// </summary>
        public virtual void UnPause()
        {
            MyFuncState = ActionState.DOING;
        }

        /// <summary>
        /// 停止播放
        /// </summary>
        public virtual void Stop(int AVIndex)
        {
            base.Stop();
        }

        /// <summary>
        /// 所有的字段必须初始化（引用型字段必须赋空值）
        /// </summary>
        public override void Init()
        {
            base.Init();
            RecoverVoice();
            myOrder = -1;
        }
        /// <summary>
        /// 静音
        /// </summary>
        public virtual void Mute()
        {

        }

        public virtual void RecoverVoice() { }

    }
}
