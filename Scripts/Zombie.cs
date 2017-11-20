using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Zombie : MonoBehaviour{

    private float ATTACK_INTERVAL = 5.0f;
    private float HEALTH_INTERVAL = 5.0f;
    const int MAX_UPDATE_INTERVAL = 6;
    const int UPDATE_INDEX_SELF = 0;
    const int UPDATE_INDEX_DATA = 1;
    const int UPDATE_INDEX_SKILL = 2;
    const int UPDATE_INDEX_MEDICINE = 3;
    const int UPDATE_INDEX_MODE = 4;
    const int UPDATE_INDEX_TIMING = 5;
    int updateInterval = 0;

    //Model：丧尸
    public float ZombieID;
    public int MaxHP;
    public int Atk;
    public int Heal;
    public int Def;
    public int Infect;
    public int Speed;
    public int HPDecay;
    public int DrainLife;
    public string AbilityID;
	public int AttackInterval;
    public string Name;
    public string Res;
    public string SkillID;

    //次级属性
    public int param;

    //战斗变量
    public int HP;
    public int ClimateBoost = 0;
    public int EnviBoost = 0;
    public Environment Envi;
    public Climate Clim;

    //预制体相关
    public GameObject HumanModel;
    private Zombie self;
    public UISprite Image;
    public GameObject HPBar;
    public UISprite ClimIcon;
    public UISprite EnviIcon;
    public UISprite SkillIcon;
    Battle_C Battle;
    public GameObject skillSEGO;
    public GameObject dieSEGO;

    //环境变量
    float healDeltaTime = 0;
    float skillDeltaTime = 0;
    

    private void Start()
    {
        self = gameObject.GetComponent<Zombie>();
        Battle = GameObject.Find(GameManager.BATTLE).GetComponent<Battle_C>();
        //GetParam();
    }

    private void FixedUpdate()
    {
        if (Battle.BattleState == BattleState.Start)
        {
            //开始阶段的游戏提示显示
        }

        if (Battle.BattleState == BattleState.Game)
        {
            updateInterval++;
            if(updateInterval == UPDATE_INDEX_SELF)
            {
                healDeltaTime += Time.fixedDeltaTime * MAX_UPDATE_INTERVAL;
                //失血
                if (healDeltaTime >= HEALTH_INTERVAL)
                {
                    self.HP -= HPDecay;
                    HPBar.GetComponent<UISlider>().value = (float)(HP * 1.0f / MaxHP);
                }

                //丧尸死亡
                if (self.HP <= 0)
                {
                    ZombieDie();
                }

                //按秒执行操作
                if (healDeltaTime >= HEALTH_INTERVAL)
                {
                    healDeltaTime = 0.0f;
                }
            }

            if(updateInterval == UPDATE_INDEX_DATA)
            {

            }

            if(updateInterval == UPDATE_INDEX_SKILL)
            {
                skillDeltaTime += Time.fixedDeltaTime * MAX_UPDATE_INTERVAL;
				Debug.Log ("AttackInterval Zombie = " + AttackInterval);
				if (skillDeltaTime * 1000 >= AttackInterval) 
                {
                    switch (SkillID)
                    {
                        case "100":   //随机单体攻击
                            RandomSingleAttack();
                            break;
                        case "200":   //随机群体攻击
                            RandomSingleAttack();
                            RandomSingleAttack();
                            RandomSingleAttack();
                            break;
                        case "300":   //随机单体治疗
                            RandomSingleHeal();
                            break;
                        case "400":   //随机三目标治疗
                            RandomSingleHeal();
                            RandomSingleHeal();
                            RandomSingleHeal();
                            break;
                        case "500":   //随机单体攻击和增加感染度
                            RandomSingle_AttackInfect();
                            break;
                        case "600":   //随机群体攻击和增加感染度
                            RandomSingle_AttackInfect();
                            RandomSingle_AttackInfect();
                            RandomSingle_AttackInfect();
                            break;
                    }
                }

                if (skillDeltaTime >= ATTACK_INTERVAL)
                {
                    skillDeltaTime = 0.0f;
                }
            }

            if(updateInterval == UPDATE_INDEX_MEDICINE)
            {

            }

            if(updateInterval == UPDATE_INDEX_MODE)
            {

            }

            if(updateInterval == UPDATE_INDEX_TIMING)
            {
                updateInterval = UPDATE_INDEX_SELF;
            }
            
        }

        if (Battle.BattleState == BattleState.End)
        {

        }

    }

    void RandomSingleAttack()
    {
        //Debug.Log("Zombie-RandomSingleAttack");
        if (Battle.HumanArray.Count > 0)
        {
            GameObject human = Formula.ArrayListRandomElement(Battle.HumanArray) as GameObject;
            Human aHuman = human.GetComponent<Human>();
            if (Atk * param / 1000 >= aHuman.Def)
            {
                int deltaHumanHP = (int)(Atk * param / 1000 - aHuman.Def);

                aHuman.HP -= deltaHumanHP;      //人类失血

                HP += deltaHumanHP * DrainLife / 1000;  //丧尸吸血
                if (HP > MaxHP)
                    HP = MaxHP;
                GenerateSEInGameobjectPosition(human, SEType.Skill, true, null);
            }
        }
    }

    void GenerateSEInGameobjectPosition(GameObject go, SEType seType, bool isSelfActive, string invokeName)
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
        //Debug.Log("Zombie-RandomSingleHeal");
        if (Battle.ZombieArray.Count > 0)
        {
            GameObject zombie = Formula.ArrayListRandomElement(Battle.ZombieArray) as GameObject;
            Zombie aZombie = zombie.GetComponent<Zombie>();
            aZombie.HP += Heal * param / 1000;
            if (aZombie.HP >= aZombie.MaxHP)
                aZombie.HP = aZombie.MaxHP;

            GenerateSEInGameobjectPosition(zombie, SEType.Skill, true, null);
        }
    }

    void RandomSingle_AttackInfect()
    {
        //Debug.Log("Zombie-RandomSingle_AttackInfect");
        if (Battle.HumanArray.Count > 0)
        {
            GameObject human = Formula.ArrayListRandomElement(Battle.HumanArray) as GameObject;
            Human aHuman = human.GetComponent<Human>();

            if (Atk * param / 1000 >= aHuman.Def)
                aHuman.HP -= (int)(Atk * param / 1000 - aHuman.Def);

            aHuman.Infection += Infect * param / 1000;
            if (aHuman.Infection >= aHuman.MaxInfection)
                aHuman.Infection = aHuman.MaxInfection;

            GenerateSEInGameobjectPosition(human, SEType.Skill, true, null);
        }
    }

    void ZombieDie()
    {
        GenerateSEInGameobjectPosition(gameObject, SEType.Die, false, "ZombieVanish");
    }

    void ZombieVanish()
    {
        Battle.ZombieArray.Remove(gameObject);
        Destroy(gameObject);
    }

    public void CreateZombie(int zombieID, int curMissionID, Climate clim, Environment envi)
    {
        //Model - DNAUp + Mission
        ZombieID = zombieID;
        Zombie_Sheet zombie = new Zombie_Sheet();
        foreach (Zombie_Sheet z in DataManager.Model_Zombie)
        {
            if (z.ZombieID == ZombieID.ToString())
            {
                zombie = z;
                break;
            }
        }

        //Model值
        MaxHP = int.Parse(zombie.MaxHP);
        Atk = int.Parse(zombie.Atk);
        Heal = int.Parse(zombie.Heal);
        Def = int.Parse(zombie.Def);
        Infect = int.Parse(zombie.Infect);
        Speed = int.Parse(zombie.Speed);
        HPDecay = int.Parse(zombie.HPDecay);
        DrainLife = int.Parse(zombie.DrainLife);
		AttackInterval = int.Parse(zombie.AttackInterval);
        Clim = clim;
        Envi = envi;

        //根据随机分配到的气候和环境来决定使用哪个参数
        switch (clim)
        {
            case Climate.Dry:
                ClimateBoost = int.Parse(zombie.ClimateBoost_1);
                break;
            case Climate.Wet:
                ClimateBoost = int.Parse(zombie.ClimateBoost_2);
                break;
            case Climate.Normal:
                ClimateBoost = int.Parse(zombie.ClimateBoost_3);
                break;
        }

        switch (envi)
        {
            case Environment.Hot:
                EnviBoost = int.Parse(zombie.EnviBoost_1);
                break;
            case Environment.Cold:
                EnviBoost = int.Parse(zombie.EnviBoost_2);
                break;
            case Environment.Balance:
                EnviBoost = int.Parse(zombie.EnviBoost_3);
                break;
        }

        SkillID = zombie.SkillID;

        //DNA值
        MaxHP = MaxHP * (1000 + Formula.FieldNameToValue("MaxHP",DataManager.DNAUp_Zombie, GameManager.user.DB_u_dna[2])) / 1000;
        Atk = Atk * (1000 + Formula.FieldNameToValue("Atk", DataManager.DNAUp_Zombie, GameManager.user.DB_u_dna[2])) / 1000;
        Heal = Heal * (1000 + Formula.FieldNameToValue("Heal", DataManager.DNAUp_Zombie, GameManager.user.DB_u_dna[2])) / 1000;
        Def = Def * (1000 + Formula.FieldNameToValue("Def", DataManager.DNAUp_Zombie, GameManager.user.DB_u_dna[2])) / 1000;
        Infect = Infect * (1000 + Formula.FieldNameToValue("Infect", DataManager.DNAUp_Zombie, GameManager.user.DB_u_dna[2])) / 1000;
        Speed = Speed * (1000 + Formula.FieldNameToValue("Speed", DataManager.DNAUp_Zombie, GameManager.user.DB_u_dna[2])) / 1000;
        HPDecay = HPDecay * (1000 + Formula.FieldNameToValue("HPDecay", DataManager.DNAUp_Zombie, GameManager.user.DB_u_dna[2])) / 1000;
        DrainLife = DrainLife * (1000 + Formula.FieldNameToValue("DrainLife", DataManager.DNAUp_Zombie, GameManager.user.DB_u_dna[2])) / 1000;
		AttackInterval = AttackInterval * (1000 + Formula.FieldNameToValue("AttackInterval", DataManager.DNAUp_Zombie, GameManager.user.DB_u_dna[2])) / 1000;

        //根据随机分配到的气候和环境来决定使用哪个参数
        switch (clim)
        {
            case Climate.Dry:
                ClimateBoost += Formula.FieldNameToValue("ClimateBoost_1", DataManager.DNAUp_Zombie, GameManager.user.DB_u_dna[2]);
                break;
            case Climate.Wet:
                ClimateBoost += Formula.FieldNameToValue("ClimateBoost_2", DataManager.DNAUp_Zombie, GameManager.user.DB_u_dna[2]);
                break;
            case Climate.Normal:
                ClimateBoost += Formula.FieldNameToValue("ClimateBoost_3", DataManager.DNAUp_Zombie, GameManager.user.DB_u_dna[2]);
                break;
        }

        switch (envi)
        {
            case Environment.Hot:
                EnviBoost += Formula.FieldNameToValue("EnviBoost_1", DataManager.DNAUp_Zombie, GameManager.user.DB_u_dna[2]);
                break;
            case Environment.Cold:
                EnviBoost += Formula.FieldNameToValue("EnviBoost_2", DataManager.DNAUp_Zombie, GameManager.user.DB_u_dna[2]);
                break;
            case Environment.Balance:
                EnviBoost += Formula.FieldNameToValue("EnviBoost_3", DataManager.DNAUp_Zombie, GameManager.user.DB_u_dna[2]);
                break;
        }

        UpdateAttributes();

        //没有Mission值

        HP = MaxHP;

        //预制体初始化
        Image.spriteName = zombie.Res;
        HPBar.GetComponent<UISlider>().value = (float)(HP * 1.0f / MaxHP);
        ClimIcon.spriteName = Formula.ClimateIcon(ref ClimIcon, Clim);
        EnviIcon.spriteName = Formula.EnviIcon(ref ClimIcon, envi);
        foreach(SpecialAbility_Sheet sas in DataManager.SpecialAbility_Ability)
        {
            if(sas.ID == SkillID)
            {
                SkillIcon.spriteName = sas.ResIcon;
                param = int.Parse(sas.Value1) + int.Parse(sas.Value1_Add);
                //特效预加载
                skillSEGO = (GameObject)(Resources.Load("SEPrefabs" + "/" + sas.SEName));
                dieSEGO = (GameObject)(Resources.Load("SEPrefabs" + "/" + "ZombieDie"));
                break;
            }
        }
        
    }

    public void UpdateAttributes()
    {
        MaxHP = MaxHP * 1000 / (1000 + ClimateBoost + EnviBoost);
        Atk = Atk * 1000 / (1000 + ClimateBoost + EnviBoost);
        Heal = Heal * 1000 / (1000 + ClimateBoost + EnviBoost);
        Def = Def * 1000 / (1000 + ClimateBoost + EnviBoost);
        Infect = Infect * 1000 / (1000 + ClimateBoost + EnviBoost);
        Speed = Speed * 1000 / (1000 + ClimateBoost + EnviBoost);
        HPDecay = HPDecay * 1000 / (1000 + ClimateBoost + EnviBoost);
        DrainLife = DrainLife * 1000 / (1000 + ClimateBoost + EnviBoost);
		AttackInterval = AttackInterval * 1000 / (1000 + ClimateBoost + EnviBoost);
    }

    public Zombie ZombieBattleEvent()
    {
        //Human基础值 + 事件影响值
        return this;
    }

}
