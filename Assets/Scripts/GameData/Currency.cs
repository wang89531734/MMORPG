using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Currency
{
    public string id;
    public Sprite icon;
    public int startAmount;

    public virtual string ToJson()
    {
        return "{\"id\":\"" + id + "\"," +
            "\"startAmount\":" + startAmount + "}";
    }
}
