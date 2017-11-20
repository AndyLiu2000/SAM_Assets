using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class GameManager :MonoBehaviour {

    //存储所有UI物体名------下一版本将战斗界面独立到另一个Scene，DontDestroyOnLoad()
    public const string LOGIN = "Login";
    public const string MAIN = "Main";
    public const string OPTION = "Option";
    public const string SHOP = "Shop";
    public const string DNA = "DNA";
    public const string VIRUSSELECT = "VirusSelect";
    public const string CAMPAIGN = "Campaign";
    public const string CAMPAIGNRESULT = "CampaignResult";
    public const string BATTLE = "Battle";
	//public const string CASINO = "Casino";
	public const string PPURL = "https://sites.google.com/view/dream-road/";

    //public static CurrLanguage;       //用户语言类型
    public static User user;            //用户数据
    public static string FilePathName;  //存档地址
    public static Dictionary<string, GameObject> UIS = new Dictionary<string, GameObject>();        //建立UI数据库

    //public static GameObject Battle;
    //public static Battle_C BC;
	public static int StandardWidth = 1920;
	public static int StandardHeight = 1080;

    void Awake () {
        Debug.Log("GameManager.start");

        Debug.Log("SystemLanguage = " + Application.systemLanguage.ToString());

        //读取数据表
        DataManager.ReadDatas();

        //新建需要使用的类
        user = new User();
        //LocalizationEx = new LocalizationEx();
        //LocalizationEx.SaveLanguage(LanguageChange.init);

        //读设置
        if (PlayerPrefs.HasKey("IsSaved"))
        {
            //读档
            Debug.Log("Loading Option....");
			//PlayerPrefs.DeleteAll();
            AudioManager.BgVolume = PlayerPrefs.GetFloat("MusicVolume");
            AudioManager.IsSoundOn = bool.Parse(PlayerPrefs.GetString("IsSoundOn"));
            LocalizationEx.LoadLanguage();

			Casino.Rank = int.Parse(PlayerPrefs.GetString("Rank"));

            Debug.Log("Load Option Complete");
        }
        else
        {
            //存储设置
            Debug.Log("Saving Option....");

            PlayerPrefs.SetString("IsSaved", "Yes");
            PlayerPrefs.SetFloat("MusicVolume", AudioManager.BgVolume);
            PlayerPrefs.SetString("IsSoundOn", AudioManager.IsSoundOn.ToString());
            LocalizationEx.SaveLanguage(LanguageChange.init);

			PlayerPrefs.SetString("Rank", Casino.Rank.ToString());

            Debug.Log("Save Option Complete");
        }

		//设定环境参数
		AudioManager.ChangeBGVolumeTo(AudioManager.BgVolume);
		AudioManager.ChangeMEToggle(AudioManager.IsSoundOn);
		GameObject.Find ("MusicBar").GetComponent<UISlider> ().value = AudioManager.BgVolume;
		GameObject.Find ("SoundSwitch").GetComponent<UIToggle>().value = AudioManager.IsSoundOn;

        //用户数据读档、存档
        //定义存档路径
        string dirpath = Application.persistentDataPath + "/Save";
        //创建存档文件夹
        IOHelper.CreateDirectory(dirpath);
        //定义存档文件路径
        string filename = dirpath + "/Zombie_GameData.sav";
        FilePathName = filename;

        //如果文件存在，读档
        if (IOHelper.IsFileExists(FilePathName))
        {
            LoadData();
        }
        //如果文件不存在，新建档案
        else
        {
            //新建数据，并保存数据
            user.Init();
            SaveData();
        }

        //把界面都包进字典
        UIS.Add(LOGIN, GameObject.Find(LOGIN));
        UIS.Add(MAIN, GameObject.Find(MAIN));
        UIS.Add(OPTION, GameObject.Find(OPTION));
        UIS.Add(SHOP, GameObject.Find(SHOP));
        UIS.Add(DNA, GameObject.Find(DNA));
        UIS.Add(VIRUSSELECT, GameObject.Find(VIRUSSELECT));
        UIS.Add(CAMPAIGN, GameObject.Find(CAMPAIGN));
        UIS.Add(CAMPAIGNRESULT, GameObject.Find(CAMPAIGNRESULT));
        //UIS.Add(BATTLE, GameObject.Find(BATTLE));
		//UIS.Add(CASINO, GameObject.Find(CASINO));

        //Battle = GameObject.Find(BATTLE);
        //BC = Battle.GetComponent<Battle_C>();

        //设置界面的初始位置
        /*
        //隐藏所有界面
        foreach (string ui in UIS.Keys)
        {
            //DontDestroyOnLoad(UIS[ui]);
            UIS[ui].SetActive(false);
        }

        //初始化第一个界面
        UIS[LOGIN].SetActive(true);
        */
    }

    private void Start()
    {
		UIRoot root = GameObject.FindObjectOfType<UIRoot> ();
		if (root != null) {
			float s = (float)root.activeHeight / Screen.height;
			StandardHeight = Mathf.CeilToInt (Screen.height * s);
			StandardWidth = Mathf.CeilToInt (Screen.width * s);
		}

        //隐藏所有界面
        foreach (string ui in UIS.Keys)
        {
            Formula.UI_IsVisible(UIS[ui], false);
        }

        //初始化第一个界面
        //UIS[LOGIN].SetActive(true);
        Formula.UI_IsVisible(UIS[LOGIN], true);
        UIS[LOGIN].GetComponent<Login_C>().enter();
    }

    /// <summary>
    /// 界面跳转方法
    /// </summary>
    /// <param name="oriUI"></param>
    /// <param name="desUI"></param>
    public static void ChangePanel(GameObject oriUI,GameObject desUI,int param1)
    {
        //desUI.SetActive(true);
        Formula.UI_IsVisible(desUI,true);

        //预加载数据
        if (desUI == UIS[MAIN])
        {
            GameObject.Find(MAIN).GetComponent<Main_C>().Enter();
        }

        if (desUI == UIS[CAMPAIGN])
        {
            GameObject.Find(CAMPAIGN).GetComponent<Campaign_C>().Enter(param1);
        }

        if (desUI == UIS[VIRUSSELECT])
        {
            GameObject.Find(VIRUSSELECT).GetComponent<VirusSelect>().Enter();
        }

        if (desUI == UIS[DNA])
        {
            GameObject.Find(DNA).GetComponent<DNA_C>().Enter();
        }

        if (desUI == UIS[SHOP])
        {
            GameObject.Find(SHOP).GetComponent<Shop_C>().Enter();
        }

        if (desUI == UIS[OPTION])
        {
            GameObject.Find(OPTION).GetComponent<Option_C>().Enter();
        }

        if(desUI == UIS[CAMPAIGNRESULT])
        {
            switch (param1)
            {
                //胜利
                case 1:
                    GameObject.Find(CAMPAIGNRESULT).GetComponent<CampaignResult_C>().Enter(true);
                    break;
                //失败
                case 0:
                    GameObject.Find(CAMPAIGNRESULT).GetComponent<CampaignResult_C>().Enter(false);
                    break;
            }
        }

		/*if (desUI == UIS[CASINO])
		{
			GameObject.Find(CASINO).GetComponent<Casino>().Enter();
		}*/

        if (oriUI)
        {
           //oriUI.SetActive(false);
           Formula.UI_IsVisible(oriUI, false);
        }

    }

    

    //界面转入的表现方法
	
    public static void SaveData()
    {
        //保存数据
        Debug.Log("Saving Data....");

        IOHelper.SetData(FilePathName, user);

        Debug.Log("Save Data Complete");
    }

    public static void LoadData()
    {
        //读取数据
        Debug.Log("Loading Data....");

        //将存档反序列化到一个临时库中，再转换成正常值
        user = user.Deserialize(IOHelper.GetData(FilePathName, typeof(F_User)) as F_User);

        Debug.Log("Load Data Complete");
    }
}
