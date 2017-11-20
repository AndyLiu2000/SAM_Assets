using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unlock : ScriptableObject
{
    //标签
    public List<UnlockMission_Sheet> UnlockMission;
}

[System.Serializable]
public class UnlockMission_Sheet
{
    //标签页内的字段
    public string MissionID;
    public string UnlockType;
    public string Param1;
    public string Param2;
    public string Param3;
    public string Param4;
    public string Param5;
    public string UnlockItemID;
    public string UnlockCost;
}
