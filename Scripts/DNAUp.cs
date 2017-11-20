using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DNAUp : ScriptableObject{
    //标签页
    public List<DNAUp_Sheet> Virus;
    public List<DNAUp_Sheet> Human;
    public List<DNAUp_Sheet> Zombie;
}

[System.Serializable]
public class DNAUp_Sheet
{
    //标签页内的字段
    public string ID;
    public string Type;
    public string Name;
    public string Value1;
    public string Value1_Add;
    public string Value2;
    public string Value2_Add;
    public string Value3;
    public string Value3_Add;
    public string GoldCost;
    public string GoldParam_1;
    public string GoldParam_2;
    public string GemCost;
    public string GemParam_1;
    public string GemParam_2;
}
