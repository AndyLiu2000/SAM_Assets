using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IAP : ScriptableObject
{
    //标签
    public List<IAP_Sheet> Item;
}

[System.Serializable]
public class IAP_Sheet
{
    //标签页内的字段
    public string IAPPackageID;
    public string PackageName;
    public string LootID;
    public string DollarPrice;
    public string IAPSlot;
}
