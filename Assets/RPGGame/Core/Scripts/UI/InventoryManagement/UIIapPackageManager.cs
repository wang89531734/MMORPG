using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UIIapPackageManager : UIBase
{
    public UIIapPackageList uiIapPackageList;

    public override void Show()
    {
        base.Show();

        if (uiIapPackageList != null)
        {
            var availableIAPPackagees = GameInstance.AvailableIapPackages;
            var allIAPPackagees = GameInstance.GameDatabase.IapPackages;
            var list = allIAPPackagees.Values.Where(a => availableIAPPackagees.Contains(a.Id)).ToList();
            uiIapPackageList.SetListItems(list);
        }
    }
}
