using UnityEngine;
using System.Collections;
namespace YouYou
{
    /// <summary>
    /// 协议接口
    /// </summary>
    public interface IProto
    {
        //协议编号
        ushort ProtoCode { get; }
        string ProtoEnName { get; }
        byte[] ToArray();
    }
}
