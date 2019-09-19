using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace YouYou
{
    public enum YouYouLanguage
    {
        /// <summary>
        /// 中文
        /// </summary>
        Chinese=0,
        /// <summary>
        /// 英文
        /// </summary>
        English=1
    }

    /// <summary>
    /// 本地化组件
    /// </summary>
    public class LocalizationComponent : YouYouBaseComponent
    {
        [SerializeField]
        private YouYouLanguage m_CurrLanguage;

        /// <summary>
        /// 当前语言(要和本地化表的语言字段 一致)
        /// </summary>
        public YouYouLanguage CurrLanguage
        {
            get
            {
                return m_CurrLanguage;
            }
        }

        private LocalizationManager m_LocalizationManager;

        protected override void OnAwake()
        {
            base.OnAwake();
            m_LocalizationManager = new LocalizationManager();
#if !UNITY_EDITOR
            Init();
#endif
        }

        /// <summary>
        /// 初始化
        /// </summary>
        private void Init()
        {
            switch (Application.systemLanguage)
            {
                case SystemLanguage.ChineseSimplified:
                case SystemLanguage.ChineseTraditional:
                case SystemLanguage.Chinese:
                    m_CurrLanguage = YouYouLanguage.Chinese;
                    break;
                case SystemLanguage.English:
                    m_CurrLanguage = YouYouLanguage.English;
                    break;
            }
        }

        /// <summary>
        /// 获取本地化文本内容
        /// </summary>
        /// <param name="key"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public string GetString(string key, params object[] args)
        {
            return m_LocalizationManager.GetString(key, args);
        }

        public override void Shutdown()
        {

        }
    }
}
