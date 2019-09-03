using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XLua;

namespace YouYou
{
    /// <summary>
    /// Í¨ÓÃÊÂ¼þ
    /// </summary>
    public class CommonEvent:IDisposable
    {
        [CSharpCallLua]
        public delegate void OnActionHandler(object userData);
        public Dictionary<ushort, List<OnActionHandler>> dic = new Dictionary<ushort, List<OnActionHandler>>();

        #region AddEventListener Ìí¼Ó¼àÌý
        /// <summary>
        /// Ìí¼Ó¼àÌý
        /// </summary>
        /// <param name="key"></param>
        /// <param name="handler"></param>
        public void AddEventListener(ushort key, OnActionHandler handler)
        {
            List<OnActionHandler> lstHandler = null;
            dic.TryGetValue(key, out lstHandler);
            if (lstHandler == null)
            {
                lstHandler = new List<OnActionHandler>();
                dic[key] = lstHandler;
            }

            lstHandler.Add(handler);
        }
        #endregion

        #region RemoveEventListener ÒÆ³ý¼àÌý
        /// <summary>
        /// ÒÆ³ý¼àÌý
        /// </summary>
        /// <param name="key"></param>
        /// <param name="handler"></param>
        public void RemoveEventListener(ushort key, OnActionHandler handler)
        {
            List<OnActionHandler> lstHandler = null;
            dic.TryGetValue(key, out lstHandler);

            if (lstHandler != null)
            {
                lstHandler.Remove(handler);
                if (lstHandler.Count == 0)
                {
                    dic.Remove(key);
                }
            }
        }
        #endregion

        #region Dispatch ÅÉ·¢
        /// <summary>
        /// ÅÉ·¢
        /// </summary>
        /// <param name="key"></param>
        /// <param name="p"></param>
        public void Dispatch(ushort key, object userData)
        {
            List<OnActionHandler> lstHandler = null;
            dic.TryGetValue(key, out lstHandler);

            if (lstHandler != null)
            {
                int lstCount = lstHandler.Count;

                for (int i = 0; i < lstCount; i++)
                {
                    OnActionHandler handler = lstHandler[i];
                    if (handler != null)
                    {
                        handler(userData);
                    }
                }
            }
        }

        public void Dispatch(ushort key)
        {
            Dispatch(key, null);
        }
        #endregion

        public void Dispose()
        {
            dic.Clear();       
        }
    }
}
