using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gene : MonoBehaviour {

	const int HALF_COLUMN = 7;
	const int WHOLE_COLUMN = 15;
	const int HALF_ROW = 3;
	const int WHOLE_ROW = 7;
    //基因本身的所有相关量
    public string GeneID;
    public string StrategyID;
    public string BoardID;
    public string EventID;
    public int Row;
    public int Column;
    public string FP1;
    public string FP2;
    public int UnlockCost_A;
    public int UnlockCost_B;

    //局内变量
    public Vector3 Pos;
    public bool IsVisible = false;
    public bool IsFatherUnlocked = false;
    public bool IsUpgradable = false;
    public bool IsUpgraded = false;

    //预制体相关
    GameObject EvolutionBtn;
    Gene self;
    UILabel LabelEvolutionCost;

    //环境变量
    UILabel Label_EvolutionDes;
	public Battle_C Battle;
    float oneSecondDeltaTime = 0;
    float fiveSecondDeltaTime = 0;

    private void Awake()
    {
        Battle = GameObject.Find(GameManager.BATTLE).GetComponent<Battle_C>();
    }

    // Use this for initialization
    void Start () {

        self = gameObject.GetComponent<Gene>();

        UIEventListener.Get(gameObject).onClick = SelfBtn_Click;

        Battle = GameObject.Find(GameManager.BATTLE).GetComponent<Battle_C>();

        Label_EvolutionDes = GameObject.Find("Label_EvolutionDes").GetComponent<UILabel>();

        LabelEvolutionCost = GameObject.Find("LabelEvolutionCost").GetComponent<UILabel>();

        EvolutionBtn = GameObject.Find("EvolutionBtn");
    }
	
	public void CreateGene(string geneID)
    {
		Battle = GameObject.Find(GameManager.BATTLE).GetComponent<Battle_C>();

        GeneID = geneID;
        //根据GeneID遍历查找基因
        foreach (BattleStrategy_Sheet bss in DataManager.BattleStrategy_Strategy)
        {
            if (bss.GeneID == GeneID)
            {
                //基因值
                StrategyID = bss.StrategyID;
                BoardID = bss.BoardID;
                EventID = bss.EventID;
                Row = int.Parse(bss.Row);
                Column = int.Parse(bss.Column);
                FP1 = bss.FP1;
                FP2 = bss.FP2;
                UnlockCost_A = int.Parse(bss.UnlockCost_A);
                UnlockCost_B = int.Parse(bss.UnlockCost_B);

                break;
            }
        }

        IsVisible = false;
        IsUpgraded = false;
        IsUpgradable = false;
        IsFatherUnlocked = false;
        //State = GeneState.UnVisible;

        //设定位置
		Pos = new Vector3((Column -HALF_COLUMN)* Battle.UpgradeMapWidth / WHOLE_COLUMN, - (Row - HALF_ROW) * Battle.UpgradeMapHeigth / WHOLE_ROW,1.0f);

        //如果两个父节点都为0，则为初始可见的 if both father nodes are 0, it's visible
        if(FP1 == "0" && FP2 == "0")
        {
            IsFatherUnlocked = true;
            IsVisible = true;
        }

        foreach(InGameEvent_Sheet ige in DataManager.InGameEvent_InGameEvents)
        {
            if(EventID == ige.EventID)
            {
                gameObject.GetComponent<UISprite>().spriteName = ige.SkillIconName;
                gameObject.GetComponent<UIButton>().normalSprite = ige.SkillIconName;
                gameObject.GetComponent<UIButton>().hoverSprite = ige.SkillIconName;
                gameObject.GetComponent<UIButton>().pressedSprite = ige.SkillIconName;
                gameObject.GetComponent<UIButton>().disabledSprite = ige.SkillIconName;
            }
        }
    }

    public void UpdateData(string boardID)
    {
        if(boardID == BoardID)
        {
            //如果两个父节点都解锁了，那么自己为等待解锁状态 if both father nodes are unlocked, it's waiting for unlock
            bool isFatherUnlocked_1 = false;
            bool isFatherUnlocked_2 = false;
            ArrayList battleArray = new ArrayList();

            switch (BoardID)
            {
                case "1":
                    battleArray = Battle.VirusGeneArray;
                    break;
                case "2":
                    battleArray = Battle.HumanGeneArray;
                    break;
                case "3":
                    battleArray = Battle.ZombieGeneArray;
                    break;
                default:
                    break;
            }

            if (FP1 == "0")
            {
                isFatherUnlocked_1 = true;
            }
            else
            {
                foreach (GameObject ga in battleArray)
                {
                    if (FP1 == ga.GetComponent<Gene>().GeneID && ga.GetComponent<Gene>().IsUpgraded)
                    {
                        isFatherUnlocked_1 = true;
                        break;
                    }
                }
            }

            if (FP2 == "0")
            {
                isFatherUnlocked_2 = true;
            }
            else
            {
                foreach (GameObject ga in battleArray)
                {
                    if (FP2 == ga.GetComponent<Gene>().GeneID && ga.GetComponent<Gene>().IsUpgraded)
                    {
                        isFatherUnlocked_2 = true;
                        break;
                    }
                }
            }

            if (isFatherUnlocked_1 && isFatherUnlocked_2)
            {
                IsFatherUnlocked = true;
                IsVisible = true;
            }

            //如果策略点足够，那么为可升级状态 if sp is enough to upgrade,it's upgradable
            if (Battle.StrategyPoint >= Formula.StrategyPointCal(this) && IsVisible && IsFatherUnlocked)
            {
                IsUpgradable = true;
            }

            //如果已升级，那么为已升级状态
            if (IsUpgraded)
            {
                IsUpgradable = false;
            }

            gameObject.SetActive(IsVisible);
            //Formula.Btn_IsVisible(gameObject, IsVisible);
        }
        else
        {
            gameObject.SetActive(false);
            //Formula.Btn_IsVisible(gameObject, false);
        }
    }

    void SelfBtn_Click(GameObject button)
    {
        Debug.Log("SelfBtn_Click");
        UpdateData(BoardID);
        foreach(InGameEvent_Sheet ige in DataManager.InGameEvent_InGameEvents)
        {
            if (ige.EventID == EventID)
            {
                Label_EvolutionDes.text = LocalizationEx.LoadLanguageTextName(ige.UpgradeEffectID);
                break;
            }
        }
        //未进化过,显示进化消耗
        if (!IsUpgraded)
        {
            foreach (BattleStrategy_Sheet bs in DataManager.BattleStrategy_Strategy)
            {
                if (bs.GeneID == GeneID)
                {
					LabelEvolutionCost.text = Formula.StrategyPointCal(this).ToString() + "/" + Battle.StrategyPoint.ToString();
                    break;
                }
            }

            if(Battle.StrategyPoint >= Formula.StrategyPointCal(self))
            {
                Formula.ChangeButtonEnable(EvolutionBtn);
            }
            else
            {
                Formula.ChangeButtonDisable(EvolutionBtn);
            }
            
        }
        //已进化过不显示数值 upgraded gene doesn't show cost value
        else
        {
            LabelEvolutionCost.text = "";
            //这里改变进化按钮的外观为不可用状态
            Formula.ChangeButtonDisable(EvolutionBtn);
        }

        Battle.GeneSelected = this;
    }
}
