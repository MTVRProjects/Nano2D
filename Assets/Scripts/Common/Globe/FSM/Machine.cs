//================================================
//描述 ： 状态机
//解释：1.一个状态类型对应一个状态组（stateList）；
//2.一个目标状态可以被加入到多个状态组中，但每个状态组只能包含一个目标状态。
//作者 ：HML
//创建时间 ：2019/01/30 09:47:23  
//版本： 2.0
//================================================
using System.Collections.Generic;
using HMLFramwork.Log;
using System;

namespace HMLFramwork.FSM
{

    public class Machine
    {
        public Machine()
        {

        }

        /// <summary>
        /// 当前的状态
        /// </summary>
        protected IStateBase cur_State;
        /// <summary>
        /// 当前状态的ID
        /// </summary>
        protected object cur_StateID = default(object);
        /// <summary>
        /// 状态集
        /// </summary>
        protected Dictionary<object, IStateBase> states = new Dictionary<object, IStateBase>();

        protected event Action OnUpdateHandle = () => { };
        protected event Action OnFixedUpdateHandle = () => { };
        protected event Action OnLateUpdateHandle = () => { };

        private event Action<object> onChangeHandle;
        /// <summary>
        /// 通过ID获取一个状态
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        protected virtual IStateBase getSate(object id)
        {
            IStateBase _state = default(IStateBase);
            if (states.ContainsKey(id))
            {
                states.TryGetValue(id, out _state);
            }
            else LogQueue.Add(string.Format("提醒：状态机中并不存在ID为{0}的状态", id.ToString()));
            return _state;
        }

        public IStateBase getCur_State
        {
            get
            {
                if (cur_State == null)//如果当前状态为空，则选择第一个状态
                {
                    cur_State = states.getValues()[0];
                }
                return cur_State;
            }
        }

        /// <summary>
        /// 添加状态
        /// </summary>
        /// <param name="state_id">状态ID</param>
        /// <param name="state"></param>
        public virtual void Add(object state_id, IStateBase state)
        {
            if (!states.ContainsValue(state))
            {
                states.Add(state_id, state);
            }
            else LogQueue.Add(string.Format("提醒：{0}已存在...", state.ToString()));
        }
        /// <summary>
        /// 删除一个状态
        /// </summary>
        /// <param name="state"></param>
        public void Remove(IStateBase state)
        {
            if (states.ContainsValue(state))
            {
                states.Remove(state);
            }
            else LogQueue.Add(string.Format("提醒：{0}不存在...", state.ToString()));
        }
        /// <summary>
        /// 删除一个状态
        /// </summary>
        public void Remove(object id)
        {
            if (states.ContainsKey(id))
            {
                states.Remove(id);
            }
            else LogQueue.Add(string.Format("提醒：{0}不存在...", id.ToString()));
        }

        public virtual void StartMachine()
        {
            //1. 加入函数
            UpdateCenter.Add(OnUpdateHandle, UpdateType.Update);
            UpdateCenter.Add(OnFixedUpdateHandle, UpdateType.FixedUpdate);
            UpdateCenter.Add(OnLateUpdateHandle, UpdateType.LateUpdate);

        }

        /// <summary>
        /// 状态切换
        /// </summary>
        /// <param name="id"></param>
        public void Change(object id, object changeHandleParam = null)
        {
            if (id.Equals(cur_StateID)) return;
            else
            {
                //1.退出状态
                if (cur_State != null) cur_State.Exit();
                //2.记录新切换的状态
                cur_StateID = id;
                cur_State = getSate(id);
                //3.进入新状态
                cur_State.Enter();
                //4.绑定新状态更新内容
                OnUpdateHandle = cur_State.update;
                OnFixedUpdateHandle = cur_State.fixedUpdate;
                OnLateUpdateHandle = cur_State.lateUpdate;

                if (onChangeHandle != null && changeHandleParam != null)
                {
                    onChangeHandle.Invoke(changeHandleParam);
                }
            }
        }

        public virtual void Init()
        {
            OnUpdateHandle = () => { };
            OnFixedUpdateHandle = () => { };
            OnLateUpdateHandle = () => { };
        }

    }

    public class Machine<StateID> : Machine
    {
        public Machine()
        {

        }
        private event Action<object> onChangeHandle;
        /// <summary>
        /// 当前的状态
        /// </summary>
        protected new StateBase<StateID> cur_State;

        /// <summary>
        /// 状态集
        /// </summary>
        protected new Dictionary<StateID, StateBase<StateID>> states = new Dictionary<StateID, StateBase<StateID>>();
        /// <summary>
        /// 通过ID获取一个状态
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        private StateBase<StateID> getSate(StateID id)
        {
            StateBase<StateID> _state = default(StateBase<StateID>);
            if (states.ContainsKey(id))
            {
                states.TryGetValue(id, out _state);
            }
            else LogQueue.Add(string.Format("提醒：状态机中并不存在ID为{0}的状态", id.ToString()));
            return _state;
        }

        public new StateBase<StateID> getCur_State
        {
            get
            {
                if (cur_State == null)
                {
                    cur_State = states.getValues()[0];
                }
                return cur_State;
            }
        }
        /// <summary>
        /// 添加状态
        /// </summary>
        /// <param name="state"></param>
        public void Add(StateBase<StateID> state)
        {
            if (!states.ContainsValue(state))
            {
                states.Add(state.ID, state);
            }
            else LogQueue.Add(string.Format("提醒：{0}已存在...", state.ToString()));
        }

        /// <summary>
        /// 添加状态
        /// </summary>
        /// <param name="state_id">状态ID</param>
        /// <param name="state"></param>
        public void Add(StateID state_id, StateBase<StateID> state)
        {
            if (!states.ContainsValue(state))
            {
                state.ID = state_id;
                states.Add(state_id, state);
            }
            else LogQueue.Add(string.Format("提醒：{0}已存在...", state.ToString()));
        }
        /// <summary>
        /// 删除一个状态
        /// </summary>
        /// <param name="state"></param>
        public void Remove(StateBase<StateID> state)
        {
            if (states.ContainsValue(state))
            {
                states.Remove(state);
            }
            else LogQueue.Add(string.Format("提醒：{0}不存在...", state.ToString()));
        }
        /// <summary>
        /// 删除一个状态
        /// </summary>
        public void Remove(StateID id)
        {
            if (states.ContainsKey(id))
            {
                states.Remove(id);
            }
            else LogQueue.Add(string.Format("提醒：{0}不存在...", id.ToString()));
        }



        /// <summary>
        /// 状态切换
        /// </summary>
        /// <param name="id"></param>
        public void Change(StateID id,object changeHandleParam)
        {
            if (id.Equals(cur_StateID)) return;
            else
            {
                //1.退出状态
                if (cur_State != null) cur_State.Exit();
                OnUpdateHandle -= cur_State.update;
                OnFixedUpdateHandle -= cur_State.fixedUpdate;
                OnLateUpdateHandle -= cur_State.lateUpdate;

                //2.记录新切换的状态
                cur_StateID = id;
                cur_State = getSate(id);
                //3.进入新状态
                cur_State.Enter();
                //4.绑定新状态更新内容
                OnUpdateHandle += cur_State.update;
                OnFixedUpdateHandle += cur_State.fixedUpdate;
                OnLateUpdateHandle += cur_State.lateUpdate;

                if (onChangeHandle != null && changeHandleParam != null)
                {
                    onChangeHandle.Invoke(changeHandleParam);
                }
            }
        }

        public override void Init()
        {
            base.Init();
        }

    }
}
