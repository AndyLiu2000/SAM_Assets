using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum TileDurection
{
    Up,
    Down,
    Left,
    Right
}

public class BoardManager : MonoBehaviour {

	//单例
	public static BoardManager instance;
	//随机图案
	public UISprite[] randomUISprite;
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
                List<UISprite> possibleCharacters = new List<UISprite>(); // 1
                possibleCharacters.AddRange(randomUISprite); // 2

                possibleCharacters.Remove(previousBelow[j]); // 3  把自己下面的排除掉，保证不会重复
                possibleCharacters.Remove(previousLeft);//把自己左边的排除掉，保证不会重复

                UISprite newSprite = possibleCharacters[Random.Range(0, possibleCharacters.Count)];//在剩下的不重复的选择项中随机一个
                current.sprite.spriteName = newSprite.spriteName; // 3

                previousBelow[j] = newSprite;
                previousLeft = newSprite;
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
		foreach (var tile in Tiles) {
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
			GetTileByDirection(current,TileDurection.Up),
            GetTileByDirection(current,TileDurection.Down),
            GetTileByDirection(current,TileDurection.Left),
            GetTileByDirection(current,TileDurection.Right)};

		for (int i = 0; i < tempTileList.Length; i++) {
			//如果Item不合法，跳过
			if (tempTileList [i] == null)
				continue;
			if (current.sprite.spriteName == tempTileList [i].sprite.spriteName) {
				FillSameTilesList (tempTileList[i]);//迭代的方法，继续找相邻的相同元素
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
				//判断该点与Curren中间有无间隙
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
		//如果没有消除对象，返回
		if (MatchList.Count == 0)
			return;
		//创建临时的BoomList
		List<Tile> tempBoomList = new List<Tile> ();
		//转移到临时列表
		tempBoomList.AddRange (MatchList);
		//开启处理BoomList的协程
		StartCoroutine (ManipulateBoomList (tempBoomList));
	}
	/// <summary>
	/// 处理BoomList
	/// </summary>
	/// <returns>The boom list.</returns>
	IEnumerator ManipulateBoomList(List<Tile> tempBoomList)
	{
		foreach (var item in tempBoomList) {
			item.hasCheck = true;
			item.GetComponent<UISprite> ().alpha = 0;
			//离开动画
            
			//item.GetComponent<AnimatedButton> ().Exit ();
			//Boom声音
			//AudioManager.instance.PlayMagicalAudio();
			//将被消除的Item在全局列表中移除
			Tiles [item.tileRow, item.tileColumn] = null;
		}
		//检测Item是否已经开发播放离开动画
		//while (!tempBoomList [0].GetComponent<AnimatedButton> ().CheckPlayExit ()) {
			//yield return 0;
		//}

		//延迟0.2秒
		yield return new WaitForSeconds (0.2f);
		//开启下落
		yield return StartCoroutine (ItemsDrop ());
		//延迟0.38秒
		yield return new WaitForSeconds (0.38f);

		foreach (var item in tempBoomList) {
			//回收Item
			ObjectPool.instance.SetGameObject(item.gameObject);
		}
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
				//随机数
				int random = Random.Range (0, randomUISprite.Length);
				//修改脚本中的图片
				currentItem.sprite.spriteName = randomUISprite [random].spriteName;
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
				//异常处理（中间未生成，标识为不合法）
				if (Tiles [begin.tileRow, i] == null)
					return false;
				//如果中间有间隙（有图案不一致的）
				if (Tiles [begin.tileRow, i].sprite.spriteName != spr.spriteName) {
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
				//如果中间有间隙（有图案不一致的）
				if (Tiles [i, begin.tileColumn].sprite.spriteName != spr.spriteName) {
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
	private Tile GetTileByDirection(Tile current,TileDurection td)
	{
        int row = -1;
        int column = -1;
        switch (td)
        {
            case TileDurection.Up:
                row = current.tileRow + 1;
                column = current.tileColumn;
                break;
            case TileDurection.Down:
                row = current.tileRow - 1;
                column = current.tileColumn;
                break;
            case TileDurection.Left:
                row = current.tileRow;
                column = current.tileColumn - 1;
                break;
            case TileDurection.Right:
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