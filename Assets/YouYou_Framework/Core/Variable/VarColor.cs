using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace YouYou
{
    public class VarColor : Variable<Color>
    {
        /// <summary>
        /// 分配一个对象
        /// </summary>
        /// <returns></returns>
        public static VarColor Alloc()
        {
            VarColor var = GameEntry.Pool.DequeueVarObject<VarColor>();
            var.Value = Color.clear;
            var.Retain();
            return var;
        }

        /// <summary>
        /// 分配一个对象
        /// </summary>
        /// <param name="value">初始值</param>
        /// <returns></returns>
        public static VarColor Alloc(Color value)
        {
            VarColor var = Alloc();
            var.Value = value;
            return var;
        }

        /// <summary>
        /// VarInt->int
        /// </summary>
        /// <param name="value"></param>
        public static implicit operator Color(VarColor value)
        {
            return value.Value;
        }
    }
}
