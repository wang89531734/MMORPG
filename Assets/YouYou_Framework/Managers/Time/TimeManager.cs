using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace YouYou
{
    public class TimeManager : ManagerBase, IDisposable
    {
        /// <summary>
        /// 定时器链表
        /// </summary>
        private LinkedList<TimeAction> m_TimeActionList;

        public TimeManager()
        {
            m_TimeActionList = new LinkedList<TimeAction>();
        }

        /// <summary>
        /// 注册定时器
        /// </summary>
        /// <param name="action"></param>
        internal void RegisterTimeAction(TimeAction action)
        {
            m_TimeActionList.AddLast(action);
        }

        /// <summary>
        /// 移除定时器
        /// </summary>
        /// <param name="action"></param>
        internal void RemoveTimeAction(TimeAction action)
        {
            m_TimeActionList.Remove(action);
        }

        public void OnUpdate()
        {
            for(LinkedListNode<TimeAction> curr = m_TimeActionList.First; curr != null; curr = curr.Next)
            {
                curr.Value.OnUpdate();
            }
        }

        public void Dispose()
        {
            m_TimeActionList.Clear();
        }
    }
}
