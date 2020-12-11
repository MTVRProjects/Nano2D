//================================================
//描述 ：状态基类 
//作者 ：
//创建时间 ：2019/01/30 09:30:21  
//版本： 
//================================================

using System;

namespace HMLFramwork.FSM
{
    public class StateBase<Ins_T, ID_T> : StateBase<ID_T> where Ins_T : new()
    {
        static Ins_T _ins;
        public static Ins_T Ins
        {
            get
            {
                if (_ins == null)
                {
                    _ins = new Ins_T();
                }
                return _ins;
            }
        }
        public ActionState cur_ActionState = ActionState.DEFAULT;

    }

    public class StateBase<T> : StateBase
    {
        /// <summary>
        /// 状态类型
        /// </summary>
        public T ID;

        public StateBase()
        {
        }
        public StateBase(T state_id)
        {
            ID = state_id;
        }
        public StateBase(T state_id, Action _onEnterHandle, Action _onExitHandle, Action _onInitHandle = null)
        {
            ID = state_id;
            onEnterHandle += _onEnterHandle;
            onExitHandle += _onExitHandle;
            onInitHandle += _onInitHandle;
        }
    }
    public class StateBase : IStateBase
    {
        public event Action onEnterHandle = () => { };
        public event Action onExitHandle = () => { };
        public event Action onInitHandle = () => { };

        public StateBase()
        {

        }
        public StateBase(Action _onEnterHandle, Action _onExitHandle, Action _onInitHandle = null)
        {
            onEnterHandle = _onEnterHandle;
            onExitHandle = _onExitHandle;
            onInitHandle = _onInitHandle;
        }

        /// <summary>
        /// 进入状态
        /// </summary>
        public virtual void Enter()
        {
            onEnterHandle();
        }
        /// <summary>
        /// 退出状态
        /// </summary>
        public virtual void Exit()
        {
            onExitHandle();
        }
        /// <summary>
        /// 状态初始化
        /// </summary>
        public virtual void Init()
        {
            onInitHandle();
        }

        public virtual void Reset()
        {
            onEnterHandle = () => { };
            onExitHandle = () => { };
            onInitHandle = () => { };
        }
        #region Unity函数

        /// <summary>
        /// 状态更新
        /// </summary>
        public virtual void update() { }

        public virtual void fixedUpdate() { }

        public virtual void lateUpdate() { }
        #endregion

    }

    public interface IStateBase : IState
    {

        #region Unity函数
        void update();
        void fixedUpdate();
        void lateUpdate();
        #endregion
    }

    public interface IState
    {
        /// <summary>
        /// 进入状态
        /// </summary>
        void Enter();
        /// <summary>
        /// 退出状态
        /// </summary>
        void Exit();
        /// <summary>
        /// 状态初始化
        /// </summary>
        void Init();
    }
}


