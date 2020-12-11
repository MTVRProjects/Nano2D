using System;
using System.Collections.Generic;

namespace HMLFramwork
{
    public enum UpdateType
    {
        Update,
        FixedUpdate,
        LateUpdate
    }
    /// <summary>
    /// 固定更新中心
    /// </summary>
    public class UpdateCenter : DontDestroyGO<UpdateCenter>
    {
        #region Update
        static List<Action> updateEventList = new List<Action>();
        static event Action OnUpdateHandle;

        /// <summary>
        /// 将需要更新的函数注册到事件中
        /// </summary>
        /// <param name="update">需要更新的函数</param>
        private static void Add(Action update)
        {
            if (update == null) return;
            CreateDontDestroyGO();
            if (updateEventList.Count < 1)
            {
                updateEventList.Add(update);
                OnUpdateHandle = update;
            }
            else
            {
                if (!updateEventList.Contains(update))
                {
                    updateEventList.Add(update);
                    OnUpdateHandle += update;
                }
            }
        }
        void Update()
        {
            if (OnUpdateHandle == null) return;
            OnUpdateHandle();
        }
        #endregion

        #region FixedUpdate
        static List<Action> fixedUpdateEventList = new List<Action>();
        static event Action OnFixedUpdateHandle;
        /// <summary>
        /// 将需要更新的函数注册到事件中
        /// </summary>
        /// <param name="update">需要更新的函数</param>
        private static void Add_Fixed(Action fixedUpdate)
        {
            if (fixedUpdate == null) return;
            CreateDontDestroyGO();
            if (fixedUpdateEventList.Count < 1)
            {
                fixedUpdateEventList.Add(fixedUpdate);
                OnFixedUpdateHandle = fixedUpdate;
            }
            else
            {
                if (!fixedUpdateEventList.Contains(fixedUpdate))
                {
                    fixedUpdateEventList.Add(fixedUpdate);
                    OnFixedUpdateHandle += fixedUpdate;
                }
            }
        }

        void FixedUpdate()
        {
            if (OnFixedUpdateHandle == null) return;
            OnFixedUpdateHandle();
        }

        #endregion

        #region LateUpdate
        static List<Action> lateUpdateEventList = new List<Action>();
        static event Action OnLateUpdateHandle;
        /// <summary>
        /// 将需要更新的函数注册到事件中
        /// </summary>
        /// <param name="update">需要更新的函数</param>
        static void Add_Late(Action lateUpdate)
        {
            if (lateUpdate == null) return;
            CreateDontDestroyGO();
            if (lateUpdateEventList.Count < 1)
            {
                lateUpdateEventList.Add(lateUpdate);
                OnLateUpdateHandle = lateUpdate;
            }
            else
            {
                if (!lateUpdateEventList.Contains(lateUpdate))
                {
                    lateUpdateEventList.Add(lateUpdate);
                    OnLateUpdateHandle += lateUpdate;
                }
            }
        }

        private void LateUpdate()
        {
            if (OnLateUpdateHandle == null) return;
            OnLateUpdateHandle();
        }

        #endregion
        #region Public
        /// <summary>
        /// 添加函数（默认添加到Update中）
        /// </summary>
        /// <param name="func"></param>
        /// <param name="type"></param>
        public static void Add(Action func, UpdateType type = UpdateType.Update)
        {
            switch (type)
            {
                case UpdateType.Update:
                    Add(func);
                    break;
                case UpdateType.FixedUpdate:
                    Add_Fixed(func);
                    break;
                case UpdateType.LateUpdate:
                    Add_Late(func);
                    break;
            }
        }

        /// <summary>
        /// 将需要更新的函数从事件中移除
        /// </summary>
        /// <param name="update">需要更新的函数</param>
        public static void Remove(Action func)
        {
            if (func == null) return;
            if (updateEventList.Contains(func))
            {
                updateEventList.Remove(func);
                OnUpdateHandle -= func;
            }
            else if (fixedUpdateEventList.Contains(func))
            {
                fixedUpdateEventList.Remove(func);
                OnFixedUpdateHandle -= func;
            }
            else if (lateUpdateEventList.Contains(func))
            {
                lateUpdateEventList.Remove(func);
                OnLateUpdateHandle -= func;
            }
        }

        #endregion
    }

}