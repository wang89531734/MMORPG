using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace YouYou
{
    internal class UIManager : ManagerBase
    {
        /// <summary>
        /// 已经打开的UI链表
        /// </summary>
        private LinkedList<UIFormBase> m_OpenUIFormList;

        public UIManager()
        {
            m_OpenUIFormList = new LinkedList<UIFormBase>();
        }

        /// <summary>
        /// 打开UI窗体
        /// </summary>
        /// <param name="uiFormId"></param>
        /// <param name="userData"></param>
        internal void OpenUIForm(int uiFormId,object userData=null)
        {
            if (IsExists(uiFormId))
            {
                return;
            }
            //1.读表

            Sys_UIFormEntity entity = GameEntry.DataTable.DataTableManager.Sys_UIFormDBModel.Get(uiFormId);

            if (entity == null)
            {
                return;
            }
#if DISABLE_ASSETBUNDLE && UNITY_EDITOR

            UIFormBase formBase = GameEntry.UI.Dequeue(uiFormId);

            if (formBase == null)
            {
                string assetPath = string.Empty;
                switch (GameEntry.Localization.CurrLanguage)
                {
                    case YouYouLanguage.Chinese:
                        assetPath = entity.AssetPath_Chinese;
                        break;
                    case YouYouLanguage.English:
                        assetPath = entity.AssetPath_English;
                        break;
                }
                string path = string.Format("Assets/Download/UI/UIPrefab/{0}.prefab", assetPath);

                //加载镜像
                Object obj = UnityEditor.AssetDatabase.LoadAssetAtPath<GameObject>(path);

                GameObject uiObj = Object.Instantiate(obj) as GameObject;

                uiObj.transform.SetParent(GameEntry.UI.GetUIGroup(entity.UIGroupId).Group);
                uiObj.transform.localPosition = Vector3.zero;
                uiObj.transform.localScale = Vector3.one;

                formBase = uiObj.GetComponent<UIFormBase>();
                formBase.Init(uiFormId, entity.UIGroupId, entity.DisableUILayer == 1, entity.IsLock == 1, userData);
            }
            else
            {
                formBase.gameObject.SetActive(true);
                formBase.Open(userData);
            }
#else

#endif
            m_OpenUIFormList.AddLast(formBase);
        }

        /// <summary>
        /// 检查UI是否已经打开
        /// </summary>
        /// <param name="uiformId"></param>
        /// <returns></returns>
        public bool IsExists(int uiformId)
        {
            for (LinkedListNode<UIFormBase> curr = m_OpenUIFormList.First; curr != null; curr = curr.Next)
            {
                if(curr.Value.UIFormId== uiformId)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 根据UIFormId关闭UI
        /// </summary>
        /// <param name="uiformId"></param>
        internal void CloseUIForm(int uiformId)
        {
            for (LinkedListNode<UIFormBase> curr = m_OpenUIFormList.First; curr != null; curr = curr.Next)
            {
                if (curr.Value.UIFormId == uiformId)
                {
                    CloseUIForm(curr.Value);
                    break;
                }
            }
        }

        internal void CloseUIForm(UIFormBase formBase)
        {
            m_OpenUIFormList.Remove(formBase);
            formBase.ToClose();
        }
    }
}
