using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SPList : ScriptableObject
{
    //标签
    public List<Infection_Sheet> InfectionSheet;
    public List<Damage_Sheet> DamageSheet;
}

[System.Serializable]
public class Infection_Sheet{

    //标签页内的字段
    public string TotalInfection;
    public string GainSP;
}

[System.Serializable]
public class Damage_Sheet
{

    //标签页内的字段
    public string TotalDamage;
    public string GainSP;
}
