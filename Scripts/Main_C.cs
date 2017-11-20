using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class Main_C : MonoBehaviour {

    public UILabel LabelGold;
    public UILabel LabelGem;
    public GameObject Gold;
    public GameObject Gem;

    public GameObject Main_StartBtn;
	public GameObject Main_CasinoBtn;
    public GameObject Main_DNABtn;
    public GameObject Main_OptionBtn;
    public GameObject Main_ShopBtn;
	public GameObject Main_PrivacyPolicyBtn;

    private void Start()
    {
        UIEventListener.Get(Main_StartBtn).onClick = Main_StartBtn_Click;
		UIEventListener.Get(Main_CasinoBtn).onClick = Main_CasinoBtn_Click;
        UIEventListener.Get(Main_DNABtn).onClick = Main_DNABtn_Click;
        UIEventListener.Get(Main_OptionBtn).onClick = Main_OptionBtn_Click;
        UIEventListener.Get(Main_ShopBtn).onClick = Main_ShopBtn_Click;
		UIEventListener.Get(Main_PrivacyPolicyBtn).onClick = Main_PrivacyPolicyBtn_Click;
		float factor = Screen.width / GameManager.StandardWidth;
		//Gold.transform.localScale = new Vector3(factor, factor, 1);
		//Gem.transform.localScale = new Vector3(factor, factor, 1);
    }

    public void Enter()
    {
        LabelGold.text = GameManager.user.Gold.ToString();
        LabelGem.text = GameManager.user.Gem.ToString();

		Main_StartBtn.GetComponentInChildren<UILabel>().text = LocalizationEx.LoadLanguageTextName("Start");
		Main_CasinoBtn.GetComponentInChildren<UILabel>().text = LocalizationEx.LoadLanguageTextName("Luck");
		Main_DNABtn.GetComponentInChildren<UILabel>().text = LocalizationEx.LoadLanguageTextName("Upgrade");
		Main_OptionBtn.GetComponentInChildren<UILabel>().text = LocalizationEx.LoadLanguageTextName("Option");
		Main_ShopBtn.GetComponentInChildren<UILabel>().text = LocalizationEx.LoadLanguageTextName("Shop");//
    }

    public void Main_StartBtn_Click(GameObject button)
    {
        Debug.Log("StartBtn_Click");
		GameManager.ChangePanel(GameManager.UIS[GameManager.MAIN], GameManager.UIS[GameManager.VIRUSSELECT],0);
    }

	public void Main_CasinoBtn_Click(GameObject button)
	{
		Debug.Log("CasinoBtn_Click");
		//GameManager.ChangePanel(GameManager.UIS[GameManager.MAIN], GameManager.UIS[GameManager.CASINO],0);
		Formula.UI_IsVisible(GameManager.UIS[GameManager.MAIN],false);
		GameObject casino = Resources.Load ("Casino") as GameObject;
		NGUITools.AddChild (GameObject.Find ("UI Root"), casino);
		AudioManager.playMusicByName(AudioManager.CasinoBGM);
	}

    public void Main_DNABtn_Click(GameObject b)
    {
        Debug.Log("DNABtn_Click");
        GameManager.ChangePanel(GameManager.UIS[GameManager.MAIN], GameManager.UIS[GameManager.DNA],0);
    }

    public void Main_OptionBtn_Click(GameObject b)
    {
        Debug.Log("OptionBtn_Click");
        GameManager.ChangePanel(GameManager.UIS[GameManager.MAIN], GameManager.UIS[GameManager.OPTION],0);
    }

    public void Main_ShopBtn_Click(GameObject b)
    {
        Debug.Log("ShopBtn_Click");
        GameManager.ChangePanel(GameManager.UIS[GameManager.MAIN], GameManager.UIS[GameManager.SHOP],0);
    }

	public void Main_PrivacyPolicyBtn_Click(GameObject b){
		Application.OpenURL (GameManager.PPURL);
	}
}
