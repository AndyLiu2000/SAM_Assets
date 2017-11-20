using System.Collections.Generic;
using UnityEngine;
using System;

public class Cards_Excel : ScriptableObject
{
    //标签
    public List<Cards_Sheet> Card;
}

[Serializable]
public class Cards_Sheet
{
    public string ID;
    public string Value;
    public string DirectionType;
    public string CardType;
    public string ImgName;
    public string Weight_1;
    public string Weight_2;
    public string Weight_3;
    public string Weight_4;
    public string Weight_5;
}
