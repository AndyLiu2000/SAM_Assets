using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class GameManager :MonoBehaviour {

    public static GameManager instance;

    public string SCENCE_LOGIN = "Login";
    public string SCENCE_MAIN = "Main";
    public string SCENCE_MATCH = "Match";

    public GameObject FadeObj;
    float fadeSpeed = .02f;

    //存储所有UI物体名
    public const string LOGIN_LOGIN = "Login";

    public const string MAIN_MAIN = "Main";
    public const string MAIN_OPTION = "Option";
    public const string MAIN_SHOP = "Shop";

    public const string MATCH_Match = "Match";
    public const string MATCH_TIPS = "Tips";
    public const string MATCH_PAUSE = "Pause";
    public const string MATCH_CONCLUDE = "Conclude";
    public const string MATCH_PURCHASEINMATCH = "PurchaseInMatch";

    //目录
    //public const string ResourcesPrefab = "Prefabs/";
    //文件名称
    public const string Obstacle = "Obstacle";
    public const string Tile = "Tile";
    public const string Ice = "Ice";
    public const string Cage = "Cage";
    public const string Block = "Block";

    //动画参数名称
    public const string Pressed = "Pressed";
    public const string Exit = "Exit";

    //public const string CASINO = "Casino";
    public const string PPURL = "https://sites.google.com/view/dream-road/";

    //public static CurrLanguage;       //用户语言类型
    public static User user;            //用户数据
    public static string FilePathName;  //存档地址
    public static Dictionary<string, GameObject> UIS = new Dictionary<string, GameObject>();        //建立UI数据库

    //public static GameObject Battle;
    //public static Battle_C BC;
	public static int StandardWidth = 1080;
	public static int StandardHeight = 1920;

    private AsyncOperation async;
    private string currentScene;

    void Awake () {
        Debug.Log("GameManager.Awake");

        Debug.Log("SystemLanguage = " + Application.systemLanguage.ToString());

        // Only 1 Game Manager can exist at a time
        if (instance == null)
        {
            DontDestroyOnLoad(gameObject);
            instance = GetComponent<GameManager>();
            SceneManager.sceneLoaded += OnLevelFinishedLoading;
            
            //读取数据表
            DataManager.ReadDatas();

            //新建需要使用的类
            user = new User();

            /*
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
            */

            /*
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
            */
        }
        else
        {
            Destroy(gameObject);
        }

        /*
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
        /*
        //隐藏所有界面
        foreach (string ui in UIS.Keys)
        {
            Formula.UI_IsVisible(UIS[ui], false);
        }

        //初始化第一个界面
        //UIS[LOGIN].SetActive(true);
        Formula.UI_IsVisible(UIS[LOGIN], true);
        UIS[LOGIN].GetComponent<Login>().enter();
        */
    }

    /// <summary>
    /// 界面跳转方法
    /// </summary>
    /// <param name="oriUI"></param>
    /// <param name="desUI"></param>
    public static void ChangePanel(GameObject oriUI,GameObject desUI,int param1)
    {
        
        desUI.SetActive(true);
        //Formula.UI_IsVisible(desUI,true);

        //预加载数据
        if (desUI == UIS[MAIN_MAIN])
        {
            GameObject.Find(MAIN_MAIN).GetComponent<Main>().Enter();
        }

        if (desUI == UIS[MAIN_SHOP])
        {
            GameObject.Find(MAIN_SHOP).GetComponent<Shop>().Enter();
        }

        if (desUI == UIS[MAIN_OPTION])
        {
            GameObject.Find(MAIN_OPTION).GetComponent<Option>().Enter();
        }
        
        if (oriUI)
        {
           oriUI.SetActive(false);
           //Formula.UI_IsVisible(oriUI, false);
        }
        
    }

    // Get the current scene name
    public string CurrentSceneName
    {
        get
        {
            return currentScene;
        }
    }

    private void OnLevelFinishedLoading(Scene scene, LoadSceneMode mode)
    {
        Debug.Log("OnLevelFinishedLoading");
        currentScene = scene.name;

        Debug.Log("currentScene =" + currentScene);

        if (currentScene == SCENCE_LOGIN)
        {
            Debug.Log("Init Login UI");
            UIS.Clear();
            UIS.Add(LOGIN_LOGIN, GameObject.Find(LOGIN_LOGIN));

        }

        if (currentScene == SCENCE_MAIN)
        {
            Debug.Log("Init Main UI");
            UIS.Clear();
            UIS.Add(MAIN_MAIN, GameObject.Find(MAIN_MAIN));
            UIS.Add(MAIN_OPTION, GameObject.Find(MAIN_OPTION));
            UIS.Add(MAIN_SHOP, GameObject.Find(MAIN_SHOP));

            //设置界面的初始位置

            //隐藏所有界面
            foreach (string ui in UIS.Keys)
            {
                UIS[ui].SetActive(false);
            }

            //初始化第一个界面
            UIS[MAIN_MAIN].SetActive(true);

        }

        if (currentScene == SCENCE_MATCH)
        {
            Debug.Log("Init Match UI");
            UIS.Clear();
            UIS.Add(MATCH_Match, GameObject.Find(MATCH_Match));
            UIS.Add(MATCH_TIPS, GameObject.Find(MATCH_TIPS));
            UIS.Add(MATCH_PAUSE, GameObject.Find(MATCH_PAUSE));
            UIS.Add(MATCH_CONCLUDE, GameObject.Find(MATCH_CONCLUDE));
            UIS.Add(MATCH_PURCHASEINMATCH, GameObject.Find(MATCH_PURCHASEINMATCH));

            //隐藏所有界面
            foreach (string ui in UIS.Keys)
            {
                //DontDestroyOnLoad(UIS[ui]);
                UIS[ui].SetActive(false);
            }

            //初始化第一个界面
            UIS[MATCH_Match].SetActive(true);

        }

        instance.StartCoroutine(FadeIn());
    }

    // Load a scene with a specified string name
    public void LoadScene(string sceneName)
    {
        //SceneManager.LoadScene(sceneName);
        instance.StartCoroutine(Load(sceneName));
        instance.StartCoroutine(FadeOut());
    }

    // Begin loading a scene with a specified string asynchronously
    IEnumerator Load(string sceneName)
    {
        async = SceneManager.LoadSceneAsync(sceneName);
        async.allowSceneActivation = false;
        yield return async;
    }

    //Iterate the fader transparency to 100%
    IEnumerator FadeOut()
    {
        Debug.Log("fade out,遮罩变浓");
        //GameObject fade = Instantiate(FadeObj);
        //NGUITools.AddChild(GameObject.Find("UI Root"), fade);
        FadeObj.transform.GetChild(0).GetComponent<UISprite>().width = (int)GameObject.Find("UI Root").GetComponent<UIPanel>().GetViewSize().x;
        FadeObj.transform.GetChild(0).GetComponent<UISprite>().height = (int)GameObject.Find("UI Root").GetComponent<UIPanel>().GetViewSize().y;
        GameObject fade = NGUITools.AddChild(GameObject.Find("UI Root"), FadeObj);
        fade.SetActive(true);
        fade.transform.GetChild(0).GetComponent<UISprite>().alpha = 0;
        while (fade.transform.GetChild(0).GetComponent<UISprite>().alpha < 1)
        {
            //Debug.Log("fade out....");
            fade.transform.GetChild(0).GetComponent<UISprite>().alpha += .04f;
            yield return new WaitForSeconds(fadeSpeed);
        }
        ActivateScene(); //Activate the scene when the fade ends
        
    }

    // Iterate the fader transparency to 0%
    IEnumerator FadeIn()
    {
        Debug.Log("fade in,遮罩变淡");
        //GameObject fade = Instantiate(FadeObj);
        //NGUITools.AddChild(GameObject.Find("UI Root"), fade);
        FadeObj.transform.GetChild(0).GetComponent<UISprite>().width = (int)GameObject.Find("UI Root").GetComponent<UIPanel>().GetViewSize().x;
        FadeObj.transform.GetChild(0).GetComponent<UISprite>().height = (int)GameObject.Find("UI Root").GetComponent<UIPanel>().GetViewSize().y;
        GameObject fade = NGUITools.AddChild(GameObject.Find("UI Root"), FadeObj);
        fade.SetActive(true);
        fade.transform.GetChild(0).GetComponent<UISprite>().alpha = 1;
        while (fade.transform.GetChild(0).GetComponent<UISprite>().alpha > 0)
        {
            //Debug.Log("fade in....");
            fade.transform.GetChild(0).GetComponent<UISprite>().alpha -= .04f;
            yield return new WaitForSeconds(fadeSpeed);
        }
        //fade.SetActive(false);
        Destroy(fade);
    }

    // Allows the scene to change once it is loaded
    public void ActivateScene()
    {
        async.allowSceneActivation = true;
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
