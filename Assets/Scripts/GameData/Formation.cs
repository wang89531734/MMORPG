using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Formation
{
    public string id;
    public string title;
    public Sprite icon;

    public virtual string ToJson()
    {
        return "{\"id\":\"" + id + "\"}";
    }
}
