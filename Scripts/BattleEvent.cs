using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleEvent : ScriptableObject
{
    //标签
    public List<BattleEvent_Sheet> Package;
}

[System.Serializable]
public class BattleEvent_Sheet
{
    //标签页内的字段
    public string EventPackageID;
    public string EventPackageName;
    public string EventID;
    public string EventValue;
    public string Weight;
}
