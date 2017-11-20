using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InGameEvent : ScriptableObject
{
    //标签
    public List<InGameEvent_Sheet> InGameEvents;
}

[System.Serializable]
public class InGameEvent_Sheet
{
    //标签页内的字段
    public string EventID;
    public string Type;
    public string TypeParam;
    public string FieldName;
    public string EventType;
    public string Value;
    public string SkillIconName;
    public string DesID;
    public string UpgradeEffectID;
}
