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
            else
            {
                Debug.Log("Nothing Loot");
            }
        }
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

	
}
