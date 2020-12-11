//================================================
//描述 ： 当一些资源需要多次重复使用
//作者 ：
//创建时间 ：2019/04/04 17:20:17  
//版本： 
//================================================
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HMLFramwork;
namespace HMLFramwork
{

    public class MgrBase2<TARGET> : MonoBehaviour where TARGET:Component
    {
        /// <summary>
        /// 空闲的成员对象
        /// </summary>
        protected Stack<TARGET> leisureMembers = new Stack<TARGET>();
        /// <summary>
        /// 忙碌的成员对象
        /// </summary>
        protected Stack<TARGET> abustleMembers = new Stack<TARGET>();
        private void Start()
        {
            InitMembers();
        }

        /// <summary>
        /// 初始化成员
        /// </summary>
        private void InitMembers()
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                leisureMembers.Push(transform.GetChild(i).GetComponent<TARGET>());
            }
        }

        public TARGET getTarget()
        {
            TARGET target = default(TARGET);
            if (leisureMembers.Count < 1)
            {
                if (abustleMembers.Count > 0)
                {
                    target = abustleMembers.Pop();
                }
                else Debug.Log("提醒：可能没有回收不用的目标对象...");
            }
            else target = leisureMembers.Pop();
            //取出来后，一般是有用的，所以会存入忙碌的成员对象库中
            abustleMembers.Push(target);
            return target;
        }

    }
}