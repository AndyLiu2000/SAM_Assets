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
        IAP holder1 = ScriptableObject.CreateInstance<IAP>();

        holder1.Item = ExcelAccess.SelectIAPTable(1);

        AssetDatabase.CreateAsset(holder1, HolderPath(ExcelAccess.IAP));
        AssetImporter import4 = AssetImporter.GetAtPath(HolderPath(ExcelAccess.IAP));
        import4.assetBundleName = ExcelAccess.IAP;

        //创建Language.asset
        Language holder2 = ScriptableObject.CreateInstance<Language>();

        holder2.Localization = ExcelAccess.SelectLanguageTable(1);

        AssetDatabase.CreateAsset(holder2, HolderPath(ExcelAccess.LANGUAGE));
        AssetImporter import6 = AssetImporter.GetAtPath(HolderPath(ExcelAccess.LANGUAGE));
        import6.assetBundleName = ExcelAccess.LANGUAGE;

        //创建Loot.asset
        Loot holder3 = ScriptableObject.CreateInstance<Loot>();

        holder3.Package = ExcelAccess.SelectLootTable(1);

        AssetDatabase.CreateAsset(holder3, HolderPath(ExcelAccess.LOOT));
        AssetImporter import7 = AssetImporter.GetAtPath(HolderPath(ExcelAccess.LOOT));
        import7.assetBundleName = ExcelAccess.LOOT;

        //创建Mission.asset
        Mission holder4 = ScriptableObject.CreateInstance<Mission>();

        holder4.Parameter = ExcelAccess.SelectMissionTable(1);

        AssetDatabase.CreateAsset(holder4, HolderPath(ExcelAccess.MISSION));
        AssetImporter import8 = AssetImporter.GetAtPath(HolderPath(ExcelAccess.MISSION));
        import8.assetBundleName = ExcelAccess.MISSION;

        //创建Unlock.asset
        Unlock holder5 = ScriptableObject.CreateInstance<Unlock>();

        holder5.UnlockMission = ExcelAccess.SelectUnlockTable(1);

        AssetDatabase.CreateAsset(holder5, HolderPath(ExcelAccess.UNLOCK));
        AssetImporter import11 = AssetImporter.GetAtPath(HolderPath(ExcelAccess.UNLOCK));
        import11.assetBundleName = ExcelAccess.UNLOCK;

        Debug.Log("BuildAsset Success!");
    }

    public static string HolderPath(string holderName)
    {
        return "Assets/Resources/Datas/" + holderName + ".asset";
    }
}
