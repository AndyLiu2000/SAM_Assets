using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DataManager : MonoBehaviour {

    //供外部调取的数据，细化到标签页
    public static List<IAP_Sheet> IAP_Item;

    public static List<Language_Sheet> Language_Localization;

    public static List<Loot_Sheet> Loot_Package;

    public static List<Mission_Sheet> Mission_Parameter;

    public static List<UnlockMission_Sheet> Unlock_UnlockMission;


    //内部读数据需要用到的属性
    private static string[] assetNames = { "IAP","Language", "Loot", "Mission", "Unlock"};

    public static void ReadDatas()
    {

        IAP_Item = (Resources.Load<Object>("Datas/" + assetNames[3]) as IAP).Item;

        Language_Localization = (Resources.Load<Object>("Datas/" + assetNames[5]) as Language).Localization;

        Loot_Package = (Resources.Load<Object>("Datas/" + assetNames[6]) as Loot).Package;

        Mission_Parameter = (Resources.Load<Object>("Datas/" + assetNames[7]) as Mission).Parameter;

        Unlock_UnlockMission = (Resources.Load<Object>("Datas/" + assetNames[10]) as Unlock).UnlockMission;
    }

}
