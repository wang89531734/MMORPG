using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using YouYou;

public class UITaskFormItemView : MonoBehaviour
{
    private Button btnDetail;

    private Text txtName;

    private Action<int> m_OnClick;

    private int m_TaskId;

    private void Awake()
    {
        btnDetail.onClick.AddListener(() => 
        {
            if (m_OnClick != null)
            {
                m_OnClick(m_TaskId);
            }
        });
    }

    //public void SetUI(TaskEntity entity, Action<int> onClick)
    //{
    //    m_TaskId = entity.Id;
    //    m_OnClick = onClick;

    //    txtName.text = GameEntry.Localization.GetString(entity.Name);
    //}

    public void SetUI(ServerTaskEntity entity, Action<int> onClick)
    {
        m_TaskId = entity.Id;
        m_OnClick = onClick;

        txtName.text = GameEntry.Localization.GetString(GameEntry.DataTable.DataTableManager.TaskDBModel.Get(m_TaskId).Name);
    }
}
