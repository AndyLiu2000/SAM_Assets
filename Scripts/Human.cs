using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SEType
{
    Skill,
    Die
}

public class Human : MonoBehaviour{

    const int KILL_HUMAN_SP = 5;
    public const int INFECT_HUMAN_SP = 4;
    const float INFECT_BTN_DISAPPEAR = 1.0f;
    //const int INFECTION_THRESHOLD = 30; //传染的感染度临界值
    const float HEAL_INTERVAL = 5.0f;
    const float INFECT_INTERVAL = 1.0f;
    const float COMMUNICATE_INTERVAL = 2.0f;
    const float ATTACK_INTERVAL = 5.0f;
    const int MAX_UPDATE_INTERVAL = 6;
    const int UPDATE_INDEX_SELF = 0;
    const int UPDATE_INDEX_DATA = 1;
    const int UPDATE_INDEX_SKILL = 2;
    const int UPDATE_INDEX_MEDICINE = 3;
    const int UPDATE_INDEX_MODE = 4;
    const int UPDATE_INDEX_TIMING = 5;
	float DELTA_TIME_INTERVAL = 0;
    int updateInterval = 0;

    //Model：Human
    public int HumanID;
    public int MaxHP;
    public int MaxInfection;
    public int Atk;
    public int Heal;
    public int Def;
    public int Cure;
    public int InfectShield;
    public int InfectionAnti;
	public int ResearchStart;
	public int AttackInterval;
    public int CommunicationAnti;
    public int HPHealing;
    public string Name;
    public string Res;
    public string SkillID;

    //次级属性
    public int param;

    //战斗变量
    public int HP;
    public int Infection;
    public int ClimateBoost = 0;
    public int EnviBoost = 0;
    public Environment Envi;
    public Climate Clim;

    public bool Infected = false;

    //预制体相关
    public GameObject ZombieModel;
    GameObject Entity;
    public UISprite Image;
    public GameObject HPBar;
    public GameObject InfectionBar;
    public UISprite ClimIcon;
    public UISprite EnviIcon;
    public UISprite SkillIcon;
    public GameObject StartBubble;
    IEnumerator DestroySEParam;
    public GameObject skillSEGO;
    public GameObject dieSEGO;

    //环境变量
    Battle_C Battle;
    float healDeltaTime = 0;
    float skillDeltaTime = 0;
    float infectDeltaTime = 0;
    float communicateDeltaTime = 0;
	float infectedDuration = 0;
    Human self;

    private void Awake()
    {
        Battle = GameObject.Find(GameManager.BATTLE).GetComponent<Battle_C>();
        //GetParam();

    }

    private void Start()
    {
        self = gameObject.GetComponent<Human>();
        Entity = GameObject.Find("Entity");

        UIEventListener.Get(StartBubble).onClick = InfectBtn_Click;

		DELTA_TIME_INTERVAL = Time.fixedDeltaTime * MAX_UPDATE_INTERVAL;
    }

    private void FixedUpdate()
    {
        if (Battle.BattleState == BattleState.Start)
        {

        }

        if (Battle.BattleState == BattleState.Game)
        {
            updateInterval++;
            if(updateInterval == UPDATE_INDEX_SELF)
            {
				healDeltaTime += DELTA_TIME_INTERVAL;

                //回血 restore
                if (self.HP < self.MaxHP)
                {
                    if (healDeltaTime >= HEAL_INTERVAL)
                    {
                        self.HP += HPHealing;
                        healDeltaTime = 0.0f;
                    }
                }

                //人类死亡 human die
                if (self.HP <= 0)
                {
                    Battle.ZombieKillNum += 1;
                    HumanDie();
                }
            }

            if (updateInterval == UPDATE_INDEX_DATA)
            {
				infectDeltaTime += DELTA_TIME_INTERVAL;
				communicateDeltaTime += DELTA_TIME_INTERVAL;

                //感染 infect
                if (self.Infected)
                {
					//被感染时一直累积时间，当被感染时间过长时，自动开始研发解药
					infectedDuration += DELTA_TIME_INTERVAL;

                    if (infectDeltaTime >= INFECT_INTERVAL)
                    {
                        //感染加深
                        int infect = Battle.CurVirus.InfectSpeed
                            * (1000 + Battle.CurVirus.InfectHumans[HumanID - 1]) / 1000
                            * (1000 + Battle.CurVirus.InfectClims[(int)Clim]) / 1000
                            * (1000 + Battle.CurVirus.InfectEnvis[(int)Envi]) / 1000
                            - InfectionAnti;
                        Infection += infect > 0 ? infect : 0;
                        InfectionBar.GetComponent<UISlider>().value = (float)(Infection * 1.0f / MaxInfection);

                        //解药研发 research
                        //Debug.Log("zuobian = " + Infection * 1000 / Battle.CurVirus.Medi_Start + ", youbian = " + MaxInfection);
						if (Infection * 1000 / Battle.CurVirus.Medi_Start >= MaxInfection) {
							Battle.Medicine += Battle.MEDICINESPD * 1000 / Battle.CurVirus.Medi_Spd;
						} else if(infectedDuration >= ResearchStart){
							Battle.Medicine += Battle.MEDICINESPD * 1000 / Battle.CurVirus.Medi_Spd;
						}
                    }

                    //感染度为满，人类死亡，变成丧尸 if infection exceeds maxinfection, then human dies and turns into zombie
                    if (Infection >= MaxInfection)
                    {
                        Battle.InfectKillNum += 1;

                        HumanDie();

                        //后面的代码全部不再执行
                        //return;
                    }

                    //传染，大于临界值才开始传染
                    if (MaxInfection <= Battle.CurVirus.CommunicationThreshold * Infection)
                    {
                        //Battle.CurVirus.CommunicateRate用在这里，缩短传染的时间间隔
                        if (communicateDeltaTime >= COMMUNICATE_INTERVAL * 1000 / (1000 + Battle.CurVirus.CommunicateRate))
                        {
                            List<GameObject> unInfectedHumans = new List<GameObject>();
                            foreach (GameObject h in Battle.HumanArray)
                            {
                                if (h.GetComponent<Human>().Infected == false)
                                {
                                    //取得所有未感染的人类
                                    unInfectedHumans.Add(h);
                                }
                            }

                            if (unInfectedHumans.Count > 0)
                            {
                                Debug.Log("找随机目标传染一次");
                                GameObject unInfectedHuman = Formula.ListRandomElement(unInfectedHumans);
                                int random = UnityEngine.Random.Range(0, 1000);
                                int c = CommunicationAnti
                                    * (1000 + Battle.CurVirus.CommunicateHumans[HumanID - 1]) / 1000
                                    * (1000 + Battle.CurVirus.CommunicateClimates[(int)Clim]) / 1000
                                    * (1000 + Battle.CurVirus.CommunicateEnvis[(int)Envi]) / 1000;
                                //Debug.Log("random = " + random + ", c = " + c);
                                //随机数大于抗传染概率时，才成功传染
                                if (random > CommunicationAnti
                                    * (1000 + Battle.CurVirus.CommunicateHumans[HumanID - 1]) / 1000
                                    * (1000 + Battle.CurVirus.CommunicateClimates[(int)Clim]) / 1000
                                    * (1000 + Battle.CurVirus.CommunicateEnvis[(int)Envi]) / 1000)
                                {
                                    unInfectedHuman.GetComponent<Human>().InfectShield -= Battle.CurVirus.InfectSpeed;
                                    if (unInfectedHuman.GetComponent<Human>().InfectShield < 0)
                                    {
                                        unInfectedHuman.GetComponent<Human>().InfectShield = 0;
                                        unInfectedHuman.GetComponent<Human>().Infected = true;
                                        Debug.Log("传染一个新人类");
                                        Battle.InfectNum += 1;
                                        Battle.SP_Add(INFECT_HUMAN_SP, Battle.StrategyBtn, Battle.LabelStrategy, false);
                                    }
                                    //Debug.Log("InfectShield = " + unInfectedHuman.GetComponent<Human>().InfectShield);
                                }

                            }

                        }
                    }
						
                }
                //按秒执行操作
                if (infectDeltaTime >= INFECT_INTERVAL)
                {
                    infectDeltaTime = 0.0f;
                }

                if (communicateDeltaTime >= COMMUNICATE_INTERVAL * 1000 / (1000 + Battle.CurVirus.CommunicateRate))
                {
                    communicateDeltaTime = 0.0f;
                }
            }

            if (updateInterval == UPDATE_INDEX_SKILL)
            {
				skillDeltaTime += DELTA_TIME_INTERVAL;
				Debug.Log ("AttackInterval = " + AttackInterval);
                //每5秒使用一次技能 cast skill every 5 seconds
				if (skillDeltaTime * 1000 >= AttackInterval)
                {
                    switch (SkillID)
                    {
                        case "1":   //随机单体攻击
                            RandomSingleAttack();
                            break;
                        case "2":   //治愈自身感染度
                            CureSelfInfection();
                            break;
                        case "3":   //随机单体治疗
                            RandomSingleHeal();
                            break;
                        case "4":   //随机三目标治疗
                            RandomSingleHeal();
                            RandomSingleHeal();
                            RandomSingleHeal();
                            break;
                        case "5":   //随机群体治疗生命值和感染度
                            RandomSingle_HealCure();
                            break;
                    }

                    skillDeltaTime = 0.0f;
                }
            }

            if (updateInterval == UPDATE_INDEX_MEDICINE)
            {
                //解药相关 medicine
                if (Battle.medicineOK == true)
                {
                    self.Infected = false;
                }
            }

            if (updateInterval == UPDATE_INDEX_MODE)
            {

            }

            if (updateInterval == UPDATE_INDEX_TIMING)
            {
                updateInterval = UPDATE_INDEX_SELF - 1;
            }
            
        }

        if (Battle.BattleState == BattleState.End)
        {
            
        }

    }

    void RandomSingleAttack()
    {
        Debug.Log("Human-RandomSingleAttack");
        if (Battle.ZombieArray.Count > 0)
        {
            GameObject zombie = Formula.ArrayListRandomElement(Battle.ZombieArray) as GameObject;
            Zombie aZombie = zombie.GetComponent<Zombie>();

            if (Atk * param / 1000 >= aZombie.Def)
            {
                aZombie.HP -= (int)(Atk * param / 1000 - aZombie.Def);
                GenerateSEInGameobjectPosition(zombie, SEType.Skill, true, null);
            }
        }
    }

    void HumanDie()
    {
        GenerateSEInGameobjectPosition(gameObject, SEType.Die, false, "GenerateZombie");
    }

    void CureSelfInfection()
    {
        //Debug.Log("Human-CureSelfInfection");

        Infection += Cure * param / 1000;
        if (Infection <= 0)
            Infection = 0;

        GenerateSEInGameobjectPosition(gameObject, SEType.Skill, true, null);
    }

    void GenerateSEInGameobjectPosition(GameObject go, SEType seType, bool isSelfActive,string invokeName)
    {
        //GameObject se = NGUITools.AddChild(Battle.Entity, (GameObject)(Resources.Load("SEPrefabs" + "/" + seName)));
        GameObject se = null;
        switch (seType)
        {
            case SEType.Skill:
                se = NGUITools.AddChild(Battle.Entity, skillSEGO);
                break;
            case SEType.Die:
                se = NGUITools.AddChild(Battle.Entity, dieSEGO);
                break;
        }
        se.transform.localScale = new Vector3(80, 80, 1);        //该死的Unity，把动画文件加载的时候默认缩小为1/100了，所以这里要扩大100倍。注意，改Prefabs的缩放比例是没用的
        NGUITools.SetDirty(Battle.Entity);
        Transform desGO = go.GetComponent<Transform>();
        se.transform.localPosition = desGO.localPosition;

        //go.SetActive(isSelfActive);
        Formula.Btn_IsVisible(go, isSelfActive);
        if (invokeName != null)
            Invoke(invokeName, se.GetComponent<Animator>().runtimeAnimatorController.animationClips[0].length);

        Destroy(se, se.GetComponent<Animator>().runtimeAnimatorController.animationClips[0].length);
    }

    void RandomSingleHeal()
    {
        //Debug.Log("Human-RandomSingleHeal");
        if (Battle.HumanArray.Count > 0)
        {
            GameObject human = Formula.ArrayListRandomElement(Battle.HumanArray) as GameObject;
            Human aHuman = human.GetComponent<Human>();
            aHuman.HP += Heal * param / 1000;
            if (aHuman.HP >= aHuman.MaxHP)
                aHuman.HP = aHuman.MaxHP;

            GenerateSEInGameobjectPosition(human, SEType.Skill, true, null);
        }
    }

    void RandomSingle_HealCure()
    {
        //Debug.Log("Human-RandomSingle_HealCure");
        if (Battle.HumanArray.Count > 0)
        {
            GameObject human = Formula.ArrayListRandomElement(Battle.HumanArray) as GameObject;
            Human aHuman = human.GetComponent<Human>();

            aHuman.HP += Heal * param / 1000;
            if (aHuman.HP >= aHuman.MaxHP)
                aHuman.HP = aHuman.MaxHP;

            aHuman.Infection -= Cure * param / 1000;
            if (aHuman.Infection <= 0)
                aHuman.Infection = 0;

            GenerateSEInGameobjectPosition(human, SEType.Skill, true, null);
        }
    }

    void GenerateZombie()
    {
        //GenerateDestroySE();
        //杀死人类获得SP
        Battle.SP_Add(KILL_HUMAN_SP, Battle.StrategyBtn, Battle.LabelStrategy, false);

        //生成丧尸，只能从已解锁的丧尸ID中随机。通过IAP可以购买新的丧尸
        int randomZombieID = Formula.RandomZombie();

        ZombieModel.GetComponent<Zombie>().CreateZombie(randomZombieID, Battle.MissionID, Clim, Envi);

		int battleBGHeight = GameObject.Find ("BattleBG").GetComponent<UISprite> ().height; 

		int width = (int)(Battle.HUMANMODEL_WIDTH * Battle.ModelAspect);
		int height = (int)(Battle.HUMANMODEL_HEIGHT * Battle.ModelAspect);

		ZombieModel.transform.GetChild (0).GetComponent<UISprite> ().width = width;
		ZombieModel.transform.GetChild (0).GetComponent<UISprite> ().height = height;

        GameObject zm = NGUITools.AddChild(Entity, ZombieModel);
        zm.transform.localPosition = gameObject.transform.localPosition;

        NGUITools.SetDirty(Entity);

        Battle.ZombieArray.Add(zm);

        Battle.HumanArray.Remove(gameObject);
        Destroy(gameObject);
    }

    public void CreateHuman(int curMissionID)
    {
        //数据初始化
        //Model - DNAUp + Mission

        //随机HumanID
        HumanID = Formula.RandomHuman();

        Human_Sheet human = new Human_Sheet();
        foreach(Human_Sheet h in DataManager.Model_Human)
        {
            if (h.HumanID == HumanID.ToString())
            {
                human = h;
                break;
            }
        }

        //Model值
        MaxHP = int.Parse(human.MaxHP);
        MaxInfection = int.Parse(human.MaxInfection);
        Atk = int.Parse(human.Atk);
        Heal = int.Parse(human.Heal);
        Def = int.Parse(human.Def);
        Cure = int.Parse(human.Cure);
        InfectShield = int.Parse(human.InfectShield);
        InfectionAnti = int.Parse(human.InfectionAnti);
		ResearchStart = int.Parse(human.ResearchStart) / 1000;
		AttackInterval = int.Parse(human.AttackInterval);
        CommunicationAnti = int.Parse(human.CommunicationAnti);
        HPHealing = int.Parse(human.HPHealing);

        //根据随机分配到的气候和环境来决定使用哪个参数

        //随机气候        
        Clim = (Climate)UnityEngine.Random.Range(0, Enum.GetNames(Type.GetType("Climate")).Length);

        switch (Clim)
        {
            case Climate.Dry:
                ClimateBoost = int.Parse(human.ClimateBoost_1);
                break;
            case Climate.Wet:
                ClimateBoost = int.Parse(human.ClimateBoost_2);
                break;
            case Climate.Normal:
                ClimateBoost = int.Parse(human.ClimateBoost_3);
                break;
        }

        //随机环境 random environment
        Envi = (Environment)UnityEngine.Random.Range(0, Enum.GetNames(Type.GetType("Environment")).Length);

        switch (Envi)
        {
            case Environment.Hot:
                EnviBoost = int.Parse(human.EnviBoost_1);
                break;
            case Environment.Cold:
                EnviBoost = int.Parse(human.EnviBoost_2);
                break;
            case Environment.Balance:
                EnviBoost = int.Parse(human.EnviBoost_3);
                break;
        }

        SkillID = human.SkillID;

        //DNA值
        MaxHP = MaxHP * 1000 / (1000 + Formula.FieldNameToValue("MaxHP", DataManager.DNAUp_Human, GameManager.user.DB_u_dna[1]));
        MaxInfection = MaxInfection * 1000 / (1000 + Formula.FieldNameToValue("MaxInfection", DataManager.DNAUp_Human, GameManager.user.DB_u_dna[1]));
        Atk = Atk * 1000 / (1000 + Formula.FieldNameToValue("Atk", DataManager.DNAUp_Human, GameManager.user.DB_u_dna[1]));
        Heal = Heal * 1000 / (1000 + Formula.FieldNameToValue("Heal", DataManager.DNAUp_Human, GameManager.user.DB_u_dna[1]));
        Def = Def * 1000 / (1000 + Formula.FieldNameToValue("Def", DataManager.DNAUp_Human, GameManager.user.DB_u_dna[1]));
        Cure = Cure * 1000 / (1000 + Formula.FieldNameToValue("Cure", DataManager.DNAUp_Human, GameManager.user.DB_u_dna[1]));
        InfectShield = InfectShield * 1000 / (1000 + Formula.FieldNameToValue("InfectShield", DataManager.DNAUp_Human, GameManager.user.DB_u_dna[1]));
        InfectionAnti = InfectionAnti * 1000 / (1000 + Formula.FieldNameToValue("InfectionAnti", DataManager.DNAUp_Human, GameManager.user.DB_u_dna[1]));
		AttackInterval = AttackInterval * 1000 / (1000 + Formula.FieldNameToValue("AttackInterval", DataManager.DNAUp_Human, GameManager.user.DB_u_dna[1]));
        CommunicationAnti = CommunicationAnti * 1000 / (1000 + Formula.FieldNameToValue("CommunicationAnti", DataManager.DNAUp_Human, GameManager.user.DB_u_dna[1]));
        HPHealing = HPHealing * 1000 / (1000 + Formula.FieldNameToValue("HPHealing", DataManager.DNAUp_Human, GameManager.user.DB_u_dna[1]));

        //根据随机分配到的气候和环境来决定使用哪个参数
        switch (Clim)
        {
            case Climate.Dry:
                ClimateBoost += Formula.FieldNameToValue("ClimateBoost_1", DataManager.DNAUp_Human, GameManager.user.DB_u_dna[1]);
                break;
            case Climate.Wet:
                ClimateBoost += Formula.FieldNameToValue("ClimateBoost_2", DataManager.DNAUp_Human, GameManager.user.DB_u_dna[1]);
                break;
            case Climate.Normal:
                ClimateBoost += Formula.FieldNameToValue("ClimateBoost_3", DataManager.DNAUp_Human, GameManager.user.DB_u_dna[1]);
                break;
        }

        switch (Envi)
        {
            case Environment.Hot:
                EnviBoost += Formula.FieldNameToValue("EnviBoost_1", DataManager.DNAUp_Human, GameManager.user.DB_u_dna[1]);
                break;
            case Environment.Cold:
                EnviBoost += Formula.FieldNameToValue("EnviBoost_2", DataManager.DNAUp_Human, GameManager.user.DB_u_dna[1]);
                break;
            case Environment.Balance:
                EnviBoost += Formula.FieldNameToValue("EnviBoost_3", DataManager.DNAUp_Human, GameManager.user.DB_u_dna[1]);
                break;
        }

        UpdateAttributes(curMissionID);

        HP = MaxHP;
        Infection = 0;
        Infected = false;

        //预制体初始化
        Image.spriteName = human.Res;
        HPBar.GetComponent<UISlider>().value = (float)(HP * 1.0f/MaxHP);
        InfectionBar.GetComponent<UISlider>().value = (float)(Infection * 1.0f / MaxInfection);
        ClimIcon.spriteName = Formula.ClimateIcon(ref ClimIcon, Clim);
        EnviIcon.spriteName = Formula.EnviIcon(ref ClimIcon, Envi);

        foreach (SpecialAbility_Sheet sas in DataManager.SpecialAbility_Ability)
        {
            if (sas.ID == SkillID)
            {
                SkillIcon.spriteName = sas.ResIcon;
                param = int.Parse(sas.Value1) + int.Parse(sas.Value1_Add);
                //特效预加载
                skillSEGO = (GameObject)(Resources.Load("SEPrefabs" + "/" + sas.SEName));
                dieSEGO = (GameObject)(Resources.Load("SEPrefabs" + "/" + "HumanDie"));
                break;
            }
        }
    }

    public void UpdateAttributes(int curMissionID)
    {
        //Mission值
        Mission_Sheet mission = new Mission_Sheet();
        foreach (Mission_Sheet m in DataManager.Mission_Parameter)
        {
            if (m.MissionID == curMissionID.ToString())
            {
                mission = m;
                break;
            }
        }

        int missionBoost = int.Parse(mission.ClimateBoost) + int.Parse(mission.EnviBoost);

        MaxHP = MaxHP * (1000 + int.Parse(mission.MaxHPBoost)) / 1000 * (1000 + missionBoost + ClimateBoost + EnviBoost) / 1000;
        MaxInfection = MaxInfection * (1000 + int.Parse(mission.InfectionBoost)) / 1000 * (1000 + missionBoost + ClimateBoost + EnviBoost) / 1000;
        Atk = Atk * (1000 + int.Parse(mission.Atk_Boost)) / 1000 * (1000 + missionBoost + ClimateBoost + EnviBoost) / 1000;
        Heal = Heal * (1000 + int.Parse(mission.Heal_Boost)) / 1000 * (1000 + missionBoost + ClimateBoost + EnviBoost) / 1000;
        Def = Def * (1000 + int.Parse(mission.Def_Boost)) / 1000 * (1000 + missionBoost + ClimateBoost + EnviBoost) / 1000;
        Cure = Cure * (1000 + int.Parse(mission.Cure_Boost)) / 1000 * (1000 + missionBoost + ClimateBoost + EnviBoost) / 1000;
        InfectShield = InfectShield * (1000 + int.Parse(mission.Speed_Boost)) / 1000 * (1000 + missionBoost + ClimateBoost + EnviBoost) / 1000;
        InfectionAnti = InfectionAnti * (1000 + int.Parse(mission.InfectionAntiBoost)) / 1000 * (1000 + missionBoost + ClimateBoost + EnviBoost) / 1000;
        CommunicationAnti = CommunicationAnti * (1000 + int.Parse(mission.CommunicationAntiBoost)) / 1000 * (1000 + missionBoost + ClimateBoost + EnviBoost) / 1000;
        HPHealing = HPHealing * (1000 + int.Parse(mission.HPHealingBoost)) / 1000 * (1000 + missionBoost + ClimateBoost + EnviBoost) / 1000;
    }

    public Human HumanBattleEvent()
    {
        //Human基础值 + 事件影响值
        return this;
    }

    public void HumanBtn_Click()
    {
        if (Battle.BattleState == BattleState.Start)
        {
            Debug.Log("感染按钮");

            //把所有其他人类上的感染按钮消除
            foreach (GameObject h in Battle.HumanArray)
            {
                if(h.transform.Find("InfectBtn") != null)
                {
                    Destroy(h.transform.Find("InfectBtn").gameObject);
                }
            }
			Debug.Log ("Battle.BUBBLE_WIDTH = " + Battle.BUBBLE_WIDTH);
			StartBubble.GetComponent<UISprite> ().width = (int)(Battle.BUBBLE_WIDTH * Battle.ModelAspect);
			StartBubble.GetComponent<UISprite> ().height = (int)(Battle.BUBBLE_HEIGHT * Battle.ModelAspect);
			Debug.Log ("width = " + StartBubble.GetComponent<UISprite> ().width);
            GameObject startBubble = NGUITools.AddChild(gameObject, StartBubble);
            UIEventListener.Get(startBubble).onClick = InfectBtn_Click;
            startBubble.transform.localPosition = Vector3.zero;
        }
    }

    public void InfectBtn_Click(GameObject button)
    {
        if (Battle.BattleState == BattleState.Start)
        {
            if (Battle.VirusNum > 0)
            {
                //如果未被感染，则将病毒附着于本人类上 if no one is infected,then pick one to infect
                Debug.Log("有感染种子");
                if (!self.Infected)
                {
                    Debug.Log("感染成功");
                    self.Infected = true;
                    Battle.InfectNum += 1;
                    Battle.VirusNum -= 1;
                    Battle.SP_Add(INFECT_HUMAN_SP, Battle.StrategyBtn, Battle.LabelStrategy, false);

                    button.GetComponent<UIButton>().state = UIButton.State.Disabled;
                    button.GetComponent<BoxCollider>().enabled = false;

                    Destroy(button, INFECT_BTN_DISAPPEAR);
                    Battle.BattleState = BattleState.Game;
                }
            }
        }
    }
}
