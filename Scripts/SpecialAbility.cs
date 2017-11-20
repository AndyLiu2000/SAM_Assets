using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpecialAbility : ScriptableObject
{
    //标签
    public List<SpecialAbility_Sheet> Ability;
}

[System.Serializable]
public class SpecialAbility_Sheet
{
    //标签页内的字段
    public string ID;
    public string ResIcon;
    public string Name;
    public string Value1;
    public string Value1_Add;
    public string Value2;
    public string Value2_Add;
    public string Value3;
    public string Value3_Add;
    public string DesTextID;
    public string SEName;
}
