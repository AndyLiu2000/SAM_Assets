using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Ice : MonoBehaviour {

    //常量
    public const int MAX_LAYER = 2;
    static string[] spriteName = { "BotLayer", "MidLayer", "TopLayer" };
    public int layer;

    public int row;
    public int col;

    //当前图案
    public UISprite sprite;

    private BoardManager boardManager;

    void Awake()
    {
        sprite = GetComponent<UISprite>();
        sprite.depth = 5;
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
            //回收Item
            ObjectPool.instance.SetGameObject(gameObject);
        }

    }
}
