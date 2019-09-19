using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace YouYou
{
    public class LocalizationManager : ManagerBase
    {
        /// <summary>
        /// 获取本地化文本内容
        /// </summary>
        /// <param name="key"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public string GetString(string key,params object[] args)
        {
            string value = null;
            if(GameEntry.DataTable.DataTableManager.LocalizationBModel.LocalizationDic.TryGetValue(key,out value))
            {
                return string.Format(value,args);
            }
            return value;
        }

    }
}
