using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace YouYou
{
    public class HttpCallBackArgs : EventArgs
    {
        /// <summary>
        /// 是否有错
        /// </summary>
        public bool HasError;

        /// <summary>
        /// 返回值
        /// </summary>
        public string Value;
    }
}
