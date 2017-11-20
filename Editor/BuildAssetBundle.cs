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

        //创建IAP.asset
        IAP holder4 = ScriptableObject.CreateInstance<IAP>();

        holder4.Item = ExcelAccess.SelectIAPTable(1);

        AssetDatabase.CreateAsset(holder4, HolderPath(ExcelAccess.IAP));
        AssetImporter import4 = AssetImporter.GetAtPath(HolderPath(ExcelAccess.IAP));
        import4.assetBundleName = ExcelAccess.IAP;

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

        //创建Unlock.asset
        Unlock holder11 = ScriptableObject.CreateInstance<Unlock>();

        holder11.UnlockMission = ExcelAccess.SelectUnlockTable(1);

        AssetDatabase.CreateAsset(holder11, HolderPath(ExcelAccess.UNLOCK));
        AssetImporter import11 = AssetImporter.GetAtPath(HolderPath(ExcelAccess.UNLOCK));
        import11.assetBundleName = ExcelAccess.UNLOCK;

        Debug.Log("BuildAsset Success!");
    }

    public static string HolderPath(string holderName)
    {
        return "Assets/Resources/Datas/" + holderName + ".asset";
    }
}
