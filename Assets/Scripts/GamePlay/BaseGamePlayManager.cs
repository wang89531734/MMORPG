using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public abstract class BaseGamePlayManager : MonoBehaviour
{
    public static string BattleSession { get; private set; }
    public static BaseStage PlayingStage { get; protected set; }
    public static Player Helper { get; protected set; }
    [Header("Combat Texts")]
    public Transform combatTextContainer;
    public UICombatText combatDamagePrefab;
    public UICombatText combatCriticalPrefab;
    public UICombatText combatBlockPrefab;
    public UICombatText combatHealPrefab;
    public UICombatText combatPoisonPrefab;
    public UICombatText combatMissPrefab;
    [Header("Gameplay UI")]
    public UIWin uiWin;
    public UILose uiLose;
    public UIPlayer uiFriendRequest;
    public UIPauseGame uiPauseGame;
    public float winGameDelay = 2f;
    public float loseGameDelay = 2f;

    private bool? isAutoPlay;
    public bool IsAutoPlay {
        get
        {
            if (!isAutoPlay.HasValue)
                isAutoPlay = PlayerPrefs.GetInt(Consts.KeyIsAutoPlay, 0) > 0;
            return isAutoPlay.Value;
        }
        set
        {
            isAutoPlay = value;
            PlayerPrefs.SetInt(Consts.KeyIsAutoPlay, value ? 1 : 0);
            PlayerPrefs.Save();
        }
    }
    private bool? isSpeedMultiply;
    public bool IsSpeedMultiply
    {
        get
        {
            if (!isSpeedMultiply.HasValue)
                isSpeedMultiply = PlayerPrefs.GetInt(Consts.KeyIsSpeedMultiply, 0) > 0;
            return isSpeedMultiply.Value;
        }
        set
        {
            isSpeedMultiply = value;
            PlayerPrefs.SetInt(Consts.KeyIsSpeedMultiply, value ? 1 : 0);
            PlayerPrefs.Save();
        }
    }
    protected bool isAutoPlayDirty;
    protected bool isEnding;

    public void SpawnDamageText(int amount, BaseCharacterEntity character)
    {
        SpawnCombatText(combatDamagePrefab, amount, character);
    }

    public void SpawnCriticalText(int amount, BaseCharacterEntity character)
    {
        SpawnCombatText(combatCriticalPrefab, amount, character);
    }

    public void SpawnBlockText(int amount, BaseCharacterEntity character)
    {
        SpawnCombatText(combatBlockPrefab, amount, character);
    }

    public void SpawnHealText(int amount, BaseCharacterEntity character)
    {
        SpawnCombatText(combatHealPrefab, amount, character);
    }

    public void SpawnPoisonText(int amount, BaseCharacterEntity character)
    {
        SpawnCombatText(combatPoisonPrefab, amount, character);
    }

    public void SpawnMissText(BaseCharacterEntity character)
    {
        var combatText = Instantiate(combatMissPrefab, combatTextContainer);
        combatText.transform.localScale = Vector3.one;
        combatText.TempObjectFollower.targetObject = character.bodyEffectContainer;
        combatText.Amount = 0;
        combatText.TempText.text = LanguageManager.Texts[GameText.COMBAT_MISS];
    }

    public void SpawnCombatText(UICombatText prefab, int amount, BaseCharacterEntity character)
    {
        var combatText = Instantiate(prefab, combatTextContainer);
        combatText.transform.localScale = Vector3.one;
        combatText.TempObjectFollower.targetObject = character.bodyEffectContainer;
        combatText.Amount = amount;
    }


    protected IEnumerator WinGameRoutine()
    {
        isEnding = true;
        yield return new WaitForSeconds(winGameDelay);
        WinGame();
    }

    protected virtual void WinGame()
    {
        var deadCharacters = CountDeadCharacters();
        GameInstance.GameService.FinishStage(BattleSession, BaseGameService.BATTLE_RESULT_WIN, deadCharacters, (result) =>
        {
            isEnding = true;
            Time.timeScale = 1;
            GameInstance.Singleton.OnGameServiceFinishStageResult(result);
            uiWin.SetData(result);
            if (uiFriendRequest != null && Helper != null && !Helper.isFriend)
            {
                uiFriendRequest.SetData(Helper);
                uiFriendRequest.eventFriendRequestSuccess.AddListener(() =>
                {
                    uiFriendRequest.Hide();
                });
                uiFriendRequest.eventHide.AddListener(() =>
                {
                    uiWin.Show();
                });
                uiFriendRequest.Show();
            }
            else
            {
                uiWin.Show();
            }
        }, (error) =>
        {
            GameInstance.Singleton.OnGameServiceError(error, WinGame);
        });
    }

    protected IEnumerator LoseGameRoutine()
    {
        isEnding = true;
        yield return new WaitForSeconds(loseGameDelay);
        uiLose.Show();
    }

    public virtual void Revive(UnityAction onError)
    {
        GameInstance.GameService.ReviveCharacters((result) =>
        {
            OnRevive();
        }, (error) =>
        {
            GameInstance.Singleton.OnGameServiceError(error, onError);
        });
    }

    public void Giveup(UnityAction onError)
    {
        var deadCharacters = CountDeadCharacters();
        GameInstance.GameService.FinishStage(BattleSession, BaseGameService.BATTLE_RESULT_LOSE, deadCharacters, (result) =>
        {
            isEnding = true;
            Time.timeScale = 1;
            GameInstance.Singleton.GetAllPlayerData(GameInstance.LoadAllPlayerDataState.GoToManageScene);
        }, (error) =>
        {
            GameInstance.Singleton.OnGameServiceError(error, onError);
        });
    }

    public void Restart()
    {
        StartStage(PlayingStage, Helper);
    }

    public static void StartStage(BaseStage data, Player helper)
    {
        PlayingStage = data;
        Helper = helper;
        GameInstance.GameService.StartStage(data.Id, (result) =>
        {
            GameInstance.Singleton.OnGameServiceStartStageResult(result);
            BattleSession = result.session;
            GameInstance.Singleton.LoadBattleScene();
        }, (error) =>
        {
            GameInstance.Singleton.OnGameServiceError(error);
        });
    }

    public virtual void OnRevive()
    {
        isEnding = false;
    }

    public abstract int CountDeadCharacters();
}
