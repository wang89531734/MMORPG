using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace YouYou
{
    internal class UIManager : ManagerBase
    {
        /// <summary>
        /// 打开UI窗体
        /// </summary>
        /// <param name="uiFormId"></param>
        /// <param name="userData"></param>
        internal void OpenUIForm(int uiFormId,object userData=null)
        {
            //1.读表

            Sys_UIFormEntity entity = GameEntry.DataTable.DataTableManager.Sys_UIFormDBModel.Get(uiFormId);

            if (entity == null)
            {
                return;
            }
#if DISABLE_ASSETBUNDLE && UNITY_EDITOR

            string path = string.Format("Assets/Download/UI/UIPrefab/{0}.prefab", entity.AssetPath_Chinese);
 
            //加载镜像
            Object obj = UnityEditor.AssetDatabase.LoadAssetAtPath<GameObject>(path);

            if (obj==null)
            {
                Debug.Log("找不到物体");
            }
            GameObject uiObj = Object.Instantiate(obj) as GameObject;

            uiObj.transform.SetParent(GameEntry.UI.GetUIGroup(entity.UIGroupId).Group);
            uiObj.transform.localPosition = Vector3.zero;
            uiObj.transform.localScale = Vector3.one;
#else

#endif
        }
    }
}
