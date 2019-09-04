using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UICharacterStats : UICharacterStatsGeneric
{
    public GameObject selectableObject;
    public GameObject activatingObject;

    protected override void Update()
    {
        base.Update();

        if (character == null)
            return;

        var castedCharacter = character as CharacterEntity;

        if (selectableObject != null)
            selectableObject.SetActive(castedCharacter.selectable);

        if (activatingObject != null)
            activatingObject.SetActive(castedCharacter.IsActiveCharacter);
    }
}
