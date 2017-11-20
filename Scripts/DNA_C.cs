using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public enum DNAType
{
    Virus,
    Human,
    Zombie
}

public class DNA_C : MonoBehaviour {
    
    public GameObject prefabs_Cell;
    public UILabel LabelGold;
    public UILabel LabelGem;
    public GameObject DNAScroll;
    public GameObject DNAGrid;
    public GameObject DNA_BackBtn;
    public GameObject DNA_VirusBtn;
    public GameObject DNA_HumanBtn;
    public GameObject DNA_ZombieBtn;

    ObjectPool<GameObject, DNAUp_Sheet> OP;
    int prefabscellChildrenNum = 0;

    // Use this for initialization
    void Start () {
        UIEventListener.Get(DNA_BackBtn).onClick = DNA_BackBtn_Click;
        UIEventListener.Get(DNA_VirusBtn).onClick = DNA_VirusBtn_Click;
        UIEventListener.Get(DNA_HumanBtn).onClick = DNA_HumanBtn_Click;
        UIEventListener.Get(DNA_ZombieBtn).onClick = DNA_ZombieBtn_Click;

        OP = new ObjectPool<GameObject,DNAUp_Sheet>(10,ResetDNAData,InitDNAData);

        prefabscellChildrenNum = prefabs_Cell.GetComponentsInChildren<Transform>().Length;

		DNAGrid.GetComponent<UIGrid> ().cellWidth = DNAScroll.GetComponent<UIPanel> ().GetViewSize ().x;
		DNAGrid.transform.localPosition = new Vector3 (0, DNAScroll.GetComponent<UIPanel> ().GetViewSize().y *0.5f- prefabs_Cell.GetComponent<UISprite> ().height *0.5f);
		prefabs_Cell.GetComponent<UISprite> ().width = (int)DNAGrid.GetComponent<UIGrid> ().cellWidth;
    }

    public void Enter()
    {
        //LabelGold.text = GameManager.user.Gold.ToString();
        //LabelGem.text = GameManager.user.Gem.ToString();

        LoadDNAData(DataManager.DNAUp_Virus);
		DNA_VirusBtn.GetComponentInChildren<UILabel>().text = LocalizationEx.LoadLanguageTextName("Virus");//
		DNA_HumanBtn.GetComponentInChildren<UILabel>().text = LocalizationEx.LoadLanguageTextName("Human");//
		DNA_ZombieBtn.GetComponentInChildren<UILabel>().text = LocalizationEx.LoadLanguageTextName("Zombie");//
    }

    public void LoadDNAData(List<DNAUp_Sheet> sheet)
    {
        LabelGold.text = GameManager.user.Gold.ToString();
        LabelGem.text = GameManager.user.Gem.ToString();

        //Use object pool to restore objects here
        OP.ObjectSheet = sheet;

        for (int i = 1; i < sheet.Count; i++)
        {
            OP.New(prefabs_Cell,i,0);
        }

        DeleteUnusedCell(DNAScroll, sheet.Count - 1);
    }

    void DeleteUnusedCell(GameObject go, int groupNum)
    {
        Transform[] children = DNAGrid.GetComponentsInChildren<Transform>();

        //若不大于1，说明此时刚刚初始化
        if (children.Length > groupNum * prefabscellChildrenNum + 1)
        {
            //从1开始，不要删除DNAGrid物体本身
            for (int i = groupNum * prefabscellChildrenNum; i < children.Length; i++)
            {
                OP.ObjectList.Remove(children[i].gameObject);
                Destroy(children[i].gameObject);
            }
            go.GetComponent<UIScrollView>().ResetPosition();
        }

        //重排位置
        DNAGrid.GetComponent<UIGrid>().Reposition();
        DNAGrid.GetComponent<UIGrid>().repositionNow = true;
        NGUITools.SetDirty(DNAGrid);

        OP.ResetAll();
    }

    void ResetDNAData(GameObject GO, List<DNAUp_Sheet> sheet, int i1,int i2)
    {
        GO.GetComponent<DNACell>().Name.text = LocalizationEx.LoadLanguageTextName(sheet[i1].Name);
        GO.GetComponent<DNACell>().Des.text = LocalizationEx.LoadLanguageTextName(sheet[i1].Name);

        //传递Cell数据
        GO.GetComponent<DNACell>().CellID = int.Parse(sheet[i1].ID);
        int cellID = int.Parse(sheet[i1].ID);

        //添加用户数据，显示用户数据
        if (sheet == DataManager.DNAUp_Virus)
        {
            //金币消耗的算法要改
            GO.GetComponent<DNACell>().GLabel.text = (long.Parse(DataManager.DNAUp_Virus[cellID].GoldCost) + long.Parse(DataManager.DNAUp_Virus[cellID].GoldParam_1) * GameManager.user.DB_u_dna[0][cellID - 1].Lv).ToString();
            GO.GetComponent<DNACell>().CLabel.text = (long.Parse(DataManager.DNAUp_Virus[cellID].GemCost) + long.Parse(DataManager.DNAUp_Virus[cellID].GemParam_1) * GameManager.user.DB_u_dna[0][cellID - 1].Lv).ToString();

            //传递Cell数据
            GO.GetComponent<DNACell>().CellType = DNAType.Virus;
            foreach (U_DNA virusData in GameManager.user.DB_u_dna[0])
            {
                if (virusData.ID.ToString() == sheet[i1].ID)
                {
                    GO.GetComponent<DNACell>().Lv.text = string.Format("Lv:" + virusData.Lv);
                    
                    break;
                }
            }
        }

        if (sheet == DataManager.DNAUp_Human)
        {
            GO.GetComponent<DNACell>().GLabel.text = (long.Parse(DataManager.DNAUp_Human[cellID].GoldCost) + long.Parse(DataManager.DNAUp_Human[cellID].GemParam_1) * GameManager.user.DB_u_dna[1][cellID - 1].Lv).ToString();
            GO.GetComponent<DNACell>().CLabel.text = (long.Parse(DataManager.DNAUp_Human[cellID].GemCost) + long.Parse(DataManager.DNAUp_Human[cellID].GemParam_1) * GameManager.user.DB_u_dna[1][cellID - 1].Lv).ToString();

            GO.GetComponent<DNACell>().CellType = DNAType.Human;
            foreach (U_DNA humanData in GameManager.user.DB_u_dna[1])
            {
                if (humanData.ID.ToString() == sheet[i1].ID)
                {
                    GO.GetComponent<DNACell>().Lv.text = string.Format("Lv:" + humanData.Lv);
                    
                    break;
                }
            }
        }

        if (sheet == DataManager.DNAUp_Zombie)
        {
            GO.GetComponent<DNACell>().GLabel.text = (long.Parse(DataManager.DNAUp_Zombie[cellID].GoldCost) + long.Parse(DataManager.DNAUp_Zombie[cellID].GoldParam_1) * GameManager.user.DB_u_dna[2][cellID - 1].Lv).ToString();
            GO.GetComponent<DNACell>().CLabel.text = (long.Parse(DataManager.DNAUp_Zombie[cellID].GemCost) + long.Parse(DataManager.DNAUp_Zombie[cellID].GemParam_1) * GameManager.user.DB_u_dna[2][cellID - 1].Lv).ToString();

            GO.GetComponent<DNACell>().CellType = DNAType.Zombie;
            foreach (U_DNA zombieData in GameManager.user.DB_u_dna[2])
            {
                if (zombieData.ID.ToString() == sheet[i1].ID)
                {
                    GO.GetComponent<DNACell>().Lv.text = string.Format("Lv:" + zombieData.Lv);
                    
                    break;
                }
            }
        }
    }

    void InitDNAData(GameObject GO, List<DNAUp_Sheet> sheet,int i1,int i2)
    {
        //设定每个cell的相对位置
        Vector3 pos = new Vector3(0, -DNAGrid.GetComponent<UIGrid>().cellHeight * i1, 0);
        GO.transform.localPosition = pos;

        //添加配置数据，显示配置数据
        ///////////////////////////////////////////////////////////
        ///////////////////////////////////////////////////////////
        /////////////////对金币、水晶数小于持有数的升级项，将按钮变为不可用，降低其他界面的复杂性
        ////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////
        ///////////////////////////////////////////////////////
        ResetDNAData(GO, sheet, i1,i2);
        //添加用户数据结束

        //添加为子物体
        OP.ObjectList.Add(NGUITools.AddChild(DNAGrid, GO));

        //重排位置
        DNAGrid.GetComponent<UIGrid>().Reposition();
        DNAGrid.GetComponent<UIGrid>().repositionNow = true;
        NGUITools.SetDirty(DNAGrid);
    }

    public void DNA_BackBtn_Click(GameObject button)
    {
        Debug.Log("BackBtn_Click");
        GameManager.ChangePanel(GameManager.UIS[GameManager.DNA], GameManager.UIS[GameManager.MAIN],0);
    }

    //点击按钮切换到这种研究项目，移除原来的加载cell
    public void DNA_VirusBtn_Click(GameObject button)
    {
        Debug.Log("VirusBtn_Click");

        LoadDNAData(DataManager.DNAUp_Virus);
    }

    //点击按钮切换到这种研究项目，移除原来的加载cell
    public void DNA_HumanBtn_Click(GameObject button)
    {
        Debug.Log("HumanBtn_Click");

        LoadDNAData(DataManager.DNAUp_Human);
    }

    //点击按钮切换到这种研究项目，移除原来的加载cell
    public void DNA_ZombieBtn_Click(GameObject button)
    {
        Debug.Log("ZombieBtn_Click");

        LoadDNAData(DataManager.DNAUp_Zombie);
    }
}
