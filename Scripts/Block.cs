using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block : MonoBehaviour {

    //常量
    public const int MAX_LAYER = 2;
    static string[] spriteName = { "BotBlock", "MidBlock", "TopBlock" };
    public int layer;

    public int row;
    public int col;

    //当前图案
    public UISprite sprite;

    private BoardManager boardManager;

    void Awake()
    {
        sprite = GetComponent<UISprite>();
        sprite.depth = 20;
        sprite.spriteName = spriteName[MAX_LAYER];
        layer = MAX_LAYER;
    }

    void OnEnable()
    {
        boardManager = BoardManager.instance;
    }

    public void Boom()
    {
        layer -= 1;
        if (layer >= 0)
        {
            sprite.spriteName = spriteName[layer];
        }
        else
        {
            boardManager.Blocks[row, col] = null;
            //回收Item
            ObjectPool.instance.SetGameObject(gameObject);
        }
    }
}
