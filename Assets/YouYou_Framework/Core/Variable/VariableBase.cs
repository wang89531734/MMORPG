using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace YouYou
{
    /// <summary>
    /// 变量基类
    /// </summary>
    public abstract class VariableBase 
    {
        public abstract Type Type
        {
            get;
        }

        public byte ReferenceCount
        {
            get;
            private set;
        }

        /// <summary>
        /// 保留对象
        /// </summary>
        public void Retain()
        {
            ReferenceCount++;
        }

        /// <summary>
        /// 释放对象
        /// </summary>
        public void Release()
        {
            ReferenceCount--;
            if (ReferenceCount<1)
            {
                //回池操作
                GameEntry.Pool.EnqueueVarObject(this);
            }
        }
    }
}
