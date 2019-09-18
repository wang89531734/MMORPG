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
        /// <summary>
        /// 当前语言(要和本地化表的语言字段 一致)
        /// </summary>
        public YouYouLanguage CurrLanguage
        {
            get;
            private set;
        }

        protected override void OnAwake()
        {
            base.OnAwake();
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
                    CurrLanguage = YouYouLanguage.Chinese;
                    break;
                case SystemLanguage.English:
                    CurrLanguage = YouYouLanguage.English;
                    break;
            }
        }

        public override void Shutdown()
        {

        }

    }
}
