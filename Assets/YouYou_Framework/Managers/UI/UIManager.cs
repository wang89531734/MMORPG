using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace YouYou
{
    public class UIManager : ManagerBase
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

            string path = string.Format("Assets/Download/UI/UIPrefab", entity.AssetPath_Chinese);
            Object obj = UnityEditor.AssetDatabase.LoadAssetAtPath<GameObject>(path);

#else

#endif
        }
    }
}
