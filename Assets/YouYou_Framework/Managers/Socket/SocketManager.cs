using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace YouYou
{
    public class SocketManager : ManagerBase,IDisposable
    {
        /// <summary>
        /// SocketTcp·ÃÎÊÆ÷Á´±í
        /// </summary>
        private LinkedList<SocketTcpRoutine> m_SocketTcpRoutineList;

        public SocketManager()
        {
            m_SocketTcpRoutineList = new LinkedList<SocketTcpRoutine>();
        }

        /// <summary>
        /// ×¢²áSocketTcp·ÃÎÊÆ÷
        /// </summary>
        /// <param name="routine"></param>
        internal void RegisterSocketTcpRoutine(SocketTcpRoutine routine)
        {
            m_SocketTcpRoutineList.AddFirst(routine);
        }

        /// <summary>
        /// ÒÆ³ýSocketTcp·ÃÎÊÆ÷
        /// </summary>
        /// <param name="routine"></param>
        internal void RemoveSocketTcpRoutine(SocketTcpRoutine routine)
        {
            m_SocketTcpRoutineList.Remove(routine);
        }

        internal void OnUpdate()
        {
            for(LinkedListNode<SocketTcpRoutine> curr = m_SocketTcpRoutineList.First; curr != null; curr = curr.Next)
            {
                curr.Value.OnUpdate();
            }
        }

        public void Dispose()
        {
            m_SocketTcpRoutineList.Clear();
        }
    }
}
