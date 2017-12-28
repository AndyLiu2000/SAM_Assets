using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Tile : MonoBehaviour {

	public int row;//行
	public int col;//列
	//当前图案
	public UISprite sprite;

    public UISprite Img;

	private BoardManager boardManager;
	//被检测
	public bool hasCheck = false;

	void Awake()
	{
		sprite = GetComponent<UISprite>();
        sprite.depth = 10;
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
        //每个tile检查自己是否能否作为消除中心
		boardManager.SameTilesList.Clear ();
		boardManager.MatchList.Clear ();

		boardManager.randomColor = Color.white;
        //new Color (Random.Range (0.1f, 1f), Random.Range (0.1f, 1f), Random.Range (0.1f, 1f), 1);
        boardManager.FillSameTilesList(this);
        boardManager.FillMatchList (this);
	}
}