//================================================
//描 述 ： UI管理基类
//作 者 ：HML
//创建时间 ：2018/05/18  
//版 本： 1.0
// ================================================
using HMLFramwork;
using System.Collections.Generic;
using UnityEngine;

namespace HMLFramwork.CommonMgr
{
    /// <summary>
    /// 管理通用基类；ID：被管理对象的类型ID，TARGET：被管理对象
    /// </summary>
    /// <typeparam name="ID">子节点类型ID</typeparam>
    /// <typeparam name="TARGET">被管理对象</typeparam>
    public class MgrBase<ID, TARGET> : MonoBehaviour where TARGET : Component
    {
        protected virtual void Awake()
        {
            EventCenter.Ins.Add(EventSign.Init, Init);
        }

        #region Basic Fields
        /// <summary>
        /// 当前打开的panel
        /// </summary>
        [HideInInspector]
        public TARGET cur_Target = default(TARGET);
        /// <summary>
        /// 当前打开的panel
        /// </summary>
        [HideInInspector]
        public ID cur_TargetType = default(ID);

        #endregion

        #region 面板管理

        protected Dictionary<object, TARGET> _targets = new Dictionary<object, TARGET>();

        public virtual Dictionary<object, TARGET> targets { get { return null; } }

        #endregion

        #region HelpFunc

        /// <summary>
        /// 获取字典内的一个对象（TARGET）
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        protected virtual TARGET getTarget(ID type)
        {
            TARGET target = null;

            if (targets.TryGetValue(type, out target)) return target;
            else
            {
                Debug.Log("获取目标对象失败，其中ID为："+ type.ToString() +"，类型为："+ typeof(TARGET).ToString());
                return null;
            }
        }

        /// <summary>
        /// 所有的字段必须初始化（引用型字段必须赋空值）
        /// </summary>
        public virtual void Init()
        {
            cur_TargetType = default(ID);
            cur_Target = default(TARGET);
        }
        #endregion
        public virtual void OnDestroy()
        {
            Init();
            EventCenter.Ins.Remove(EventSign.Init, Init);
        }
    }
}
