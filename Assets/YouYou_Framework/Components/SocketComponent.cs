using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace YouYou
{
    /// <summary>
    /// Socket组件
    /// </summary>
    public class SocketComponent : YouYouBaseComponent,IUpdateComponent
    {
        [Header("每帧最大发送数量")]
        public int MaxSendCount=5;

        [Header("每次发包最大的字节")]
        public int MaxSendByteCount = 1024;

        [Header("每帧最大处理包数量")]
        public int MaxReceiveCount = 5;

        private SocketManager m_SocketManager;

        ///// <summary>
        ///// 通用的MemoryStream
        ///// </summary>
        //public MMO_MemoryStream CommonMemoryStream
        //{
        //    get;
        //    private set;
        //}

        /// <summary>
        /// 发送用的MS
        /// </summary>
        public MMO_MemoryStream SocketSendMS
        {
            get;
            private set;
        }

        /// <summary>
        /// 接收用的MS
        /// </summary>
        public MMO_MemoryStream SocketReceiveMS
        {
            get;
            private set;
        }

        protected override void OnAwake()
        {
            base.OnAwake();
            GameEntry.RegisterUpdateComponent(this);
            m_SocketManager = new SocketManager();
            SocketSendMS = new MMO_MemoryStream();
            SocketReceiveMS = new MMO_MemoryStream();
        }

        protected override void OnStart()
        {
            base.OnStart();
            m_MainSocket = CreateSocketTcpRoutine();
        }

        /// <summary>
        /// 注册SocketTcp访问器
        /// </summary>
        /// <param name="routine"></param>
        internal void RegisterSocketTcpRoutine(SocketTcpRoutine routine)
        {
            m_SocketManager.RegisterSocketTcpRoutine(routine);
        }

        /// <summary>
        /// 移除SocketTcp访问器
        /// </summary>
        /// <param name="routine"></param>
        internal void RemoveSocketTcpRoutine(SocketTcpRoutine routine)
        {
            m_SocketManager.RemoveSocketTcpRoutine(routine);
        }

        /// <summary>
        /// 创建SocketTcp访问器
        /// </summary>
        /// <returns></returns>
        public SocketTcpRoutine CreateSocketTcpRoutine()
        {
            return GameEntry.Pool.DequeueClassObject<SocketTcpRoutine>();
        }

        public override void Shutdown()
        {
            m_SocketManager.Dispose();

            GameEntry.Pool.EnqueueClassObject(m_MainSocket);

            SocketSendMS.Dispose();
            SocketReceiveMS.Dispose();

            SocketSendMS.Close();
            SocketReceiveMS.Close();
        }

        public void OnUpdate()
        {
            m_SocketManager.OnUpdate();
        }

        //===============================================
        /// <summary>
        /// 主Socket
        /// </summary>
        private SocketTcpRoutine m_MainSocket;

        /// <summary>
        /// 连接到主Socket
        /// </summary>
        /// <param name="ip"></param>
        /// <param name="port"></param>
        public void ConnectToMainSocket(string ip,int port)
        {
            m_MainSocket.Connect(ip, port);
        }

        /// <summary>
        /// 发送消息
        /// </summary>
        /// <param name="buffer"></param>
        public void SendMsg(byte[] buffer)
        {
            m_MainSocket.SendMsg(buffer);
        }       
    }
}
