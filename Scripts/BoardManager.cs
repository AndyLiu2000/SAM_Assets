using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TileDirection
{
    Up,
    Down,
    Left,
    Right
}

public enum MatchType
{
    Three_Normal,
    Four_Horizental,
    Four_Vertical,
    Five_Bomb,
    Five_All
}

public class BoardManager : MonoBehaviour {

	//单例
	public static BoardManager instance;
	//随机图案
	public UISprite[] randomUISprite;
    public string[] randomGemName = new string[] { "10", "20", "30", "40", "50" };
    //行列
    public int BoardRow = 9;
	public int BoardColumn = 9;
	//偏移量
	public Vector2 offset = new Vector2(0,0);
	//所有的Item
	public Tile[,] Tiles;
	//所有Item的坐标
	public Vector3[,] TilePos;
	//相同Item列表
	public List<Tile> SameTilesList;
	//要消除的Item列表
	public List<Tile> MatchList;

    //随机颜色
    public Color randomColor;
	//正在操作
	public bool isOperation = false;
	//是否正在执行AllBoom
	public bool isMatch = false;

	//tile的边长
	private float tileSize = 0;

    public GameObject TilePrefabs;

    private MatchType mt = MatchType.Three_Normal;

	void Awake()
	{
		instance = this;
		Tiles = new Tile[BoardRow,BoardColumn];
		TilePos = new Vector3[BoardRow,BoardColumn];
		SameTilesList = new List<Tile> ();
		MatchList = new List<Tile> ();
    }

    void Start()
    {
        //初始化游戏
        InitBoard();
        ClearAllMatches();
    }

    /// <summary>
    /// 获取Item边长
    /// </summary>
    /// <returns>The item size.</returns>
    private float GetItemSize()
	{
        return Resources.Load<GameObject>(GameManager.Tile).GetComponent<UISprite>().width;
    }

	/// <summary>
	/// 初始化游戏
	/// </summary>
	private void InitBoard()
	{
		//获取Item边长
		tileSize = GetItemSize ();
        offset.x = -gameObject.GetComponent<UIPanel>().width / 2 + tileSize / 2;
        offset.y = -gameObject.GetComponent<UIPanel>().height / 2 + tileSize / 2;
        UISprite[] previousBelow = new UISprite[BoardRow];
        UISprite previousLeft = null;

        string[] previousBelowString = new string[BoardRow];
        string previousLeftString = null;

        //生成ITEM
        for (int i = 0; i < BoardRow; i++) {
			for (int j = 0; j < BoardColumn; j++) {
				//生成
				GameObject currentItem = ObjectPool.instance.GetGameObject(GameManager.Tile, gameObject.transform);
                
                //设置坐标
                currentItem.transform.localPosition = 
					new Vector3(j * tileSize,i * tileSize,0) + new Vector3(offset.x,offset.y,0);

                //随机图案编号
                //int random = Random.Range (0, randomSprites.Length);
				//获取Item组件
				Tile current = currentItem.GetComponent<Tile> ();
				//设置行列
				current.tileRow = i;
				current.tileColumn = j;
				//设置图案
				//current.sprite = randomSprites[random];
                /*List<UISprite> possibleCharacters = new List<UISprite>(); // 1
                possibleCharacters.AddRange(randomUISprite); // 2

                possibleCharacters.Remove(previousBelow[j]); // 3  把自己下面的排除掉，保证不会重复
                possibleCharacters.Remove(previousLeft);//把自己左边的排除掉，保证不会重复

                UISprite newSprite = possibleCharacters[Random.Range(0, possibleCharacters.Count)];//在剩下的不重复的选择项中随机一个
                current.sprite.spriteName = newSprite.spriteName; // 3
                */
                List<string> possibleStrings = new List<string>(); // 1
                possibleStrings.AddRange(randomGemName); // 2

                possibleStrings.Remove(previousBelowString[j]); // 3  把自己下面的排除掉，保证不会重复
                possibleStrings.Remove(previousLeftString);//把自己左边的排除掉，保证不会重复

                string newString = possibleStrings[Random.Range(0, possibleStrings.Count)];//在剩下的不重复的选择项中随机一个
                current.sprite.spriteName = newString; // 3

                //测试All组合
                /*
                if (i == 4 && (j == 2 || j == 3 || j == 5 || j == 6))
                {
                    current.sprite.spriteName = "10";
                }
                if (i == 5 && j == 4)
                {
                    current.sprite.spriteName = "10";
                }*/

                //测试双炸弹组合
                /*
                if (i == 4 && j == 4)
                {
                    current.sprite.spriteName = "10_30";
                }
                if (i == 5 && j == 4)
                {
                    current.sprite.spriteName = "10_30";
                }
                */
                //测试双直线组合
                /*
                if (i == 4 && j == 4)
                {
                    current.sprite.spriteName = "10_10";
                }
                if (i == 5 && j == 4)
                {
                    current.sprite.spriteName = "10_20";
                }*/

                //测试双all组合
                /*
                if (i == 4 && j == 4)
                {
                    current.sprite.spriteName = "All";
                }
                if (i == 5 && j == 4)
                {
                    current.sprite.spriteName = "All";
                }*/

                //测试all+linear组合
                /*
                if (i == 4 && j == 4)
                {
                    current.sprite.spriteName = "10_10";
                }
                if (i == 5 && j == 4)
                {
                    current.sprite.spriteName = "All";
                }
                */

                //测试all+bomb组合
                
                if (i == 4 && j == 4)
                {
                    current.sprite.spriteName = "10_10";
                }
                if (i == 5 && j == 4)
                {
                    current.sprite.spriteName = "20_30";
                }

                //previousBelow[j] = newSprite;
                //previousLeft = newSprite;
                previousBelowString[j] = newString;
                previousLeftString = newString;
                //设置图片
                //current.Img = randomSprites[random];
                //保存到数组
                Tiles [i, j] = current;
				//记录世界坐标
				TilePos [i, j] = currentItem.transform.position;
			}
		}
	}

	void ClearAllMatches()
	{
		//有消除
		bool hasMatch = false;
		foreach (Tile tile in Tiles) {
			//指定位置的Item存在，且没有被检测过
			if (tile && !tile.hasCheck) {
				//检测周围的消除
				tile.CheckAdjacentMatch ();
				if (MatchList.Count > 0) {
					hasMatch = true;
					isOperation = true;
				}
			}
		}
		if (!hasMatch) {
			//操作结束
			isOperation = false;
		}
	}

	/// <summary>
	/// 填充相同tile列表
	/// </summary>
	public void FillSameTilesList(Tile current)
	{
		//如果已存在，跳过
		if (SameTilesList.Contains (current))
			return;
		//添加到列表
		SameTilesList.Add (current);
        //上下左右的Item
        Tile[] tempTileList = new Tile[]{
            GetTileByDirection(current,TileDirection.Up),
            GetTileByDirection(current,TileDirection.Down),
            GetTileByDirection(current,TileDirection.Left),
            GetTileByDirection(current,TileDirection.Right)};

        string currentStart = current.sprite.spriteName.Substring(0, 2);
        for (int i = 0; i < tempTileList.Length; i++) {
			//如果Item不合法，跳过
			if (tempTileList [i] == null)
				continue;
            /*
			if (current.sprite.spriteName == tempTileList [i].sprite.spriteName) {
				FillSameTilesList (tempTileList[i]);//迭代的方法，继续找相邻的相同元素
			}*/
            
            string tempStart = tempTileList[i].sprite.spriteName.Substring(0, 2);
            if (currentStart == tempStart)
            {
                FillSameTilesList(tempTileList[i]);//迭代的方法，继续找相邻的相同元素
            }
        }
	}

	/// <summary>
	/// 填充待消除列表
	/// </summary>
	/// <param name="current">Current.</param>
	public void FillMatchList(Tile current)
	{
		//计数器
		int rowCount = 0;
		int columnCount = 0;
		//临时列表
		List<Tile> rowTempList = new List<Tile> ();
		List<Tile> columnTempList = new List<Tile> ();
		///横向纵向检测
		foreach (Tile tile in SameTilesList) {

			//如果在同一行
			if (tile.tileRow == current.tileRow) {
				//判断该点与Current中间有无间隙
				bool rowCanBoom  = CheckItemsInterval(true,current,tile);
				if (rowCanBoom) {
					//计数
					rowCount++;
					//添加到行临时列表
					rowTempList.Add (tile);
                    
				}
			}
			//如果在同一列
			if (tile.tileColumn == current.tileColumn) {
				//判断该点与Curren中间有无间隙
				bool columnCanBoom  = CheckItemsInterval(false,current,tile);
				if (columnCanBoom) {
					//计数
					columnCount++;
					//添加到列临时列表
					columnTempList.Add (tile);
                    
                }
			}
		}
		//横向消除
		bool horizontalBoom = false;
		//如果横向三个以上
		if (rowCount > 2) {
			//将临时列表中的Item全部放入BoomList
			MatchList.AddRange (rowTempList);
			//横向消除
			horizontalBoom = true;
		}
		//如果纵向三个以上
		if (columnCount > 2) {
			if (horizontalBoom) {
				//剔除自己
				MatchList.Remove (current);
			}
			//将临时列表中的Item全部放入BoomList
			MatchList.AddRange (columnTempList);
		}
        if(rowCount == 4)
        {
            mt = MatchType.Four_Vertical;
        }
        if(columnCount == 4)
        {
            mt = MatchType.Four_Horizental;
        }
        if(rowCount >= 3 && columnCount >= 3)
        {
            mt = MatchType.Five_Bomb;
        }
        if(rowCount >= 5 || columnCount >= 5)
        {
            mt = MatchType.Five_All;
        }

		//如果没有消除对象，返回
		if (MatchList.Count == 0)
			return;
		//创建临时的BoomList
		List<Tile> tempBoomList = new List<Tile> ();
		//转移到临时列表
		tempBoomList.AddRange (MatchList);
		//开启处理BoomList的协程
		StartCoroutine (ManipulateBoomList (tempBoomList,current));
	}
	/// <summary>
	/// 处理BoomList
	/// </summary>
	/// <returns>The boom list.</returns>
	IEnumerator ManipulateBoomList(List<Tile> tempBoomList, Tile current)
	{
        List<Tile> boomList = new List<Tile>();
		foreach (Tile tile in tempBoomList) {
			tile.hasCheck = true;
			tile.GetComponent<UISprite> ().alpha = 0.5f;

            //离开动画

            //item.GetComponent<AnimatedButton> ().Exit ();
            //Boom声音
            //AudioManager.instance.PlayMagicalAudio();
            //将被消除的Item在全局列表中移除

            //Tile的形态变化
            if(tile == current && mt != MatchType.Three_Normal && tile.sprite.spriteName.Length == 2)
            {
                switch (mt)
                {
                    case MatchType.Four_Horizental:
                        tile.GetComponent<UISprite>().alpha = 1;
                        tile.sprite.spriteName = tile.sprite.spriteName.Substring(0,2) + "_10";
                        break;
                    case MatchType.Four_Vertical:
                        tile.GetComponent<UISprite>().alpha = 1;
                        tile.sprite.spriteName = tile.sprite.spriteName.Substring(0, 2) + "_20";
                        break;
                    case MatchType.Five_Bomb:
                        tile.GetComponent<UISprite>().alpha = 1;
                        tile.sprite.spriteName = tile.sprite.spriteName.Substring(0, 2) + "_30";
                        break;
                    case MatchType.Five_All:
                        tile.GetComponent<UISprite>().alpha = 1;
                        tile.sprite.spriteName = "All";
                        break;

                    default:
                        break;
                }
            }
            else
            {
                //特殊Tile的效果触发
                if (tile.sprite.spriteName.Length > 2)
                {
                    boomList.Add(tile);
                    string end = tile.sprite.spriteName.Substring(2, 3);
                    switch (end)
                    {
                        case "_10":
                            foreach (Tile rowTile in Tiles)
                            {
                                if (tile == null || rowTile == null) continue;
                                if (rowTile.tileRow == tile.tileRow && rowTile.tileColumn != tile.tileColumn)
                                {
                                    boomList.Add(Tiles[rowTile.tileRow, rowTile.tileColumn]);
                                }
                            }
                            break;
                        case "_20":
                            foreach (Tile columnTile in Tiles)
                            {
                                if (tile == null || columnTile == null) continue;
                                if (columnTile.tileColumn == tile.tileColumn && columnTile.tileRow != tile.tileRow)
                                {
                                    boomList.Add(Tiles[columnTile.tileRow, columnTile.tileColumn]);
                                }
                            }
                            break;
                        case "_30":
                            foreach (Tile nearTile in Tiles)
                            {
                                if (tile == null || nearTile == null) continue;
                                int distance = Mathf.Abs(nearTile.tileColumn - tile.tileColumn) + Mathf.Abs(nearTile.tileRow - tile.tileRow);
                                if (distance > 0 && distance <= 2)
                                {
                                    boomList.Add(Tiles[nearTile.tileRow, nearTile.tileColumn]);
                                }
                            }
                            break;
                        default:
                            break;
                    }

                }
                else
                {
                    Tiles[tile.tileRow, tile.tileColumn] = null;
                }
            }
        }

        //如果是产生特殊效果，核心Tile要保留下来不消除
        if(mt != MatchType.Three_Normal)
        {
            tempBoomList.Remove(current);
        }
        mt = MatchType.Three_Normal;
        tempBoomList.AddRange(boomList);

        yield return StartCoroutine(EffectBoomList(boomList));

        //检测Item是否已经开发播放离开动画
        //while (!tempBoomList [0].GetComponent<AnimatedButton> ().CheckPlayExit ()) {
        //yield return 0;
        //}

        yield return BoomClear(tempBoomList);

	}

    List<Tile> EffectTriggered(Tile tile)
    {
        if (tile.hasCheck) return null;
        List<Tile> boomList = new List<Tile>();
        //特殊Tile的效果触发
        if (tile.sprite.spriteName.Length > 2)
        {
            boomList.Add(tile);
            string end = tile.sprite.spriteName.Substring(2, 3);
            switch (end)
            {
                case "_10":
                    foreach (Tile rowTile in Tiles)
                    {
                        if (tile == null || rowTile == null) continue;
                        if (rowTile.tileRow == tile.tileRow && rowTile.tileColumn != tile.tileColumn)
                        {
                            Tiles[rowTile.tileRow, rowTile.tileColumn].hasCheck = true;
                            boomList.Add(Tiles[rowTile.tileRow, rowTile.tileColumn]);
                        }
                    }
                    break;
                case "_20":
                    foreach (Tile columnTile in Tiles)
                    {
                        if (tile == null || columnTile == null) continue;
                        if (columnTile.tileColumn == tile.tileColumn && columnTile.tileRow != tile.tileRow)
                        {
                            Tiles[columnTile.tileRow, columnTile.tileColumn].hasCheck = true;
                            boomList.Add(Tiles[columnTile.tileRow, columnTile.tileColumn]);
                        }
                    }
                    break;
                case "_30":
                    foreach (Tile nearTile in Tiles)
                    {
                        if (tile == null || nearTile == null) continue;
                        int distance = Mathf.Abs(nearTile.tileColumn - tile.tileColumn) + Mathf.Abs(nearTile.tileRow - tile.tileRow);
                        if (distance > 0 && distance <= 2)
                        {
                            Tiles[nearTile.tileRow, nearTile.tileColumn].hasCheck = true;
                            boomList.Add(Tiles[nearTile.tileRow, nearTile.tileColumn]);
                        }
                    }
                    break;
                default:
                    break;
            }
        }
        else
        {
            Tiles[tile.tileRow, tile.tileColumn] = null;
        }

        return boomList;
    }

    IEnumerator EffectBoomList(List<Tile> boomList)
    {
        foreach (Tile tile in boomList)
        {
            tile.hasCheck = true;
            tile.GetComponent<UISprite>().alpha = 0.5f;
            Tiles[tile.tileRow, tile.tileColumn] = null;
        }
        yield break;
    }

    public void Boom(List<Tile> boomList)
    {
        //两个横竖效果
        int linearCount = 0;
        int bombCount = 0;
        int allCount = 0;
        foreach (Tile t in boomList)
        {
            if (t.sprite.spriteName.EndsWith("_10") || t.sprite.spriteName.EndsWith("_20"))
            {
                linearCount++;
            }

            if (t.sprite.spriteName.EndsWith("_30"))
            {
                bombCount++;
            }

            if(t.sprite.spriteName == "All")
            {
                allCount++;
            }
        }
        //双直线
        if (linearCount == 2)
        {
            StartCoroutine(ManipulateBoomList(boomList, boomList[0]));
            StartCoroutine(ManipulateBoomList(boomList, boomList[1]));
            return;
        }
        //双炸弹
        if(bombCount == 2)
        {
            StartCoroutine(DuoBomb(boomList));
            return;
        }
        //直线+炸弹
        if(linearCount == 1 && bombCount == 1)
        {
            StartCoroutine(LinearBomb(boomList));
            return;
        }
        //双全消
        if(allCount == 2)
        {
            StartCoroutine(DuoAll(boomList));
            return;
        }

        Debug.Log("allCount = " + allCount + ", " + "linearCount = " + linearCount + ", "+ "bombCount = " + bombCount);
        if(allCount == 1)
        {
            //全消+单色
            if(linearCount == 0 && bombCount == 0)
            {
                Tile gemTile;
                if (boomList[0].sprite.spriteName != "All")
                {
                    gemTile = boomList[0];
                }
                else
                {
                    gemTile = boomList[1];
                }
                StartCoroutine(AllGem(gemTile));
                return;
            }
            //全消+直线
            if(linearCount == 1)
            {
                Tile linearTile;
                if(boomList[0].sprite.spriteName != "All")
                {
                    linearTile = boomList[0];
                }
                else
                {
                    linearTile = boomList[1];
                }
                StartCoroutine(AllLinear(linearTile));
                return;
            }
            //全消+炸弹
            if(bombCount == 1)
            {
                Tile bombTile;
                if (boomList[0].sprite.spriteName != "All")
                {
                    bombTile = boomList[0];
                }
                else
                {
                    bombTile = boomList[1];
                }
                StartCoroutine(AllBomb(bombTile));
                return;
            }
        }
    }

    IEnumerator BoomClear(List<Tile> boomList)
    {
        //延迟0.2秒
        yield return new WaitForSeconds(0.2f);
        //开启下落
        yield return StartCoroutine(ItemsDrop());
        //延迟0.38秒
        yield return new WaitForSeconds(0.38f);

        foreach (Tile item in boomList)
        {
            //回收Item
            ObjectPool.instance.SetGameObject(item.gameObject);
        }
    }

    IEnumerator DuoBomb(List<Tile> boomList)
    {
        List<Tile> finalList = new List<Tile>();
        finalList.AddRange(boomList);
        foreach (Tile nearTile in Tiles)
        {
            //if (nearTile == boomList[0] || nearTile == boomList[1]) continue;
            float distance = Mathf.Abs(nearTile.tileColumn - (boomList[0].tileColumn + boomList[1].tileColumn) / 2.0f) + Mathf.Abs(nearTile.tileRow - (boomList[0].tileRow + boomList[1].tileRow) / 2.0f);
            if (distance >= 1.5f && distance <= 3.5f)
            {
                finalList.Add(Tiles[nearTile.tileRow, nearTile.tileColumn]);
            }
        }
        
        yield return StartCoroutine(EffectBoomList(finalList));

        //检测Item是否已经开发播放离开动画
        //while (!tempBoomList [0].GetComponent<AnimatedButton> ().CheckPlayExit ()) {
        //yield return 0;
        //}

        yield return BoomClear(finalList);
    }

    IEnumerator DuoAll(List<Tile> boomList)
    {
        List<Tile> finalList = new List<Tile>();
        foreach (Tile anyTile in Tiles)
        {
            finalList.Add(anyTile);
        }

        yield return StartCoroutine(EffectBoomList(finalList));

        //检测Item是否已经开发播放离开动画
        //while (!tempBoomList [0].GetComponent<AnimatedButton> ().CheckPlayExit ()) {
        //yield return 0;
        //}

        yield return BoomClear(finalList);
    }

    IEnumerator LinearBomb(List<Tile> linearBombList)
    {
        Tile bomb;
        Tile linear;
        if (linearBombList[0].sprite.spriteName.EndsWith("_30"))
        {
            bomb = linearBombList[0];
            linear = linearBombList[1];
        }
        else
        {
            bomb = linearBombList[1];
            linear = linearBombList[0];
        }

        List<Tile> finalList = new List<Tile>();
        finalList.AddRange(linearBombList);
        //横向数排一起消除
        if (linear.sprite.spriteName.EndsWith("_10"))
        {
            foreach(Tile gem in Tiles)
            {
                int rowdistance = Mathf.Abs(gem.tileRow - linear.tileRow);
                if(rowdistance <= 2)
                {
                    finalList.Add(gem);
                }
            }
        }
        if (linear.sprite.spriteName.EndsWith("_20"))
        {
            foreach (Tile gem in Tiles)
            {
                int columnistance = Mathf.Abs(gem.tileColumn - linear.tileColumn);
                if (columnistance <= 2)
                {
                    finalList.Add(gem);
                }
            }
        }

        yield return StartCoroutine(EffectBoomList(finalList));

        //检测Item是否已经开发播放离开动画
        //while (!tempBoomList [0].GetComponent<AnimatedButton> ().CheckPlayExit ()) {
        //yield return 0;
        //}

        yield return BoomClear(finalList);
    }

    IEnumerator AllLinear(Tile linearTile)
    {
        List<Tile> superLinearList = new List<Tile>();
        superLinearList.Add(linearTile);
        foreach (Tile anyTile in Tiles)
        {
            if (anyTile.sprite.spriteName.Length > 2) continue;
            if(anyTile.sprite.spriteName.Substring(0,2) == linearTile.sprite.spriteName.Substring(0, 2))
            {
                int randomFour = Random.Range(0, 2);
                if (randomFour == 1)
                {
                    anyTile.sprite.spriteName = anyTile.sprite.spriteName.Substring(0, 2) + "_10";
                }
                else
                {
                    anyTile.sprite.spriteName = anyTile.sprite.spriteName.Substring(0, 2) + "_20";
                }
                superLinearList.Add(anyTile);
            }

        }

        List<Tile> finalList = new List<Tile>();
        finalList.AddRange(superLinearList);
        foreach (Tile randomTile in superLinearList)
        {
            string end = randomTile.sprite.spriteName.Substring(2, 3);
            if(end == "_10")
            {
                foreach (Tile rowTile in Tiles)
                {
                    if (rowTile == null || randomTile == null) continue;
                    if (rowTile.tileRow == randomTile.tileRow && rowTile.tileColumn != randomTile.tileColumn)
                    {
                        finalList.Add(Tiles[rowTile.tileRow, rowTile.tileColumn]);
                    }
                }
            }
            else
            {
                foreach (Tile column in Tiles)
                {
                    if (column == null || randomTile == null) continue;
                    if (column.tileColumn == randomTile.tileColumn && column.tileRow != randomTile.tileRow)
                    {
                        finalList.Add(Tiles[column.tileRow, column.tileColumn]);
                    }
                }
            }
            
        }

        yield return StartCoroutine(EffectBoomList(finalList));

        //检测Item是否已经开发播放离开动画
        //while (!tempBoomList [0].GetComponent<AnimatedButton> ().CheckPlayExit ()) {
        //yield return 0;
        //}
        yield return BoomClear(finalList);
    }

    IEnumerator AllBomb(Tile bombTile)
    {
        List<Tile> allBombList = new List<Tile>();
        allBombList.Add(bombTile);
        foreach (Tile anyTile in Tiles)
        {
            if (anyTile.sprite.spriteName.Substring(0, 2) == bombTile.sprite.spriteName.Substring(0, 2))
            {
                anyTile.sprite.spriteName = anyTile.sprite.spriteName.Substring(0, 2) + "_30";
                allBombList.Add(anyTile);
            }

        }

        List<Tile> finalList = new List<Tile>();
        finalList.AddRange(allBombList);
        foreach (Tile bomb in finalList)
        {
            foreach(Tile nearTile in Tiles)
            {
                if (bomb == null || nearTile == null) continue;
                int distance = Mathf.Abs(nearTile.tileColumn - bomb.tileColumn) + Mathf.Abs(nearTile.tileRow - bomb.tileRow);
                if (distance > 0 && distance <= 2)
                {
                    allBombList.Add(Tiles[nearTile.tileRow, nearTile.tileColumn]);
                }
            }
            
        }

        finalList.AddRange(allBombList);

        yield return StartCoroutine(EffectBoomList(finalList));

        //检测Item是否已经开发播放离开动画
        //while (!tempBoomList [0].GetComponent<AnimatedButton> ().CheckPlayExit ()) {
        //yield return 0;
        //}

        yield return BoomClear(finalList);
    }

    IEnumerator AllGem(Tile gemTile)
    {
        List<Tile> allGemList = new List<Tile>();
        allGemList.Add(gemTile);
        foreach (Tile anyTile in Tiles)
        {
            if (anyTile.sprite.spriteName.Substring(0, 2) == gemTile.sprite.spriteName.Substring(0, 2))
            {
                allGemList.Add(anyTile);
            }
        }

        List<Tile> finalList = new List<Tile>();
        finalList.AddRange(allGemList);

        foreach (Tile gem in finalList)
        {
            //特殊Tile的效果触发
            if (gem.sprite.spriteName.Length > 2)
            {
                allGemList.Add(gem);
                string end = gem.sprite.spriteName.Substring(2, 3);
                switch (end)
                {
                    case "_10":
                        foreach (Tile rowTile in Tiles)
                        {
                            if (gem == null || rowTile == null) continue;
                            if (rowTile.tileRow == gem.tileRow && rowTile.tileColumn != gem.tileColumn)
                            {
                                allGemList.Add(Tiles[rowTile.tileRow, rowTile.tileColumn]);
                            }
                        }
                        break;
                    case "_20":
                        foreach (Tile columnTile in Tiles)
                        {
                            if (gem == null || columnTile == null) continue;
                            if (columnTile.tileColumn == gem.tileColumn && columnTile.tileRow != gem.tileRow)
                            {
                                allGemList.Add(Tiles[columnTile.tileRow, columnTile.tileColumn]);
                            }
                        }
                        break;
                    case "_30":
                        foreach (Tile nearTile in Tiles)
                        {
                            if (gem == null || nearTile == null) continue;
                            int distance = Mathf.Abs(nearTile.tileColumn - gem.tileColumn) + Mathf.Abs(nearTile.tileRow - gem.tileRow);
                            if (distance > 0 && distance <= 2)
                            {
                                allGemList.Add(Tiles[nearTile.tileRow, nearTile.tileColumn]);
                            }
                        }
                        break;
                    default:
                        break;
                }
            }
            else
            {
                Tiles[gem.tileRow, gem.tileColumn] = null;
            }
        }

        finalList.AddRange(allGemList);

        yield return StartCoroutine(EffectBoomList(finalList));

        //检测Item是否已经开发播放离开动画
        //while (!tempBoomList [0].GetComponent<AnimatedButton> ().CheckPlayExit ()) {
        //yield return 0;
        //}

        yield return BoomClear(finalList);
    }


    /// <summary>
    /// Items下落
    /// </summary>
    /// <returns>The drop.</returns>
    IEnumerator ItemsDrop()
	{
		isOperation = true;
		//逐列检测
		for (int i = 0; i < BoardColumn; i++) {
			//计数器
			int count = 0;
			//下落队列
			Queue<Tile> dropQueue = new Queue<Tile> ();
			//逐行检测
			for (int j = 0; j < BoardRow; j++) {
				if (Tiles [j, i] != null) {
					//计数
					count++;
					//放入队列
					dropQueue.Enqueue(Tiles [j, i]);
				}
			}
			//下落
			for (int k = 0; k < count; k++) {
				//获取要下落的Item
				Tile current = dropQueue.Dequeue ();
				//修改全局数组(原位置情况)
				Tiles[current.tileRow,current.tileColumn] = null;
				//修改Item的行数
				current.tileRow = k;
				//修改全局数组(填充新位置)
				Tiles[current.tileRow,current.tileColumn] = current;
				//下落
				current.GetComponent<ItemOperation>().
					CurrentItemDrop(TilePos[current.tileRow,current.tileColumn]);
			}
		}

		yield return new WaitForSeconds (0.2f);

		StartCoroutine (CreateNewItem());
		yield return new WaitForSeconds (0.2f);
		ClearAllMatches ();
	}
	/// <summary>
	/// 生成新的Item
	/// </summary>
	/// <returns>The new item.</returns>
	public IEnumerator CreateNewItem()
	{
		isOperation = true;
		for (int i = 0; i < BoardColumn; i++) {
			int count = 0;
			Queue<GameObject> newItemQueue = new Queue<GameObject> ();
			for (int j = 0; j < BoardRow; j++) {
				if (Tiles [j, i] == null) {
                    //生成一个Item
                    //GameObject current = Instantiate(Resources.Load<GameObject> (GameManager.Tile));
                    //						ObjectPool.instance.GetGameObject (Util.Item, transform);
                    GameObject current = ObjectPool.instance.GetGameObject(GameManager.Tile, transform);

                    //current.transform.parent = transform;
					current.transform.position = TilePos [BoardRow - 1, i];
					newItemQueue.Enqueue (current);
					count++;
				}
			}
			for (int k = 0; k < count; k++) {
				//获取Item组件
				Tile currentItem = newItemQueue.Dequeue ().GetComponent<Tile>();
                /*
				//随机数
				int random = Random.Range (0, randomUISprite.Length);
				//修改脚本中的图片
				currentItem.sprite.spriteName = randomUISprite [random].spriteName;
                */
                //随机数
                int random = Random.Range(0, randomGemName.Length);
                //修改脚本中的图片
                currentItem.sprite.spriteName = randomGemName[random];

                //修改真实图片
                //currentItem.Img = randomSprites [random];
                //获取要移动的行数
                int r = BoardRow - count + k;
				//移动
				currentItem.GetComponent<ItemOperation> ().ItemMove (r,i,TilePos[r,i]);
			}
		}
		yield break;
	}

	/// <summary>
	/// 检测两个Item之间是否有间隙（图案不一致）
	/// </summary>
	/// <param name="isHorizontal">是否是横向.</param>
	/// <param name="begin">检测起点.</param>
	/// <param name="end">检测终点.</param>
	private bool CheckItemsInterval(bool isHorizontal,Tile begin,Tile end)
	{
		//获取图案
		UISprite spr = begin.sprite;
		//如果是横向
		if (isHorizontal) {
			//起点终点列号
			int beginIndex = begin.tileColumn;
			int endIndex = end.tileColumn;
			//如果起点在右，交换起点终点列号
			if (beginIndex > endIndex) {
				beginIndex = end.tileColumn;
				endIndex = begin.tileColumn;
			}
			//遍历中间的Item
			for (int i = beginIndex + 1; i < endIndex; i++) {
				//异常处理（中间之前已被消除，此时未生成，标识为不合法）
				if (Tiles [begin.tileRow, i] == null)
					return false;
                /*
				//如果中间有间隙（有图案不一致的）
				if (Tiles [begin.tileRow, i].sprite.spriteName != spr.spriteName) {
					return false;
				}*/
                string currentStart = spr.spriteName.Substring(0, 2);
                string tempStart = Tiles[begin.tileRow, i].sprite.spriteName.Substring(0, 2);
                //如果中间有间隙（有图案不一致的）
                if (currentStart != tempStart)
                {
                    return false;
                }
            }
			return true;
		} else {
			//起点终点行号
			int beginIndex = begin.tileRow;
			int endIndex = end.tileRow;
			//如果起点在上，交换起点终点列号
			if (beginIndex > endIndex) {
				beginIndex = end.tileRow;
				endIndex = begin.tileRow;
			}
			//遍历中间的Item
			for (int i = beginIndex + 1; i < endIndex; i++) {
                /*
				//如果中间有间隙（有图案不一致的）
				if (Tiles [i, begin.tileColumn].sprite.spriteName != spr.spriteName) {
					return false;
				}*/
                string currentStart = spr.spriteName.Substring(0, 2);
                string tempStart = Tiles[i, begin.tileColumn].sprite.spriteName.Substring(0, 2);
                //如果中间有间隙（有图案不一致的）
                if (currentStart != tempStart)
                {
                    return false;
                }
            }
			return true;
		}
	}

	/// <summary>
    /// 获取相邻方向上的Tile
    /// </summary>
    /// <param name="current"></param>
    /// <param name="td"></param>
    /// <returns></returns>
	private Tile GetTileByDirection(Tile current,TileDirection td)
	{
        int row = -1;
        int column = -1;
        switch (td)
        {
            case TileDirection.Up:
                row = current.tileRow + 1;
                column = current.tileColumn;
                break;
            case TileDirection.Down:
                row = current.tileRow - 1;
                column = current.tileColumn;
                break;
            case TileDirection.Left:
                row = current.tileRow;
                column = current.tileColumn - 1;
                break;
            case TileDirection.Right:
                row = current.tileRow;
                column = current.tileColumn + 1;
                break;
        }
		if (!CheckRCLegal (row, column))
			return null;
		return Tiles [row, column];
	}
	/// <summary>
	/// 检测行列是否合法
	/// </summary>
	/// <returns><c>true</c>, if RC legal was checked, <c>false</c> otherwise.</returns>
	/// <param name="itemRow">Item row.</param>
	/// <param name="itemColumn">Item column.</param>
	public bool CheckRCLegal(int itemRow,int itemColumn)
	{
		if (itemRow >= 0 && itemRow < BoardRow && itemColumn >= 0 && itemColumn < BoardColumn)
			return true;
		return false;
	}
}