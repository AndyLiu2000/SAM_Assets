using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class Main : MonoBehaviour {

    public UILabel LabelGold;
    public UILabel LabelGem;
    public GameObject Gold;
    public GameObject Gem;

    public GameObject MatchBtn;
    public GameObject OptionBtn;
    public GameObject ShopBtn;
    public GameObject PrivicyPolicyBtn;

    private void Start()
    {
        UIEventListener.Get(MatchBtn).onClick = MatchBtn_Click;
		UIEventListener.Get(OptionBtn).onClick = OptionBtn_Click;
        UIEventListener.Get(ShopBtn).onClick = ShopBtn_Click;
        UIEventListener.Get(PrivicyPolicyBtn).onClick = PrivicyPolicyBtn_Click;
        float factor = Screen.width / GameManager.StandardWidth;
		//Gold.transform.localScale = new Vector3(factor, factor, 1);
		//Gem.transform.localScale = new Vector3(factor, factor, 1);
    }

    public void Enter()
    {
        LabelGold.text = GameManager.user.Gold.ToString();
        LabelGem.text = GameManager.user.Gem.ToString();

        MatchBtn.GetComponentInChildren<UILabel>().text = LocalizationEx.LoadLanguageTextName("Start");
        OptionBtn.GetComponentInChildren<UILabel>().text = LocalizationEx.LoadLanguageTextName("Option");
        ShopBtn.GetComponentInChildren<UILabel>().text = LocalizationEx.LoadLanguageTextName("Shop");//
    }

    public void MatchBtn_Click(GameObject button)
    {
        Debug.Log("StartBtn_Click");
        GameManager.instance.LoadScene(GameManager.instance.SCENCE_MATCH);
    }

    public void OptionBtn_Click(GameObject b)
    {
        Debug.Log("OptionBtn_Click");
        GameManager.ChangePanel(GameManager.UIS[GameManager.MAIN_MAIN], GameManager.UIS[GameManager.MAIN_OPTION],0);
    }

    public void ShopBtn_Click(GameObject b)
    {
        Debug.Log("ShopBtn_Click");
        GameManager.ChangePanel(GameManager.UIS[GameManager.MAIN_MAIN], GameManager.UIS[GameManager.MAIN_SHOP],0);
    }

	public void PrivicyPolicyBtn_Click(GameObject b){
		Application.OpenURL (GameManager.PPURL);
	}
}
