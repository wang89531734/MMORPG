using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using XLua;

namespace YouYou
{
    /// <summary>
    /// Lua管理器
    /// </summary>
    public class LuaManager : ManagerBase
    {
        /// <summary>
        /// 全局的xLua引擎
        /// </summary>
        public static LuaEnv luaEnv;

        public void Init()
        {
            //1.实例化 xLua引擎
            luaEnv = new LuaEnv();

#if DISABLE_ASSETBUNDLE && UNITY_EDITOR
            //2.设置xLua的脚本路径
            luaEnv.DoString(string.Format("package.path = '{0}/?.bytes'", Application.dataPath+ "Download/xLuaLogic/"));
#else
            luaEnv.AddLoader(MyLoader);
            //luaEnv.DoString(string.Format("package.path = '{0}/?.lua'", Application.persistentDataPath));
#endif
            DoString("require'Main'");
        }

        /// <summary>
        /// 自定义的Loader
        /// </summary>
        /// <param name="filepath"></param>
        /// <returns></returns>
        private byte[] MyLoader(ref string filepath)
        {
            //string path = Application.persistentDataPath + "/" + filepath + ".lua";
            byte[] buffer = null;
            //using (FileStream fs = new FileStream(path, FileMode.Open))
            //{
            //    buffer = new byte[fs.Length];
            //    fs.Read(buffer, 0, buffer.Length);
            //}
            return buffer;
        }

        /// <summary>
        /// 执行lua脚本
        /// </summary>
        /// <param name="str"></param>
        public void DoString(string str)
        {
            luaEnv.DoString(str);
        }
    }
}
