using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Tile : MonoBehaviour {

	public int tileRow;//行
	public int tileColumn;//列
	//当前图案
	public UISprite sprite;

    public UISprite Img;

	private BoardManager boardManager;
	//被检测
	public bool hasCheck = false;

	void Awake()
	{
		sprite = GetComponent<UISprite>();
        //Img = transform.GetChild(0).GetComponent<UISprite>();
    }

	void OnEnable()
	{
		boardManager = BoardManager.instance;
	}

	/// <summary>
	/// 点击事件
	/// </summary>
	public void CheckAdjacentMatch()
	{
        Debug.Log("CheckAdjacentMatch");
        //每个tile检查自己是否能否作为消除中心
		boardManager.SameTilesList.Clear ();
		boardManager.MatchList.Clear ();

		boardManager.randomColor = Color.white;
        //new Color (Random.Range (0.1f, 1f), Random.Range (0.1f, 1f), Random.Range (0.1f, 1f), 1);
        boardManager.FillSameTilesList(this);
        boardManager.FillMatchList (this);
	}
}