using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartSceneManager : MonoBehaviour
{
    public static StartSceneManager Singleton { get; private set; }
    public UIAuthentication loginDialog;
    public UIAuthentication registerDialog;
    public GameObject clickStartObject;
    private void Awake()
    {
        if (Singleton != null)
        {
            Destroy(gameObject);
            return;
        }
        Singleton = this;
        ShowClickStart();
    }

    public void OnClickStart()
    {
        var gameInstance = GameInstance.Singleton;
        var gameService = GameInstance.GameService;
        gameService.ValidateLoginToken(true, OnValidateLoginTokenSuccess, OnValidateLoginTokenError);
        HideClickStart();
    }

    private void OnValidateLoginTokenSuccess(PlayerResult result)
    {
        var gameInstance = GameInstance.Singleton;
        gameInstance.OnGameServiceLogin(result);
    }

    private void OnValidateLoginTokenError(string error)
    {
        var gameInstance = GameInstance.Singleton;
    }

    /// <summary>
    /// 显示开始按钮
    /// </summary>
    public void ShowClickStart()
    {
        if (clickStartObject != null)
            clickStartObject.SetActive(true);
    }

    /// <summary>
    /// 关闭开始按钮
    /// </summary>
    public void HideClickStart()
    {
        if (clickStartObject != null)
            clickStartObject.SetActive(false);
    }
}
