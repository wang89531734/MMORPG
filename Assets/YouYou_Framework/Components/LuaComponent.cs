using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace YouYou
{
    /// <summary>
    /// Lua×é¼þ
    /// </summary>
    public class LuaComponent : YouYouBaseComponent
    {
        private LuaManager m_LuaManager;

        protected override void OnAwake()
        {
            base.OnAwake();

            m_LuaManager = new LuaManager();
        }

        protected override void OnStart()
        {
            base.OnStart();
            m_LuaManager.Init();
        }

        public override void Shutdown()
        {
            
        }
    }
}
