using UnityEngine;
using System.Collections;

namespace HMLFramwork
{
    public class FuncBase : MonoBehaviour
    {

        [HideInInspector]
        /// <summary>
        /// 功能进行状态
        /// </summary>
        public ActionState MyFuncState = ActionState.DEFAULT;

        /// <summary>
        /// 执行功能
        /// </summary>
        public virtual void Excute()
        {
            MyFuncState = ActionState.DOING;
        }

        public virtual void Stop()
        {
            MyFuncState = ActionState.DONE;
        }

        public virtual void Init()
        {
            MyFuncState = ActionState.DEFAULT;
        }

    }
}
