using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mission : ScriptableObject
{
    //标签
    public List<Mission_Sheet> Parameter;
}

[System.Serializable]
public class Mission_Sheet
{
    //标签页内的字段
    public string MissionID;
    public string MaxHPBoost;
    public string InfectionBoost;
    public string Atk_Boost;
    public string Heal_Boost;
    public string Def_Boost;
    public string Cure_Boost;
    public string Speed_Boost;
    public string InfectionAntiBoost;
    public string CommunicationAntiBoost;
    public string HPHealingBoost;
    public string ClimateBoost;
    public string EnviBoost;
    public string DistributionParam1;
    public string DistributionParam2;
    public string DistributionParam3;
    public string DistributionParam4;
    public string AbilityID_1;
    public string AbilityLv_1;
    public string AbilityID_2;
    public string AbilityLv_2;
    public string AbilityID_3;
    public string AbilityLv_3;
    public string ModeType;
    public string ModeParam1;
    public string ModeParam2;
    public string ModeParam3;
    public string EventMin;
    public string EventMax;
    public string EventPackageID;
    public string LootPackageID;

}
