using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CampaignResult_C : MonoBehaviour {

    public UILabel LabelFlag;
    public UILabel LabelTime;
    public UILabel LabelInfect;
    public UILabel LabelInfectKill;
    public UILabel LabelZombieKill;

    public UILabel LabelFlagResult;
    public UILabel LabelTimeSecond;
    public UILabel LabelInfectNum;
    public UILabel LabelInfectKillNum;
    public UILabel LabelZombieKillNum;

    public UILabel LabelGold;
    public UILabel LabelGem;

    public bool ResultFlag;
    public int TimeSecond = 0;
    public int InfectNum = 0;
    public int InfectKillNum = 0;
    public int ZombieKillNum = 0;

	Battle_C BC;

    public GameObject CampaignResult_BackBtn;
    List<Loot_Sheet> loot = new List<Loot_Sheet>();

    private void Start()
    {
        UIEventListener.Get(CampaignResult_BackBtn).onClick = CampaignResult_BackBtn_Click;
		//BC = GameObject.Find ("Battle").GetComponent<Battle_C>();
    }

    public void Enter(bool flag)
    {
		BC = GameObject.Find ("Battle").GetComponent<Battle_C>();
        //产生掉落 generate loot
        foreach(Mission_Sheet ms in DataManager.Mission_Parameter)
        {
            if(ms.MissionID == BC.MissionID.ToString())
            {
                loot = Formula.Loot(ms.LootPackageID);
                break;
            }
        }

        int lootGold = 0;
        int lootGem = 0;

        foreach(Loot_Sheet ls in loot)
        {
            if(ls.ItemID == "1")
            {
                lootGold += int.Parse(ls.ItemNum);
            }
            if(ls.ItemID == "2")
            {
                lootGem += int.Parse(ls.ItemNum);
            }
        }

        LabelGold.text = lootGold.ToString();
        LabelGem.text = lootGem.ToString();

        //数据传递 receive data
        TimeSecond = (int)(BC.TimeSecond);
        InfectNum = BC.InfectNum;
        InfectKillNum = BC.InfectKillNum;
        ZombieKillNum = BC.ZombieKillNum;

        LabelFlag.text = LocalizationEx.LoadLanguageTextName("BattleResult");
        LabelTime.text = LocalizationEx.LoadLanguageTextName("BattleTime");
        LabelInfect.text = LocalizationEx.LoadLanguageTextName("Infect");
        LabelInfectKill.text = LocalizationEx.LoadLanguageTextName("InfectKill");
        LabelZombieKill.text = LocalizationEx.LoadLanguageTextName("ZombieKill");

        LabelFlagResult.text = ResultFlag? "Win":"Lose";
        LabelTimeSecond.text = TimeSecond.ToString();
        LabelInfectNum.text = InfectNum.ToString();
        LabelInfectKillNum.text = InfectKillNum.ToString();
        LabelZombieKillNum.text = ZombieKillNum.ToString();

        int completeMissionNum = 0;
        //判断关卡总星数，决定是否解锁新病毒 check if it will unlock new virus
        foreach (U_MissionFlag mf in GameManager.user.DB_u_mf)
        {
            if(mf.Flag == true)
            {
                completeMissionNum += 1;
            }
        }

        for(int i = 1; i < DataManager.Model_Virus.Count; i++)
        {
            if (!GameManager.user.DB_u_UnlockedViruses.Contains(DataManager.Model_Virus[i].VirusID) & int.Parse(DataManager.Model_Virus[i].UnlockNum) <= completeMissionNum)
            {
                //如果不包含该病毒ID，则解锁 if new virus id is not contained, unlock it
                Debug.Log("解锁新病毒");
                GameManager.user.DB_u_UnlockedViruses.Add(DataManager.Model_Virus[i].VirusID);
                GameManager.SaveData();
            }
        }
    }

    public void CampaignResult_BackBtn_Click(GameObject b)
    {
        Debug.Log("BackBtn_Click");
        GameManager.ChangePanel(GameManager.UIS[GameManager.CAMPAIGNRESULT], GameManager.UIS[GameManager.VIRUSSELECT],0);
    }
}
