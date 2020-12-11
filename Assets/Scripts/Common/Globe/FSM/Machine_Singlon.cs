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
    public class Machine_Singlon<Child,StateID>:StateBase where Child : new()
    {
        static Child _ins;
        public static Child Ins
        {
            get
            {
                if (_ins == null)
                {
                    _ins = new Child();
                }
                return _ins;
            }
        }
        /// <summary>
        /// 当前的状态
        /// </summary>
        StateBase<StateID> cur_State;
        /// <summary>
        /// 当前状态的ID
        /// </summary>
        StateID cur_StateID = default(StateID);
        /// <summary>
        /// 状态集
        /// </summary>
        Dictionary<StateID, StateBase<StateID>> states = new Dictionary<StateID, StateBase<StateID>>();

        private event Action OnUpdateHandle = () => { };
        private event Action OnFixedUpdateHandle = () => { };
        private event Action OnLateUpdateHandle = () => { };

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

        public StateBase<StateID> getCur_State
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

        public void StartMachine()
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
        public void Change(StateID id)
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
            }
        }

        public override void Init()
        {
            OnUpdateHandle = () => { };
            OnFixedUpdateHandle = () => { };
            OnLateUpdateHandle = () => { };
        }

    }
}
