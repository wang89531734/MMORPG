using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace YouYou
{
    public class VarTransform : Variable<Transform>
    {
        /// <summary>
        /// 分配一个对象
        /// </summary>
        /// <returns></returns>
        public static VarTransform Alloc()
        {
            VarTransform var = GameEntry.Pool.DequeueVarObject<VarTransform>();
            var.Value.position =Vector3.zero;
            //var.Value.rotation = Vector3.zero;
            var.Value.localScale = Vector3.one;
            var.Retain();
            return var;
        }

        /// <summary>
        /// 分配一个对象
        /// </summary>
        /// <param name="value">初始值</param>
        /// <returns></returns>
        public static VarTransform Alloc(Transform value)
        {
            VarTransform var = Alloc();
            var.Value = value;
            return var;
        }

        /// <summary>
        /// VarInt->int
        /// </summary>
        /// <param name="value"></param>
        public static implicit operator Transform(VarTransform value)
        {
            return value.Value;
        }
    }
}
