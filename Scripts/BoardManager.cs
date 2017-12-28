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
    Five_Any
}

public enum SEType
{
    Normal_Boom,
    Horizontal_Boom,
    Vertical_Boom,
    Bomb_Boom,
    All_Boom,
    DuoAll_Boom
}

public class BoardManager : MonoBehaviour {

	//单例
	public static BoardManager instance;
    //常量
    public const string FIVE_ALL = "00_00";
    public const string Horizon = "_10";
    public const string Vertical = "_20";
    public const string Bomb = "_30";
    //随机图案
    public UISprite[] randomUISprite;
    public string[] randomGemName = new string[] { "10", "20", "30", "40", "50" };
    //行列
    public int BoardRow = 9;
	public int BoardColumn = 9;
	//偏移量
	public Vector2 offset = new Vector2(0,0);
	//所有的Tile
	public Tile[,] Tiles;
	//所有Tile的坐标
	public Vector3[,] TilePos;
	//相同Item列表
	public List<Tile> SameTilesList;
	//要消除的Item列表
	public List<Tile> MatchList;

    public Ice[,] Ices;

    public Cage[,] Cages;

    public Block[,] Blocks;

    public Obstacle[,] Obstacles;

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
    GameObject Normal_Boom;
    GameObject Horizontal_Boom;
    GameObject Vertical_Boom;
    GameObject Bomb_Boom;
    GameObject All_Boom;
    GameObject DuoAll_Boom;

    void Awake()
	{
		instance = this;
		Tiles = new Tile[BoardRow,BoardColumn];
		TilePos = new Vector3[BoardRow,BoardColumn];
		SameTilesList = new List<Tile> ();
		MatchList = new List<Tile> ();
        Ices = new Ice[BoardRow, BoardColumn];
        Cages = new Cage[BoardRow, BoardColumn];
        Blocks = new Block[BoardRow, BoardColumn];
        Obstacles = new Obstacle[BoardRow, BoardColumn];
    }

    void Start()
    {
        //初始化游戏
        InitBoard();
        ClearAllMatches();

        Normal_Boom = (GameObject)(Resources.Load("SEPrefabs" + "/" + "Skill_Behit_H"));
        Horizontal_Boom = (GameObject)(Resources.Load("SEPrefabs" + "/" + "Skill_Behit_H"));
        Vertical_Boom = (GameObject)(Resources.Load("SEPrefabs" + "/" + "Skill_Behit_H"));
        Bomb_Boom = (GameObject)(Resources.Load("SEPrefabs" + "/" + "Skill_Behit_H"));
        All_Boom = (GameObject)(Resources.Load("SEPrefabs" + "/" + "Skill_Behit_H"));
        DuoAll_Boom = (GameObject)(Resources.Load("SEPrefabs" + "/" + "Skill_Behit_H"));
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

        string[] previousBelowString = new string[BoardRow];
        string previousLeftString = null;

        //生成ITEM
        for (int i = 0; i < BoardRow; i++) {
			for (int j = 0; j < BoardColumn; j++) {
                Ices[i, j] = null;
                //保存到数组
                Tiles[i, j] = null;
                //记录世界坐标
                TilePos[i, j] = new Vector3(j * tileSize, i * tileSize, 0) + new Vector3(offset.x, offset.y, 0);
                //保存到数组
                Cages[i, j] = null;
                //保存到数组
                Blocks[i, j] = null;
                //保存到数组
                Obstacles[i, j] = null;

                if(j == 4 && i > 2 && i < 6){
                    //生成
                    GameObject currentObstacle = ObjectPool.instance.GetGameObject(GameManager.Obstacle, gameObject.transform);

                    //设置坐标
                    currentObstacle.transform.localPosition =
                        new Vector3(j * tileSize, i * tileSize, 0) + new Vector3(offset.x, offset.y, 0);
                    //获取Item组件
                    Obstacle obstacle = currentObstacle.GetComponent<Obstacle>();
                    obstacle.row = i;
                    obstacle.col = j;

                    //保存到数组
                    Obstacles[i, j] = obstacle;
                    //记录世界坐标
                    //记录世界坐标
                    TilePos[i, j] = currentObstacle.transform.position;
                    continue;
                }

                /*
                if (j == 5 && i > 2)
                {
                    //生成
                    GameObject currentBlock = ObjectPool.instance.GetGameObject(GameManager.Block, gameObject.transform);

                    //设置坐标
                    currentBlock.transform.localPosition =
                        new Vector3(j * tileSize, i * tileSize, 0) + new Vector3(offset.x, offset.y, 0);

                    //获取Item组件
                    Block block = currentBlock.GetComponent<Block>();
                    block.row = i;
                    block.col = j;

                    //保存到数组
                    Blocks[i, j] = block;
                    //记录世界坐标
                    TilePos[i, j] = currentBlock.transform.position;
                    continue;
                }*/
                /*
                //生成
                GameObject currentIce = ObjectPool.instance.GetGameObject(GameManager.Ice, gameObject.transform);

                //设置坐标
                currentIce.transform.localPosition =
                    new Vector3(j * tileSize, i * tileSize, 0) + new Vector3(offset.x, offset.y, 0);

                //获取Item组件
                Ice ice = currentIce.GetComponent<Ice>();
                ice.iceRow = i;
                ice.iceColumn = j;

                //保存到数组
                Ices[i, j] = ice;
                //记录世界坐标
                IcesPos[i, j] = currentIce.transform.position;
                */
                //生成
                GameObject currentTile = ObjectPool.instance.GetGameObject(GameManager.Tile, gameObject.transform);
                
                //设置坐标
                currentTile.transform.localPosition = 
					new Vector3(j * tileSize,i * tileSize,0) + new Vector3(offset.x,offset.y,0);

                //随机图案编号
                //int random = Random.Range (0, randomSprites.Length);
				//获取Item组件
				Tile current = currentTile.GetComponent<Tile> ();
				//设置行列
				current.row = i;
				current.col = j;
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
                
                if (i == 4 && j == 4)
                {
                    current.sprite.spriteName = "10_30";
                }
                if (i == 5 && j == 4)
                {
                    current.sprite.spriteName = "20_30";
                }
                if (i == 2 && j == 4)
                {
                    current.sprite.spriteName = "30_10";
                }
                

                //测试双直线组合
                /*
                if (i == 4 && j == 4)
                {
                    current.sprite.spriteName = "10_20";
                }
                if (i == 5 && j == 4)
                {
                    current.sprite.spriteName = "10_20";
                }
                */
                //测试双all组合
                /*
                if (i == 4 && j == 4)
                {
                    current.sprite.spriteName = FIVE_ALL;
                }
                if (i == 5 && j == 4)
                {
                    current.sprite.spriteName = FIVE_ALL;
                }*/

                //测试all+linear组合
                /*
                if (i == 4 && j == 4)
                {
                    current.sprite.spriteName = "10_10";
                }
                if (i == 5 && j == 4)
                {
                    current.sprite.spriteName = FIVE_ALL;
                }
                */

                //测试all+bomb组合
                /*
                if (i == 4 && j == 4)
                {
                    current.sprite.spriteName = FIVE_ALL;
                }
                if (i == 5 && j == 4)
                {
                    current.sprite.spriteName = "20_30";
                }
                */
                //previousBelow[j] = newSprite;
                //previousLeft = newSprite;
                previousBelowString[j] = newString;
                previousLeftString = newString;
                //保存到数组
                Tiles [i, j] = current;
				//记录世界坐标
				TilePos [i, j] = currentTile.transform.position;

                /*
                if (j!=4 || i<3)
                {
                    Cages[i, j] = null;
                    continue;
                }
                //生成
                GameObject currentCage = ObjectPool.instance.GetGameObject(GameManager.Cage, gameObject.transform);

                //设置坐标
                currentCage.transform.localPosition =
                    new Vector3(j * tileSize, i * tileSize, 0) + new Vector3(offset.x, offset.y, 0);

                //获取Item组件
                Cage cage = currentCage.GetComponent<Cage>();

                //设置行列
                cage.cageRow = i;
                cage.cageColumn = j;

                //保存到数组
                Cages[i, j] = cage;
                //记录世界坐标
                CagesPos[i, j] = currentCage.transform.position;
                */
            }
		}
	}

	void ClearAllMatches()
	{
		//有消除
		bool hasMatch = false;
		foreach (Tile tile in Tiles) {
			//指定位置的Item存在，且没有被检测过
			if (tile != null && !tile.hasCheck) {
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
			if (tile.row == current.row) {
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
			if (tile.col == current.col) {
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
            mt = MatchType.Five_Any;
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
        bool isDeleteCurrent = true;
        List<Tile> boomList = new List<Tile>();
		foreach (Tile tile in tempBoomList) {
			tile.hasCheck = true;
			tile.GetComponent<UISprite> ().alpha = 0.5f;

            
            //将被消除的Item在全局列表中移除

            //Tile的形态变化
            if(tile == current && mt != MatchType.Three_Normal && tile.sprite.spriteName.Length == 2)
            {
                isDeleteCurrent = false;
                //离开动画
                GenerateSEAtGameobjectPosition(tile.gameObject, SEType.Normal_Boom, true, null);
                //Boom声音
                //AudioManager.instance.PlayMagicalAudio();
                switch (mt)
                {
                    case MatchType.Four_Horizental:
                        tile.GetComponent<UISprite>().alpha = 1;
                        tile.sprite.spriteName = tile.sprite.spriteName.Substring(0,2) + Horizon;
                        break;
                    case MatchType.Four_Vertical:
                        tile.GetComponent<UISprite>().alpha = 1;
                        tile.sprite.spriteName = tile.sprite.spriteName.Substring(0, 2) + Vertical;
                        break;
                    case MatchType.Five_Bomb:
                        tile.GetComponent<UISprite>().alpha = 1;
                        tile.sprite.spriteName = tile.sprite.spriteName.Substring(0, 2) + Bomb;
                        break;
                    case MatchType.Five_Any:
                        tile.GetComponent<UISprite>().alpha = 1;
                        tile.sprite.spriteName = FIVE_ALL;
                        break;

                    default:
                        break;
                }
            }
            else
            {
                //特殊Tile的效果触发
                boomList.AddRange(EffectTriggered(tile));
            }
        }

        yield return StartCoroutine(BlockBoomList(tempBoomList));

        if (!isDeleteCurrent) tempBoomList.Remove(current);
        mt = MatchType.Three_Normal;
        tempBoomList.AddRange(boomList);
        if (!boomList.Contains(current))
        {
            yield return StartCoroutine(NonEffectBoomTile(current));
        }

        yield return StartCoroutine(EffectBoomList(boomList));

        //检测Item是否已经开发播放离开动画
        //while (!tempBoomList [0].GetComponent<AnimatedButton> ().CheckPlayExit ()) {
        //yield return 0;
        //}

        yield return BoomClear(tempBoomList);

	}

    List<Tile> EffectTriggered(Tile tile)
    {
        List<Tile> boomList = new List<Tile>();
        tile.hasCheck = true;
        tile.GetComponent<UISprite>().alpha = 0.5f;
        boomList.Add(tile);
        //特殊Tile的效果触发
        if (tile.sprite.spriteName.Length > 2)
        {
            //boomList.Add(tile);
            string end = tile.sprite.spriteName.Substring(2, 3);
            switch (end)
            {
                case Horizon:
                    foreach (Tile refTile in Tiles)
                    {
                        if (tile == null || refTile == null) continue;
                        if (refTile.row == tile.row && refTile.col != tile.col)
                        {
                            boomListHandler(boomList, refTile);
                        }
                    }

                    foreach (Block b in Blocks)
                    {
                        if (tile == null || b == null) continue;
                        if (b.row == tile.row && b.col != tile.col)
                        {
                            b.Boom();
                        }
                    }

                    break;
                case Vertical:
                    foreach (Tile refTile in Tiles)
                    {
                        if (tile == null || refTile == null) continue;
                        if (refTile.col == tile.col && refTile.row != tile.row)
                        {
                            boomListHandler(boomList, refTile);
                        }
                    }
                    foreach (Block b in Blocks)
                    {
                        if (tile == null || b == null) continue;
                        if (b.col == tile.col && b.row != tile.row)
                        {
                            b.Boom();
                        }
                    }
                    break;
                case Bomb:
                    foreach (Tile refTile in Tiles)
                    {
                        if (tile == null || refTile == null) continue;
                        int distance = Mathf.Abs(refTile.col - tile.col) + Mathf.Abs(refTile.row - tile.row);
                        if (distance > 0 && distance <= 2)
                        {
                            boomListHandler(boomList, refTile);
                        }
                    }
                    foreach (Block b in Blocks)
                    {
                        if (tile == null || b == null) continue;
                        int distance = Mathf.Abs(b.col - tile.col) + Mathf.Abs(b.row - tile.row);
                        if (distance > 0 && distance <= 2)
                        {
                            b.Boom();
                        }
                    }
                    break;
                case "_00":
                    //随机数
                    int random = Random.Range(0, randomGemName.Length);
                    //修改脚本中的图片
                    string randomGenName = randomGemName[random];

                    foreach (Tile refTile in Tiles)
                    {
                        if (refTile == null) continue;
                        if (refTile.sprite.spriteName.Substring(0, 2) == randomGenName)
                        {
                            boomListHandler(boomList, refTile);
                        }
                    }
                    break;
                default:
                    break;
            }
        }
        else
        {
            //boomList.Add(tile);
            
            Tiles[tile.row, tile.col] = null;

        }

        return boomList;
    }

    void boomListHandler(List<Tile> boomList,Tile tile)
    {
        boomList.Add(tile);
        if (tile.sprite.spriteName.Length > 2 && !tile.hasCheck)
        {
            boomList.AddRange(EffectTriggered(tile));
        }
    }

    IEnumerator BlockBoomList(List<Tile> boomList)
    {
        HashSet<Block> boomBlockList = new HashSet<Block>();
        foreach (Tile tile in boomList)
        {
            boomBlockList.Add(GetBlockByDirection(tile, TileDirection.Up));
            boomBlockList.Add(GetBlockByDirection(tile, TileDirection.Down));
            boomBlockList.Add(GetBlockByDirection(tile, TileDirection.Left));
            boomBlockList.Add(GetBlockByDirection(tile, TileDirection.Right));
        }
        foreach(Block b in boomBlockList)
        {
            if (b != null)
            {
                b.Boom();
            }
            
        }
        boomBlockList.Clear();
        yield break;
    }

    IEnumerator NonEffectBoomTile(Tile tile)
    {
        tile.hasCheck = true;

        if (Cages[tile.row, tile.col] != null)
        {
            Cages[tile.row, tile.col].Boom();
        }
        else if(Ices[tile.row, tile.col] != null)
        {
            Ices[tile.row, tile.col].Boom();
        }
        yield break;
    }

    IEnumerator EffectBoomList(List<Tile> boomList)
    {
        foreach (Tile tile in boomList)
        {
            if(Cages[tile.row, tile.col] != null)
            {
                Cages[tile.row, tile.col].Boom();
            }
            else if(Ices[tile.row, tile.col] != null)
            {
                Ices[tile.row, tile.col].Boom();
            }

            tile.hasCheck = true;
            tile.GetComponent<UISprite>().alpha = 0.5f;
            //离开动画
            GenerateSEAtGameobjectPosition(tile.gameObject, SEType.Normal_Boom, true, null);
            Tiles[tile.row, tile.col] = null;
        }
        yield break;
    }

    void SpecialTileNonEffectBoom(Tile seTile)
    {
        seTile.hasCheck = true;
        seTile.GetComponent<UISprite>().alpha = 0.5f;
        Tiles[seTile.row, seTile.col] = null;
        //回收Item
        ObjectPool.instance.SetGameObject(seTile.gameObject);
    }

    public void Boom(List<Tile> boomList)
    {
        //两个横竖效果
        int linearCount = 0;
        int bombCount = 0;
        int allCount = 0;
        foreach (Tile t in boomList)
        {
            if (t.sprite.spriteName.EndsWith(Horizon) || t.sprite.spriteName.EndsWith(Vertical))
            {
                linearCount++;
            }

            if (t.sprite.spriteName.EndsWith(Bomb))
            {
                bombCount++;
            }

            if(t.sprite.spriteName == FIVE_ALL)
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
            SpecialTileNonEffectBoom(boomList[0]);
            SpecialTileNonEffectBoom(boomList[1]);
            StartCoroutine(DuoBomb(boomList));
            return;
        }
        //直线+炸弹
        if(linearCount == 1 && bombCount == 1)
        {
            SpecialTileNonEffectBoom(boomList[0]);
            SpecialTileNonEffectBoom(boomList[1]);
            StartCoroutine(LinearBomb(boomList));
            return;
        }
        //双全消
        if(allCount == 2)
        {
            SpecialTileNonEffectBoom(boomList[0]);
            SpecialTileNonEffectBoom(boomList[1]);
            StartCoroutine(DuoAll(boomList));
            return;
        }

        if(allCount == 1)
        {
            //全消+单色
            if(linearCount == 0 && bombCount == 0)
            {
                Tile gemTile;
                Tile allTile;
                if (boomList[0].sprite.spriteName != FIVE_ALL)
                {
                    gemTile = boomList[0];
                    allTile = boomList[1];
                }
                else
                {
                    gemTile = boomList[1];
                    allTile = boomList[0];
                }
                SpecialTileNonEffectBoom(allTile);
                StartCoroutine(AllGem(gemTile));
                return;
            }
            //全消+直线
            if(linearCount == 1)
            {
                Tile linearTile;
                Tile allTile;
                if(boomList[0].sprite.spriteName != FIVE_ALL)
                {
                    linearTile = boomList[0];
                    allTile = boomList[1];
                }
                else
                {
                    linearTile = boomList[1];
                    allTile = boomList[0];
                }
                SpecialTileNonEffectBoom(allTile);
                StartCoroutine(AllLinear(linearTile));
                return;
            }
            //全消+炸弹
            if(bombCount == 1)
            {
                Tile bombTile;
                Tile allTile;
                if (boomList[0].sprite.spriteName != FIVE_ALL)
                {
                    bombTile = boomList[0];
                    allTile = boomList[1];
                }
                else
                {
                    bombTile = boomList[1];
                    allTile = boomList[0];
                }
                SpecialTileNonEffectBoom(allTile);
                StartCoroutine(AllBomb(bombTile));
                return;
            }
        }
    }

    IEnumerator BoomClear(List<Tile> boomList)
    {
        //延迟0.2秒
        yield return new WaitForSeconds(0.2f);

        foreach (Tile item in boomList)
        {
            //回收Item
            ObjectPool.instance.SetGameObject(item.gameObject);
        }
        //开启下落
        yield return StartCoroutine(ItemsDrop());
        //延迟0.38秒
        yield return new WaitForSeconds(0.38f);
    }

    IEnumerator DuoBomb(List<Tile> boomList)
    {
        List<Tile> finalList = new List<Tile>();
        finalList.AddRange(boomList);
        foreach (Tile nearTile in Tiles)
        {
            if (nearTile == null) continue;
            //if (nearTile == boomList[0] || nearTile == boomList[1]) continue;
            float distance = Mathf.Abs(nearTile.col - (boomList[0].col + boomList[1].col) / 2.0f) + Mathf.Abs(nearTile.row - (boomList[0].row + boomList[1].row) / 2.0f);
            if (distance >= 1.5f && distance <= 3.5f)
            {
                //特殊Tile的效果触发
                finalList.AddRange(EffectTriggered(nearTile));
            }
        }

        foreach (Block b in Blocks)
        {
            if (b == null) continue;
            //if (nearTile == boomList[0] || nearTile == boomList[1]) continue;
            float distance = Mathf.Abs(b.col - (boomList[0].col + boomList[1].col) / 2.0f) + Mathf.Abs(b.row - (boomList[0].row + boomList[1].row) / 2.0f);
            if (distance >= 1.5f && distance <= 3.5f)
            {
                //特殊Tile的效果触发
                b.Boom();
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

        foreach (Block b in Blocks)
        {
            if(b != null)
                b.Boom();
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
        if (linearBombList[0].sprite.spriteName.EndsWith(Bomb))
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
        if (linear.sprite.spriteName.EndsWith(Horizon))
        {
            foreach(Tile gem in Tiles)
            {
                if (gem == null) continue;
                int rowdistance = Mathf.Abs(gem.row - linear.row);
                if(rowdistance <= 2)
                {
                    finalList.Add(gem);
                }
            }
            foreach (Block b in Blocks)
            {
                int rowdistance = Mathf.Abs(b.row - linear.row);
                if (rowdistance <= 2)
                {
                    b.Boom();
                }
            }
        }
        if (linear.sprite.spriteName.EndsWith(Vertical))
        {
            foreach (Tile gem in Tiles)
            {
                if (gem == null) continue;
                int columnistance = Mathf.Abs(gem.col - linear.col);
                if (columnistance <= 2)
                {
                    finalList.Add(gem);
                }
            }
            foreach (Block b in Blocks)
            {
                int columnistance = Mathf.Abs(b.col - linear.col);
                if (columnistance <= 2)
                {
                    b.Boom();
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
        List<Tile> tempList = new List<Tile>();
        tempList.Add(linearTile);
        foreach (Tile anyTile in Tiles)
        {
            if(anyTile.sprite.spriteName.Substring(0,2) == linearTile.sprite.spriteName.Substring(0, 2))
            {
                int randomFour = Random.Range(0, 2);
                if (randomFour == 1)
                {
                    anyTile.sprite.spriteName = anyTile.sprite.spriteName.Substring(0, 2) + Horizon;
                }
                else
                {
                    anyTile.sprite.spriteName = anyTile.sprite.spriteName.Substring(0, 2) + Vertical;
                }
                tempList.Add(anyTile);
            }
        }

        List<Tile> finalList = new List<Tile>();
        foreach (Tile randomTile in tempList)
        {
            //特殊Tile的效果触发
            finalList.AddRange(EffectTriggered(randomTile));
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
        List<Tile> tempList = new List<Tile>();
        tempList.Add(bombTile);
        foreach (Tile anyTile in Tiles)
        {
            if (anyTile == null) continue;
            if (anyTile.sprite.spriteName.Substring(0, 2) == bombTile.sprite.spriteName.Substring(0, 2))
            {
                anyTile.sprite.spriteName = anyTile.sprite.spriteName.Substring(0, 2) + Bomb;
                tempList.Add(anyTile);
            }
        }
        List<Tile> finalList = new List<Tile>();
        foreach (Tile bomb in tempList)
        {
            //特殊Tile的效果触发
            finalList.AddRange(EffectTriggered(bomb));
        }

        yield return StartCoroutine(EffectBoomList(finalList));

        //检测Item是否已经开发播放离开动画
        //while (!tempBoomList [0].GetComponent<AnimatedButton> ().CheckPlayExit ()) {
        //yield return 0;
        //}

        yield return BoomClear(finalList);
    }

    IEnumerator AllGem(Tile gemTile)
    {
        List<Tile> tempList = new List<Tile>();

        foreach (Tile anyTile in Tiles)
        {
            if (anyTile == null) continue;
            if (anyTile.sprite.spriteName.Substring(0, 2) == gemTile.sprite.spriteName.Substring(0, 2))
            {
                tempList.Add(anyTile);
            }
        }

        List<Tile> finalList = new List<Tile>();
        finalList.AddRange(tempList);

        foreach (Tile gem in finalList)
        {
            //特殊Tile的效果触发
            tempList.AddRange(EffectTriggered(gem));
        }

        finalList.AddRange(tempList);

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
        for (int i = 0; i < BoardColumn; i++)
        {
            //计数器
            int count = 0;
            //下落队列
            Queue<Tile> dropQueue = new Queue<Tile>();
            //逐行检测
            int cageRow = -1;
            for (int j = 0; j < BoardRow; j++)
            {
                if (Tiles[j, i] != null && Cages[j, i] == null && Blocks[j,i] == null && Obstacles[j,i] == null)
                {
                    //计数
                    count++;
                    //放入队列
                    dropQueue.Enqueue(Tiles[j, i]);
                }

                //遇到障碍或触顶就下落
                if ((Cages[j, i] != null || Blocks[j,i] != null || Obstacles[j, i] != null) || j == BoardRow - 1)
                {
                    //下落
                    for (int k = 0; k < count; k++)
                    {
                        //获取要下落的Item
                        Tile current = dropQueue.Dequeue();
                        //修改全局数组(原位置情况)
                        Tiles[current.row, current.col] = null;
                        //修改Item的行数
                        current.row = k + cageRow + 1;
                        //下落
                        current.GetComponent<ItemOperation>().TileMove(current.row,i, TilePos[current.row, current.col]);
                    }

                    cageRow = j;
                    count = 0;
                    dropQueue.Clear();
                    continue;
                }

            }
            
        }

        yield return StartCoroutine(DiagonalDrop());
        yield return new WaitForSeconds(0.3f);
        yield return StartCoroutine(CreateNewItem());
        yield return new WaitForSeconds(0.3f);
        ClearAllMatches();
    }

    IEnumerator DiagonalDrop()
    {
        //斜向滑落
        for (int i = 0; i < BoardColumn; i++)
        {
            for (int j = 0; j < BoardRow; j++)
            {
                if (Tiles[j, i] == null) continue;
                //获取目标行列
                int targetRow = j - 1;
                int targetColumnRight = i + 1;
                int targetColumnLeft = i - 1;
                int targetColumn = -1;
                //检测合法
                bool isLagalRight = CheckRCLegal(targetRow, targetColumnRight);
                bool isLagalLeft = CheckRCLegal(targetRow, targetColumnLeft);
                if (isLagalRight)
                {
                    if (Tiles[targetRow, targetColumnRight] == null && Blocks[targetRow, targetColumnRight] == null && Obstacles[targetRow, targetColumnRight] == null
                        && (Cages[j, targetColumnRight] != null || Blocks[j, targetColumnRight] != null || Obstacles[j, targetColumnRight] != null))
                    {
                        targetColumn = targetColumnRight;
                    }
                }
                else if (isLagalLeft)
                {
                    if (Tiles[targetRow, targetColumnLeft] == null && Blocks[targetRow, targetColumnLeft] == null && Obstacles[targetRow, targetColumnLeft] == null
                        && (Cages[j, targetColumnLeft] != null || Blocks[j, targetColumnLeft] != null || Obstacles[j, targetColumnLeft] != null))
                    {
                        targetColumn = targetColumnLeft;
                    }
                }
                else
                {
                    continue;
                }

                if (targetColumn != -1)
                {
                    Tile current = Tiles[j, i];
                    //修改全局数组(原位置情况)
                    Tiles[j, i] = null;

                    //下落
                    current.GetComponent<ItemOperation>().
                        TileMove(targetRow, targetColumn, TilePos[targetRow, targetColumn]);

                    //yield return new WaitForSeconds(0.3f);

                    yield return StartCoroutine(ItemsDrop());
                    yield break;
                }
            }
        }

        
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
            int obstacleRow = 0;
			Queue<GameObject> newItemQueue = new Queue<GameObject> ();
			for (int j = BoardRow - 1; j >= 0; j--) {

                if (Cages[j, i] != null || Blocks[j,i] != null || Tiles[j, i] != null || Obstacles[j,i] != null)
                //if (Cages[j, i] != null)
                {
                    obstacleRow = j;
                    break;
                }
                Debug.Log("Obstacles = " + Obstacles[j, i]);
                if (Tiles [j, i] == null && Blocks[j, i] == null && Obstacles[j,i] == null) {

                    //生成一个Item
                    GameObject current = ObjectPool.instance.GetGameObject(GameManager.Tile, gameObject.transform);

					current.transform.position = TilePos [BoardRow - 1, i];
					newItemQueue.Enqueue (current);
					count++;
				}
			}
			for (int k = 0; k < count; k++) {
				//获取Item组件
				Tile currentItem = newItemQueue.Dequeue ().GetComponent<Tile>();
                currentItem.hasCheck = false;
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
                currentItem.sprite.alpha = 1.0f;

                //修改真实图片
                //currentItem.Img = randomSprites [random];
                //获取要移动到的行数
                int r = BoardRow - count + k;
                //移动                
                currentItem.GetComponent<ItemOperation> ().TileMove (r,i,TilePos[r,i]);
			}
		}

        yield return StartCoroutine(DiagonalDrop());
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
			int beginIndex = begin.col;
			int endIndex = end.col;
			//如果起点在右，交换起点终点列号
			if (beginIndex > endIndex) {
				beginIndex = end.col;
				endIndex = begin.col;
			}
			//遍历中间的Item
			for (int i = beginIndex + 1; i < endIndex; i++) {
				//异常处理（中间之前已被消除，此时未生成，标识为不合法）
				if (Tiles [begin.row, i] == null)
					return false;
                /*
				//如果中间有间隙（有图案不一致的）
				if (Tiles [begin.tileRow, i].sprite.spriteName != spr.spriteName) {
					return false;
				}*/
                string currentStart = spr.spriteName.Substring(0, 2);
                string tempStart = Tiles[begin.row, i].sprite.spriteName.Substring(0, 2);
                //如果中间有间隙（有图案不一致的）
                if (currentStart != tempStart)
                {
                    return false;
                }
            }
			return true;
		} else {
			//起点终点行号
			int beginIndex = begin.row;
			int endIndex = end.row;
			//如果起点在上，交换起点终点列号
			if (beginIndex > endIndex) {
				beginIndex = end.row;
				endIndex = begin.row;
			}
			//遍历中间的Item
			for (int i = beginIndex + 1; i < endIndex; i++) {
                /*
				//如果中间有间隙（有图案不一致的）
				if (Tiles [i, begin.tileColumn].sprite.spriteName != spr.spriteName) {
					return false;
				}*/
                string currentStart = spr.spriteName.Substring(0, 2);
                string tempStart = Tiles[i, begin.col].sprite.spriteName.Substring(0, 2);
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
                row = current.row + 1;
                column = current.col;
                break;
            case TileDirection.Down:
                row = current.row - 1;
                column = current.col;
                break;
            case TileDirection.Left:
                row = current.row;
                column = current.col - 1;
                break;
            case TileDirection.Right:
                row = current.row;
                column = current.col + 1;
                break;
        }
		if (!CheckRCLegal (row, column))
			return null;
		return Tiles [row, column];
	}

    private Block GetBlockByDirection(Tile current, TileDirection td)
    {
        int row = -1;
        int column = -1;
        switch (td)
        {
            case TileDirection.Up:
                row = current.row + 1;
                column = current.col;
                break;
            case TileDirection.Down:
                row = current.row - 1;
                column = current.col;
                break;
            case TileDirection.Left:
                row = current.row;
                column = current.col - 1;
                break;
            case TileDirection.Right:
                row = current.row;
                column = current.col + 1;
                break;
        }
        if (!CheckRCLegal(row, column))
            return null;
        if (Blocks[row, column] != null) return Blocks[row, column];
        return null;
    }

    /// <summary>
    /// 检测行列是否合法
    /// </summary>
    /// <returns><c>true</c>, if RC legal was checked, <c>false</c> otherwise.</returns>
    /// <param name="itemRow">Item row.</param>
    /// <param name="itemColumn">Item column.</param>
    public bool CheckRCLegal(int itemRow,int itemColumn)
	{
		if (itemRow >= 0 && itemRow < BoardRow && itemColumn >= 0 && itemColumn < BoardColumn && Obstacles[itemRow,itemColumn] == null)
			return true;
		return false;
	}

    //播放特效
    /// <summary>
    /// invokeName是当特效播放结束后要触发其他时，添加的回调方法名
    /// </summary>
    /// <param name="go"></param>
    /// <param name="seType"></param>
    /// <param name="isSelfActive"></param>
    /// <param name="invokeName"></param>
    public void GenerateSEAtGameobjectPosition(GameObject go, SEType seType, bool isSelfActive, string invokeName)
    {
        //GameObject se = NGUITools.AddChild(Battle.Entity, (GameObject)(Resources.Load("SEPrefabs" + "/" + seName)));
        GameObject se = null;
        switch (seType)
        {
            case SEType.Normal_Boom:
                se = NGUITools.AddChild(instance.gameObject, Normal_Boom);
                break;
            case SEType.Horizontal_Boom:
                se = NGUITools.AddChild(instance.gameObject, Horizontal_Boom);
                break;
            case SEType.Vertical_Boom:
                se = NGUITools.AddChild(instance.gameObject, Vertical_Boom);
                break;
            case SEType.Bomb_Boom:
                se = NGUITools.AddChild(instance.gameObject, Bomb_Boom);
                break;
            case SEType.All_Boom:
                se = NGUITools.AddChild(instance.gameObject, All_Boom);
                break;
            case SEType.DuoAll_Boom:
                se = NGUITools.AddChild(instance.gameObject, DuoAll_Boom);
                break;
        }
        se.transform.localScale = new Vector3(80, 80, 1);        //该死的Unity，把动画文件加载的时候默认缩小为1/100了，所以这里要扩大100倍。注意，改Prefabs的缩放比例是没用的
        NGUITools.SetDirty(instance.gameObject);
        Transform desGO = go.GetComponent<Transform>();
        se.transform.localPosition = desGO.localPosition;

        Formula.Btn_IsVisible(go, isSelfActive);
        if (invokeName != null)
            Invoke(invokeName, se.GetComponent<Animator>().runtimeAnimatorController.animationClips[0].length);

        Destroy(se, se.GetComponent<Animator>().runtimeAnimatorController.animationClips[0].length);
    }
}