using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseGamePlayFormation : MonoBehaviour
{
    public Transform[] containers;
    public Transform helperContainer;
    public readonly Dictionary<int, BaseCharacterEntity> Characters = new Dictionary<int, BaseCharacterEntity>();

    public virtual void SetFormationCharacters()
    {
        var formationName = Player.CurrentPlayer.SelectedFormation;
        ClearCharacters();
        for (var i = 0; i < containers.Length; ++i)
        {
            PlayerFormation playerFormation = null;
            if (PlayerFormation.TryGetData(formationName, i, out playerFormation))
            {
                var itemId = playerFormation.ItemId;
                PlayerItem item = null;
                if (!string.IsNullOrEmpty(itemId) && PlayerItem.DataMap.TryGetValue(itemId, out item))
                    SetCharacter(i, item);
            }
        }
        if (BaseGamePlayManager.Helper != null &&
            !string.IsNullOrEmpty(BaseGamePlayManager.Helper.MainCharacter) &&
            GameInstance.GameDatabase.Items.ContainsKey(BaseGamePlayManager.Helper.MainCharacter) &&
            helperContainer != null)
        {
            var item = new PlayerItem();
            item.Id = "_Helper";
            item.DataId = BaseGamePlayManager.Helper.MainCharacter;
            item.Exp = BaseGamePlayManager.Helper.MainCharacterExp;
            SetHelperCharacter(item);
        }
    }

    public virtual void SetCharacters(PlayerItem[] items)
    {
        ClearCharacters();
        for (var i = 0; i < containers.Length; ++i)
        {
            if (items.Length <= i)
                break;
            var item = items[i];
            SetCharacter(i, item);
        }
    }

    public virtual BaseCharacterEntity SetCharacter(int position, PlayerItem item)
    {
        if (position < 0 || position >= containers.Length || item == null || item.CharacterData == null)
            return null;

        if (item.CharacterData.model == null)
        {
            Debug.LogWarning("Character's model is empty, this MUST be set");
            return null;
        }

        var container = containers[position];
        container.RemoveAllChildren();

        var character = Instantiate(item.CharacterData.model);
        character.SetFormation(this, position, container);
        character.Item = item;
        Characters[position] = character;

        return character;
    }

    public virtual BaseCharacterEntity SetHelperCharacter(PlayerItem item)
    {
        if (helperContainer == null)
            return null;

        var position = containers.Length;

        if (item.CharacterData.model == null)
        {
            Debug.LogWarning("Character's model is empty, this MUST be set");
            return null;
        }

        var container = helperContainer;
        container.RemoveAllChildren();

        var character = Instantiate(item.CharacterData.model);
        character.SetFormation(this, position, container);
        character.Item = item;
        Characters[position] = character;

        return character;
    }

    public virtual void ClearCharacters()
    {
        foreach (var container in containers)
        {
            container.RemoveAllChildren();
        }
        if (helperContainer != null)
            helperContainer.RemoveAllChildren();
        Characters.Clear();
    }
}
