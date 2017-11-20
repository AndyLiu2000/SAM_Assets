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
    public const string BATTLESTRATEGY = "BattleStrategy";
    public const string DNAUP = "DNAUp";
    public const string BATTLEEVENT = "BattleEvent";
    public const string IAP = "IAP";
    public const string LANGUAGE = "Language";
    public const string LOOT = "Loot";
    public const string MISSION = "Mission";
    public const string MODEL = "Model";
    public const string SPECIALABILITY = "SpecialAbility";
    public const string UNLOCK = "Unlock";
    public const string INGAMEEVENT = "InGameEvent";
    public const string SPLIST = "SPList";
	public const string CARDS = "Cards";

    //存储所有页签名
    public static string[] BATTLESTRATEGY_SheetNames = { "Strategy" };
    public static string[] DNAUP_SheetNames = { "Virus","Human","Zombie" };
    public static string[] BATTLEEVENT_SheetNames = { "Package" };
    public static string[] IAP_SheetNames = { "Item" };
    public static string[] LANGUAGE_SheetNames = { "Localization" };
    public static string[] LOOT_SheetNames = { "Package" };
    public static string[] MISSION_SheetNames = { "Parameter" };
    public static string[] MODEL_SheetNames = { "Virus", "Human", "Zombie" };
    public static string[] SPECIALABILITY_SheetNames = { "Ability" };
    public static string[] UNLOCK_SheetNames = { "UnlockMission" };
    public static string[] INGAMEEVENT_SheetNames = { "InGameEvents" };
    public static string[] SPLIST_SheetNames = { "Infection","Damage" };
	public static string[] CARDS_SheetNames = { "Card" };


    //读BattleEvent.xlsx表
    public static List<BattleEvent_Sheet> SelectEventTable(int tableId)
    {
        DataRowCollection collect = ExcelAccess.ReadExcel(BATTLEEVENT + ".xlsx", BATTLEEVENT_SheetNames[tableId - 1]);
        List<BattleEvent_Sheet> array = new List<BattleEvent_Sheet>();

        for (int i = 1; i < collect.Count; i++)
        {
            if (collect[i][1].ToString() == "") continue;

            BattleEvent_Sheet Package = new BattleEvent_Sheet();
            Package.EventPackageID = collect[i][0].ToString();
            Package.EventPackageName = collect[i][1].ToString();
            Package.EventID = collect[i][2].ToString();
            Package.EventValue = collect[i][3].ToString();
            Package.Weight = collect[i][4].ToString();

            array.Add(Package);
        }
        return array;
    }

    //读BattleStrategy.xlsx表
    public static List<BattleStrategy_Sheet> SelectStrategyTable(int tableId)
    {
        DataRowCollection collect = ExcelAccess.ReadExcel(BATTLESTRATEGY + ".xlsx", BATTLESTRATEGY_SheetNames[tableId-1]);
        List<BattleStrategy_Sheet> array = new List<BattleStrategy_Sheet>();

        for (int i = 1; i < collect.Count; i++)
        {
            if (collect[i][1].ToString() == "") continue;

            BattleStrategy_Sheet strategy = new BattleStrategy_Sheet();
            strategy.GeneID = collect[i][0].ToString();
            strategy.StrategyID = collect[i][1].ToString();
            strategy.BoardID = collect[i][2].ToString();
            strategy.EventID = collect[i][3].ToString();
            strategy.Row = collect[i][4].ToString();
            strategy.Column = collect[i][5].ToString();
            strategy.FP1 = collect[i][6].ToString();
            strategy.FP2 = collect[i][7].ToString();
            strategy.UnlockCost_A = collect[i][8].ToString();
            strategy.UnlockCost_B = collect[i][9].ToString();

            array.Add(strategy);
        }
        return array;
    }

    //读DNAUp.xlsx表
    public static List<DNAUp_Sheet> SelectDNAUpTable(int tableId)
    {
        DataRowCollection collect = ExcelAccess.ReadExcel(DNAUP + ".xlsx", DNAUP_SheetNames[tableId - 1]);
        List<DNAUp_Sheet> array = new List<DNAUp_Sheet>();

        for (int i = 1; i < collect.Count; i++)
        {
            if (collect[i][1].ToString() == "") continue;

            DNAUp_Sheet DNAUp = new DNAUp_Sheet();
            DNAUp.ID = collect[i][0].ToString();
            DNAUp.Type = collect[i][1].ToString();
            DNAUp.Name = collect[i][2].ToString();
            DNAUp.Value1 = collect[i][3].ToString();
            DNAUp.Value1_Add = collect[i][4].ToString();
            DNAUp.Value2 = collect[i][5].ToString();
            DNAUp.Value2_Add = collect[i][6].ToString();
            DNAUp.Value3 = collect[i][7].ToString();
            DNAUp.Value3_Add = collect[i][8].ToString();
            DNAUp.GoldCost = collect[i][9].ToString();
            DNAUp.GoldParam_1 = collect[i][10].ToString();
            DNAUp.GoldParam_2 = collect[i][11].ToString();
            DNAUp.GemCost = collect[i][12].ToString();
            DNAUp.GemParam_1 = collect[i][13].ToString();
            DNAUp.GemParam_2 = collect[i][14].ToString();

            array.Add(DNAUp);
        }
        return array;
    }

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

    //读BattleEvent.xlsx表
    public static List<InGameEvent_Sheet> SelectInGameEventTable(int tableId)
    {
        DataRowCollection collect = ExcelAccess.ReadExcel(INGAMEEVENT + ".xlsx", INGAMEEVENT_SheetNames[tableId - 1]);
        List<InGameEvent_Sheet> array = new List<InGameEvent_Sheet>();

        for (int i = 1; i < collect.Count; i++)
        {
            if (collect[i][1].ToString() == "") continue;

            InGameEvent_Sheet Package = new InGameEvent_Sheet();
            Package.EventID = collect[i][0].ToString();
            Package.Type = collect[i][1].ToString();
            Package.TypeParam = collect[i][2].ToString();
            Package.FieldName = collect[i][3].ToString();
            Package.EventType = collect[i][4].ToString();
            Package.Value = collect[i][5].ToString();
            Package.SkillIconName = collect[i][6].ToString();
            Package.DesID = collect[i][7].ToString();
            Package.UpgradeEffectID = collect[i][8].ToString();

            array.Add(Package);
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

    //读Model.xlsx的Virus表
    public static List<Virus_Sheet> SelectModel_VirusTable(int tableId)
    {
        DataRowCollection collect = ExcelAccess.ReadExcel(MODEL + ".xlsx", MODEL_SheetNames[tableId - 1]);
        List<Virus_Sheet> array = new List<Virus_Sheet>();

        for (int i = 1; i < collect.Count; i++)
        {
            if (collect[i][1].ToString() == "") continue;

            Virus_Sheet Virus = new Virus_Sheet();
            Virus.VirusID = collect[i][0].ToString();
            Virus.InfectSpeed = collect[i][1].ToString();
            Virus.InfectHuman_1 = collect[i][2].ToString();
            Virus.InfectHuman_2 = collect[i][3].ToString();
            Virus.InfectHuman_3 = collect[i][4].ToString();
            Virus.InfectHuman_4 = collect[i][5].ToString();
            Virus.InfectHuman_5 = collect[i][6].ToString();
            Virus.InfectBlock_Climate_1 = collect[i][7].ToString();
            Virus.InfectBlock_Climate_2 = collect[i][8].ToString();
            Virus.InfectBlock_Climate_3 = collect[i][9].ToString();
            Virus.InfectBlock_Envi_1 = collect[i][10].ToString();
            Virus.InfectBlock_Envi_2 = collect[i][11].ToString();
            Virus.InfectBlock_Envi_3 = collect[i][12].ToString();
            Virus.CommunicateRate = collect[i][13].ToString();
            Virus.CommunicateHuman_1 = collect[i][14].ToString();
            Virus.CommunicateHuman_2 = collect[i][15].ToString();
            Virus.CommunicateHuman_3 = collect[i][16].ToString();
            Virus.CommunicateHuman_4 = collect[i][17].ToString();
            Virus.CommunicateHuman_5 = collect[i][18].ToString();
            Virus.CommunicateBlock_Climate_1 = collect[i][19].ToString();
            Virus.CommunicateBlock_Climate_2 = collect[i][20].ToString();
            Virus.CommunicateBlock_Climate_3 = collect[i][21].ToString();
            Virus.CommunicateBlock_Envi_1 = collect[i][22].ToString();
            Virus.CommunicateBlock_Envi_2 = collect[i][23].ToString();
            Virus.CommunicateBlock_Envi_3 = collect[i][24].ToString();
            Virus.InitialSP = collect[i][25].ToString();
            Virus.UnlockNum = collect[i][26].ToString();
            Virus.Name = collect[i][27].ToString();
            Virus.Des = collect[i][28].ToString();
            Virus.Res = collect[i][29].ToString();
            Virus.StrategyID = collect[i][30].ToString();
            Virus.Medi_Start = collect[i][31].ToString();
            Virus.Medi_Work = collect[i][32].ToString();
            Virus.Medi_Spd = collect[i][33].ToString();
            Virus.CommunicationThreshold = collect[i][34].ToString();

            array.Add(Virus);
        }
        return array;
    }

    //读Model.xlsx的Human表
    public static List<Human_Sheet> SelectModel_HumanTable(int tableId)
    {
        DataRowCollection collect = ExcelAccess.ReadExcel(MODEL + ".xlsx", MODEL_SheetNames[tableId - 1]);
        List<Human_Sheet> array = new List<Human_Sheet>();

        for (int i = 1; i < collect.Count; i++)
        {
            if (collect[i][1].ToString() == "") continue;

            Human_Sheet Human = new Human_Sheet();
            Human.HumanID = collect[i][0].ToString();
            Human.MaxHP = collect[i][1].ToString();
            Human.MaxInfection = collect[i][2].ToString();
            Human.Atk = collect[i][3].ToString();
            Human.Heal = collect[i][4].ToString();
            Human.Def = collect[i][5].ToString();
            Human.Cure = collect[i][6].ToString();
            Human.InfectShield = collect[i][7].ToString();
            Human.InfectionAnti = collect[i][8].ToString();
            Human.CommunicationAnti = collect[i][9].ToString();
            Human.HPHealing = collect[i][10].ToString();
            Human.ClimateBoost_1 = collect[i][11].ToString();
            Human.ClimateBoost_2 = collect[i][12].ToString();
            Human.ClimateBoost_3 = collect[i][13].ToString();
            Human.EnviBoost_1 = collect[i][14].ToString();
            Human.EnviBoost_2 = collect[i][15].ToString();
            Human.EnviBoost_3 = collect[i][16].ToString();
            Human.Weight = collect[i][17].ToString();
			Human.ResearchStart = collect[i][18].ToString();
			Human.AttackInterval = collect[i][19].ToString();
            Human.Name = collect[i][20].ToString();
            Human.Res = collect[i][21].ToString();
            Human.SkillID = collect[i][22].ToString();

            array.Add(Human);
        }
        return array;
    }

    //读Model.xlsx的Zombie表
    public static List<Zombie_Sheet> SelectModel_ZombieTable(int tableId)
    {
        DataRowCollection collect = ExcelAccess.ReadExcel(MODEL + ".xlsx", MODEL_SheetNames[tableId - 1]);
        List<Zombie_Sheet> array = new List<Zombie_Sheet>();

        for (int i = 1; i < collect.Count; i++)
        {
            if (collect[i][1].ToString() == "") continue;

            Zombie_Sheet Zombie = new Zombie_Sheet();
            Zombie.ZombieID = collect[i][0].ToString();
            Zombie.MaxHP = collect[i][1].ToString();
            Zombie.Atk = collect[i][2].ToString();
            Zombie.Heal = collect[i][3].ToString();
            Zombie.Def = collect[i][4].ToString();
            Zombie.Infect = collect[i][5].ToString();
            Zombie.Speed = collect[i][6].ToString();
            Zombie.HPDecay = collect[i][7].ToString();
            Zombie.DrainLife = collect[i][8].ToString();
            Zombie.AbilityID = collect[i][9].ToString();
            Zombie.ClimateBoost_1 = collect[i][10].ToString();
            Zombie.ClimateBoost_2 = collect[i][11].ToString();
            Zombie.ClimateBoost_3 = collect[i][12].ToString();
            Zombie.EnviBoost_1 = collect[i][13].ToString();
            Zombie.EnviBoost_2 = collect[i][14].ToString();
            Zombie.EnviBoost_3 = collect[i][15].ToString();
            Zombie.Weight = collect[i][16].ToString();
			Zombie.AttackInterval = collect[i][17].ToString();
            Zombie.Name = collect[i][18].ToString();
            Zombie.Res = collect[i][19].ToString();
            Zombie.SkillID = collect[i][20].ToString();

            array.Add(Zombie);
        }
        return array;
    }

    //读SpecialAbility.xlsx表
    public static List<SpecialAbility_Sheet> SelectSpeialAbilityTable(int tableId)
    {
        DataRowCollection collect = ExcelAccess.ReadExcel(SPECIALABILITY + ".xlsx", SPECIALABILITY_SheetNames[tableId - 1]);
        List<SpecialAbility_Sheet> array = new List<SpecialAbility_Sheet>();

        for (int i = 1; i < collect.Count; i++)
        {
            if (collect[i][1].ToString() == "") continue;

            SpecialAbility_Sheet Ability = new SpecialAbility_Sheet();
            Ability.ID = collect[i][0].ToString();
            Ability.ResIcon = collect[i][1].ToString();
            Ability.Name = collect[i][2].ToString();
            Ability.Value1 = collect[i][3].ToString();
            Ability.Value1_Add = collect[i][4].ToString();
            Ability.Value2 = collect[i][5].ToString();
            Ability.Value2_Add = collect[i][6].ToString();
            Ability.Value3 = collect[i][7].ToString();
            Ability.Value3_Add = collect[i][8].ToString();
            Ability.DesTextID = collect[i][9].ToString();
            Ability.SEName = collect[i][10].ToString();

            array.Add(Ability);
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

    //读SPList.xlsx表
    public static List<Infection_Sheet> SelectInfectionTable(int tableId)
    {
        DataRowCollection collect = ExcelAccess.ReadExcel(SPLIST + ".xlsx", SPLIST_SheetNames[tableId - 1]);
        List<Infection_Sheet> array = new List<Infection_Sheet>();

        for (int i = 1; i < collect.Count; i++)
        {
            if (collect[i][1].ToString() == "") continue;

            Infection_Sheet spList = new Infection_Sheet();
            spList.TotalInfection = collect[i][0].ToString();
            spList.GainSP = collect[i][1].ToString();

            array.Add(spList);
        }
        return array;
    }

    //读SPList.xlsx表
    public static List<Damage_Sheet> SelectDamageTable(int tableId)
    {
        DataRowCollection collect = ExcelAccess.ReadExcel(SPLIST + ".xlsx", SPLIST_SheetNames[tableId - 1]);
        List<Damage_Sheet> array = new List<Damage_Sheet>();

        for (int i = 1; i < collect.Count; i++)
        {
            if (collect[i][1].ToString() == "") continue;

            Damage_Sheet spList = new Damage_Sheet();
            spList.TotalDamage = collect[i][0].ToString();
            spList.GainSP = collect[i][1].ToString();

            array.Add(spList);
        }
        return array;
    }

	//读Cards.xlsx表

	public static List<Cards_Sheet> SelectCardsTable(int tableId)
	{
		DataRowCollection collect = ReadExcel(CARDS + ".xlsx", CARDS_SheetNames[tableId - 1]);
		List<Cards_Sheet> array = new List<Cards_Sheet>();

		for (int i = 1; i < collect.Count; i++)
		{
			if (collect[i][1].ToString() == "") continue;

			Cards_Sheet card = new Cards_Sheet();
			//
			card.ID = collect[i][0].ToString();
			card.Value = collect[i][1].ToString();
			card.DirectionType = collect[i][2].ToString();
			card.CardType = collect[i][3].ToString();
			card.ImgName = collect[i][4].ToString();
			card.Weight_1 = collect[i][5].ToString();
			card.Weight_2 = collect[i][6].ToString();
			card.Weight_3 = collect[i][7].ToString();
			card.Weight_4 = collect[i][8].ToString();
			card.Weight_5 = collect[i][9].ToString();

			array.Add(card);
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

