using UnityEngine;
using UnityEditor;
using Excel;
using System.Data;
using System.IO;
using System.Collections.Generic;
using OfficeOpenXml;

public class ExcelAccess
{
    //存储所有Excel文件名
    public const string IAP = "IAP";
    public const string LANGUAGE = "Language";
    public const string LOOT = "Loot";
    public const string MISSION = "Mission";
    public const string UNLOCK = "Unlock";

    //存储所有页签名
    public static string[] IAP_SheetNames = { "Item" };
    public static string[] LANGUAGE_SheetNames = { "Localization" };
    public static string[] LOOT_SheetNames = { "Package" };
    public static string[] MISSION_SheetNames = { "Parameter" };
    public static string[] UNLOCK_SheetNames = { "UnlockMission" };


    //读IAP.xlsx表
    public static List<IAP_Sheet> SelectIAPTable(int tableId)
    {
        DataRowCollection collect = ExcelAccess.ReadExcel(IAP + ".xlsx", IAP_SheetNames[tableId - 1]);
        List<IAP_Sheet> array = new List<IAP_Sheet>();

        for (int i = 1; i < collect.Count; i++)
        {
            if (collect[i][1].ToString() == "") continue;

            IAP_Sheet Item = new IAP_Sheet();
            Item.IAPPackageID = collect[i][0].ToString();
            Item.PackageName = collect[i][1].ToString();
            Item.LootID = collect[i][2].ToString();
            Item.DollarPrice = collect[i][3].ToString();
            Item.IAPSlot = collect[i][4].ToString();

            array.Add(Item);
        }
        return array;
    }

    //读Language.xlsx表
    public static List<Language_Sheet> SelectLanguageTable(int tableId)
    {
        DataRowCollection collect = ExcelAccess.ReadExcel(LANGUAGE + ".xlsx", LANGUAGE_SheetNames[tableId - 1]);
        List<Language_Sheet> array = new List<Language_Sheet>();

        for (int i = 1; i < collect.Count; i++)
        {
            if (collect[i][1].ToString() == "") continue;

            Language_Sheet Localization = new Language_Sheet();
            Localization.TextID = collect[i][0].ToString();
            Localization.ZH = collect[i][1].ToString();
            Localization.EN = collect[i][2].ToString();

            array.Add(Localization);
        }
        return array;
    }

    //读Loot.xlsx表
    public static List<Loot_Sheet> SelectLootTable(int tableId)
    {
        DataRowCollection collect = ExcelAccess.ReadExcel(LOOT + ".xlsx", LOOT_SheetNames[tableId - 1]);
        List<Loot_Sheet> array = new List<Loot_Sheet>();

        for (int i = 1; i < collect.Count; i++)
        {
            if (collect[i][1].ToString() == "") continue;

            Loot_Sheet Package = new Loot_Sheet();
            Package.LootPackageID = collect[i][0].ToString();
            Package.PackageName = collect[i][1].ToString();
            Package.ItemID = collect[i][2].ToString();
            Package.ItemNum = collect[i][3].ToString();
            Package.Weight = collect[i][4].ToString();

            array.Add(Package);
        }
        return array;
    }

    //读Mission.xlsx表
    public static List<Mission_Sheet> SelectMissionTable(int tableId)
    {
        DataRowCollection collect = ExcelAccess.ReadExcel(MISSION + ".xlsx", MISSION_SheetNames[tableId - 1]);
        List<Mission_Sheet> array = new List<Mission_Sheet>();

        for (int i = 1; i < collect.Count; i++)
        {
            if (collect[i][1].ToString() == "") continue;

            Mission_Sheet Parameter = new Mission_Sheet();
            Parameter.MissionID = collect[i][0].ToString();
            Parameter.MaxHPBoost = collect[i][1].ToString();
            Parameter.InfectionBoost = collect[i][2].ToString();
            Parameter.Atk_Boost = collect[i][3].ToString();
            Parameter.Heal_Boost = collect[i][4].ToString();
            Parameter.Def_Boost = collect[i][5].ToString();
            Parameter.Cure_Boost = collect[i][6].ToString();
            Parameter.Speed_Boost = collect[i][7].ToString();
            Parameter.InfectionAntiBoost = collect[i][8].ToString();
            Parameter.CommunicationAntiBoost = collect[i][9].ToString();
            Parameter.HPHealingBoost = collect[i][10].ToString();
            Parameter.ClimateBoost = collect[i][11].ToString();
            Parameter.EnviBoost = collect[i][12].ToString();
            Parameter.DistributionParam1 = collect[i][13].ToString();
            Parameter.DistributionParam2 = collect[i][14].ToString();
            Parameter.DistributionParam3 = collect[i][15].ToString();
            Parameter.DistributionParam4 = collect[i][16].ToString();
            Parameter.AbilityID_1 = collect[i][17].ToString();
            Parameter.AbilityLv_1 = collect[i][18].ToString();
            Parameter.AbilityID_2 = collect[i][19].ToString();
            Parameter.AbilityLv_2 = collect[i][20].ToString();
            Parameter.AbilityID_3 = collect[i][21].ToString();
            Parameter.AbilityLv_3 = collect[i][22].ToString();
            Parameter.ModeType = collect[i][23].ToString();
            Parameter.ModeParam1 = collect[i][24].ToString();
            Parameter.ModeParam2 = collect[i][25].ToString();
            Parameter.ModeParam3 = collect[i][26].ToString();
            Parameter.EventMin = collect[i][27].ToString();
            Parameter.EventMax = collect[i][28].ToString();
            Parameter.EventPackageID = collect[i][29].ToString();
            Parameter.LootPackageID = collect[i][30].ToString();

            array.Add(Parameter);
        }
        return array;
    }

    //读Unlock.xlsx表
    public static List<UnlockMission_Sheet> SelectUnlockTable(int tableId)
    {
        DataRowCollection collect = ExcelAccess.ReadExcel(UNLOCK + ".xlsx", UNLOCK_SheetNames[tableId - 1]);
        List<UnlockMission_Sheet> array = new List<UnlockMission_Sheet>();

        for (int i = 1; i < collect.Count; i++)
        {
            if (collect[i][1].ToString() == "") continue;

            UnlockMission_Sheet UnlockMission = new UnlockMission_Sheet();
            UnlockMission.MissionID = collect[i][0].ToString();
            UnlockMission.UnlockType = collect[i][1].ToString();
            UnlockMission.Param1 = collect[i][2].ToString();
            UnlockMission.Param2 = collect[i][3].ToString();
            UnlockMission.Param3 = collect[i][4].ToString();
            UnlockMission.Param4 = collect[i][5].ToString();
            UnlockMission.Param5 = collect[i][6].ToString();
            UnlockMission.UnlockItemID = collect[i][7].ToString();
            UnlockMission.UnlockCost = collect[i][8].ToString();

            array.Add(UnlockMission);
        }
        return array;
    }

    /// <summary>
    /// 读取excel的sheet下的内容
    /// </summary>
    /// <param name="excelName"></param>
    /// <param name="sheet"></param>
    /// <returns></returns>
    static DataRowCollection ReadExcel(string excelName,string sheet)
    {
        FileStream stream = File.Open(FilePath(excelName), FileMode.Open, FileAccess.Read, FileShare.Read);
        IExcelDataReader excelReader = ExcelReaderFactory.CreateOpenXmlReader(stream);

        DataSet result = excelReader.AsDataSet();
        //int columns = result.Tables[0].Columns.Count;
        //int rows = result.Tables[0].Rows.Count;
        Debug.Log("excelName = " + excelName);
        return result.Tables[sheet].Rows;
    }

    public static string FilePath(string name)
    {
        return Application.dataPath+"/OriginalDatas/" + name;
    }

}

