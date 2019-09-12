using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPair : BasePlayerData, IPlayerPair
{
    public string Id { get { return GetId(PlayerId, PairPlayerId); } set { } }
    public string playerId;
    public string PlayerId { get { return playerId; } set { playerId = value; } }
    public string pairPlayerId;
    public string PairPlayerId { get { return pairPlayerId; } set { pairPlayerId = value; } }

    public PlayerPair Clone()
    {
        var result = new PlayerPair();
        CloneTo(this, result);
        return result;
    }

    public static void CloneTo(IPlayerPair from, IPlayerPair to)
    {
        to.Id = from.Id;
        to.PlayerId = from.PlayerId;
        to.PairPlayerId = from.PairPlayerId;
    }

    public static string GetId(string playerId, string pairPlayerId)
    {
        return playerId + "_" + pairPlayerId;
    }
}
