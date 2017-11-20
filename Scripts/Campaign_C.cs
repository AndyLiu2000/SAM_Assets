using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class Campaign_C : MonoBehaviour {

    public GameObject CampainScroll;
    public GameObject prefabs_Cell;
    public GameObject Campaign_BackBtn;
    public GameObject CampainGrid;

    public int VirusID;

    ObjectPool<GameObject, Mission_Sheet> OP;
    

    int prefabscellChildrenNum = 0;

    // Use this for initialization
    void Start () {
        UIEventListener.Get(Campaign_BackBtn).onClick = Campaign_BackBtn_Click;

        OP = new ObjectPool<GameObject, Mission_Sheet>(10, ResetMissionData, InitMissioinData);
        prefabscellChildrenNum = prefabs_Cell.GetComponentsInChildren<Transform>().Length;

		CampainGrid.GetComponent<UIGrid> ().cellWidth = (int)(CampainScroll.GetComponent<UIPanel> ().GetViewSize().x / 5);
		CampainGrid.GetComponent<UIGrid> ().cellHeight = CampainGrid.GetComponent<UIGrid> ().cellWidth;
		prefabs_Cell.GetComponent<UISprite> ().width = (int)CampainGrid.GetComponent<UIGrid> ().cellWidth;
		prefabs_Cell.GetComponent<UISprite> ().height = prefabs_Cell.GetComponent<UISprite> ().width;
		prefabs_Cell.GetComponent<BoxCollider> ().size = new Vector3 ((int)CampainGrid.GetComponent<UIGrid> ().cellWidth, (int)CampainGrid.GetComponent<UIGrid> ().cellHeight);

		CampainGrid.transform.localPosition = new Vector3 ((CampainScroll.GetComponent<UIPanel> ().GetViewSize().x - CampainGrid.GetComponent<UIGrid> ().cellWidth) / 2 * -1,
			(CampainScroll.GetComponent<UIPanel> ().GetViewSize().y - CampainGrid.GetComponent<UIGrid> ().cellHeight) / 2 - 20,0);
    }

    public void Enter(int curVirusID)
    {
        VirusID = curVirusID;
        LoadCampaignData();
    }

    public void LoadCampaignData()
    {
        //创建数据 init mission data
        Debug.Log("Init Mission-data");
        //Use object pool to restore objects here

		/*CampainGrid.GetComponent<UIGrid> ().cellWidth = (int)(CampainScroll.GetComponent<UIPanel> ().GetViewSize().x / 5);
		CampainGrid.GetComponent<UIGrid> ().cellHeight = CampainGrid.GetComponent<UIGrid> ().cellWidth;
		prefabs_Cell.GetComponent<UISprite> ().width = (int)CampainGrid.GetComponent<UIGrid> ().cellWidth;
		prefabs_Cell.GetComponent<UISprite> ().height = prefabs_Cell.GetComponent<UISprite> ().width;
		prefabs_Cell.GetComponent<BoxCollider> ().size = new Vector3 ((int)CampainGrid.GetComponent<UIGrid> ().cellWidth, (int)CampainGrid.GetComponent<UIGrid> ().cellHeight);

		CampainGrid.transform.localPosition = new Vector3 ((CampainScroll.GetComponent<UIPanel> ().GetViewSize().x - CampainGrid.GetComponent<UIGrid> ().cellWidth) / 2 * -1,
			(CampainScroll.GetComponent<UIPanel> ().GetViewSize().y - CampainGrid.GetComponent<UIGrid> ().cellHeight) / 2 - 20,0);*/

        //添加数据 set data
        OP.ObjectSheet = DataManager.Mission_Parameter;
        int missionNum = 0;

        for (int i = DataManager.Mission_Parameter.Count - 1; i > 0; i--)
        {
            foreach (U_MissionFlag missionData in GameManager.user.DB_u_mf)
            {
                if (missionData.MissionID.ToString() == DataManager.Mission_Parameter[i].MissionID && missionData.VirusID == VirusID)
                {
                    //已通关的关卡显示出来 show completed missioins
                    if (missionData.Flag)
                    {
                        OP.New(prefabs_Cell, i,1);
                        missionNum++;
                    }
                    else
                    {
                        //刚开启的关卡显示出来
                        if (Formula.FarthestMission(missionData.VirusID) == i)
                        {
                            OP.New(prefabs_Cell, i,2);
                            missionNum++;
                        }
                    }
                }
            }
        }

        DeleteUnusedCell(CampainScroll,missionNum);
    }

    void DeleteUnusedCell(GameObject go,int groupNum)
    {
        Transform[] children = CampainGrid.GetComponentsInChildren<Transform>();

        //多余的关卡先删掉
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
        CampainGrid.GetComponent<UIGrid>().Reposition();
        CampainGrid.GetComponent<UIGrid>().repositionNow = true;
        NGUITools.SetDirty(CampainGrid);

        OP.ResetAll();
    }

    void ResetMissionData(GameObject GO, List<Mission_Sheet> sheet, int i1,int i2)
    {
        //添加配置数据，显示配置数据
        GO.GetComponent<CampaignCell>().LabelMissionName.text = LocalizationEx.LoadLanguageTextName(sheet[i1].MissionID);

        //传递Cell数据
        GO.GetComponent<CampaignCell>().CellID = int.Parse(sheet[i1].MissionID);

        //添加用户数据，显示用户数据
        foreach (U_MissionFlag md in GameManager.user.DB_u_mf)
        {
            if (md.MissionID.ToString() == sheet[i1].MissionID)
            {
                if (i2 == 1)
                {
                    GO.GetComponent<CampaignCell>().LabelMissionFlag.text = "Complete!";
                }
                else if (i2 == 2)
                {
                    GO.GetComponent<CampaignCell>().LabelMissionFlag.text = "New!";
                }
                break;
            }
        }
    }

    void InitMissioinData(GameObject GO, List<Mission_Sheet> sheet, int i1,int i2)
    {
        //设定每个cell的相对位置
        Vector3 pos = new Vector3(0, -CampainGrid.GetComponent<UIGrid>().cellHeight * i1, 0);
        GO.transform.localPosition = pos;

        ResetMissionData(GO, sheet, i1, i2);

        //添加为子物体
        OP.ObjectList.Add(NGUITools.AddChild(CampainGrid, GO));

        //重排位置
        CampainGrid.GetComponent<UIGrid>().Reposition();
        CampainGrid.GetComponent<UIGrid>().repositionNow = true;
        NGUITools.SetDirty(CampainGrid);
    }

    public void Campaign_BackBtn_Click(GameObject b)
    {
        Debug.Log("BackBtn_Click");
        GameManager.ChangePanel(GameManager.UIS[GameManager.CAMPAIGN], GameManager.UIS[GameManager.VIRUSSELECT],0);
    }
}
