using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public partial class LiteDbGameService
{
    protected override void DoGetFriendList(string playerId, string loginToken, UnityAction<FriendListResult> onFinish)
    {
        var result = new FriendListResult();
        onFinish(result);
    }

    protected override void DoGetFriendRequestList(string playerId, string loginToken, UnityAction<FriendListResult> onFinish)
    {
        var result = new FriendListResult();
        onFinish(result);
    }

    protected override void DoFriendRequest(string playerId, string loginToken, string targetPlayerId, UnityAction<GameServiceResult> onFinish)
    {
        var result = new GameServiceResult();
        onFinish(result);
    }

    protected override void DoFriendAccept(string playerId, string loginToken, string targetPlayerId, UnityAction<GameServiceResult> onFinish)
    {
        var result = new PlayerResult();
        onFinish(result);
    }

    protected override void DoFriendDecline(string playerId, string loginToken, string targetPlayerId, UnityAction<GameServiceResult> onFinish)
    {
        var result = new PlayerResult();
        onFinish(result);
    }

    protected override void DoFriendDelete(string playerId, string loginToken, string targetPlayerId, UnityAction<GameServiceResult> onFinish)
    {
        var result = new PlayerResult();
        onFinish(result);
    }
}
