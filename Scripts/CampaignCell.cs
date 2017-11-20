using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CampaignCell : MonoBehaviour {

    public UILabel LabelMissionName;
    public UILabel LabelMissionFlag;

    private CampaignCell cell;

    private void Start()
    {
        
    }

    //数据相关
    public int CellID;

    public void Cell_Click()
    {
        cell = gameObject.GetComponent<CampaignCell>();

        //GameManager.ChangePanel(GameManager.UIS[GameManager.CAMPAIGN], GameManager.UIS[GameManager.BATTLE],0);

		GameObject battle = Resources.Load ("Battle") as GameObject;
		GameObject b = NGUITools.AddChild (GameObject.Find ("UI Root"), battle);
		b.GetComponent<Battle_C> ().Enter (GameManager.UIS [GameManager.CAMPAIGN].GetComponent<Campaign_C> ().VirusID, cell.CellID, Modes.Campaign);
		Formula.UI_IsVisible(GameManager.UIS[GameManager.CAMPAIGN],false);

		AudioManager.playMusicByName(AudioManager.BattleBG);


        //传递关卡参数
        //GameObject.Find(GameManager.BATTLE).GetComponent<Battle_C>().Enter(GameManager.UIS[GameManager.CAMPAIGN].GetComponent<Campaign_C>().VirusID, cell.CellID,Modes.Campaign);

        //AudioManager.playMusicByName(AudioManager.BattleBG);
    }
}
