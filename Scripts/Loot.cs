using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Loot : ScriptableObject
{
    //标签
    public List<Loot_Sheet> Package;
}

[System.Serializable]
public class Loot_Sheet
{
    //标签页内的字段
    public string LootPackageID;
    public string PackageName;
    public string ItemID;
    public string ItemNum;
    public string Weight;
}
