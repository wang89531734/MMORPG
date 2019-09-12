using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseGameData : ScriptableObject
{
    public string title;
    [Multiline]
    public string description;

    public virtual string Id { get { return name; } }
}
