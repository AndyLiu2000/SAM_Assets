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

public class Tile : MonoBehaviour {
	private static Color selectedColor = new Color(.5f, .5f, .5f, 1.0f);
	private static Tile previousSelected = null;

	private UISprite render;
	private bool isSelected = false;

	private Vector2[] adjacentDirections = new Vector2[] { Vector2.up, Vector2.down, Vector2.left, Vector2.right };

    private bool matchFound = false;

    void Awake() {
		render = GetComponent<UISprite>();
        UIEventListener.Get(gameObject).onClick = Tile_Click;

        //render.width = (int)GameObject.Find("UI Root").GetComponent<UIPanel>().GetViewSize().x / 9;
        //render.height = render.width;
    }

	private void Select() {
		isSelected = true;
		render.color = selectedColor;
		previousSelected = gameObject.GetComponent<Tile>();
		//SFXManager.instance.PlaySFX(Clip.Select);
	}

	private void Deselect() {
		isSelected = false;
		render.color = Color.white;
		previousSelected = null;
	}

    public void Tile_Click(GameObject button)
    {
        // 1
        if (render.GetComponent<UISprite>().alpha == 0 || BoardManager.instance.IsShifting)
        {
            return;
        }

        if (isSelected)
        { // 2 Is it already selected?
            Deselect();
        }
        else
        {
            if (previousSelected == null)
            { // 3 Is it the first tile selected?
                Select();
            }
            else
            {
                Debug.Log("Match 1");

                if (GetAllAdjacentTiles().Contains(previousSelected.gameObject))
                { // 1
                    Debug.Log("Match 2");
                    SwapSprite(previousSelected.render); // 2
                    previousSelected.ClearAllMatches();
                    //previousSelected.Deselect();
                    ClearAllMatches();
                    if (!matchFound)//若不能消除，还得互换过来
                    {
                        SwapSprite(previousSelected.render);
                    }
                    matchFound = false;

                    previousSelected.Deselect();
                }
                else
                { // 3
                    previousSelected.GetComponent<Tile>().Deselect();
                    Select();
                }
            }
        }
    }

    public void SwapSprite(UISprite render2)
    { // 1
        if (render.spriteName == render2.spriteName)
        { // 2
            return;
        }

        string tempSprite = render2.spriteName; // 3
        render2.spriteName = render.spriteName; // 4
        render.spriteName = tempSprite; // 5
        //SFXManager.instance.PlaySFX(Clip.Swap); // 6
    }

    private GameObject GetAdjacent(Vector2 castDir)
    {
        RaycastHit[] hits = Physics.RaycastAll(transform.position, castDir);
        if (hits.Length > 0)
        {
            return hits[0].collider.gameObject;
        }
        return null;
    }

    private List<GameObject> GetAllAdjacentTiles()
    {
        List<GameObject> adjacentTiles = new List<GameObject>();
        for (int i = 0; i < adjacentDirections.Length; i++)
        {
            adjacentTiles.Add(GetAdjacent(adjacentDirections[i]));
        }
        return adjacentTiles;
    }

    private List<GameObject> FindMatch(Vector2 castDir)
    { // 1
        
        List<GameObject> matchingTiles = new List<GameObject>(); // 2
        
        RaycastHit[] hits = Physics.RaycastAll(transform.position, castDir); // 3
        if(hits.Length > 0)
        {
            Debug.Log("hits[0] 2= " + hits[0]);
            
            while (hits[0].collider.GetComponent<UISprite>().spriteName == render.spriteName)
            { // 4
                matchingTiles.Add(hits[0].collider.gameObject);
                hits = Physics.RaycastAll(hits[0].collider.transform.position, castDir);
                if (hits.Length > 0)
                {
                    continue;
                }
                else
                {
                    break;
                }

            }
        }
        
        return matchingTiles; // 5
        
    }

    private void ClearMatch(Vector2[] paths) // 1
    {
        List<GameObject> matchingTiles = new List<GameObject>(); // 2
        for (int i = 0; i < paths.Length; i++) // 3
        {
            matchingTiles.AddRange(FindMatch(paths[i]));
        }
        if (matchingTiles.Count >= 2) // 4
        {
            Debug.Log("Mactch Found");
            for (int i = 0; i < matchingTiles.Count; i++) // 5
            {
                matchingTiles[i].GetComponent<UISprite>().alpha = 0;
                //Destroy(matchingTiles[i]);
                //matchingTiles[i].GetComponent<UISprite>().spriteName = "Human_1";//找到了就消除掉
            }
            matchFound = true; // 6
        }
    }

    public void ClearAllMatches()
    {
        if (render.GetComponent<UISprite>().alpha == 0)
            return;

        ClearMatch(new Vector2[2] { Vector2.left, Vector2.right });
        ClearMatch(new Vector2[2] { Vector2.up, Vector2.down });
        if (matchFound)
        {
            //Destroy(gameObject);
            render.alpha = 0;
            //render.spriteName = "Human_1";
            matchFound = false;

            StopCoroutine(BoardManager.instance.FindNullTiles());
            StartCoroutine(BoardManager.instance.FindNullTiles());
            //SFXManager.instance.PlaySFX(Clip.Clear);
            //GUIManager.instance.MoveCounter--;
        }
    }
}