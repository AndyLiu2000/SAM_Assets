using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DataManager : MonoBehaviour {

    //供外部调取的数据，细化到标签页
    public static List<BattleEvent_Sheet> BattleEvent_Package;

    public static List<BattleStrategy_Sheet> BattleStrategy_Strategy;

    public static List<DNAUp_Sheet> DNAUp_Virus;
    public static List<DNAUp_Sheet> DNAUp_Human;
    public static List<DNAUp_Sheet> DNAUp_Zombie;

    public static List<IAP_Sheet> IAP_Item;

    public static List<InGameEvent_Sheet> InGameEvent_InGameEvents;

    public static List<Language_Sheet> Language_Localization;

    public static List<Loot_Sheet> Loot_Package;

    public static List<Mission_Sheet> Mission_Parameter;

    public static List<Virus_Sheet> Model_Virus;
    public static List<Human_Sheet> Model_Human;
    public static List<Zombie_Sheet> Model_Zombie;

    public static List<SpecialAbility_Sheet> SpecialAbility_Ability;

    public static List<UnlockMission_Sheet> Unlock_UnlockMission;

    public static List<Infection_Sheet> InfectionSheet;
    public static List<Damage_Sheet> DamageSheet;

	public static IList<Cards_Sheet> Cards_Card;

    //内部读数据需要用到的属性
    private static string[] assetNames = { "BattleEvent","BattleStrategy", "DNAUp", "IAP","InGameEvent", "Language", "Loot", "Mission", "Model", "SpecialAbility", "Unlock","SPList","Cards" };

    public static void ReadDatas()
    {
        BattleEvent_Package = (Resources.Load<Object>("Datas/" + assetNames[0]) as BattleEvent).Package;

        BattleStrategy_Strategy = (Resources.Load<Object>("Datas/" + assetNames[1]) as BattleStrategy).Strategy;

        DNAUp_Virus = (Resources.Load<Object>("Datas/" + assetNames[2]) as DNAUp).Virus;
        DNAUp_Human = (Resources.Load<Object>("Datas/" + assetNames[2]) as DNAUp).Human;
        DNAUp_Zombie = (Resources.Load<Object>("Datas/" + assetNames[2]) as DNAUp).Zombie;

        IAP_Item = (Resources.Load<Object>("Datas/" + assetNames[3]) as IAP).Item;

        InGameEvent_InGameEvents = (Resources.Load<Object>("Datas/" + assetNames[4]) as InGameEvent).InGameEvents;

        Language_Localization = (Resources.Load<Object>("Datas/" + assetNames[5]) as Language).Localization;

        Loot_Package = (Resources.Load<Object>("Datas/" + assetNames[6]) as Loot).Package;

        Mission_Parameter = (Resources.Load<Object>("Datas/" + assetNames[7]) as Mission).Parameter;

        Model_Virus = (Resources.Load<Object>("Datas/" + assetNames[8]) as Model).Virus_Sheet;
        Model_Human = (Resources.Load<Object>("Datas/" + assetNames[8]) as Model).Human_Sheet;
        Model_Zombie = (Resources.Load<Object>("Datas/" + assetNames[8]) as Model).Zombie_Sheet;

        SpecialAbility_Ability = (Resources.Load<Object>("Datas/" + assetNames[9]) as SpecialAbility).Ability;

        Unlock_UnlockMission = (Resources.Load<Object>("Datas/" + assetNames[10]) as Unlock).UnlockMission;

        InfectionSheet = (Resources.Load<Object>("Datas/" + assetNames[11]) as SPList).InfectionSheet;
        DamageSheet = (Resources.Load<Object>("Datas/" + assetNames[11]) as SPList).DamageSheet;

		Cards_Card = (Resources.Load<Object>("Datas/" + assetNames[12]) as Cards_Excel).Card;
    }

}
