using HMLFramwork.Singleton;
using System;
using System.Collections.Generic;

namespace HMLFramwork
{
    /// <summary>
    /// 事件参数管理类
    /// </summary>
    public class EventPara
    {
        private object[] _paras;
        public object[] Paras { get { return _paras; } }
        /// <summary>
        /// 给事件参数添加索引
        /// </summary>
        /// <param name="index">需要访问的参数的下标</param>
        /// <returns></returns>
        public object this[int index] { get { return _paras[index]; } }

        public EventPara(params object[] paras)
        {
            this._paras = paras;
        }
    }

    /// <summary>
    /// 事件体
    /// </summary>
    public class EventBody
    {
        /// <summary>
        /// 事件ID
        /// </summary>
        public object ID = null;
        /// <summary>
        /// 事件体
        /// </summary>
        public Delegate body;

        public EventBody(Delegate _body)
        {
            body = _body;
        }
        public EventBody(object id, Delegate _body)
        {
            ID = id;
            body = _body;
        }

    }

    /************需要用到的委托************/
    /// <summary>
    /// 带一个参数，无返回值的委托（参数类型为：EventPara）
    /// </summary>
    /// <param name="ep"></param>
    public delegate void EventDele_1(EventPara ep);

    /// <summary>
    /// 封装事件组（一个ID，对应一个事件组）
    /// </summary>
    public class EventGroup
    {
        /// <summary>
        /// 事件组ID
        /// </summary>
        private object _groupID;

        /// <summary>
        /// 事件体链表
        /// </summary>
        private List<EventBody> _eventBodyList;
        private List<EventBody> _excuteList;
        /// <summary>
        /// 绑定事件组的ID
        /// </summary>
        /// <param name="eventID">事件ID</param>
        public EventGroup(object groupID)
        {
            _groupID = groupID;//事件ID
            _eventBodyList = new List<EventBody>();//一个事件ID对应的事件组
            _excuteList = new List<EventBody>();
        }

        /// <summary>
        /// 添加指定事件
        /// </summary>
        /// <param name="eventDle"></param>
        public void Add(Delegate body)
        {
            lock (_eventBodyList)
            {
                _eventBodyList.Add(new EventBody(body));
            }
        }

        /// <summary>
        /// 添加指定事件
        /// </summary>
        /// <param name="bodyID">事件体ID</param>
        /// <param name="body">事件体</param>
        public void Add(object _bodyID, Delegate _body)
        {
            lock (_eventBodyList)
            {
                _eventBodyList.Add(new EventBody(_bodyID, _body));
            }
        }

        public void Remove(object _bodyID)
        {
            lock (_eventBodyList)
            {
                for (int i = 0; i < _eventBodyList.Count; i++)
                {
                    if (_eventBodyList[i].ID.Equals(_bodyID))
                    {
                        _eventBodyList.RemoveAt(i);
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// 移除指定事件
        /// </summary>
        /// <param name="eventDle">待移除事件</param>
        public void Remove(Delegate _body)
        {
            lock (_eventBodyList)
            {
                for (int i = 0; i < _eventBodyList.Count; i++)
                {
                    if (_eventBodyList[i].body.Equals(_body))
                    {
                        _eventBodyList.RemoveAt(i);
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// 执行事件组内指定事件
        /// </summary>
        /// <param name="_bodyID">指定事件的ID</param>
        /// <param name="ep">所需参数</param>
        public void Excute(object _bodyID, EventPara ep)
        {
            lock (_excuteList)
            {
                _excuteList.Clear();
                _excuteList.AddRange(_eventBodyList);

                EventBody eventBody = null;
                for (int i = 0; i < _excuteList.Count; i++)
                {
                    eventBody = _excuteList[i];
                    if (_bodyID.Equals(eventBody.ID))
                    {
                        if (eventBody.body.GetType().Equals(typeof(Action)))
                            ((Action)eventBody.body)();
                        else if (eventBody.body.GetType().Equals(typeof(EventDele_1)) && ep != null)
                            ((EventDele_1)eventBody.body)(ep);
                    }
                }
            }
        }

        /// <summary>
        /// 执行事件组内指定事件
        /// </summary>
        /// <param name="_body">指定事件</param>
        /// <param name="ep">所需参数</param>
        public void Excute(Delegate _body, EventPara ep)
        {
            lock (_excuteList)
            {
                _excuteList.Clear();
                _excuteList.AddRange(_eventBodyList);

                EventBody eventBody = null;
                for (int i = 0; i < _excuteList.Count; i++)
                {
                    eventBody = _excuteList[i];
                    if (_body.Equals(eventBody.body))
                    {
                        if (eventBody.body.GetType().Equals(typeof(Action)))
                            ((Action)eventBody.body)();
                        else if (eventBody.body.GetType().Equals(typeof(EventDele_1)))
                            ((EventDele_1)eventBody.body)(ep);
                    }
                }
            }
        }
        /// <summary>
        /// 执行事件组所有事件
        /// </summary>
        /// <param name="ep"></param>
        public void Excute(EventPara ep)
        {
            lock (_excuteList)
            {
                _excuteList.Clear();
                _excuteList.AddRange(_eventBodyList);

                for (int i = 0; i < _excuteList.Count; i++)
                {
                    if (_excuteList[i].body.GetType().Equals(typeof(Action)))
                        ((Action)_excuteList[i].body)();
                    else if (_excuteList[i].body.GetType().Equals(typeof(EventDele_1)))
                        ((EventDele_1)_excuteList[i].body)(ep);

                }
            }
        }
    }
    /// <summary>
    /// 事件管理中心
    /// 1.添加事件；2.执行事件；3.移除事件。
    /// </summary>
    public class EventCenter : SingleInstance<EventCenter>
    {

        private Dictionary<object, EventGroup> eventDic = new Dictionary<object, EventGroup>();
        #region 添加事件
        /// <summary>
        /// 添加无参，无返回值的事件到事件代理中心
        /// </summary>
        /// <param name="eventID">事件ID</param>
        /// <param name="func">无参，无返回值的事件</param>
        public void Add(object groupID, Action func)
        {
            lock (eventDic)
            {
                if (!eventDic.ContainsKey(groupID))
                    eventDic[groupID] = new EventGroup(groupID);
                eventDic[groupID].Add(func);
            }
        }
        /// <summary>
        /// 添加事件
        /// </summary>
        /// <param name="groupID">事件组ID</param>
        /// <param name="bodyID">事件体ID</param>
        /// <param name="func">事件</param>
        public void Add(object groupID, object bodyID, Action func)
        {
            lock (eventDic)
            {
                if (!eventDic.ContainsKey(groupID))
                    eventDic[groupID] = new EventGroup(groupID);
                eventDic[groupID].Add(bodyID, func);
            }
        }

        /// <summary>
        /// 添加带一个参数（EventPara），无返回值的事件到事件代理中心
        /// </summary>
        /// <param name="eventID">事件ID</param>
        /// <param name="func1">带一个参数（EventPara），无返回值的事件</param>
        public void Add(object groupID, EventDele_1 func1)
        {
            lock (eventDic)
            {
                if (!eventDic.ContainsKey(groupID))
                    eventDic[groupID] = new EventGroup(groupID);
                eventDic[groupID].Add(func1);
            }
        }
        /// <summary>
        /// 添加事件
        /// </summary>
        /// <param name="groupID">事件组ID</param>
        /// <param name="bodyID">事件体ID</param>
        /// <param name="func">事件</param>
        public void Add(object groupID, object bodyID, EventDele_1 func1)
        {
            lock (eventDic)
            {
                if (!eventDic.ContainsKey(groupID))
                    eventDic[groupID] = new EventGroup(groupID);
                eventDic[groupID].Add(bodyID, func1);
            }
        }

        #endregion

        #region 移除事件

        /// <summary>
        /// 移除某个事件组
        /// </summary>
        /// <param name="eventID">事件体ID</param>
        public void Remove(object groupID)
        {
            lock (eventDic)
            {
                if (eventDic.ContainsKey(groupID)) eventDic.Remove(groupID);
            }
        }

        /// <summary>
        /// 移除某个事件体
        /// </summary>
        /// <param name="eventID">事件组ID</param>
        /// <param name="bodyID">事件体ID</param>
        public void Remove(object groupID, object bodyID)
        {
            lock (eventDic)
            {
                if (eventDic.ContainsKey(groupID)) eventDic[groupID].Remove(bodyID);
            }
        }

        /// <summary>
        /// 移除某个事件体里面具体事件（非事件组）
        /// </summary>
        /// <param name="eventID">事件体ID</param>
        /// <param name="func">具体事件</param>
        public void Remove(object groupID, Action func)
        {
            lock (eventDic)
            {
                if (eventDic.ContainsKey(groupID)) eventDic[groupID].Remove(func);
            }
        }
        /// <summary>
        /// 移除某个事件体里面具体事件（非事件组）
        /// </summary>
        /// <param name="eventID">事件体ID</param>
        /// <param name="func1">具体事件</param>
        public void Remove(object groupID, EventDele_1 func1)
        {
            lock (eventDic)
            {
                if (eventDic.ContainsKey(groupID)) eventDic[groupID].Remove(func1);
            }
        }
        #endregion

        #region 执行事件
        /// <summary>
        /// 执行事件
        /// </summary>
        /// <param name="eventID">执行事件的ID</param>
        /// <param name="ep">执行指定事件所需要的参数</param>
        public void Excute(object groupID, EventPara ep)
        {
            EventGroup eg;
            lock (eventDic)
            {
                if (eventDic.TryGetValue(groupID, out eg))
                {
                    eg.Excute(ep);
                }
            }
        }
        /// <summary>
        /// 执行事件
        /// </summary>
        /// <param name="eventID">执行事件的ID</param>
        public void Excute(object groupID)
        {
            Excute(groupID, new EventPara());
        }

        /// <summary>
        /// 执行事件
        /// </summary>
        /// <param name="groupID"></param>
        /// <param name="bodyID"></param>
        /// <param name="ep"></param>
        public void Excute(object groupID, object bodyID, EventPara ep = null)
        {
            EventGroup eg;
            lock (eventDic)
            {
                if (eventDic.TryGetValue(groupID, out eg))
                {
                    eg.Excute(bodyID, ep);
                }
            }
        }

        #endregion
    }
}
