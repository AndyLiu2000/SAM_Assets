using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System;

public enum Modes
{
    None,
    Campaign,
    Trial,
    Endless
}

public enum Environment
{
    Hot,
    Cold,
    Balance
}

public enum Climate
{
    Dry,
    Wet,
    Normal
}

public enum BattleState
{
    Start,
    Game,
    End
}

public class Battle_C : MonoBehaviour {

    //常量值 constant
    const int SPBubble_SP = 3;
    const float SPBUBBLEDIAPPEAR = 3.0f;
    const int VIRUSNUM = 2;
    const int MEDICINEWORK = 50000;
    public int MEDICINESPD = 10;
    const int MAX_UPDATE_INTERVAL = 6;
    const int UPDATE_INDEX_GAMING = 0;
    const int UPDATE_INDEX_DATA = 1;
    const int UPDATE_INDEX_SKILL = 2;
    const int UPDATE_INDEX_MEDICINE = 3;
    const int UPDATE_INDEX_MODE = 4;
    const int UPDATE_INDEX_TIMING = 5;
    int updateInterval = 0;
	public int UpgradeMapWidth = 0;
	public int UpgradeMapHeigth = 0;
	public int HUMANMODEL_WIDTH = 200;
	public int HUMANMODEL_HEIGHT = 225;
	public int BUBBLE_WIDTH = 150;
	public int BUBBLE_HEIGHT = 150;


    //关卡数据
    public int MissionID;
    public int VirusID;
    Modes mode;
    //float deltaTime = 0;
    float deltaTime2 = 0;
    float deltaTime3 = 0;
    float currentTimeScale = 1.0f;
    public int VirusNum = VIRUSNUM;                //单局病毒量
    public float Medicine = 0;                      //解药当前进度
    public float MedicineWork = MEDICINEWORK;       //解药总进度
    public bool medicineOK = false;
    
    public BattleState BattleState;
    public bool Result = false;
    public int StrategyPoint;
    public float TotalInfection;
    public int TotalDamage;
    bool Accelarate;
    //Battle_C BC;
	public UISprite ResultSprite;

    public float TimeSecond;
    public int InfectNum;
    public int InfectKillNum;
    public int ZombieKillNum;
	public float ModelAspect;

    //物件引用
    public GameObject StrategyBtn;
    public GameObject LabelStrategy;
    public GameObject Battle;
    public GameObject Entity;
    public GameObject EndBattleBtn;
    public GameObject InGameUpgradePanel;
    public GameObject UpgradeMap;
    public GameObject VirusUpBtn;
    public GameObject HumanUpBtn;
    public GameObject ZombieUpBtn;
    public GameObject EvolutionBtn;
    public GameObject SpeedBtn;
    public GameObject strategyCloseBtn;
    public GameObject MedicineBar;
    public GameObject VirusProBar;
    public GameObject HumanProBar;
    public GameObject ZombieProBar;
    public UILabel Timer;
    UILabel Label_EvolutionDes;
    UILabel LabelEvolutionCost;
    UILabel LabelSpeed;
    public UILabel MediName;
    public UILabel MediPercent;

    //模具引用 prefabs
    public GameObject HumanModel;
    public GameObject ZombieModel;
    public GameObject LabelStrategyPoint;
    public GameObject GeneModel;
    public Gene GeneSelected;
    public GameObject LabelSP;
    public GameObject SP_Bubble;


    //各单位集合 entity arraylists
    public Virus CurVirus;
    public ArrayList UpgradedGenes_Virus = new ArrayList();
    public ArrayList UpgradedGenes_Human = new ArrayList();
    public ArrayList UpgradedGenes_Zombie = new ArrayList();
    public ArrayList VirusArray = new ArrayList();
    public ArrayList HumanArray = new ArrayList();
    public ArrayList ZombieArray = new ArrayList();

    public ArrayList VirusGeneArray = new ArrayList();
    public ArrayList HumanGeneArray = new ArrayList();
    public ArrayList ZombieGeneArray = new ArrayList();
    public ArrayList InfectedHumans = new ArrayList();
    public ArrayList UnInfectedHumans = new ArrayList();

    public List<Infection_Sheet> InfectionList = new List<Infection_Sheet>();
    public List<Damage_Sheet> DamageList = new List<Damage_Sheet>();

    private void Awake()
    {
        //BC = GameObject.Find(GameManager.BATTLE).GetComponent<Battle_C>();
    }

    private void Start()
    {
        


    }

    //接收不同模式的数据，方便以后独立处理，Enter结束之后才开始Update. init mode data
    public void Enter(int curVirusID, int curMissionID,Modes curMode)
    {
        //EndBattleBtn.SetActive(false);
        //InGameUpgradePanel.SetActive(false);
        //StrategyBtn.SetActive(true);
        //SpeedBtn.SetActive(true);

		UIEventListener.Get(EndBattleBtn).onClick = EndBattleBtn_Click;
		UIEventListener.Get(StrategyBtn).onClick = StrategyBtn_Click;
		UIEventListener.Get(EvolutionBtn).onClick = EvolutionBtn_Click;
		UIEventListener.Get(SpeedBtn).onClick = SpeedBtn_Click;
		UIEventListener.Get(VirusUpBtn).onClick = VirusUpBtn_Click;
		UIEventListener.Get(HumanUpBtn).onClick = HumanUpBtn_Click;
		UIEventListener.Get(ZombieUpBtn).onClick = ZombieUpBtn_Click;
		UIEventListener.Get(strategyCloseBtn).onClick = StrategyCloseBtn_Click;

		VirusUpBtn.GetComponent<UISprite> ().width = 260;
		VirusUpBtn.GetComponent<UISprite> ().height = 72;
		float factorX = VirusUpBtn.GetComponent<UISprite> ().width * 1.0f / GameManager.StandardWidth;
		float factorY = VirusUpBtn.GetComponent<UISprite> ().height * 1.0f / GameManager.StandardHeight;
		VirusUpBtn.GetComponent<UISprite> ().width = (int)(GameManager.StandardWidth * factorX);
		VirusUpBtn.GetComponent<UISprite> ().height = (int)(GameManager.StandardHeight * factorY);
		int battleBGWidth = GameObject.Find ("BattleBG").GetComponent<UISprite> ().width;
		Debug.Log ("battleBGWidth = " + battleBGWidth);
		VirusUpBtn.transform.localPosition = new Vector3 (-battleBGWidth * 3 / 8, strategyCloseBtn.transform.localPosition.y, 0);
		HumanUpBtn.transform.localPosition = new Vector3 (-battleBGWidth / 8, strategyCloseBtn.transform.localPosition.y, 0);
		ZombieUpBtn.transform.localPosition = new Vector3 (battleBGWidth / 8, strategyCloseBtn.transform.localPosition.y, 0);

		HumanUpBtn.GetComponent<UISprite> ().width = VirusUpBtn.GetComponent<UISprite> ().width;
		HumanUpBtn.GetComponent<UISprite> ().height = VirusUpBtn.GetComponent<UISprite> ().height;
		ZombieUpBtn.GetComponent<UISprite> ().width = VirusUpBtn.GetComponent<UISprite> ().width;
		ZombieUpBtn.GetComponent<UISprite> ().height = VirusUpBtn.GetComponent<UISprite> ().height;

		VirusProBar.transform.localPosition = new Vector3 (-battleBGWidth * 3 / 8, -strategyCloseBtn.transform.localPosition.y, 0);
		HumanProBar.transform.localPosition = new Vector3 (-battleBGWidth / 8, -strategyCloseBtn.transform.localPosition.y, 0);
		ZombieProBar.transform.localPosition = new Vector3 (battleBGWidth / 8, -strategyCloseBtn.transform.localPosition.y, 0);

		VirusProBar.GetComponent<UISprite> ().width = VirusUpBtn.GetComponent<UISprite> ().width;
		HumanProBar.GetComponent<UISprite> ().width = VirusUpBtn.GetComponent<UISprite> ().width;
		ZombieProBar.GetComponent<UISprite> ().width = VirusUpBtn.GetComponent<UISprite> ().width;

		UpgradeMapWidth = (int)GameObject.Find("UpgradeMapBG").GetComponent<UISprite>().width;
		UpgradeMapHeigth = (int)GameObject.Find("UpgradeMapBG").GetComponent<UISprite>().height;

		VirusUpBtn.GetComponentInChildren<UILabel>().text = LocalizationEx.LoadLanguageTextName("Virus");
		HumanUpBtn.GetComponentInChildren<UILabel>().text = LocalizationEx.LoadLanguageTextName("Human");
		ZombieUpBtn.GetComponentInChildren<UILabel>().text = LocalizationEx.LoadLanguageTextName("Zombie");

		VirusProBar.GetComponentInChildren<UILabel>().text = LocalizationEx.LoadLanguageTextName("Virus");
		HumanProBar.GetComponentInChildren<UILabel>().text = LocalizationEx.LoadLanguageTextName("Human");
		ZombieProBar.GetComponentInChildren<UILabel>().text = LocalizationEx.LoadLanguageTextName("Zombie");

		StrategyBtn.transform.GetChild (3).GetComponent<UILabel> ().text = LocalizationEx.LoadLanguageTextName("Upgrade");
		EndBattleBtn.GetComponentInChildren<UILabel>().text = LocalizationEx.LoadLanguageTextName("Conclude");

		Label_EvolutionDes = GameObject.Find("Label_EvolutionDes").GetComponent<UILabel>();
		LabelEvolutionCost = GameObject.Find ("LabelEvolutionCost").GetComponent<UILabel> ();
		LabelSpeed = GameObject.Find("LabelSpeed").GetComponent<UILabel>();

        //Formula.Btn_IsVisible(EndBattleBtn, false);
		EndBattleBtn.SetActive (false);
		ResultSprite.alpha = 0.0f;
        //Formula.Btn_IsVisible(InGameUpgradePanel, false);
		InGameUpgradePanel.SetActive (false);
        //Formula.Btn_IsVisible(StrategyBtn, true);
		StrategyBtn.SetActive (true);
        //Formula.Btn_IsVisible(SpeedBtn, true);
		SpeedBtn.SetActive (true);

        Accelarate = false;
        LabelSpeed.text = "X 1";

		TimeSecond = 0.0f;
        InfectNum = 0;
        InfectKillNum = 0;
        ZombieKillNum = 0;

        //missionID = curMissionID;
        //StrategyPoint = 100;
        MissionID = curMissionID;
        VirusID = curVirusID;
        //要读取病毒特性
        VirusNum = VIRUSNUM;
        mode = curMode;

        LoadEntity(VirusID, MissionID);

        Medicine = 0.0f;
        MedicineWork = MEDICINEWORK * CurVirus.Medi_Work / 1000;
        medicineOK = false;

        MediName.text = LocalizationEx.LoadLanguageTextName("MediName");
        MediPercent.text = "";

        for (int i = 0; i < DataManager.InfectionSheet.Count; i++)
        {
            InfectionList.Add(DataManager.InfectionSheet[i]);
        }

        for (int i = 0; i < DataManager.DamageSheet.Count; i++)
        {
            DamageList.Add(DataManager.DamageSheet[i]);
        }

        //LoadBattleEvent(MissionID);
        //LoadBattleStrategy("1");
        MedicineBar.GetComponent<UISlider>().value = Medicine * 1.0f / MedicineWork;

        LabelStrategyPoint.GetComponent<UILabel>().text = StrategyPoint.ToString();

        Timer.text = "";

        
		BattleState = BattleState.Start;
    }

    void LoadEntity(int curVirusID, int curMissionID)
    {
        //病毒数据 virus data
        CurVirus = new Virus(curVirusID, MissionID);
        StrategyPoint = CurVirus.InitialSP;

        //随机生成人类数据，矩阵排布 line all humans in matrix
        int row = int.Parse(DataManager.Mission_Parameter[MissionID].DistributionParam1);
        int column = int.Parse(DataManager.Mission_Parameter[MissionID].DistributionParam2);
        //int row = 2;
        //int column = 1;
		int battleBGHeight = GameObject.Find ("BattleBG").GetComponent<UISprite> ().height; 

		ModelAspect = battleBGHeight * 1.0f / GameManager.StandardHeight;

		int width = (int)(HUMANMODEL_WIDTH * ModelAspect);
		int height = (int)(HUMANMODEL_HEIGHT * ModelAspect);

		HumanModel.transform.GetChild (0).GetComponent<UISprite> ().width = width;
		HumanModel.transform.GetChild (0).GetComponent<UISprite> ().height = height;
		HumanModel.GetComponent<BoxCollider> ().size = new Vector2 (width, height);

		int space = (int)SpeedBtn.GetComponent<UISprite> ().localSize.y;

        for (int i = 0; i < row; i++)
        {
            for (int j = 0; j < column; j++)
            {
                HumanModel.GetComponent<Human>().CreateHuman(curMissionID);

                GameObject hm = NGUITools.AddChild(Entity, HumanModel);
				Vector3 pos = new Vector3((-column / 2 + j) * width + space, (-row / 2 + i) * height + height / 2 + space,0);
                hm.transform.localPosition = pos;

                NGUITools.SetDirty(Entity);

                HumanArray.Add(hm);
            }
        }
    }

    void LoadBattleEvent(int curMissionID)
    {
        //根据关卡ID决定使用什么战斗事件 load battle event according to mission id
    }

    void LoadBattleStrategy(string boardID)
    {
        //Use object pool to restore objects here, modify later
        //先清除数据 clear data
        Transform[] children = UpgradeMap.GetComponentsInChildren<Transform>();

        //若小于等于2，此时新建数据 re-init data
        if (children.Length <= 2)
        {
            Debug.Log("初始化战术板");
            GameObject gm;
            //根据病毒ID决定使用什么策略面板 change strategy board accroding to virus id
            foreach (BattleStrategy_Sheet bss in DataManager.BattleStrategy_Strategy)
            {
                if (bss.StrategyID == CurVirus.StrategyID)
                {
                    //该策略ID的全部加载
                    GeneModel.GetComponent<Gene>().CreateGene(bss.GeneID);
                    gm = NGUITools.AddChild(UpgradeMap, GeneModel);
                    gm.transform.localPosition = gm.GetComponent<Gene>().Pos;
                    //Formula.Btn_IsVisible(gm, gm.GetComponent<Gene>().IsVisible);
					gm.SetActive (gm.GetComponent<Gene> ().IsVisible);
					gm.GetComponent<UISprite> ().depth = 60;

                    switch (bss.BoardID)
                    {
                        case "1":
                            VirusGeneArray.Add(gm);
                            break;
                        case "2":
                            HumanGeneArray.Add(gm);
                            break;
                        case "3":
                            ZombieGeneArray.Add(gm);
                            break;
                        default:
                            break;
                    }
                }
            }
        }

        Debug.Log("更新数据");
        int virusUpgradedQua = 0;
        foreach (GameObject go in VirusGeneArray)
        {
            go.GetComponent<Gene>().UpdateData(boardID);
            if (go.GetComponent<Gene>().IsUpgraded)
                virusUpgradedQua++;
        }
        VirusProBar.GetComponent<UIProgressBar>().value = virusUpgradedQua * 1.0f / VirusGeneArray.Count;

        int humanUpgradedQua = 0;
        foreach (GameObject go in HumanGeneArray)
        {
            go.GetComponent<Gene>().UpdateData(boardID);
            if (go.GetComponent<Gene>().IsUpgraded)
                humanUpgradedQua++;
        }
        HumanProBar.GetComponent<UIProgressBar>().value = humanUpgradedQua * 1.0f / HumanGeneArray.Count;

        int zombieUpgradedQua = 0;
        foreach (GameObject go in ZombieGeneArray)
        {
            go.GetComponent<Gene>().UpdateData(boardID);
            if (go.GetComponent<Gene>().IsUpgraded)
                zombieUpgradedQua++;
        }
        ZombieProBar.GetComponent<UIProgressBar>().value = zombieUpgradedQua * 1.0f / ZombieGeneArray.Count;

        //清空Label的内容
        switch (boardID)
        {
            case "1":
                Label_EvolutionDes.text = LocalizationEx.LoadLanguageTextName("VirusUpgradeDefaultDes");
                break;
            case "2":
                Label_EvolutionDes.text = LocalizationEx.LoadLanguageTextName("HumanUpgradeDefaultDes");
                break;
            case "3":
                Label_EvolutionDes.text = LocalizationEx.LoadLanguageTextName("ZombieUpgradeDefaultDes");
                break;
            default:
                break;
        }

        LabelEvolutionCost.text = "";
    }

    void EvolutionBtn_Click(GameObject button)
    {
        Debug.Log("基因进化处理");
        if (GeneSelected == null) return;
        if(StrategyPoint >= Formula.StrategyPointCal(GeneSelected) && GeneSelected.IsUpgradable && !GeneSelected.IsUpgraded)
        {
            //消耗策略点 consume sp
            StrategyPoint -= Formula.StrategyPointCal(GeneSelected);
            //效果生效 
            Formula.ResolveBattleUpgrade(GeneSelected.EventID);
            //标记基因的进化状态 change the state of gene
            GeneSelected.IsUpgraded = true;

            //记录已升级的Gene record upgraded gene
            switch (GeneSelected.BoardID)
            {
                case "1":
                    UpgradedGenes_Virus.Add(GeneSelected);
                    break;
                case "2":
                    UpgradedGenes_Human.Add(GeneSelected);
                    break;
                case "3":
                    UpgradedGenes_Zombie.Add(GeneSelected);
                    break;
                default:
                    break;
            }

            //更新数据状态 update data
            LoadBattleStrategy(GeneSelected.BoardID);
            //这里改变进化按钮的外观为不可用状态 set evolution button disable
            Formula.ChangeButtonDisable(button);
        }
    }

    //final plan of genetating humans
    void GenerateHuman()
    {

    }

    private void FixedUpdate()
    {
        if (BattleState == BattleState.Start)
        {
            Debug.Log("BattleState.Start");
            
        }

        if (BattleState == BattleState.Game)
        {
            updateInterval++;
            if(updateInterval == UPDATE_INDEX_GAMING)
            {
				Debug.Log ("updateInterval = " + updateInterval);
                //系统出现黄色DNA气泡计时器 dna timer
                deltaTime3 += Time.fixedDeltaTime * MAX_UPDATE_INTERVAL;
				//Debug.Log ("deltaTime3 = " + deltaTime3);
                if (deltaTime3 >= float.Parse(DataManager.Mission_Parameter[MissionID].EventMin) && deltaTime3 <= float.Parse(DataManager.Mission_Parameter[MissionID].EventMax))
                {
                    Debug.Log("进入随机出现气泡");
                    float random = UnityEngine.Random.Range(0.0f, 1.0f);
                    if (random > 0.1f && HumanArray.Count > 0)
                    {
                        //出现黄色气泡 yellow bubble
                        Debug.Log("出现黄色气泡");

                        int randomHuman = UnityEngine.Random.Range(0, HumanArray.Count);
						SP_Bubble.GetComponent<UISprite> ().width = (int)(BUBBLE_WIDTH * ModelAspect);
						SP_Bubble.GetComponent<UISprite> ().height = (int)(BUBBLE_HEIGHT * ModelAspect);
                        GameObject spBubble = NGUITools.AddChild(HumanArray[randomHuman] as GameObject, SP_Bubble);
                        UIEventListener.Get(spBubble).onClick = SPBubble_Click;
                        spBubble.transform.localPosition = Vector3.zero;

                        Destroy(spBubble, SPBUBBLEDIAPPEAR);
                        deltaTime3 = 0.0f;
                    }
                }
                else if (deltaTime3 >= float.Parse(DataManager.Mission_Parameter[MissionID].EventMax))
                {
                    deltaTime3 = 0.0f;
                }
            }

            if(updateInterval == UPDATE_INDEX_DATA)
            {
                //系统定时给予策略点计时器 sp timer
                deltaTime2 += Time.fixedDeltaTime * MAX_UPDATE_INTERVAL;

                if (deltaTime2 >= 2.0f)
                {
                    StrategyPoint += 1;
                    deltaTime2 = 0.0f;
                }

                //感染度总值达到一定值，就增加策略点 infection reaches a certain value,then increase sp
                TotalInfection = 0.0f;
                
                foreach (GameObject h in HumanArray)
                {
                    TotalInfection += h.GetComponent<Human>().Infection;
                }

                if (InfectionList.Count > 1)
                {
                    //从表的上方往下检查
                    if (TotalInfection >= float.Parse(InfectionList[1].TotalInfection))
                    {
                        SP_Add(int.Parse(InfectionList[1].GainSP), StrategyBtn, LabelStrategy, false);
                        InfectionList.RemoveAt(1);
                    }
                }
            }

            if(updateInterval == UPDATE_INDEX_SKILL)
            {
                //丧尸造成伤害达到一定值，就增加策略点 total dmg by zombies reaches a certain value,then increase sp
                TotalDamage = 0;

                foreach (GameObject h in HumanArray)
                {
                    TotalDamage += h.GetComponent<Human>().MaxHP - h.GetComponent<Human>().HP;
                }

                if (DamageList.Count > 1)
                {
                    //从表的上方往下检查
                    if (TotalDamage >= int.Parse(DamageList[1].TotalDamage))
                    {
                        SP_Add(int.Parse(DamageList[1].GainSP), StrategyBtn, LabelStrategy, false);
                        DamageList.RemoveAt(1);
                    }
                }

                LabelStrategyPoint.GetComponent<UILabel>().text = StrategyPoint.ToString();
            }

            if(updateInterval == UPDATE_INDEX_MEDICINE)
            {
                //解药 remedy
                if (Medicine >= MedicineWork)
                {
                    //治愈人类 cure the human
                    medicineOK = true;
                }

                MedicineBar.GetComponent<UISlider>().value = Medicine * 1.0f / MedicineWork;
                MediPercent.text = (int)(MedicineBar.GetComponent<UISlider>().value * 100) + "%";
            }

            if(updateInterval == UPDATE_INDEX_MODE)
            {
                //模式专属模块
                switch (mode)
                {
                    case Modes.Campaign:
                        bool anyInfected = true;
                        foreach (GameObject h in HumanArray)
                        {
                            if (h.GetComponent<Human>().Infected)
                            {
                                anyInfected = true;
                                break;
                            }
                            else
                            {
                                anyInfected = false;
                            }
                        }

                        if (ZombieArray.Count > 0 && HumanArray.Count == 0)         //如果所有人类都死亡，且还存有丧尸,玩家胜利
                        {
                            //玩家胜利 player win
                            Result = true;
                            Debug.Log("Win");
                            //标记关卡为通关
                            foreach (U_MissionFlag m in GameManager.user.DB_u_mf)
                            {
                                if (m.VirusID == VirusID && m.MissionID == MissionID)
                                {
                                    m.Flag = true;
                                    Debug.Log("m.VirusID = " + m.VirusID + ",  m.MissionID = " + m.MissionID);
                                    GameManager.SaveData();
                                    break;
                                }
                            }
							Conclude();

                            BattleState = BattleState.End;
                        }
                        else if (ZombieArray.Count == 0 && !anyInfected)                                          //如果丧尸全部死亡，且人类都没有被感染，则玩家失败
                        {
                            //玩家失败
                            Debug.Log("Lose:All Zombies Die and None Human is Infected");
                            Result = false;
                            Conclude();
                            BattleState = BattleState.End;
                        }
                        else if (medicineOK)                                                                      //如果研发出了解药，玩家失败
                        {
                            //玩家失败
                            Debug.Log("Lose:Medicine is Done");
                            Result = false;
                            Conclude();
                            BattleState = BattleState.End;
                        }
                        else
                        {
                            //Result = false;
                            //BattleState = BattleState.End;                                                           //任何其他情形，玩家失败
                        }

                        break;
                    case Modes.Trial:
                        break;
                    case Modes.Endless:
                        break;
                }
            }

            if (updateInterval == UPDATE_INDEX_TIMING)
            {
                //Debug.Log("BattleState.Game");
                
                //战斗用时计时器 battle timer
                updateInterval = UPDATE_INDEX_GAMING - 1;
                TimeSecond += Time.fixedDeltaTime * MAX_UPDATE_INTERVAL;
                Timer.text = "" + (int)TimeSecond + LocalizationEx.LoadLanguageTextName("Day");                
            }
            
        }

        if (BattleState == BattleState.End)
        {
            Debug.Log("BattleState.End");
            
        }
    }

    void Conclude()
    {
        //先清除数据
        VirusArray.Clear();
        ZombieArray.Clear();
        HumanArray.Clear();
        InfectionList.Clear();
        DamageList.Clear();
        UpgradedGenes_Virus.Clear();
        UpgradedGenes_Human.Clear();
        UpgradedGenes_Zombie.Clear();

        foreach (GameObject go in VirusGeneArray)
        {
            Destroy(go);
        }
        foreach (GameObject go in HumanGeneArray)
        {
            Destroy(go);
        }
        foreach (GameObject go in ZombieGeneArray)
        {
            Destroy(go);
        }

        VirusGeneArray.Clear();
        HumanGeneArray.Clear();
        ZombieGeneArray.Clear();

        //Formula.Btn_IsVisible(StrategyBtn, false);

        //Formula.Btn_IsVisible(SpeedBtn, false);
		StrategyBtn.SetActive (false);
		SpeedBtn.SetActive (false);

        //Use object pool to restore objects here, modify later
        //销毁所有人类和丧尸
        Transform[] children = Entity.GetComponentsInChildren<Transform>();

        //若不大于1，说明此时刚刚初始化
        if (children.Length > 1)
        {
            //从1开始，不要删除DNAGrid物体本身
            for (int i = 1; i < children.Length; i++)
            {
                Destroy(children[i].gameObject);
            }
        }
        EndBattleBtn.SetActive(true);
        //Formula.Btn_IsVisible(EndBattleBtn, true);
		ResultSprite.alpha = 1.0f;
        if (Result)
        {
            //EndBattleBtn.transform.GetChild(0).GetComponent<UILabel>().text = "You Win";
			ResultSprite.spriteName = "Victory";
        }
        else
        {
            //EndBattleBtn.transform.GetChild(0).GetComponent<UILabel>().text = "You Lose";
			ResultSprite.spriteName = "Defeat";
        }
    }

    public  void SP_Add(int sp_A,GameObject fatherObj,GameObject cordinateObj,bool isDisplayOnly)
    {
        GameObject lsp = NGUITools.AddChild(fatherObj, LabelSP);

        lsp.transform.localPosition = cordinateObj.transform.localPosition + new Vector3(0, 20);
        lsp.GetComponent<UILabel>().color = new Color(252.0f / 255.0f, 255.0f / 255.0f, 34.0f / 255.0f);

        lsp.GetComponent<UILabel>().text = "+ " + sp_A;
        Rigidbody rb = lsp.GetComponent<Rigidbody>();
        rb.velocity = new Vector3(0, 0.1f);
        Destroy(lsp, 1.0f);

        if (!isDisplayOnly)
        {
            //给StrategyBtn用时
            //要有逻辑变化时
            StrategyPoint += sp_A;
            
        }
        else
        {
            //气泡本身用时
            //无逻辑变化时
            fatherObj.GetComponent<UIButton>().state = UIButton.State.Disabled;
            fatherObj.GetComponent<BoxCollider>().enabled = false;
            Destroy(fatherObj,1.5f);
        }
    }

    public  void SP_Decrease(int sp_D, GameObject fatherObj, GameObject cordinateObj)
    {
        StrategyPoint -= sp_D;

        GameObject lsp = NGUITools.AddChild(fatherObj, LabelSP);

        lsp.transform.localPosition = cordinateObj.transform.localPosition + new Vector3(0, 20);
        lsp.GetComponent<UILabel>().color = new Color(1.0f, 0.0f, 0.0f);

        lsp.GetComponent<UILabel>().text = "- " + sp_D;
        Rigidbody rb = lsp.GetComponent<Rigidbody>();
        rb.velocity = new Vector3(0, 0.1f);
        Destroy(lsp, 1.0f);
    }

    void BattleEnd()
    {
        //胜负判断

        //胜负处理
    }

    //暂停面板处理
    public void PauseBtn_Click()
    {

    }

    //调节音量
    public void ChangeBGVolumeTo(float value)
    {

    }

    //开关音效
    public void ChangeMEToggle(bool value)
    {

    }

    //中途退出处理
    public void QuitMission()
    {

    }

    //继续按钮处理
    public void ContinueBtn_Click()
    {

    }

    public void SPBubble_Click(GameObject button)
    {
        SP_Add(SPBubble_SP, button, button.transform.Find("Cordinate").gameObject,true);
        SP_Add(SPBubble_SP, StrategyBtn, LabelStrategy,false);
    }

    public void SpeedBtn_Click(GameObject button)
    {
        Accelarate = !Accelarate;

        if (Accelarate)
        {
            LabelSpeed.text = "X 4";
            currentTimeScale = 4.0f;
            Time.timeScale = currentTimeScale;
        }
        else
        {
            LabelSpeed.text = "X 1";
            currentTimeScale = 1.0f;
            Time.timeScale = currentTimeScale;
        }
    }

    //正常结束，跳转到结算界面
    public void EndBattleBtn_Click(GameObject button)
    {
        Debug.Log("EndBattleBtn_Click");

        switch (Result)
        {
			case true:
				GameManager.ChangePanel(null, GameManager.UIS[GameManager.CAMPAIGNRESULT], 1);
				Destroy (gameObject);
				AudioManager.playMusicByName(AudioManager.MainBG);
				
                //GameManager.ChangePanel(GameManager.UIS[GameManager.BATTLE], GameManager.UIS[GameManager.CAMPAIGNRESULT], 1);
                break;
            case false:
				GameManager.ChangePanel(null, GameManager.UIS[GameManager.CAMPAIGNRESULT], 0);
				Destroy (gameObject);
				AudioManager.playMusicByName(AudioManager.MainBG);

                //GameManager.ChangePanel(GameManager.UIS[GameManager.BATTLE], GameManager.UIS[GameManager.CAMPAIGNRESULT], 0);
                break;
        }

        AudioManager.playMusicByName(AudioManager.MainBG);
    }

    //点击策略加点按钮
    public void StrategyBtn_Click(GameObject button)
    {
        Debug.Log("StrategyBtn_Click");
        InGameUpgradePanel.SetActive(true);
        //Formula.Btn_IsVisible(InGameUpgradePanel, true);

        Time.timeScale = 0;
        LoadBattleStrategy("1");
    }

    //点击病毒升级按钮
    public void VirusUpBtn_Click(GameObject button)
    {
        LoadBattleStrategy("1");
    }

    //点击病毒升级按钮
    public void HumanUpBtn_Click(GameObject button)
    {
        LoadBattleStrategy("2");
    }

    //点击病毒升级按钮
    public void ZombieUpBtn_Click(GameObject button)
    {
        LoadBattleStrategy("3");
    }

    //关闭策略面板按钮
    public void StrategyCloseBtn_Click(GameObject button)
    {
        Debug.Log("StrategyCloseBtn_Click");
        InGameUpgradePanel.SetActive(false);
        //Formula.Btn_IsVisible(InGameUpgradePanel, false);
        Time.timeScale = currentTimeScale;
    }
}
