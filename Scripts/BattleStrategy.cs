using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleStrategy : ScriptableObject {

    //标签页
    public List<BattleStrategy_Sheet> Strategy;
}

[System.Serializable]
public class BattleStrategy_Sheet
{
    //标签页内的字段
    public string GeneID;
    public string StrategyID;
    public string BoardID;
    public string EventID;
    public string Row;
    public string Column;
    public string FP1;
    public string FP2;
    public string UnlockCost_A;
    public string UnlockCost_B;
}
