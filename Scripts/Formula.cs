using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Formula {

    //界面跳出的表现方法

    public static void UI_IsVisible(GameObject ui, bool isVisible)
    {
        if (isVisible)
        {
            ui.transform.localPosition = new Vector3(0, 0);
        }
        else
        {
            ui.transform.localPosition = new Vector3(10000, 10000);
        }
    }

    public static void Btn_IsVisible(GameObject btn, bool isVisible)
    {
        if (isVisible)
        {
            btn.transform.localScale = new Vector3(1, 1);
        }
        else
        {
            btn.transform.localScale = new Vector3(0, 0);
        }
    }

    //属性计算
    //DNAUP属性计算
    public static int FieldNameToValue(string fieldName, List<DNAUp_Sheet> dnaSheet, List<U_DNA> useData)
    {
        int baseVal_1 = 0;
        int lvVal_1 = 0;
        int baseVal_2 = 0;
        int lvVal_2 = 0;
        int baseVal_3 = 0;
        int lvVal_3 = 0;
        int sum = 0;

        foreach (DNAUp_Sheet sheetData in dnaSheet)
        {
            if (fieldName == sheetData.Type)
            {
                int id = int.Parse(sheetData.ID);
                int lv = useData[id - 1].Lv;

                baseVal_1 = int.Parse(sheetData.Value1);
                lvVal_1 = int.Parse(sheetData.Value1_Add);
                baseVal_2 = int.Parse(sheetData.Value2);
                lvVal_2 = int.Parse(sheetData.Value2_Add);
                baseVal_3 = int.Parse(sheetData.Value3);
                lvVal_3 = int.Parse(sheetData.Value3_Add);

                //临时公式
                sum += baseVal_1 + lvVal_1 * lv + baseVal_2 + lvVal_2 * lv + baseVal_3 + lvVal_3 * lv;
            }
        }

        return sum;
    }

    public static string ClimateIcon(ref UISprite sprite,Climate clim)
    {
        switch (clim)
        {
            case Climate.Dry:
                return "icon_longzhu_1";
            case Climate.Wet:
                return "icon_longzhu_2";
            case Climate.Normal:
                return "icon_longzhu_3";
            default:
                return "";
        }
    }

    public static string EnviIcon(ref UISprite sprite, Environment envi)
    {
        switch (envi)
        {
            case Environment.Hot:
                return "icon_gem_10";
            case Environment.Cold:
                return "icon_gem_20";
            case Environment.Balance:
                return "icon_gem_30";
            default:
                return "";
        }
    }

    //升级消耗计算

    //掉落计算
    public static List<Loot_Sheet> Loot(string lootID)
    {
        //本lootid的所有对象
        List<Loot_Sheet> package = new List<Loot_Sheet>();
        //最终掉落对象集合
        List<Loot_Sheet> loot = new List<Loot_Sheet>();
        //参与权重计算的对象集合
        List<Loot_Sheet> weightLoot = new List<Loot_Sheet>();

        //把同一个lootId的对象添加到一个集合中
        foreach (Loot_Sheet l in DataManager.Loot_Package)
        {
            if(l.LootPackageID == lootID)
            {
                package.Add(l);
            }
        }
        
        for(int i = 0; i < package.Count; i++)
        {
            //0则必然掉落
            if (package[i].Weight == "0")
            {
                loot.Add(package[i]);
            }
            else
            {
                weightLoot.Add(package[i]);
            }
        }

        //权重集合中还有对象时，才计算权重
        if (weightLoot.Count > 0)
        {
            //计算权重之和
            int weightSum = 0;
            foreach (Loot_Sheet w in weightLoot)
            {
                weightSum += int.Parse(w.Weight);
            }

            float random = UnityEngine.Random.Range(0.0f, 1.0f);

            for (int i = 0; i < weightLoot.Count; i++)
            {
                float curWeightSum = 0;
                for (int j = 0; j <= i; j++)
                {
                    curWeightSum += float.Parse(weightLoot[j].Weight) / (float)weightSum;
                    //随机数落在掉落区间内
                    if (random < curWeightSum)
                    {
                        loot.Add(weightLoot[j]);
                        goto End;
                    }
                }
            }
        }
        
        End:
        //以上确定了loot内的对象，以下来解析这个掉落
        if (loot.Count > 0)
        {
            ResolveLoot(loot);
        }

        return loot;
        
    }

    static void ResolveLoot(List<Loot_Sheet> loot)
    {
        foreach(Loot_Sheet rl in loot)
        {
            //金币掉落
            if(rl.ItemID == "1")
            {
                GameManager.user.Gold += int.Parse(rl.ItemNum);
                GameManager.SaveData();
            }
            //水晶掉落
            else if (rl.ItemID == "2")
            {
                GameManager.user.Gem += int.Parse(rl.ItemNum);
                GameManager.SaveData();
            }
            //解锁病毒
            else if(rl.ItemID == "3")
            {
                GameManager.user.DB_u_UnlockedViruses.Add(rl.ItemNum);
                GameManager.SaveData();
            }
            //解锁丧尸
            else if(rl.ItemID == "4")
            {
                GameManager.user.DB_u_UnlockedZombies.Add(rl.ItemNum);
                GameManager.SaveData();
            }
            else
            {
                Debug.Log("Nothing Loot");
            }
        }
    }

    //事件随机
    public static void BattleEventRandom(string eventPackageID)
    {
        //本lootid的所有对象
        List<BattleEvent_Sheet> package = new List<BattleEvent_Sheet>();
        //最终掉落对象集合
        List<BattleEvent_Sheet> battleEvents = new List<BattleEvent_Sheet>();
        //参与权重计算的对象集合
        List<BattleEvent_Sheet> weightLoot = new List<BattleEvent_Sheet>();

        //把同一个lootId的对象添加到一个集合中
        foreach (BattleEvent_Sheet be in DataManager.BattleEvent_Package)
        {
            if (be.EventPackageID == eventPackageID)
            {
                package.Add(be);
            }
        }

        for (int i = 0; i < package.Count; i++)
        {
            //0则必然掉落
            if (package[i].Weight == "0")
            {
                battleEvents.Add(package[i]);
            }
            else
            {
                weightLoot.Add(package[i]);
            }
        }

        //权重集合中还有对象时，才计算权重
        if (weightLoot.Count > 0)
        {
            //计算权重之和
            int weightSum = 0;
            foreach (BattleEvent_Sheet w in weightLoot)
            {
                weightSum += int.Parse(w.Weight);
            }

            float random = UnityEngine.Random.Range(0.0f, 1.0f);

            for (int i = 0; i < weightLoot.Count; i++)
            {
                float curWeightSum = 0;
                for (int j = 0; j <= i; j++)
                {
                    curWeightSum += float.Parse(weightLoot[j].Weight) / (float)weightSum;
                    //随机数落在掉落区间内
                    if (random < curWeightSum)
                    {
                        battleEvents.Add(weightLoot[j]);
                        goto End;
                    }
                }
            }
        }

        End:
        //以上确定了battleevent内的对象，以下来解析这个掉落
        ResolveBattleEvent(battleEvents);
    }

    static void ResolveBattleEvent(List<BattleEvent_Sheet> battleEvents)            //需要对每种处理添加表现效果
    {
        foreach (BattleEvent_Sheet be in battleEvents)
        {
            foreach (InGameEvent_Sheet ige in DataManager.InGameEvent_InGameEvents)
            {
                if (be.EventID == ige.EventID)
                {
                    Debug.Log("关卡事件");
                    InGameEvent(ige);

                    if (ige.EventType == "1")
                    {
                        //关卡事件的表现处理
                    }

                    break;
                }
            }
        }
    }

    public static void ResolveBattleUpgrade(string inGameEventID)
    {
        foreach (InGameEvent_Sheet ige in DataManager.InGameEvent_InGameEvents)
        {
            if (inGameEventID == ige.EventID)
            {
                Debug.Log("局内升级");
                InGameEvent(ige);

                break;
            }
        }
    }

    static void InGameEvent(InGameEvent_Sheet ige)
    {
        Battle_C bc = GameObject.Find(GameManager.BATTLE).GetComponent<Battle_C>();

        int fieldValue = 0;
        Type t;
        Type tf;

        switch (ige.Type)
        {
            //病毒处理
            case "1":
                Debug.Log("病毒处理");
                Debug.Log("ige.FieldName = " + ige.FieldName);
                if(ige.FieldName == "Split")
                {
                    List<GameObject> unInfectedHumans = new List<GameObject>();
                    foreach (GameObject h in bc.HumanArray)
                    {
                        if (h.GetComponent<Human>().Infected == false)
                        {
                            //取得所有未感染的人类
                            unInfectedHumans.Add(h);
                        }
                    }

                    if (unInfectedHumans.Count > 0)
                    {
                        Debug.Log("找随机目标使其变为感染状态");
                        GameObject unInfectedHuman = Formula.ListRandomElement(unInfectedHumans);

                        unInfectedHuman.GetComponent<Human>().InfectShield = 0;
                        unInfectedHuman.GetComponent<Human>().Infected = true;
                        Debug.Log("感染一个新人类");
                        bc.InfectNum += 1;
                        bc.SP_Add(Human.INFECT_HUMAN_SP, bc.StrategyBtn, bc.LabelStrategy, false);
                    }

                    return;
                }

                //根据属性名，得到该属性现在的值
                t = typeof(Virus);
                fieldValue = (int)(t.GetField(ige.FieldName).GetValue(bc.CurVirus));
                Debug.Log("fieldValue = " + fieldValue);
                //对这个值进行修改
                bc.CurVirus.GetType().GetField(ige.FieldName).SetValue(bc.CurVirus, fieldValue * (1000 + int.Parse(ige.Value)) / 1000);
                break;
            //人类处理
            case "2":
                //0是作用于全体人类
                Debug.Log("人类处理");
                Debug.Log("ige.FieldName = " + ige.FieldName);

                t = typeof(Human);
                tf = null;
                if (!ige.FieldName.Contains("ClimateBoost") && !ige.FieldName.Contains("EnviBoost"))
                    tf = t.GetField(ige.FieldName).FieldType;

                if (ige.TypeParam == "0")
                {
                    foreach (GameObject h in bc.HumanArray)
                    {
                        Debug.Log("Clim = " + h.GetComponent<Human>().Clim + ", Envi = " + h.GetComponent<Human>().Envi);
                        if (ige.FieldName == "ClimateBoost_1" && h.GetComponent<Human>().Clim == Climate.Dry)
                        {
                            h.GetComponent<Human>().ClimateBoost *= (1000 + int.Parse(ige.Value)) / 1000;
                            h.GetComponent<Human>().UpdateAttributes(bc.MissionID);
                            continue;
                        }
                        if (ige.FieldName == "ClimateBoost_2" && h.GetComponent<Human>().Clim == Climate.Wet)
                        {
                            h.GetComponent<Human>().ClimateBoost *= (1000 + int.Parse(ige.Value)) / 1000;
                            h.GetComponent<Human>().UpdateAttributes(bc.MissionID);
                            continue;
                        }
                        if (ige.FieldName == "ClimateBoost_3" && h.GetComponent<Human>().Clim == Climate.Normal)
                        {
                            h.GetComponent<Human>().ClimateBoost *= (1000 + int.Parse(ige.Value)) / 1000;
                            h.GetComponent<Human>().UpdateAttributes(bc.MissionID);
                            continue;
                        }
                        if (ige.FieldName == "EnviBoost_1" && h.GetComponent<Human>().Envi == Environment.Hot)
                        {
                            h.GetComponent<Human>().EnviBoost *= (1000 + int.Parse(ige.Value)) / 1000;
                            h.GetComponent<Human>().UpdateAttributes(bc.MissionID);
                            continue;
                        }
                        if (ige.FieldName == "EnviBoost_2" && h.GetComponent<Human>().Envi == Environment.Cold)
                        {
                            h.GetComponent<Human>().EnviBoost *= (1000 + int.Parse(ige.Value)) / 1000;
                            h.GetComponent<Human>().UpdateAttributes(bc.MissionID);
                            continue;
                        }
                        if (ige.FieldName == "EnviBoost_3" && h.GetComponent<Human>().Envi == Environment.Balance)
                        {
                            h.GetComponent<Human>().EnviBoost *= (1000 + int.Parse(ige.Value)) / 1000;
                            h.GetComponent<Human>().UpdateAttributes(bc.MissionID);
                            continue;
                        }
                        if (tf == null) continue;
                        fieldValue = Convert.ToInt32(t.GetField(ige.FieldName).GetValue(h.GetComponent<Human>()));
                        t.GetField(ige.FieldName).SetValue(h.GetComponent<Human>(), Convert.ChangeType(fieldValue * (1000 + int.Parse(ige.Value)) / 1000, tf));
                    }
                }
                //非0时作用于该ID的人类
                else
                {
                    foreach (GameObject h in bc.HumanArray)
                    {
                        if (h.GetComponent<Human>().HumanID.ToString() == ige.TypeParam)
                        {
                            if (ige.FieldName == "ClimateBoost_1" && h.GetComponent<Human>().Clim == Climate.Dry)
                            {
                                h.GetComponent<Human>().ClimateBoost *= (1000 + int.Parse(ige.Value)) / 1000;
                                h.GetComponent<Human>().UpdateAttributes(bc.MissionID);
                                continue;
                            }
                            if (ige.FieldName == "ClimateBoost_2" && h.GetComponent<Human>().Clim == Climate.Wet)
                            {
                                h.GetComponent<Human>().ClimateBoost *= (1000 + int.Parse(ige.Value)) / 1000;
                                h.GetComponent<Human>().UpdateAttributes(bc.MissionID);
                                continue;
                            }
                            if (ige.FieldName == "ClimateBoost_3" && h.GetComponent<Human>().Clim == Climate.Normal)
                            {
                                h.GetComponent<Human>().ClimateBoost *= (1000 + int.Parse(ige.Value)) / 1000;
                                h.GetComponent<Human>().UpdateAttributes(bc.MissionID);
                                continue;
                            }
                            if (ige.FieldName == "EnviBoost_1" && h.GetComponent<Human>().Envi == Environment.Hot)
                            {
                                h.GetComponent<Human>().EnviBoost *= (1000 + int.Parse(ige.Value)) / 1000;
                                h.GetComponent<Human>().UpdateAttributes(bc.MissionID);
                                continue;
                            }
                            if (ige.FieldName == "EnviBoost_2" && h.GetComponent<Human>().Envi == Environment.Cold)
                            {
                                h.GetComponent<Human>().EnviBoost *= (1000 + int.Parse(ige.Value)) / 1000;
                                h.GetComponent<Human>().UpdateAttributes(bc.MissionID);
                                continue;
                            }
                            if (ige.FieldName == "EnviBoost_3" && h.GetComponent<Human>().Envi == Environment.Balance)
                            {
                                h.GetComponent<Human>().EnviBoost *= (1000 + int.Parse(ige.Value)) / 1000;
                                h.GetComponent<Human>().UpdateAttributes(bc.MissionID);
                                continue;
                            }
                            if (tf == null) continue;
                            fieldValue = Convert.ToInt32(t.GetField(ige.FieldName).GetValue(h.GetComponent<Human>()));
                            t.GetField(ige.FieldName).SetValue(h.GetComponent<Human>(), Convert.ChangeType(fieldValue * (1000 + int.Parse(ige.Value)) / 1000, tf));
                            break;
                        }
                    }
                }
                break;
            //丧尸处理
            case "3":
                Debug.Log("丧尸处理");
                Debug.Log("ige.FieldName = " + ige.FieldName);

                t = typeof(Zombie);
                tf = null;
                if(!ige.FieldName.Contains("ClimateBoost") && !ige.FieldName.Contains("EnviBoost"))
                    tf = t.GetField(ige.FieldName).FieldType;

                if (ige.TypeParam == "0")
                {
                    foreach (GameObject z in bc.ZombieArray)
                    {
                        if (ige.FieldName == "ClimateBoost_1" && z.GetComponent<Zombie>().Clim == Climate.Dry)
                        {
                            z.GetComponent<Zombie>().ClimateBoost *= (1000 + int.Parse(ige.Value)) / 1000;
                            z.GetComponent<Zombie>().UpdateAttributes();
                            continue;
                        }
                        if (ige.FieldName == "ClimateBoost_2" && z.GetComponent<Zombie>().Clim == Climate.Wet)
                        {
                            z.GetComponent<Zombie>().ClimateBoost *= (1000 + int.Parse(ige.Value)) / 1000;
                            z.GetComponent<Zombie>().UpdateAttributes();
                            continue;
                        }
                        if (ige.FieldName == "ClimateBoost_3" && z.GetComponent<Zombie>().Clim == Climate.Normal)
                        {
                            z.GetComponent<Zombie>().ClimateBoost *= (1000 + int.Parse(ige.Value)) / 1000;
                            z.GetComponent<Zombie>().UpdateAttributes();
                            continue;
                        }
                        if (ige.FieldName == "EnviBoost_1" && z.GetComponent<Zombie>().Envi == Environment.Hot)
                        {
                            z.GetComponent<Zombie>().EnviBoost *= (1000 + int.Parse(ige.Value)) / 1000;
                            z.GetComponent<Zombie>().UpdateAttributes();
                            continue;
                        }
                        if (ige.FieldName == "EnviBoost_2" && z.GetComponent<Zombie>().Envi == Environment.Cold)
                        {
                            z.GetComponent<Zombie>().EnviBoost *= (1000 + int.Parse(ige.Value)) / 1000;
                            z.GetComponent<Zombie>().UpdateAttributes();
                            continue;
                        }
                        if (ige.FieldName == "EnviBoost_3" && z.GetComponent<Zombie>().Envi == Environment.Balance)
                        {
                            z.GetComponent<Zombie>().EnviBoost *= (1000 + int.Parse(ige.Value)) / 1000;
                            z.GetComponent<Zombie>().UpdateAttributes();
                            continue;
                        }
                        if (tf == null) continue;
                        fieldValue = Convert.ToInt32(t.GetField(ige.FieldName).GetValue(z.GetComponent<Zombie>()));
                        t.GetField(ige.FieldName).SetValue(z.GetComponent<Zombie>(), Convert.ChangeType(fieldValue * (1000 + int.Parse(ige.Value)) / 1000, tf));
                    }
                }
                //非0时作用于该ID的丧尸
                else
                {
                    foreach (GameObject z in bc.ZombieArray)
                    {
                        if (z.GetComponent<Zombie>().ZombieID.ToString() == ige.TypeParam)
                        {
                            if (ige.FieldName == "ClimateBoost_1" && z.GetComponent<Zombie>().Clim == Climate.Dry)
                            {
                                z.GetComponent<Zombie>().ClimateBoost *= (1000 + int.Parse(ige.Value)) / 1000;
                                z.GetComponent<Zombie>().UpdateAttributes();
                                continue;
                            }
                            if (ige.FieldName == "ClimateBoost_2" && z.GetComponent<Zombie>().Clim == Climate.Wet)
                            {
                                z.GetComponent<Zombie>().ClimateBoost *= (1000 + int.Parse(ige.Value)) / 1000;
                                z.GetComponent<Zombie>().UpdateAttributes();
                                continue;
                            }
                            if (ige.FieldName == "ClimateBoost_3" && z.GetComponent<Zombie>().Clim == Climate.Normal)
                            {
                                z.GetComponent<Zombie>().ClimateBoost *= (1000 + int.Parse(ige.Value)) / 1000;
                                z.GetComponent<Zombie>().UpdateAttributes();
                                continue;
                            }
                            if (ige.FieldName == "EnviBoost_1" && z.GetComponent<Zombie>().Envi == Environment.Hot)
                            {
                                z.GetComponent<Zombie>().EnviBoost *= (1000 + int.Parse(ige.Value)) / 1000;
                                z.GetComponent<Zombie>().UpdateAttributes();
                                continue;
                            }
                            if (ige.FieldName == "EnviBoost_2" && z.GetComponent<Zombie>().Envi == Environment.Cold)
                            {
                                z.GetComponent<Zombie>().EnviBoost *= (1000 + int.Parse(ige.Value)) / 1000;
                                z.GetComponent<Zombie>().UpdateAttributes();
                                continue;
                            }
                            if (ige.FieldName == "EnviBoost_3" && z.GetComponent<Zombie>().Envi == Environment.Balance)
                            {
                                z.GetComponent<Zombie>().EnviBoost *= (1000 + int.Parse(ige.Value)) / 1000;
                                z.GetComponent<Zombie>().UpdateAttributes();
                                break;
                            }
                            if (tf == null) continue;
                            fieldValue = Convert.ToInt32(t.GetField(ige.FieldName).GetValue(z.GetComponent<Zombie>()));
                            t.GetField(ige.FieldName).SetValue(z.GetComponent<Zombie>(), Convert.ChangeType(fieldValue * (1000 + int.Parse(ige.Value)) / 1000, tf));
                            break;
                        }
                    }
                }
                break;
            default:
                break;
        }
    }

    //需要消耗的策略点计算
    public static int StrategyPointCal(Gene gene)
    {
        Battle_C bc = GameObject.Find(GameManager.BATTLE).GetComponent<Battle_C>();
        int extraSP = gene.UnlockCost_A;
        ArrayList upgradedGenes = new ArrayList();
        //根据基因的BoardId，找出已升级的基因
        switch (gene.BoardID)
        {
            case "1":
                upgradedGenes = bc.GetComponent<Battle_C>().UpgradedGenes_Virus;
                break;
            case "2":
                upgradedGenes = bc.GetComponent<Battle_C>().UpgradedGenes_Human;
                break;
            case "3":
                upgradedGenes = bc.GetComponent<Battle_C>().UpgradedGenes_Zombie;
                break;
            default:
                break;
        }

        foreach(Gene g in upgradedGenes)
        {
            extraSP += g.UnlockCost_B;
        }
        return extraSP;
    }

    public static void ChangeButtonDisable(GameObject button)
    {
        button.GetComponent<UIButton>().state = UIButton.State.Disabled;
        button.GetComponent<BoxCollider>().enabled = false;
    }

    public static void ChangeButtonEnable(GameObject button)
    {
        button.GetComponent<UIButton>().state = UIButton.State.Normal;
        button.GetComponent<BoxCollider>().enabled = true;
    }

    public static int FarthestMission(int virusID)
    {
        List<U_MissionFlag> missions = new List<U_MissionFlag>();
        foreach (U_MissionFlag m in GameManager.user.DB_u_mf)
        {
            if (m.VirusID == virusID)
            {
                missions.Add(m);
            }
        }

        for (int i = 0; i < missions.Count; i++)
        {
            if (missions[i].Flag == false)
            {
                return i+1;
            }
        }
        return missions.Count;
    }

    public static int RandomHuman()
    {
        //计算权重之和
        int weightSum = 0;
        for(int i = 1; i < DataManager.Model_Human.Count; i++)
        {
            weightSum += int.Parse(DataManager.Model_Human[i].Weight);
        }

        float random = UnityEngine.Random.Range(0.0f, 1.0f);

        for (int i = 1; i < DataManager.Model_Human.Count; i++)
        {
            float curWeightSum = 0;
            for (int j = 1; j <= i; j++)
            {
				curWeightSum += float.Parse (DataManager.Model_Human [j].Weight) / weightSum;
                //随机数落在掉落区间内
                if (random < curWeightSum)
                {
                    return j;
                }
            }
        }

        return DataManager.Model_Human.Count - 1;
    }

    public static int RandomZombie()
    {
        //本次随机的所有可用丧尸对象集合
        List<Zombie_Sheet> zombies = new List<Zombie_Sheet>();

        //把同一个lootId的对象添加到一个集合中
        foreach (Zombie_Sheet z in DataManager.Model_Zombie)
        {
            foreach(string s in GameManager.user.DB_u_UnlockedZombies)
            {
                if (z.ZombieID == s)
                {
                    zombies.Add(z);
                    break;
                }
            }
        }

        //计算权重之和
        int weightSum = 0;
        foreach (Zombie_Sheet w in zombies)
        {
            weightSum += int.Parse(w.Weight);
        }

        float random = UnityEngine.Random.Range(0.0f, 1.0f);

        for (int i = 0; i < zombies.Count; i++)
        {
            float curWeightSum = 0;
            for (int j = 0; j <= i; j++)
            {
                curWeightSum += float.Parse(zombies[j].Weight) / (float)weightSum;
                //随机数落在掉落区间内
                if (random < curWeightSum)
                {
                    return int.Parse(zombies[j].ZombieID);
                }
            }
        }
        return 1;
    }

    public static GameObject ListRandomElement(List<GameObject> list)
    {
        int i = UnityEngine.Random.Range(0, list.Count);
        return list[i];
    }

    public static object ArrayListRandomElement(ArrayList list)
    {
        int i = UnityEngine.Random.Range(0, list.Count);
        return list[i];
    }

	//出牌概率
	public static int Spin(int rotationID)
	{
		//weight objects list
		List<Cards_Sheet> weightLoot = new List<Cards_Sheet>();
		Type t = typeof(Cards_Sheet);

		foreach (Cards_Sheet cs in DataManager.Cards_Card)
		{
			//objects with the same rotationid will be in the same weightloot
			switch (rotationID)
			{
			case 1:
				//will not count into loot if equals to 0
				if (cs.Weight_1 != "0") weightLoot.Add(cs);
				break;
			case 2:
				//will not count into loot if equals to 0
				if (cs.Weight_2 != "0") weightLoot.Add(cs);
				break;
			case 3:
				//will not count into loot if equals to 0
				if (cs.Weight_3 != "0") weightLoot.Add(cs);
				break;
			case 4:
				//will not count into loot if equals to 0
				if (cs.Weight_4 != "0") weightLoot.Add(cs);
				break;
			case 5:
				//will not count into loot if equals to 0
				if (cs.Weight_5 != "0") weightLoot.Add(cs);
				break;
			default:
				break;
			}
		}

		int finalLoot = -1;
		//count weight only if number of objects is greater than 0
		if (weightLoot.Count > 0)
		{
			//sum weight
			int weightSum = 0;
			foreach (Cards_Sheet w in weightLoot)
			{
				switch (rotationID)
				{
				case 1:
					weightSum += int.Parse(w.Weight_1);
					break;
				case 2:
					weightSum += int.Parse(w.Weight_2);
					break;
				case 3:
					weightSum += int.Parse(w.Weight_3);
					break;
				case 4:
					weightSum += int.Parse(w.Weight_4);
					break;
				case 5:
					weightSum += int.Parse(w.Weight_5);
					break;
				default:
					break;
				}
			}

			float random = UnityEngine.Random.Range(0.0f, 1.0f);

			for (int i = 0; i < weightLoot.Count; i++)
			{
				float curWeightSum = 0;
				for (int j = 0; j <= i; j++)
				{
					switch (rotationID)
					{
					case 1:
						curWeightSum += float.Parse(weightLoot[j].Weight_1) / (float)weightSum;
						break;
					case 2:
						curWeightSum += float.Parse(weightLoot[j].Weight_2) / (float)weightSum;
						break;
					case 3:
						curWeightSum += float.Parse(weightLoot[j].Weight_3) / (float)weightSum;
						break;
					case 4:
						curWeightSum += float.Parse(weightLoot[j].Weight_4) / (float)weightSum;
						break;
					case 5:
						curWeightSum += float.Parse(weightLoot[j].Weight_5) / (float)weightSum;
						break;
					default:
						break;
					}
					//random number drops in the area of curWeightSum
					if (random < curWeightSum)
					{
						finalLoot = int.Parse(weightLoot[j].ID);
						goto End;
					}
				}
			}
		}

		End:
		return finalLoot;
	}

	//Line1,2,3 win
	public static int Win1_2_3()
	{
		int WinNum = 0;
		for (int i = 0; i < 3; i++)
		{
			int SameNum = 1;
			for (int j = 0; j < 4; j++)
			{
				if (Casino.ResultSlots[j, i].GetComponent<Slot>().Card.CT == Casino.ResultSlots[j + 1, i].GetComponent<Slot>().Card.CT)
				{
					if (Casino.ResultSlots[j, i].GetComponent<Slot>().Card.DT == Casino.ResultSlots[j + 1, i].GetComponent<Slot>().Card.DT
						&& Casino.ResultSlots[j, i].GetComponent<Slot>().Card.DT == DirectionType.H)
					{
						if (Casino.ResultSlots[j, i].GetComponent<Slot>().Card.Value == Casino.ResultSlots[j + 1, i].GetComponent<Slot>().Card.Value)
						{
							Debug.Log("Line" + i + "Same:" + j);
							SameNum++;
						}
						else break;
					}
					else break;
				}
				else break;
			}

			if (SameNum > 1)
			{
				WinNum += (int)(Casino.ResultSlots[0, i].GetComponent<Slot>().Card.Value * SameNum * Casino.WinRate * Casino.Rank);
				Debug.Log("Line" + i + "Win:");

				Debug.Log("WinNum = " + WinNum);
				for (int k = 0; k < SameNum; k++)
				{
					Casino.ResultSlots[k, i].GetComponent<UISprite>().color = new Color(1, 0, 0);
				}
			}
		}
		return WinNum;
	}

	//Line 4,8 win
	public static int Win4_8(int line1, int index1, int line2, int index2)
	{
		if (Casino.ResultSlots[index1, line1].GetComponent<Slot>().Card.CT == Casino.ResultSlots[index2, line2].GetComponent<Slot>().Card.CT)
		{
			if (Casino.ResultSlots[index1, line1].GetComponent<Slot>().Card.DT == DirectionType.L && Casino.ResultSlots[index2, line2].GetComponent<Slot>().Card.DT == DirectionType.L)
			{
				if (Casino.ResultSlots[index1, line1].GetComponent<Slot>().Card.Value == Casino.ResultSlots[index2, line2].GetComponent<Slot>().Card.Value)
				{
					Debug.Log("Line4 or Line8 Win");
					Casino.ResultSlots[index1, line1].GetComponent<UISprite>().color = new Color(1, 0, 0);
					Casino.ResultSlots[index2, line2].GetComponent<UISprite>().color = new Color(1, 0, 0);
					return (int)(Casino.ResultSlots[index1, line1].GetComponent<Slot>().Card.Value * 2 * Casino.WinRate * Casino.Rank);
				}
			}
		}
		return 0;
	}

	//Line 5,6,7 win
	public static int Win5_6_7(int line1, int index1, int line2, int index2, int line3, int index3)
	{

		if (Casino.ResultSlots[index1, line1].GetComponent<Slot>().Card.CT == Casino.ResultSlots[index2, line2].GetComponent<Slot>().Card.CT
			&& Casino.ResultSlots[index1, line1].GetComponent<Slot>().Card.CT == Casino.ResultSlots[index3, line3].GetComponent<Slot>().Card.CT)
		{
			if (Casino.ResultSlots[index1, line1].GetComponent<Slot>().Card.DT == DirectionType.L && Casino.ResultSlots[index2, line2].GetComponent<Slot>().Card.DT == DirectionType.L
				&& Casino.ResultSlots[index3, line3].GetComponent<Slot>().Card.DT == DirectionType.L)
			{
				if (Casino.ResultSlots[index1, line1].GetComponent<Slot>().Card.Value == Casino.ResultSlots[index2, line2].GetComponent<Slot>().Card.Value
					&& Casino.ResultSlots[index1, line1].GetComponent<Slot>().Card.Value == Casino.ResultSlots[index3, line3].GetComponent<Slot>().Card.Value)
				{
					Debug.Log("Line5 or Line6 or Line7 Win");
					Casino.ResultSlots[index1, line1].GetComponent<UISprite>().color = new Color(1, 0, 0);
					Casino.ResultSlots[index2, line2].GetComponent<UISprite>().color = new Color(1, 0, 0);
					Casino.ResultSlots[index3, line3].GetComponent<UISprite>().color = new Color(1, 0, 0);
					return (int)(Casino.ResultSlots[index1, line1].GetComponent<Slot>().Card.Value * 3 * Casino.WinRate * Casino.Rank);
				}
			}
		}
		return 0;
	}

	//Line 9,13 win
	public static int Win9_13(int line1, int index1, int line2, int index2)
	{

		if (Casino.ResultSlots[index1, line1].GetComponent<Slot>().Card.CT == Casino.ResultSlots[index2, line2].GetComponent<Slot>().Card.CT)
		{
			if (Casino.ResultSlots[index1, line1].GetComponent<Slot>().Card.DT == DirectionType.R && Casino.ResultSlots[index2, line2].GetComponent<Slot>().Card.DT == DirectionType.R)
			{
				if (Casino.ResultSlots[index1, line1].GetComponent<Slot>().Card.Value == Casino.ResultSlots[index2, line2].GetComponent<Slot>().Card.Value)
				{
					Debug.Log("Line9 or Line13 Win");
					Casino.ResultSlots[index1, line1].GetComponent<UISprite>().color = new Color(1, 0, 0);
					Casino.ResultSlots[index2, line2].GetComponent<UISprite>().color = new Color(1, 0, 0);
					return (int)(Casino.ResultSlots[index1, line1].GetComponent<Slot>().Card.Value * 2 * Casino.WinRate * Casino.Rank);
				}
			}
		}
		return 0;
	}

	//Line 10,11,12 win
	public static int Win10_11_12(int line1, int index1, int line2, int index2, int line3, int index3)
	{

		if (Casino.ResultSlots[index1, line1].GetComponent<Slot>().Card.CT == Casino.ResultSlots[index2, line2].GetComponent<Slot>().Card.CT
			&& Casino.ResultSlots[index1, line1].GetComponent<Slot>().Card.CT == Casino.ResultSlots[index3, line3].GetComponent<Slot>().Card.CT)
		{
			if (Casino.ResultSlots[index1, line1].GetComponent<Slot>().Card.DT == DirectionType.R && Casino.ResultSlots[index2, line2].GetComponent<Slot>().Card.DT == DirectionType.R
				&& Casino.ResultSlots[index3, line3].GetComponent<Slot>().Card.DT == DirectionType.R)
			{
				if (Casino.ResultSlots[index1, line1].GetComponent<Slot>().Card.Value == Casino.ResultSlots[index2, line2].GetComponent<Slot>().Card.Value
					&& Casino.ResultSlots[index1, line1].GetComponent<Slot>().Card.Value == Casino.ResultSlots[index3, line3].GetComponent<Slot>().Card.Value)
				{
					Debug.Log("Line10 or Line11 or Line12 Win");
					Casino.ResultSlots[index1, line1].GetComponent<UISprite>().color = new Color(1, 0, 0);
					Casino.ResultSlots[index2, line2].GetComponent<UISprite>().color = new Color(1, 0, 0);
					Casino.ResultSlots[index3, line3].GetComponent<UISprite>().color = new Color(1, 0, 0);
					return (int)(Casino.ResultSlots[index1, line1].GetComponent<Slot>().Card.Value * 3 * Casino.WinRate * Casino.Rank);
				}
			}
		}
		return 0;
	}

	//Line 14 win
	public static int Win14(int line1, int index1, int line2, int index2, int line3, int index3, int line4, int index4, int line5, int index5)
	{
		if (Casino.ResultSlots[index1, line1].GetComponent<Slot>().Card.CT == Casino.ResultSlots[index2, line2].GetComponent<Slot>().Card.CT
			&& Casino.ResultSlots[index1, line1].GetComponent<Slot>().Card.CT == Casino.ResultSlots[index3, line3].GetComponent<Slot>().Card.CT
			&& Casino.ResultSlots[index1, line1].GetComponent<Slot>().Card.CT == Casino.ResultSlots[index4, line4].GetComponent<Slot>().Card.CT
			&& Casino.ResultSlots[index1, line1].GetComponent<Slot>().Card.CT == Casino.ResultSlots[index5, line5].GetComponent<Slot>().Card.CT)
		{
			if (Casino.ResultSlots[index1, line1].GetComponent<Slot>().Card.DT == DirectionType.L
				&& Casino.ResultSlots[index2, line2].GetComponent<Slot>().Card.DT == DirectionType.L
				&& Casino.ResultSlots[index3, line3].GetComponent<Slot>().Card.DT == DirectionType.MD
				&& Casino.ResultSlots[index4, line4].GetComponent<Slot>().Card.DT == DirectionType.R
				&& Casino.ResultSlots[index5, line5].GetComponent<Slot>().Card.DT == DirectionType.R)
			{
				if (Casino.ResultSlots[index1, line1].GetComponent<Slot>().Card.Value == Casino.ResultSlots[index2, line2].GetComponent<Slot>().Card.Value
					&& Casino.ResultSlots[index1, line1].GetComponent<Slot>().Card.Value == Casino.ResultSlots[index3, line3].GetComponent<Slot>().Card.Value
					&& Casino.ResultSlots[index1, line1].GetComponent<Slot>().Card.Value == Casino.ResultSlots[index4, line4].GetComponent<Slot>().Card.Value
					&& Casino.ResultSlots[index1, line1].GetComponent<Slot>().Card.Value == Casino.ResultSlots[index5, line5].GetComponent<Slot>().Card.Value)
				{
					Debug.Log("Line14 Win");
					Casino.ResultSlots[index1, line1].GetComponent<UISprite>().color = new Color(1, 0, 0);
					Casino.ResultSlots[index2, line2].GetComponent<UISprite>().color = new Color(1, 0, 0);
					Casino.ResultSlots[index3, line3].GetComponent<UISprite>().color = new Color(1, 0, 0);
					Casino.ResultSlots[index4, line4].GetComponent<UISprite>().color = new Color(1, 0, 0);
					Casino.ResultSlots[index5, line5].GetComponent<UISprite>().color = new Color(1, 0, 0);
					return (int)(Casino.ResultSlots[index1, line1].GetComponent<Slot>().Card.Value * 5 * Casino.WinRate * Casino.Rank);
				}
			}
		}
		return 0;
	}

	//Line 15 win
	public static int Win15(int line1, int index1, int line2, int index2, int line3, int index3, int line4, int index4, int line5, int index5)
	{
		if (Casino.ResultSlots[index1, line1].GetComponent<Slot>().Card.CT == Casino.ResultSlots[index2, line2].GetComponent<Slot>().Card.CT
			&& Casino.ResultSlots[index1, line1].GetComponent<Slot>().Card.CT == Casino.ResultSlots[index3, line3].GetComponent<Slot>().Card.CT
			&& Casino.ResultSlots[index1, line1].GetComponent<Slot>().Card.CT == Casino.ResultSlots[index4, line4].GetComponent<Slot>().Card.CT
			&& Casino.ResultSlots[index1, line1].GetComponent<Slot>().Card.CT == Casino.ResultSlots[index5, line5].GetComponent<Slot>().Card.CT)
		{
			if (Casino.ResultSlots[index1, line1].GetComponent<Slot>().Card.DT == DirectionType.R
				&& Casino.ResultSlots[index2, line2].GetComponent<Slot>().Card.DT == DirectionType.R
				&& Casino.ResultSlots[index3, line3].GetComponent<Slot>().Card.DT == DirectionType.MU
				&& Casino.ResultSlots[index4, line4].GetComponent<Slot>().Card.DT == DirectionType.L
				&& Casino.ResultSlots[index5, line5].GetComponent<Slot>().Card.DT == DirectionType.L)
			{
				if (Casino.ResultSlots[index1, line1].GetComponent<Slot>().Card.Value == Casino.ResultSlots[index2, line2].GetComponent<Slot>().Card.Value
					&& Casino.ResultSlots[index1, line1].GetComponent<Slot>().Card.Value == Casino.ResultSlots[index3, line3].GetComponent<Slot>().Card.Value
					&& Casino.ResultSlots[index1, line1].GetComponent<Slot>().Card.Value == Casino.ResultSlots[index4, line4].GetComponent<Slot>().Card.Value
					&& Casino.ResultSlots[index1, line1].GetComponent<Slot>().Card.Value == Casino.ResultSlots[index5, line5].GetComponent<Slot>().Card.Value)
				{
					Debug.Log("Line15 Win");
					Casino.ResultSlots[index1, line1].GetComponent<UISprite>().color = new Color(1, 0, 0);
					Casino.ResultSlots[index2, line2].GetComponent<UISprite>().color = new Color(1, 0, 0);
					Casino.ResultSlots[index3, line3].GetComponent<UISprite>().color = new Color(1, 0, 0);
					Casino.ResultSlots[index4, line4].GetComponent<UISprite>().color = new Color(1, 0, 0);
					Casino.ResultSlots[index5, line5].GetComponent<UISprite>().color = new Color(1, 0, 0);
					return (int)(Casino.ResultSlots[index1, line1].GetComponent<Slot>().Card.Value * 5 * Casino.WinRate * Casino.Rank);
				}
			}
		}
		return 0;
	}

	//SkyWheel win
	public static int Win_SkyWheel()
	{
		int skywheelCount = 0;
		int[,] skywheel = new int[3, 2];
		int index = 0;
		for(int i = 0; i <= 4; i = i + 2)
		{
			for(int j = 0;j< 3;j++)
			{
				if (Casino.ResultSlots[i, j].GetComponent<Slot>().Card.CT == CardType.SkyWheel)
				{
					skywheelCount++;
					skywheel[index, 0] = i;
					skywheel[index, 1] = j;
					index++;
					break;
				}
			}
		}
		if(skywheelCount == 3)
		{
			Debug.Log("SkyWheel Win");
			Casino.ResultSlots[skywheel[0, 0], skywheel[0, 1]].GetComponent<UISprite>().color = new Color(1, 0, 0);
			Casino.ResultSlots[skywheel[1, 0], skywheel[1, 1]].GetComponent<UISprite>().color = new Color(1, 0, 0);
			Casino.ResultSlots[skywheel[2, 0], skywheel[2, 1]].GetComponent<UISprite>().color = new Color(1, 0, 0);
			return (int)(int.Parse(DataManager.Cards_Card[DataManager.Cards_Card.Count - 1].Value) * 3 * Casino.WinRate * Casino.Rank);
		}
		return 0;
	}
}
