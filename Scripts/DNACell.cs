using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DNACell : MonoBehaviour {

    //界面相关
    public UILabel Name;
    public UILabel Lv;
    public UILabel Des;
    public UILabel GLabel;
    public UILabel CLabel;
    public GameObject GoldUpgradeBtn;
    public GameObject GemUpgradeBtn;
    DNA_C DNA_C;

    //数据相关
    public int CellID;
    public DNAType CellType;

    private void Start()
    {
        DNA_C = GameObject.Find(GameManager.DNA).GetComponent<DNA_C>();
        UIEventListener.Get(GoldUpgradeBtn).onClick = GoldUpgrade_Click;
        UIEventListener.Get(GemUpgradeBtn).onClick = GemUpgrade_Click;

		GoldUpgradeBtn.GetComponentInChildren<UILabel>().text = LocalizationEx.LoadLanguageTextName("Upgrade");//
		GemUpgradeBtn.GetComponentInChildren<UILabel>().text = LocalizationEx.LoadLanguageTextName("Upgrade");//
    }

    public void GoldUpgrade_Click(GameObject go)
    {
        int cellID = gameObject.GetComponent<DNACell>().CellID;
        int gold_cost = int.Parse(GLabel.text);

        //消耗金币，这里进行金币消耗数的计算 consume gold
        switch (gameObject.GetComponent<DNACell>().CellType)
        {
            case DNAType.Virus:
                //升级判断与处理 check upgrading
                if (GameManager.user.Gold >= gold_cost)
                {
                    //可以升级
                    GameManager.user.Gold -= gold_cost;
                    GameManager.user.DB_u_dna[0][cellID - 1].Lv += 1;

                    //存档
                    GameManager.SaveData();

                    //刷新Cell数据 update cell data
                    DNA_C.LoadDNAData(DataManager.DNAUp_Virus);
                }
                else
                {
                    //暂时不做任何操作，以后可以加缺少道具的提示
                }

                break;
            case DNAType.Human:
                //升级判断与处理
                if (GameManager.user.Gold >= gold_cost)
                {
                    //可以升级
                    GameManager.user.Gold -= gold_cost;
                    GameManager.user.DB_u_dna[1][cellID - 1].Lv += 1;

                    //存档
                    GameManager.SaveData();

                    //刷新Cell数据
                    DNA_C.LoadDNAData(DataManager.DNAUp_Human);
                }
                else
                {
                    //暂时不做任何操作，以后可以加缺少道具的提示
                }

                break;
            case DNAType.Zombie:
                //升级判断与处理
                if (GameManager.user.Gold >= gold_cost)
                {
                    //可以升级
                    GameManager.user.Gold -= gold_cost;
                    GameManager.user.DB_u_dna[2][cellID - 1].Lv += 1;

                    //存档
                    GameManager.SaveData();

                    //刷新Cell数据
                    DNA_C.LoadDNAData(DataManager.DNAUp_Zombie);
                }
                else
                {
                    //暂时不做任何操作，以后可以加缺少道具的提示
                }

                break;
        }

    }

    public void GemUpgrade_Click(GameObject go)
    {
        int cellID = gameObject.GetComponent<DNACell>().CellID;
        int gem_cost = int.Parse(CLabel.text);

        //消耗金币，这里进行金币消耗数的计算
        switch (gameObject.GetComponent<DNACell>().CellType)
        {
            case DNAType.Virus:
                //升级判断与处理
                if (GameManager.user.Gem >= gem_cost)
                {
                    //可以升级
                    GameManager.user.Gem -= gem_cost;
                    GameManager.user.DB_u_dna[0][cellID - 1].Lv += 1;

                    //存档
                    GameManager.SaveData();

                    //刷新Cell数据
                    DNA_C.LoadDNAData(DataManager.DNAUp_Virus);
                }
                else
                {
                    //暂时不做任何操作，以后可以加缺少道具的提示
                }

                break;
            case DNAType.Human:
                //升级判断与处理
                if (GameManager.user.Gem >= gem_cost)
                {
                    //可以升级
                    GameManager.user.Gem -= gem_cost;
                    GameManager.user.DB_u_dna[1][cellID - 1].Lv += 1;

                    //存档
                    GameManager.SaveData();

                    //刷新Cell数据
                    DNA_C.LoadDNAData(DataManager.DNAUp_Human);
                }
                else
                {
                    //暂时不做任何操作，以后可以加缺少道具的提示
                }

                break;
            case DNAType.Zombie:
                //升级判断与处理
                if (GameManager.user.Gem >= gem_cost)
                {
                    //可以升级
                    GameManager.user.Gem -= gem_cost;
                    GameManager.user.DB_u_dna[2][cellID - 1].Lv += 1;

                    //存档
                    GameManager.SaveData();

                    //刷新Cell数据
                    DNA_C.LoadDNAData(DataManager.DNAUp_Zombie);
                }
                else
                {
                    //暂时不做任何操作，以后可以加缺少道具的提示
                }

                break;
        }
    }
}
