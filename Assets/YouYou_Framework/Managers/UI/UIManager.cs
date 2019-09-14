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

            UIFormBase formBase = null;

            string path = string.Format("Assets/Download/UI/UIPrefab/{0}.prefab", entity.AssetPath_Chinese);
 
            //加载镜像
            Object obj = UnityEditor.AssetDatabase.LoadAssetAtPath<GameObject>(path);

            GameObject uiObj = Object.Instantiate(obj) as GameObject;

            uiObj.transform.SetParent(GameEntry.UI.GetUIGroup(entity.UIGroupId).Group);
            uiObj.transform.localPosition = Vector3.zero;
            uiObj.transform.localScale = Vector3.one;

            formBase = uiObj.GetComponent<UIFormBase>();
            formBase.Init(uiFormId, entity.UIGroupId, entity.DisableUILayer == 1, entity.IsLock == 1, userData);
            formBase.Open(userData);
#else

#endif
        }

        internal void CloseUIForm(UIFormBase formBase)
        {
            formBase.ToClose();
        }
    }
}
