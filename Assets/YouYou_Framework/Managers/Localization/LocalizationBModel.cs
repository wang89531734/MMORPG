
using System.Collections;
using System.Collections.Generic;
using System;
using YouYou;

/// <summary>
/// Sys_Localization数据管理
/// </summary>
public partial class LocalizationBModel : DataTableDBModelBase<LocalizationBModel, DataTableEntityBase>
{
    /// <summary>
    /// 文件名称
    /// </summary>
    public override string DataTableName { get { return "Localization/"+GameEntry.Localization.CurrLanguage.ToString(); } }

    /// <summary>
    /// 当前语言字典
    /// </summary>
    public Dictionary<string, string> LocalizationDic = new Dictionary<string, string>();

    /// <summary>
    /// 加载列表
    /// </summary>
    protected override void LoadList(MMO_MemoryStream ms)
    {
        int rows = ms.ReadInt();
        int columns = ms.ReadInt();

        for (int i = 0; i < rows; i++)
        {
            LocalizationDic[ms.ReadUTF8String()] = ms.ReadUTF8String();
        }
    }
}