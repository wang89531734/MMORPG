using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace YouYou
{
    /// <summary>
    /// UI组件
    /// </summary>
    public class UIComponent : YouYouBaseComponent, IUpdateComponent
    {
        [Header("标准分辨率宽度")]
        [SerializeField]
        private int m_StandardWidth = 1280;

        [Header("标准分辨率高度")]
        [SerializeField]
        private int m_StandardHeight = 720;

        [Header("UI摄像机")]
        public Camera UICamera;

        [Header("根画布")]
        [SerializeField]
        private Canvas m_UIRootCanvas;

        [Header("根画布的缩放")]
        [SerializeField]
        private CanvasScaler m_UIRootCanvasScaler;

        [Header("UI分组")]
        [SerializeField]
        private UIGroup[] UIGroups;

        private Dictionary<byte, UIGroup> m_UIGroupDic;

        /// <summary>
        /// 标准分辨率比值
        /// </summary>
        private float m_StandardScreen = 0;

        /// <summary>
        /// 当前分辨率比值
        /// </summary>
        private float m_CurrScreen = 0;

        private UIManager m_UIManager;

        protected override void OnAwake()
        {
            base.OnAwake();
            m_UIGroupDic = new Dictionary<byte, UIGroup>();

            GameEntry.RegisterUpdateComponent(this);

            m_StandardScreen = m_StandardWidth / (float)m_StandardHeight;
            m_CurrScreen = Screen.width / (float)Screen.height;

            int len = UIGroups.Length;
            for(int i = 0; i < len; i++)
            {
                UIGroup group = UIGroups[i];
                m_UIGroupDic[group.Id] = group;
            }

            m_UIManager = new UIManager();
        }

        #region UI适配
        /// <summary>
        /// LoadingForm加载窗口适配缩放
        /// </summary>
        public void LoadingFormCanvasScaler()
        {
            if (m_CurrScreen >= m_StandardScreen)
            {
                m_UIRootCanvasScaler.matchWidthOrHeight = 0;
            }
            else
            {
                m_UIRootCanvasScaler.matchWidthOrHeight = m_StandardScreen - m_CurrScreen;
            }
        }

        /// <summary>
        /// FullForm全屏窗口适配缩放
        /// </summary>
        public void FullFormCanvasScaler()
        {
            m_UIRootCanvasScaler.matchWidthOrHeight = 1;
        }

        /// <summary>
        /// NormalForm普通窗口适配缩放
        /// </summary>
        public void NormalFormCanvasScaler()
        {
            m_UIRootCanvasScaler.matchWidthOrHeight = (m_CurrScreen >= m_StandardScreen) ? 1:0; 
        }
        #endregion

        #region 根据UI分组编号获取分组
        /// <summary>
        /// 根据UI分组编号获取分组
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public UIGroup GetUIGroup(byte id)
        {
            UIGroup group = null;
            m_UIGroupDic.TryGetValue(id, out group);
            return group;
        }
        #endregion

        /// <summary>
        /// 打开UI窗体
        /// </summary>
        /// <param name="uiFormId"></param>
        /// <param name="userData"></param>
        internal void OpenUIForm(int uiFormId, object userData = null)
        {
            m_UIManager.OpenUIForm(uiFormId,userData);
        }

        internal void CloseUIForm(UIFormBase formBase)
        {
            m_UIManager.CloseUIForm(formBase);
        }

        public void OnUpdate()
        {

        }

        public override void Shutdown()
        {

        }
    }
}
