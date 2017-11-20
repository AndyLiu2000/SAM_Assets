using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using System.IO;

public class BuildAssetBundle : Editor {

    //在编辑器模式下生成asset文件，供正式环境读取
    [MenuItem("Assets/Create ResourceData")]
    public static void ExcuteBuild()
    {
        //创建BattleEvent.asset
        BattleEvent holder3 = ScriptableObject.CreateInstance<BattleEvent>();

        holder3.Package = ExcelAccess.SelectEventTable(1);

        AssetDatabase.CreateAsset(holder3, HolderPath(ExcelAccess.BATTLEEVENT));
        AssetImporter import3 = AssetImporter.GetAtPath(HolderPath(ExcelAccess.BATTLEEVENT));
        import3.assetBundleName = ExcelAccess.BATTLEEVENT;

        //创建BattleStrategy.asset
        BattleStrategy holder1 = ScriptableObject.CreateInstance<BattleStrategy>();

        holder1.Strategy = ExcelAccess.SelectStrategyTable(1);

        AssetDatabase.CreateAsset(holder1, HolderPath(ExcelAccess.BATTLESTRATEGY));
        AssetImporter import1 = AssetImporter.GetAtPath(HolderPath(ExcelAccess.BATTLESTRATEGY));
        import1.assetBundleName = ExcelAccess.BATTLESTRATEGY;

        //创建DNAUp.asset
        DNAUp holder2 = ScriptableObject.CreateInstance<DNAUp>();

        holder2.Virus = ExcelAccess.SelectDNAUpTable(1);
        holder2.Human = ExcelAccess.SelectDNAUpTable(2);
        holder2.Zombie = ExcelAccess.SelectDNAUpTable(3);

        AssetDatabase.CreateAsset(holder2, HolderPath(ExcelAccess.DNAUP));
        AssetImporter import2 = AssetImporter.GetAtPath(HolderPath(ExcelAccess.DNAUP));
        import2.assetBundleName = ExcelAccess.DNAUP;

        //创建IAP.asset
        IAP holder4 = ScriptableObject.CreateInstance<IAP>();

        holder4.Item = ExcelAccess.SelectIAPTable(1);

        AssetDatabase.CreateAsset(holder4, HolderPath(ExcelAccess.IAP));
        AssetImporter import4 = AssetImporter.GetAtPath(HolderPath(ExcelAccess.IAP));
        import4.assetBundleName = ExcelAccess.IAP;

        //创建InGameEvent.asset
        InGameEvent holder5 = ScriptableObject.CreateInstance<InGameEvent>();

        holder5.InGameEvents = ExcelAccess.SelectInGameEventTable(1);

        AssetDatabase.CreateAsset(holder5, HolderPath(ExcelAccess.INGAMEEVENT));
        AssetImporter import5 = AssetImporter.GetAtPath(HolderPath(ExcelAccess.INGAMEEVENT));
        import5.assetBundleName = ExcelAccess.INGAMEEVENT;

        //创建Language.asset
        Language holder6 = ScriptableObject.CreateInstance<Language>();

        holder6.Localization = ExcelAccess.SelectLanguageTable(1);

        AssetDatabase.CreateAsset(holder6, HolderPath(ExcelAccess.LANGUAGE));
        AssetImporter import6 = AssetImporter.GetAtPath(HolderPath(ExcelAccess.LANGUAGE));
        import6.assetBundleName = ExcelAccess.LANGUAGE;

        //创建Loot.asset
        Loot holder7 = ScriptableObject.CreateInstance<Loot>();

        holder7.Package = ExcelAccess.SelectLootTable(1);

        AssetDatabase.CreateAsset(holder7, HolderPath(ExcelAccess.LOOT));
        AssetImporter import7 = AssetImporter.GetAtPath(HolderPath(ExcelAccess.LOOT));
        import7.assetBundleName = ExcelAccess.LOOT;

        //创建Mission.asset
        Mission holder8 = ScriptableObject.CreateInstance<Mission>();

        holder8.Parameter = ExcelAccess.SelectMissionTable(1);

        AssetDatabase.CreateAsset(holder8, HolderPath(ExcelAccess.MISSION));
        AssetImporter import8 = AssetImporter.GetAtPath(HolderPath(ExcelAccess.MISSION));
        import8.assetBundleName = ExcelAccess.MISSION;

        //创建Model.asset
        Model holder9 = ScriptableObject.CreateInstance<Model>();

        holder9.Virus_Sheet = ExcelAccess.SelectModel_VirusTable(1);
        holder9.Human_Sheet = ExcelAccess.SelectModel_HumanTable(2);
        holder9.Zombie_Sheet = ExcelAccess.SelectModel_ZombieTable(3);

        AssetDatabase.CreateAsset(holder9, HolderPath(ExcelAccess.MODEL));
        AssetImporter import9 = AssetImporter.GetAtPath(HolderPath(ExcelAccess.MODEL));
        import9.assetBundleName = ExcelAccess.MODEL;

        //创建SpecialAbility.asset
        SpecialAbility holder10 = ScriptableObject.CreateInstance<SpecialAbility>();

        holder10.Ability = ExcelAccess.SelectSpeialAbilityTable(1);

        AssetDatabase.CreateAsset(holder10, HolderPath(ExcelAccess.SPECIALABILITY));
        AssetImporter import10 = AssetImporter.GetAtPath(HolderPath(ExcelAccess.SPECIALABILITY));
        import10.assetBundleName = ExcelAccess.SPECIALABILITY;

        //创建Unlock.asset
        Unlock holder11 = ScriptableObject.CreateInstance<Unlock>();

        holder11.UnlockMission = ExcelAccess.SelectUnlockTable(1);

        AssetDatabase.CreateAsset(holder11, HolderPath(ExcelAccess.UNLOCK));
        AssetImporter import11 = AssetImporter.GetAtPath(HolderPath(ExcelAccess.UNLOCK));
        import11.assetBundleName = ExcelAccess.UNLOCK;

        //创建SPList.asset
        SPList holder12 = ScriptableObject.CreateInstance<SPList>();

        holder12.InfectionSheet = ExcelAccess.SelectInfectionTable(1);
        holder12.DamageSheet = ExcelAccess.SelectDamageTable(2);

        AssetDatabase.CreateAsset(holder12, HolderPath(ExcelAccess.SPLIST));
        AssetImporter import12 = AssetImporter.GetAtPath(HolderPath(ExcelAccess.SPLIST));
        import12.assetBundleName = ExcelAccess.SPLIST;

		//Create Cards.asset
		Cards_Excel holder13 = ScriptableObject.CreateInstance<Cards_Excel>();

		holder13.Card = ExcelAccess.SelectCardsTable(1);

		AssetDatabase.CreateAsset(holder13, HolderPath(ExcelAccess.CARDS));
		AssetImporter import13 = AssetImporter.GetAtPath(HolderPath(ExcelAccess.CARDS));
		import13.assetBundleName = ExcelAccess.CARDS;

        Debug.Log("BuildAsset Success!");
    }

    public static string HolderPath(string holderName)
    {
        return "Assets/Resources/Datas/" + holderName + ".asset";
    }
}
