using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;

public partial class LiteDbGameService
{
    protected override void DoLogin(string username, string password, UnityAction<PlayerResult> onFinish)
    {
        var result = new PlayerResult();
        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            result.error = GameServiceErrorCode.EMPTY_USERNAME_OR_PASSWORD;
        else
        {
            DbPlayer player = null;
            if (!TryGetPlayer(AUTH_NORMAL, username, password, out player))
                result.error = GameServiceErrorCode.INVALID_USERNAME_OR_PASSWORD;
            else
            {
                player = UpdatePlayerLoginToken(player);
                UpdatePlayerStamina(player);
                var resultPlayer = new Player();
                Player.CloneTo(player, resultPlayer);
                result.player = resultPlayer;
            }
        }
        onFinish(result);
    }

    protected override void DoRegisterOrLogin(string username, string password, UnityAction<PlayerResult> onFinish)
    {
        if (IsPlayerWithUsernameFound(AUTH_NORMAL, username))
            DoLogin(username, password, onFinish);
        else
            DoRegister(username, password, onFinish);
    }

    protected override void DoGuestLogin(string deviceId, UnityAction<PlayerResult> onFinish)
    {
        var result = new PlayerResult();
        if (string.IsNullOrEmpty(deviceId))
            result.error = GameServiceErrorCode.EMPTY_USERNAME_OR_PASSWORD;
        else if (IsPlayerWithUsernameFound(AUTH_GUEST, deviceId))
        {
            DbPlayer player = null;
            if (!TryGetPlayer(AUTH_GUEST, deviceId, deviceId, out player))
                result.error = GameServiceErrorCode.INVALID_USERNAME_OR_PASSWORD;
            else
            {
                player = UpdatePlayerLoginToken(player);
                UpdatePlayerStamina(player);
                var resultPlayer = new Player();
                Player.CloneTo(player, resultPlayer);
                result.player = resultPlayer;
            }
        }
        else
        {
            var player = InsertNewPlayer(AUTH_GUEST, deviceId, deviceId);
            var resultPlayer = new Player();
            Player.CloneTo(player, resultPlayer);
            result.player = resultPlayer;
        }
        onFinish(result);
    }

    protected override void DoValidateLoginToken(string playerId, string loginToken, bool refreshToken, UnityAction<PlayerResult> onFinish)
    {
        var result = new PlayerResult();
        DbPlayer player = null;
        if (!string.IsNullOrEmpty(playerId) && !string.IsNullOrEmpty(loginToken))
            player = colPlayer.FindOne(a => a.Id == playerId && a.LoginToken == loginToken);
        if (player == null)
            result.error = GameServiceErrorCode.INVALID_LOGIN_TOKEN;
        else
        {
            if (refreshToken)
                player = UpdatePlayerLoginToken(player);
            UpdatePlayerStamina(player);
            var resultPlayer = new Player();
            Player.CloneTo(player, resultPlayer);
            result.player = resultPlayer;
        }
        onFinish(result);
    }

    protected override void DoSetProfileName(string playerId, string loginToken, string profileName, UnityAction<PlayerResult> onFinish)
    {
        var result = new PlayerResult();
        DbPlayer player = null;
        if (!string.IsNullOrEmpty(playerId) && !string.IsNullOrEmpty(loginToken))
            player = colPlayer.FindOne(a => a.Id == playerId && a.LoginToken == loginToken);
        var playerWithName = colPlayer.FindOne(a => a.ProfileName == profileName);
        if (player == null)
            result.error = GameServiceErrorCode.INVALID_LOGIN_TOKEN;
        else if (string.IsNullOrEmpty(profileName))
            result.error = GameServiceErrorCode.EMPTY_PROFILE_NAME;
        else if (playerWithName != null)
            result.error = GameServiceErrorCode.EXISTED_PROFILE_NAME;
        else
        {
            player.ProfileName = profileName;
            colPlayer.Update(player);
            var resultPlayer = new Player();
            Player.CloneTo(player, resultPlayer);
            result.player = resultPlayer;
        }
        onFinish(result);
    }

    protected override void DoRegister(string username, string password, UnityAction<PlayerResult> onFinish)
    {
        var result = new PlayerResult();
        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            result.error = GameServiceErrorCode.EMPTY_USERNAME_OR_PASSWORD;
        else if (IsPlayerWithUsernameFound(AUTH_NORMAL, username))
            result.error = GameServiceErrorCode.EXISTED_USERNAME;
        else
        {
            var player = InsertNewPlayer(AUTH_NORMAL, username, password);
            var resultPlayer = new Player();
            Player.CloneTo(player, resultPlayer);
            result.player = resultPlayer;
        }
        onFinish(result);
    }
}
