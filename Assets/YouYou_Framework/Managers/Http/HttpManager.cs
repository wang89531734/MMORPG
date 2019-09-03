using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace YouYou
{
    public class HttpManager : ManagerBase
    {
        /// <summary>
        /// ·¢ËÍHttpÊý¾Ý
        /// </summary>
        /// <param name="url"></param>
        /// <param name="callBack"></param>
        /// <param name="isPost"></param>
        /// <param name="json"></param>
        public void SendData(string url, HttpSendDataCallBack callBack, bool isPost = false, Dictionary<string, object> dic = null)
        {
            HttpRoutine http = GameEntry.Pool.DequeueClassObject<HttpRoutine>();
            http.SendData(url, callBack, isPost, dic);
        }
    }
}
