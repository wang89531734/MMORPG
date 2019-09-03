using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace YouYou
{

    public class PoolManager : ManagerBase,IDisposable
    {
        public ClassObjectPool ClassObjectPool
        {
            get;
            private set;
        }

        public GameObjectPool GameObjectPool
        {
            get;
            private set;
        }

        public PoolManager()
        {
            ClassObjectPool = new ClassObjectPool();
            GameObjectPool = new GameObjectPool();
        }

        /// <summary>
        /// 释放类对象池
        /// </summary>
        public void ClearClassObjectPool()
        {
            ClassObjectPool.Clear();
        }

        public void Dispose()
        {
            ClassObjectPool.Dispose();
            GameObjectPool.Dispose();
        }
    }
}
