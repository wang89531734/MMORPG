using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace YouYou
{
    /// <summary>
    /// YouYou组件基类
    /// </summary>
    public abstract class YouYouBaseComponent : YouYouComponent
    {
        protected override void OnAwake()
        {
            base.OnAwake();

            //把自己加入组件列表
            GameEntry.RegisterBaseComponent(this);
        }

        /// <summary>
        /// 关闭方法
        /// </summary>
        public abstract void Shutdown();
    }
}
