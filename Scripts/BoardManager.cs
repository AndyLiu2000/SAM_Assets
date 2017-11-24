/*
 * Copyright (c) 2017 Razeware LLC
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
 * THE SOFTWARE.
 */

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BoardManager : MonoBehaviour {
	public static BoardManager instance;
	public List<GameObject> characters = new List<GameObject>();//已在编辑器中赋值
	public GameObject tile;
	public int xSize, ySize;//面板尺寸，已在编辑器中赋值

	private GameObject[,] tiles;

	public bool IsShifting { get; set; }

	void Start () {
		instance = GetComponent<BoardManager>();
        Vector2 offset = new Vector2(gameObject.GetComponent<UIPanel>().width / 9, gameObject.GetComponent<UIPanel>().width / 9);
        CreateBoard(offset.x, offset.y);
    }

	private void CreateBoard (float xOffset, float yOffset) {
		tiles = new GameObject[xSize, ySize];

        GameObject[] previousLeft = new GameObject[ySize];
        GameObject previousBelow = null;

        tile.GetComponent<UISprite>().width = (int)xOffset;
        tile.GetComponent<UISprite>().height = tile.GetComponent<UISprite>().width;

        //gameObject.transform.localPosition = new Vector2(-(int)GameObject.Find("UI Root").GetComponent<UIPanel>().GetViewSize().x / 2, )

        //从下往上，从左往右新建tile
        for (int x = 0; x < xSize; x++) {
			for (int y = 0; y < ySize; y++) {
				//GameObject newTile = Instantiate(tile, new Vector3(startX + (xOffset * x), startY + (yOffset * y), 0), tile.transform.rotation);//按照坐标进行赋值，但没有考虑适配
                GameObject newTile = NGUITools.AddChild(gameObject, tile);
                newTile.transform.localPosition = new Vector3(-xOffset * 9 / 2 + (int)(xOffset * (x + 0.5)), -yOffset * 9 / 2 + (int)(yOffset * (y + 0.5)), 0);

                tiles[x, y] = newTile;

                newTile.transform.parent = transform; // 1

                List<GameObject> possibleCharacters = new List<GameObject>(); // 1
                possibleCharacters.AddRange(characters); // 2

                possibleCharacters.Remove(previousLeft[y]); // 3  把自己左边的排除掉，保证不会重复
                possibleCharacters.Remove(previousBelow);//把自己下面的排除掉，保证不会重复

                //Sprite newSprite = characters[Random.Range(0, characters.Count)]; // 2
                GameObject newSprite = possibleCharacters[Random.Range(0, possibleCharacters.Count)];//在剩下的不重复的选择项中随机一个
                newTile.GetComponent<UISprite>().spriteName = newSprite.GetComponent<UISprite>().spriteName; // 3

                previousLeft[y] = newSprite;
                previousBelow = newSprite;
            }
        }
    }

    public IEnumerator FindNullTiles()
    {
        for (int x = 0; x < xSize; x++)
        {
            for (int y = 0; y < ySize; y++)
            {
                //if (tiles[x, y] == null)
                if (tiles[x, y].GetComponent<UISprite>().alpha == 0)
                //if (tiles[x, y].GetComponent<UISprite>().spriteName == "Human_1")
                {
                    yield return StartCoroutine(ShiftTilesDown(x, y));//有空格就下落
                    break;
                }
            }
        }

        for (int x = 0; x < xSize; x++)
        {
            for (int y = 0; y < ySize; y++)
            {
                tiles[x, y].GetComponent<Tile>().ClearAllMatches();//每次下落完了检查是否能消除
            }
        }
    }

    private IEnumerator ShiftTilesDown(int x, int yStart, float shiftDelay = .03f)
    {
        IsShifting = true;
        List<GameObject> renders = new List<GameObject>();
        int nullCount = 0;

        for (int y = yStart; y < ySize; y++)
        {  // 1
            GameObject render = tiles[x, y];
            if (render.GetComponent<UISprite>().alpha == 0)
            {
                nullCount++;
                //StartCoroutine(ShiftOneGrid(render,nullCount));
                //nullCount = 0;
                //tiles[x, yStart] = tiles[x, y];
                //yield return new WaitForSeconds(shiftDelay);// 4
            }            
            renders.Add(render);//把空的格子放到一个集合中
        }
        
        for (int i = 0; i < nullCount; i++)//从下往上遍历空格
        { // 3
            //GUIManager.instance.Score += 50;
            yield return new WaitForSeconds(shiftDelay);// 4
            for (int k = 0; k < renders.Count - 1; k++)
            { // 5
                renders[k].GetComponent<UISprite>().alpha = 1.0f;
                renders[k].GetComponent<UISprite>().spriteName = renders[k + 1].GetComponent<UISprite>().spriteName;
                //renders[k + 1].sprite = null; // 6
                //renders[k + 1].GetComponent<UISprite>().alpha = 1.0f;
                renders[k + 1].GetComponent<UISprite>().spriteName = GetNewSprite(x, ySize - 1);//最上方的格子变成新的图片
                renders[k + 1].GetComponent<UISprite>().alpha = 1.0f;
            }

            if(renders.Count == 1)
            {
                renders[0].GetComponent<UISprite>().spriteName = GetNewSprite(x, ySize - 1);//最上方的格子变成新的图片
                renders[0].GetComponent<UISprite>().alpha = 1.0f;
            }
        }
        
        
        IsShifting = false;
    }

    public IEnumerator ShiftOneGrid(GameObject shiftTile,int nullCount)
    {
        float speed = .03f;
        Debug.Log("shiftTile = " + shiftTile.GetComponent<UISprite>().spriteName);
        Vector3 des = shiftTile.transform.localPosition + Vector3.down * shiftTile.GetComponent<UISprite>().height * nullCount;
        while (shiftTile.transform.localPosition != shiftTile.transform.localPosition - Vector3.down * shiftTile.GetComponent<UISprite>().height)
        {
            shiftTile.transform.localPosition = Vector3.MoveTowards(shiftTile.transform.localPosition, des, 30.0f);
            yield return new WaitForSeconds(speed);// 4
        }
        
         
    }


    private string GetNewSprite(int x, int y)
    {
        List<GameObject> possibleCharacters = new List<GameObject>();
        possibleCharacters.AddRange(characters);

        if (x > 0)
        {
            possibleCharacters.Remove(tiles[x - 1, y]);//不与自己左边的tile相同
        }
        if (x < xSize - 1)
        {
            possibleCharacters.Remove(tiles[x + 1, y]);//不与自己右边的tile相同
        }
        if (y > 0)
        {
            possibleCharacters.Remove(tiles[x, y - 1]);//不与自己下边的tile相同
        }

        return possibleCharacters[Random.Range(0, possibleCharacters.Count)].GetComponent<UISprite>().spriteName;
    }
}
