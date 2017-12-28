using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cage : MonoBehaviour {

    //当前图案
    public UISprite sprite;
    public int row;//行
    public int col;//列

    private BoardManager boardManager;

    void Awake()
    {
        sprite = GetComponent<UISprite>();
        sprite.depth = 15;
        sprite.spriteName = "Cage";
    }

    void OnEnable()
    {
        boardManager = BoardManager.instance;
    }

    public void Boom()
    {
        boardManager.Cages[row, col] = null;
        ObjectPool.instance.SetGameObject(gameObject);
    }
}
