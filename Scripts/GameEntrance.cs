using UnityEngine;
using System.Collections;

public class GameEntrance : MonoBehaviour {

	// 初始化管理器
	void Start () {
        AudioManager audioManager = Singleton.getInstance("AudioManager") as AudioManager;
        GameManager gameManager = Singleton.getInstance("GameManager") as GameManager;
        //DataManager dataManager = Singleton.getInstance("DataManager") as DataManager;
        //LocalizationEx localizationEx = Singleton.getInstance("LocalizationEx") as LocalizationEx;
    }
}
