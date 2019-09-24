using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using XLua;

namespace YouYou
{
    /// <summary>
    /// Lua组件类型
    /// </summary>
    public enum LuaComType
    {
        GameObject=0,
        Transform=1,
        Button=2,
        Image=3,
        YouYouImage=4,
        Text=5,
        YouYouText=6,
        RawImage=7,
        InputField=8,
        Scrollbar=9,
        ScrollView=10,
        MulityScroller=11
    }

    /// <summary>
    /// Lua窗口
    /// </summary>
    [LuaCallCSharp]
    public class LuaForm :UIFormBase
    {
        [CSharpCallLua]
        public delegate void OnInitHandler(object userData);
        OnInitHandler onInit;

        [CSharpCallLua]
        public delegate void OnOpenHandler(object userData);
        OnOpenHandler onOpen;

        [CSharpCallLua]
        public delegate void OnCloseHandler();
        OnCloseHandler onClose;

        [CSharpCallLua]
        public delegate void OnBeforDestroyHandler();
        OnBeforDestroyHandler onBeforDestroy;

        private LuaTable scriptEnv;
        private LuaEnv luaEnv;

        [Header("Lua组件")]
        [SerializeField]
        private LuaCom[] m_LuaComs;

        /// <summary>
        /// 根据索引来查找组件
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public object GetLuaComs(int index)
        {
            LuaCom com = m_LuaComs[index];

            switch (com.Type)
            {
                case LuaComType.GameObject:
                    return com.Trans.gameObject;
                case LuaComType.Transform:
                    return com.Trans.gameObject;
                case LuaComType.Button:
                    return com.Trans.GetComponent<Button>();
                case LuaComType.Image:
                    return com.Trans.GetComponent<Image>();
                case LuaComType.YouYouImage:
                    return com.Trans.GetComponent<YouYouImage>();
                case LuaComType.Text:
                    return com.Trans.GetComponent<Text>();
                case LuaComType.YouYouText:
                    return com.Trans.GetComponent<YouYouText>();
                case LuaComType.RawImage:
                    return com.Trans.GetComponent<RawImage>();
                case LuaComType.InputField:
                    return com.Trans.GetComponent<InputField>();
                case LuaComType.Scrollbar:
                    return com.Trans.GetComponent<Scrollbar>();
                case LuaComType.ScrollView:
                    return com.Trans.GetComponent<ScrollRect>();
                case LuaComType.MulityScroller:
                    return com.Trans.GetComponent<UIMultiScroller>();
            }
            return com.Trans;
        }

        protected override void OnInit(object userData)
        {
            base.OnInit(userData);

            luaEnv = LuaManager.luaEnv; //此处要从LuaManager上获取 全局只有一个

            scriptEnv = luaEnv.NewTable();

            LuaTable meta = luaEnv.NewTable();
            meta.Set("__index", luaEnv.Global);
            scriptEnv.SetMetaTable(meta);
            meta.Dispose();

            string prefabName = name;
            if (prefabName.Contains("(Clone)"))
            {
                prefabName = prefabName.Split(new string[] { "(Clone)" }, StringSplitOptions.RemoveEmptyEntries)[0]+"View";
            }

            onInit = scriptEnv.GetInPath<OnInitHandler>(prefabName + ".OnInit");
            onOpen = scriptEnv.GetInPath<OnOpenHandler>(prefabName + ".OnOpen");
            onClose = scriptEnv.GetInPath<OnCloseHandler>(prefabName + ".OnClose");
            onBeforDestroy = scriptEnv.GetInPath<OnBeforDestroyHandler>(prefabName + ".OnBeforDestroy");

            scriptEnv.Set("self", this);
            if (onInit != null)
            {
                onInit(userData);
            }
        }

        protected override void OnOpen(object userData)
        {
            base.OnOpen(userData);
            if (onOpen != null)
            {
                onOpen(userData);
            }
        }

        protected override void OnClose()
        {
            base.OnClose();
            if (onClose != null)
            {
                onClose();
            }
        }

        protected override void OnBeforDestroy()
        {
            base.OnBeforDestroy();
            //备注 调用销毁的话，经常会造成Unity崩溃
            if (onBeforDestroy != null)
            {
                onBeforDestroy();
            }
            onInit = null;
            onOpen = null;
            onClose = null;
            onBeforDestroy = null;
        }
    }

    [Serializable]
    /// <summary>
    /// Lua组件
    /// </summary>
    public class LuaCom
    {
        /// <summary>
        /// 名称
        /// </summary>
        public string Name;

        /// <summary>
        /// 类型
        /// </summary>
        public LuaComType Type;

        /// <summary>
        /// 变换
        /// </summary>
        public Transform Trans;
    }
}

