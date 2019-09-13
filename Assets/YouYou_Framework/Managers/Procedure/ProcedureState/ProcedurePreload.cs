using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace YouYou
{
    /// <summary>
    /// 预加载流程
    /// </summary>
    public class ProcedurePreload : ProcedureBase
    {
        public override void OnEnter()
        {
            base.OnEnter();
            GameEntry.Event.CommonEvent.AddEventListener(SysEventId.LoadDataTableComplete, OnLoadDataTableComplete);
            GameEntry.Event.CommonEvent.AddEventListener(SysEventId.LoadOneDataTableComplete, OnLoadOneDataTableComplete);
            GameEntry.DataTable.LoadDataTableAsync();
        }
    
        public override void OnUpdate()
        {
            base.OnUpdate();
        }

        public override void OnLeave()
        {
            base.OnLeave();
            GameEntry.Event.CommonEvent.RemoveEventListener(SysEventId.LoadDataTableComplete, OnLoadDataTableComplete);
        }

        /// <summary>
        /// 加载所有表完毕
        /// </summary>
        /// <param name="userData"></param>
        private void OnLoadDataTableComplete(object userData)
        {
            Debug.Log("加载所有表完毕");
        }

        /// <summary>
        /// 加载单一表格
        /// </summary>
        /// <param name="userData"></param>
        private void OnLoadOneDataTableComplete(object userData)
        {
            Debug.Log(userData);
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
        }
    }
}
