using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Virus {

    //Model：Virus
    public int VirusID;
    public int InfectSpeed;
    public int InfectHuman_1;
    public int InfectHuman_2;
    public int InfectHuman_3;
    public int InfectHuman_4;
    public int InfectHuman_5;
    public int InfectBlock_Climate_1;
    public int InfectBlock_Climate_2;
    public int InfectBlock_Climate_3;
    public int InfectBlock_Envi_1;
    public int InfectBlock_Envi_2;
    public int InfectBlock_Envi_3;
    public int CommunicateRate;
    public int CommunicateHuman_1;
    public int CommunicateHuman_2;
    public int CommunicateHuman_3;
    public int CommunicateHuman_4;
    public int CommunicateHuman_5;
    public int CommunicateBlock_Climate_1;
    public int CommunicateBlock_Climate_2;
    public int CommunicateBlock_Climate_3;
    public int CommunicateBlock_Envi_1;
    public int CommunicateBlock_Envi_2;
    public int CommunicateBlock_Envi_3;
    public int InitialSP;
    public string Name;
    public string Res;
    public string StrategyID;
    public int Medi_Start;
    public int Medi_Work;
    public int Medi_Spd;
    public int CommunicationThreshold;

    public List<int> InfectHumans = new List<int>();
    public List<int> InfectClims = new List<int>();
    public List<int> InfectEnvis = new List<int>();
    public List<int> CommunicateHumans = new List<int>();
    public List<int> CommunicateClimates = new List<int>();
    public List<int> CommunicateEnvis = new List<int>();

    //战斗变量
    public Environment Envi;
    public Climate Clim;

    public Virus(int virusID, int curMissionID)
    {
        //Model - DNAUp + Mission
        VirusID = virusID;
        Virus_Sheet virus = new Virus_Sheet();
        foreach (Virus_Sheet v in DataManager.Model_Virus)
        {
            if (v.VirusID == VirusID.ToString())
            {
                virus = v;
                break;
            }
        }

        //Model值
        InfectSpeed = int.Parse(virus.InfectSpeed);
        InfectHuman_1 = int.Parse(virus.InfectHuman_1);
        InfectHuman_2 = int.Parse(virus.InfectHuman_2);
        InfectHuman_3 = int.Parse(virus.InfectHuman_3);
        InfectHuman_4 = int.Parse(virus.InfectHuman_4);
        InfectHuman_5 = int.Parse(virus.InfectHuman_5);
        InfectBlock_Climate_1 = int.Parse(virus.InfectBlock_Climate_1);
        InfectBlock_Climate_2 = int.Parse(virus.InfectBlock_Climate_2);
        InfectBlock_Climate_3 = int.Parse(virus.InfectBlock_Climate_3);
        InfectBlock_Envi_1 = int.Parse(virus.InfectBlock_Envi_1);
        InfectBlock_Envi_2 = int.Parse(virus.InfectBlock_Envi_2);
        InfectBlock_Envi_3 = int.Parse(virus.InfectBlock_Envi_3);
        CommunicateRate = int.Parse(virus.CommunicateRate);
        CommunicateHuman_1 = int.Parse(virus.CommunicateHuman_1);
        CommunicateHuman_2 = int.Parse(virus.CommunicateHuman_2);
        CommunicateHuman_3 = int.Parse(virus.CommunicateHuman_3);
        CommunicateHuman_4 = int.Parse(virus.CommunicateHuman_4);
        CommunicateHuman_5 = int.Parse(virus.CommunicateHuman_5);
        CommunicateBlock_Climate_1 = int.Parse(virus.CommunicateBlock_Climate_1);
        CommunicateBlock_Climate_2 = int.Parse(virus.CommunicateBlock_Climate_2);
        CommunicateBlock_Climate_3 = int.Parse(virus.CommunicateBlock_Climate_3);
        CommunicateBlock_Envi_1 = int.Parse(virus.CommunicateBlock_Envi_1);
        CommunicateBlock_Envi_2 = int.Parse(virus.CommunicateBlock_Envi_2);
        CommunicateBlock_Envi_3 = int.Parse(virus.CommunicateBlock_Envi_3);
        InitialSP = int.Parse(virus.InitialSP);
        StrategyID = virus.StrategyID;
        Medi_Start = int.Parse(virus.Medi_Start);
        Medi_Work = int.Parse(virus.Medi_Work);
        Medi_Spd = int.Parse(virus.Medi_Spd);
        CommunicationThreshold = int.Parse(virus.CommunicationThreshold);

        //DNA值
        InfectSpeed = (int)(InfectSpeed * (1000 + Formula.FieldNameToValue("InfectSpeed", DataManager.DNAUp_Virus, GameManager.user.DB_u_dna[1])) / 1000);
        InfectHuman_1 = (int)(InfectHuman_1 * (1000 + Formula.FieldNameToValue("InfectHuman_1", DataManager.DNAUp_Virus, GameManager.user.DB_u_dna[1])) / 1000);
        InfectHuman_2 = (int)(InfectHuman_2 * (1000 + Formula.FieldNameToValue("InfectHuman_2", DataManager.DNAUp_Virus, GameManager.user.DB_u_dna[1])) / 1000);
        InfectHuman_3 = (int)(InfectHuman_3 * (1000 + Formula.FieldNameToValue("InfectHuman_3", DataManager.DNAUp_Virus, GameManager.user.DB_u_dna[1])) / 1000);
        InfectHuman_4 = (int)(InfectHuman_4 * (1000 + Formula.FieldNameToValue("InfectHuman_4", DataManager.DNAUp_Virus, GameManager.user.DB_u_dna[1])) / 1000);
        InfectHuman_5 = (int)(InfectHuman_5 * (1000 + Formula.FieldNameToValue("InfectHuman_5", DataManager.DNAUp_Virus, GameManager.user.DB_u_dna[1])) / 1000);
        InfectBlock_Climate_1 = (int)(InfectBlock_Climate_1 * (1000 + Formula.FieldNameToValue("InfectBlock_Climate_1", DataManager.DNAUp_Virus, GameManager.user.DB_u_dna[1])) / 1000);
        InfectBlock_Climate_2 = (int)(InfectBlock_Climate_2 * (1000 + Formula.FieldNameToValue("InfectBlock_Climate_2", DataManager.DNAUp_Virus, GameManager.user.DB_u_dna[1])) / 1000);
        InfectBlock_Climate_3 = (int)(InfectBlock_Climate_3 * (1000 + Formula.FieldNameToValue("InfectBlock_Climate_3", DataManager.DNAUp_Virus, GameManager.user.DB_u_dna[1])) / 1000);
        InfectBlock_Envi_1 = (int)(InfectBlock_Envi_1 * (1000 + Formula.FieldNameToValue("InfectBlock_Envi_1", DataManager.DNAUp_Virus, GameManager.user.DB_u_dna[1])) / 1000);
        InfectBlock_Envi_2 = (int)(InfectBlock_Envi_2 * (1000 + Formula.FieldNameToValue("InfectBlock_Envi_2", DataManager.DNAUp_Virus, GameManager.user.DB_u_dna[1])) / 1000);
        InfectBlock_Envi_3 = (int)(InfectBlock_Envi_3 * (1000 + Formula.FieldNameToValue("InfectBlock_Envi_3", DataManager.DNAUp_Virus, GameManager.user.DB_u_dna[1])) / 1000);
        CommunicateRate = (int)(CommunicateRate * (1000 + Formula.FieldNameToValue("CommunicateRate", DataManager.DNAUp_Virus, GameManager.user.DB_u_dna[1])) / 1000);
        CommunicateHuman_1 = (int)(CommunicateHuman_1 * (1000 + Formula.FieldNameToValue("CommunicateHuman_1", DataManager.DNAUp_Virus, GameManager.user.DB_u_dna[1])) / 1000);
        CommunicateHuman_2 = (int)(CommunicateHuman_2 * (1000 + Formula.FieldNameToValue("CommunicateHuman_2", DataManager.DNAUp_Virus, GameManager.user.DB_u_dna[1])) / 1000);
        CommunicateHuman_3 = (int)(CommunicateHuman_3 * (1000 + Formula.FieldNameToValue("CommunicateHuman_3", DataManager.DNAUp_Virus, GameManager.user.DB_u_dna[1])) / 1000);
        CommunicateHuman_4 = (int)(CommunicateHuman_4 * (1000 + Formula.FieldNameToValue("CommunicateHuman_4", DataManager.DNAUp_Virus, GameManager.user.DB_u_dna[1])) / 1000);
        CommunicateHuman_5 = (int)(CommunicateHuman_5 * (1000 + Formula.FieldNameToValue("CommunicateHuman_5", DataManager.DNAUp_Virus, GameManager.user.DB_u_dna[1])) / 1000);
        CommunicateBlock_Climate_1 = (int)(CommunicateBlock_Climate_1 * (1000 + Formula.FieldNameToValue("CommunicateBlock_Climate_1", DataManager.DNAUp_Virus, GameManager.user.DB_u_dna[1])) / 1000);
        CommunicateBlock_Climate_2 = (int)(CommunicateBlock_Climate_2 * (1000 + Formula.FieldNameToValue("CommunicateBlock_Climate_2", DataManager.DNAUp_Virus, GameManager.user.DB_u_dna[1])) / 1000);
        CommunicateBlock_Climate_3 = (int)(CommunicateBlock_Climate_3 * (1000 + Formula.FieldNameToValue("CommunicateBlock_Climate_3", DataManager.DNAUp_Virus, GameManager.user.DB_u_dna[1])) / 1000);
        CommunicateBlock_Envi_1 = (int)(CommunicateBlock_Envi_1 * (1000 + Formula.FieldNameToValue("CommunicateBlock_Envi_1", DataManager.DNAUp_Virus, GameManager.user.DB_u_dna[1])) / 1000);
        CommunicateBlock_Envi_2 = (int)(CommunicateBlock_Envi_2 * (1000 + Formula.FieldNameToValue("CommunicateBlock_Envi_2", DataManager.DNAUp_Virus, GameManager.user.DB_u_dna[1])) / 1000);
        CommunicateBlock_Envi_3 = (int)(CommunicateBlock_Envi_3 * (1000 + Formula.FieldNameToValue("CommunicateBlock_Envi_3", DataManager.DNAUp_Virus, GameManager.user.DB_u_dna[1])) / 1000);
        Medi_Start = (int)(Medi_Start * (1000 + Formula.FieldNameToValue("Medi_Start", DataManager.DNAUp_Human, GameManager.user.DB_u_dna[1])) / 1000);
        Medi_Work = (int)(Medi_Work * (1000 + Formula.FieldNameToValue("Medi_Work", DataManager.DNAUp_Virus, GameManager.user.DB_u_dna[1])) / 1000);
        Medi_Spd = (int)(Medi_Spd * (1000 + Formula.FieldNameToValue("Medi_Spd", DataManager.DNAUp_Virus, GameManager.user.DB_u_dna[1])) / 1000);

        InfectHumans.Add(InfectHuman_1);
        InfectHumans.Add(InfectHuman_2);
        InfectHumans.Add(InfectHuman_3);
        InfectHumans.Add(InfectHuman_4);
        InfectHumans.Add(InfectHuman_5);

        InfectClims.Add(InfectBlock_Climate_1);
        InfectClims.Add(InfectBlock_Climate_2);
        InfectClims.Add(InfectBlock_Climate_3);

        InfectEnvis.Add(InfectBlock_Envi_1);
        InfectEnvis.Add(InfectBlock_Envi_2);
        InfectEnvis.Add(InfectBlock_Envi_3);

        CommunicateHumans.Add(CommunicateHuman_1);
        CommunicateHumans.Add(CommunicateHuman_2);
        CommunicateHumans.Add(CommunicateHuman_3);
        CommunicateHumans.Add(CommunicateHuman_4);
        CommunicateHumans.Add(CommunicateHuman_5);

        CommunicateClimates.Add(CommunicateBlock_Climate_1);
        CommunicateClimates.Add(CommunicateBlock_Climate_2);
        CommunicateClimates.Add(CommunicateBlock_Climate_3);

        CommunicateEnvis.Add(CommunicateBlock_Envi_1);
        CommunicateEnvis.Add(CommunicateBlock_Envi_2);
        CommunicateEnvis.Add(CommunicateBlock_Envi_3);

        //return this;

        //没有Mission值
    }

    public Virus VirusBattleEvent()
    {
        //Virus基础值 + 事件影响值
        return this;
    }
}
