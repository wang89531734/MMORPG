using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace YouYou
{
    /// <summary>
    /// 数据组件
    /// </summary>
    public class DataComponent : YouYouBaseComponent
    {
        /// <summary>
        /// 临时缓存数据
        /// </summary>
        //public CacheData CacheData
        //{
        //    get;
        //    private set;
        //}

        /// <summary>
        /// 系统相关数据
        /// </summary>
        public SysDataManager SysData
        {
            get;
            private set;
        }

        /// <summary>
        /// 用户相关数据
        /// </summary>
        public UserDataManager UserDataManager
        {
            get;
            private set;
        }

        ///// <summary>
        ///// 关卡地图数据
        ///// </summary>
        //public PVEMapData PVEMapData
        //{
        //    get;
        //    private set;
        //}

        //protected override void OnAwake()
        //{
        //    base.OnAwake();
        //    CacheData = new CacheData();
        //    SysData = new SysData();
        //    UserData = new UserData();
        //    PVEMapData = new PVEMapData();
        //}

        public override void Shutdown()
        {
            //CacheData.Dispose();
            //SysData.Dispose();
            //UserData.Dispose();
            //PVEMapData.Dispose();
        }
    }
}
