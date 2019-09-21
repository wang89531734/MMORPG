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

        [Header("心跳间隔")]
        public int HeartbeatInterval = 5;

        /// <summary>
        /// 上次心跳时间
        /// </summary>
        private float m_PrevHeartbeatTime = 0;

        /// <summary>
        /// PING值(毫秒)
        /// </summary>
        [HideInInspector]
        public int PingValue;

        /// <summary>
        /// 游戏服务器的时间
        /// </summary>
        [HideInInspector]
        public long GameServerTime;

        /// <summary>
        /// 和服务器对表的时刻
        /// </summary>
        [HideInInspector]
        public float CheckServerTime;

        private SocketManager m_SocketManager;

        /// <summary>
        /// 是否已经连接到主Socket
        /// </summary>
        private bool m_IsConnectToMainSocket;

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
            m_MainSocket.OnConnectOK = () =>
            {
                //已经建立了链接
                m_IsConnectToMainSocket = true;
            };

            SocketProtoListener.AddProtoListener();
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
            m_IsConnectToMainSocket = false;

            m_SocketManager.Dispose();
            GameEntry.Pool.EnqueueClassObject(m_MainSocket);
            SocketProtoListener.RemoveProtoListener();

            SocketSendMS.Dispose();
            SocketReceiveMS.Dispose();

            SocketSendMS.Close();
            SocketReceiveMS.Close();
        }

        public void OnUpdate()
        {
            m_SocketManager.OnUpdate();

            if (m_IsConnectToMainSocket)
            {
                if (Time.realtimeSinceStartup > m_PrevHeartbeatTime + HeartbeatInterval)
                {
                    //发送心跳
                    m_PrevHeartbeatTime = Time.realtimeSinceStartup;

                    System_HeartbeatProto proto = new System_HeartbeatProto();
                    proto.LocalTime = Time.realtimeSinceStartup * 1000;
                    CheckServerTime = Time.realtimeSinceStartup;
                    SendMsg(proto);
                }
            }
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
        public void SendMsg(IProto proto)
        {
            m_MainSocket.SendMsg(proto.ToArray());
        }       
    }
}
