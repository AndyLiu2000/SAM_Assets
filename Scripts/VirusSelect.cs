using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VirusSelect : MonoBehaviour {
    public GameObject prefabs_Cell;
	public GameObject VirusSelectScroll;
    public GameObject VirusSelectGrid;
    public GameObject VirusSelectBackBtn;
    public int UnlockedMissionNum;

    ObjectPool<GameObject, Virus_Sheet> OP;
    
    UILabel LabelUnlockedMissionNum;

    // Use this for initialization
    void Start () {
        //VirusSelectBackBtn = GameObject.Find("VirusSelectBackBtn");

        UIEventListener.Get(VirusSelectBackBtn).onClick = VirusSelectBackBtn_Click;

        LabelUnlockedMissionNum = GameObject.Find("UnlockedMissionNum").GetComponent<UILabel>();

        OP = new ObjectPool<GameObject, Virus_Sheet>(10, ResetVirusData, InitVirusData);

		VirusSelectGrid.GetComponent<UIGrid> ().cellWidth = (int)(VirusSelectScroll.GetComponent<UIPanel> ().GetViewSize().x / 5);
		VirusSelectGrid.GetComponent<UIGrid> ().cellHeight = VirusSelectGrid.GetComponent<UIGrid> ().cellWidth;
		prefabs_Cell.GetComponent<UISprite> ().width = (int)VirusSelectGrid.GetComponent<UIGrid> ().cellWidth;
		prefabs_Cell.GetComponent<UISprite> ().height = prefabs_Cell.GetComponent<UISprite> ().width;
		prefabs_Cell.GetComponent<BoxCollider> ().size = new Vector3 ((int)VirusSelectGrid.GetComponent<UIGrid> ().cellWidth, (int)VirusSelectGrid.GetComponent<UIGrid> ().cellHeight);

		VirusSelectGrid.transform.localPosition = new Vector3 ((VirusSelectScroll.GetComponent<UIPanel> ().GetViewSize().x - VirusSelectGrid.GetComponent<UIGrid> ().cellWidth) / 2 * -1,
			(VirusSelectScroll.GetComponent<UIPanel> ().GetViewSize().y - VirusSelectGrid.GetComponent<UIGrid> ().cellHeight) / 2 - 20,0);
    }

    public void Enter()
    {
        LoadVirusSelectData();
    }

    public void LoadVirusSelectData()
    {
        //创建数据
        Debug.Log("Init Virus-Data");

		/*VirusSelectGrid.GetComponent<UIGrid> ().cellWidth = (int)(VirusSelectScroll.GetComponent<UIPanel> ().GetViewSize().x / 5);
		VirusSelectGrid.GetComponent<UIGrid> ().cellHeight = VirusSelectGrid.GetComponent<UIGrid> ().cellWidth;
		prefabs_Cell.GetComponent<UISprite> ().width = (int)VirusSelectGrid.GetComponent<UIGrid> ().cellWidth;
		prefabs_Cell.GetComponent<UISprite> ().height = prefabs_Cell.GetComponent<UISprite> ().width;
		prefabs_Cell.GetComponent<BoxCollider> ().size = new Vector3 ((int)VirusSelectGrid.GetComponent<UIGrid> ().cellWidth, (int)VirusSelectGrid.GetComponent<UIGrid> ().cellHeight);

		VirusSelectGrid.transform.localPosition = new Vector3 ((VirusSelectScroll.GetComponent<UIPanel> ().GetViewSize().x - VirusSelectGrid.GetComponent<UIGrid> ().cellWidth) / 2 * -1,
			(VirusSelectScroll.GetComponent<UIPanel> ().GetViewSize().y - VirusSelectGrid.GetComponent<UIGrid> ().cellHeight) / 2 - 20,0);*/

        //读取已解锁关卡数 load unlocked missions
        int completeMissionNum = 0;
        //判断关卡总星数，决定是否解锁新病毒 unlock new virus accoring to star numbers
        foreach (U_MissionFlag mf in GameManager.user.DB_u_mf)
        {
            if (mf.Flag == true)
            {
                completeMissionNum += 1;
            }
        }

        LabelUnlockedMissionNum.text = completeMissionNum.ToString();

        //Use object pool to restore objects here
        OP.ObjectSheet = DataManager.Model_Virus;
        for (int i = 1; i < DataManager.Model_Virus.Count; i++)
        {
            OP.New(prefabs_Cell, i, completeMissionNum);
        }

        //预制体会保留为最后一次操作他的状态，所以这里要重置一下预制体的状态，以便下次使用时状态不要太奇怪
        //reset status of prefabs
        Formula.ChangeButtonEnable(prefabs_Cell);

        //重排位置
        VirusSelectGrid.GetComponent<UIGrid>().Reposition();
        VirusSelectGrid.GetComponent<UIGrid>().repositionNow = true;
        NGUITools.SetDirty(VirusSelectGrid);
        OP.ResetAll();
    }

    void ResetVirusData(GameObject GO, List<Virus_Sheet> sheet, int i1, int i2)
    {
        GO.GetComponent<VirusSelectCell>().LabelVirusName.text = LocalizationEx.LoadLanguageTextName(sheet[i1].Name);
        GO.GetComponent<VirusSelectCell>().LabelVirusDes.text = LocalizationEx.LoadLanguageTextName(sheet[i1].Des);

        //传递Cell数据
        GO.GetComponent<VirusSelectCell>().CellID = int.Parse(sheet[i1].VirusID);

        //未解锁的关卡
        if (!GameManager.user.DB_u_UnlockedViruses.Contains(sheet[i1].VirusID))
        {
            int unlockNum = int.Parse(sheet[i1].UnlockNum) - i2;

            GO.GetComponent<VirusSelectCell>().LabelMissionIndex.text = string.Format("{0} {1} {2}", LocalizationEx.LoadLanguageTextName("Need"), unlockNum, LocalizationEx.LoadLanguageTextName("Stars _To_Unlock"));

            //未解锁的按钮不可点击
            Formula.ChangeButtonDisable(GO);
        }
        else
        {
            //已解锁的关卡
            GO.GetComponent<VirusSelectCell>().LabelMissionIndex.text = string.Format("{0} {1}", LocalizationEx.LoadLanguageTextName("Mission"), Formula.FarthestMission(GO.GetComponent<VirusSelectCell>().CellID));

            //解锁的按钮可以点击
            Formula.ChangeButtonEnable(GO);
        }
    }

    void InitVirusData(GameObject GO, List<Virus_Sheet> sheet, int i1, int i2)
    {
        //设定每个cell的相对位置
        Vector3 pos = new Vector3(0, -VirusSelectGrid.GetComponent<UIGrid>().cellHeight * i1, 0);
        prefabs_Cell.transform.localPosition = pos;

        ResetVirusData(GO, sheet, i1, i2);

        //添加为子物体
        OP.ObjectList.Add(NGUITools.AddChild(VirusSelectGrid, prefabs_Cell));
    }

    public void VirusSelectBackBtn_Click(GameObject button)
    {
        Debug.Log("VirusSelectBackBtn_Click");
        GameManager.ChangePanel(GameManager.UIS[GameManager.VIRUSSELECT], GameManager.UIS[GameManager.MAIN], 0);
    }
}
