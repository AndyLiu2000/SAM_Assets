using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacle : MonoBehaviour {

    //常量
    public int row;
    public int col;

    private BoardManager boardManager;

    void Awake()
    {

    }

    void OnEnable()
    {
        boardManager = BoardManager.instance;
    }
}
