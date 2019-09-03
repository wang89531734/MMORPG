using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace YouYou
{
    /// <summary>
    /// 游戏主入口
    /// </summary>
    public class GameEntry : MonoBehaviour
    {
        #region 组件属性
        /// <summary>
        /// 事件组件
        /// </summary>
        public static EventComponent Event
        {
            get;
            private set;
        }

        /// <summary>
        /// 时间组件
        /// </summary>
        public static TimeComponent Time
        {
            get;
            private set;
        }

        /// <summary>
        /// 状态机组件
        /// </summary>
        public static FsmComponent Fsm
        {
            get;
            private set;
        }

        /// <summary>
        /// 流程组件
        /// </summary>
        public static ProcedureComponent Procedure
        {
            get;
            private set;
        }

        /// <summary>
        /// 数据表组件
        /// </summary>
        public static DataTableComponent DataTable
        {
            get;
            private set;
        }

        /// <summary>
        /// Socket组件
        /// </summary>
        public static SocketComponent Socket
        {
            get;
            private set;
        }

        /// <summary>
        /// Http组件
        /// </summary>
        public static HttpComponent Http
        {
            get;
            private set;
        }

        /// <summary>
        /// 数据组件
        /// </summary>
        public static DataComponent Data
        {
            get;
            private set;
        }

        public static LocalizationComponent Localization
        {
            get;
            private set;
        }

        public static PoolComponent Pool
        {
            get;
            private set;
        }

        public static SceneComponent Scene
        {
            get;
            private set;
        }

        public static SettingComponent Setting
        {
            get;
            private set;
        }

        public static GameObjComponent GameObj
        {
            get;
            private set;
        }

        public static ResourceComponent Resource
        {
            get;
            private set;
        }

        public static DownloadComponent Download
        {
            get;
            private set;
        }

        /// <summary>
        /// UI组件
        /// </summary>
        public static UIComponent UI
        {
            get;
            private set;
        }
        #endregion

        #region 基础组件管理
        /// <summary>
        /// 基础组件的列表
        /// </summary>
        private static readonly LinkedList<YouYouBaseComponent> m_BaseComponentList = new LinkedList<YouYouBaseComponent>();

        #region 注册组件
        /// <summary>
        /// 注册组件
        /// </summary>
        /// <param name="component"></param>
        internal static void RegisterBaseComponent(YouYouBaseComponent component)
        {
            //获取到组件类型
            Type type = component.GetType();

            LinkedListNode<YouYouBaseComponent> curr = m_BaseComponentList.First;
            while (curr!=null)
            {
                if (curr.Value.GetType() == type) return;
                curr = curr.Next;
            }

            //把组件加入最后一个节点
            m_BaseComponentList.AddLast(component);
        }
        #endregion

        #region 获取基础组件
        internal static T GetBaseComponent<T>() where T : YouYouBaseComponent
        {
            return (T)GetBaseComponent(typeof(T));
        }

        /// <summary>
        /// 获取基础组件
        /// </summary>
        /// <param name="component"></param>
        internal static YouYouBaseComponent GetBaseComponent(Type type)
        {     
            LinkedListNode<YouYouBaseComponent> curr = m_BaseComponentList.First;
            while (curr != null)
            {
                if (curr.Value.GetType() == type)
                {
                    return curr.Value;
                }
                curr = curr.Next;
            }
            return null;
        }
        #endregion
        #endregion

        #region 更新组件管理
        /// <summary>
        /// 更新组件的列表
        /// </summary>
        private static readonly LinkedList<IUpdateComponent> m_UpdateComponentList = new LinkedList<IUpdateComponent>();

        #region 注册更新组件
        /// <summary>
        /// 注册更新组件
        /// </summary>
        /// <param name="component"></param>
        public static void RegisterUpdateComponent(IUpdateComponent component)
        {
            //把组件加入最后一个节点
            m_UpdateComponentList.AddLast(component);
        }
        #endregion

        #region 移除更新组件
        /// <summary>
        /// 移除更新组件
        /// </summary>
        /// <param name="component"></param>
        public static void RemoveUpdateComponent(IUpdateComponent component)
        {
           m_UpdateComponentList.Remove(component);      
        }
        #endregion
        #endregion

        void Start()
        {
            InitBaseComponents();
        }

        private static void InitBaseComponents()
        {
            Event = GetBaseComponent<EventComponent>();
            Time = GetBaseComponent<TimeComponent>();
            Fsm = GetBaseComponent<FsmComponent>();
            Procedure = GetBaseComponent<ProcedureComponent>();
            DataTable = GetBaseComponent<DataTableComponent>();
            Socket = GetBaseComponent<SocketComponent>();
            Http = GetBaseComponent<HttpComponent>();
            Data = GetBaseComponent<DataComponent>();
            Localization = GetBaseComponent<LocalizationComponent>();
            Pool = GetBaseComponent<PoolComponent>();
            Scene = GetBaseComponent<SceneComponent>();
            Setting = GetBaseComponent<SettingComponent>();
            GameObj = GetBaseComponent<GameObjComponent>();
            Resource = GetBaseComponent<ResourceComponent>();
            Download = GetBaseComponent<DownloadComponent>();
            UI = GetBaseComponent<UIComponent>();
        }

        void Update()
        {
            //循环更新组件
            for(LinkedListNode<IUpdateComponent> curr = m_UpdateComponentList.First; curr != null; curr = curr.Next)
            {
                curr.Value.OnUpdate();
            }
        }

        private void OnDestroy()
        {
            //关闭所有基础组件
            for (LinkedListNode<YouYouBaseComponent> curr = m_BaseComponentList.First; curr != null; curr = curr.Next)
            {
                curr.Value.Shutdown();
            }
        }
    }
}

