using System.Collections.Generic;
using UnityEngine;

public class Casino : MonoBehaviour {
    //const
	//const long USER_INIT_CHIPS = 3000;
    const int BET = 60;
    const int RANK = 1;
    const float WIN_RATE = 3.0f;
    const int ROTATION_BEGIN = 50;
    const int SLOT_NUM = 20;
    const int RECHARGE_NUM = 3000;
    const int TWEEN_TIME = 1500;
    const int WHEEL_NUMBER = 5;
    //game objects
    public GameObject LabelWin;
    public GameObject UpBtn;
    public GameObject DownBtn;
    public GameObject H2PBtn;
    public GameObject CasinoBtn;
    public GameObject HowToPlayPanel;
    public GameObject SpinBtn;
    public GameObject AutoSpinBtn;
    public GameObject RechargeBtn;
	public GameObject CasinoBackBtn;
	public GameObject Tutorial1;
	public GameObject Tutorial2;
	public GameObject Tutorial3;
	public GameObject Tutorial4;
    GameObject Audio_Spin;
    GameObject Audio_Scroll;
    GameObject Audio_ScrollEnd;
    GameObject Audio_WinSound;
    //components
    UILabel LabelBetNum;
    UILabel LabelChipsNum;
    UILabel LabelCreditNum;
    //static parameters
	public static long UserChips;
    public static int Bet = BET;
    public static int Rank = RANK;
    public static int CurrentBet = Bet * Rank;
    public static float WinRate = WIN_RATE;
    static int WinNum = 0;
    //private parameters
    int[] Rotation = new int[WHEEL_NUMBER];
    GameObject[] ScrollSlots = new GameObject[WHEEL_NUMBER];
    GameObject[] UIWrapContents = new GameObject[WHEEL_NUMBER];
    int RotationBegin = ROTATION_BEGIN;
    int SlotNum = SLOT_NUM;
    int RechargeNum = RECHARGE_NUM;
    //status parameters
    bool isUpActive;
    bool isDownActive;

    bool IsSpinPressed = false;
    float SpinPressedTime = 0.0f;
    bool IsAutoSpin = false;

    float tempWinNum = 0;
    int tweenTime = TWEEN_TIME;
    bool isTweenText = false;

    //object collections
    Cards[] cards;
    public Dictionary<string, GameObject> SlotDic = new Dictionary<string, GameObject>();
    public static GameObject[,] ResultSlots = new GameObject[5, 3];

    // Use this for initialization
	public void Start()
    {
		Debug.Log ("Generate Casino");
		//DataManager.ReadDatas();
		Audio_Spin = GameObject.Find("Spin");
		Audio_Scroll = GameObject.Find("Scroll");
		Audio_ScrollEnd = GameObject.Find ("ScrollEnd");
		Audio_WinSound = GameObject.Find ("WinSound");
        cards = new Cards[DataManager.Cards_Card.Count];
        foreach (Cards_Sheet card in DataManager.Cards_Card)
        {
            int index = int.Parse(card.ID);
            cards[index] = new Cards(int.Parse(DataManager.Cards_Card[index].Value),
                (DirectionType)(int.Parse(DataManager.Cards_Card[index].DirectionType)),
                (CardType)(int.Parse(DataManager.Cards_Card[index].CardType)),
                DataManager.Cards_Card[index].ImgName);
        }

        for(int i = 0; i < ScrollSlots.Length; i++)
        {
            ScrollSlots[i] = GameObject.Find("ScrollSlot" + (i + 1));
            UIWrapContents[i] = GameObject.Find("UIWrapContent" + (i + 1));
        }

        //读取Chips值 load chips
        /*if (PlayerPrefs.HasKey("IsSaved"))
        {
            //读档
            Debug.Log("Loading Option In Casino....");
            //PlayerPrefs.DeleteAll();
            UserChips = int.Parse(PlayerPrefs.GetString("Chips"));
            Rank = int.Parse(PlayerPrefs.GetString("Rank"));
            
            Debug.Log("Load Option In Casino Complete");
        }*/

		int wrapContentHeight = (int)(ScrollSlots [0].GetComponent<UIPanel> ().GetViewSize ().y / 3);
        for (int i = 1; i <= Rotation.Length; i++)
        {
            for (int j = 1; j <= SlotNum; j++)
            {
                GameObject slot = Resources.Load("Slot") as GameObject;
                slot.name = "Slot" + i + j;
				Vector2 pos = new Vector2(0, -1 * wrapContentHeight * (j - 1));
                int randomCardIndex = Random.Range(0, cards.Length);
                slot.GetComponent<Slot>().Card = cards[randomCardIndex];
                slot.GetComponent<UISprite>().spriteName = slot.GetComponent<Slot>().Card.ImgName;
                slot.GetComponent<Slot>().Color = slot.GetComponent<UISprite>().color;

                GameObject s = NGUITools.AddChild(UIWrapContents[i-1], slot);

                s.transform.localPosition = pos;
                SlotDic.Add(slot.name, s);
            }
            //重排位置 recenter to align with the scrollview
            UIWrapContents[i-1].GetComponent<UICenterOnChild>().Recenter();
        }

        GameObject scrollSLot5 = GameObject.Find("ScrollSlot" + 5);
        scrollSLot5.GetComponent<UIScrollView>().onStoppedMoving += ScrollSlot5_onStoppedMoving;

		HowToPlayPanel.SetActive(false);
		SpinBtn.SetActive(true);
		AutoSpinBtn.SetActive(false);

        UIEventListener.Get(AutoSpinBtn).onClick = AutoSpinBtn_onClick;
        UIEventListener.Get(SpinBtn).onClick = SpinBtn_onClick;
        UIEventListener.Get(SpinBtn).onPress = SpinBtn_onPress;
        UIEventListener.Get(RechargeBtn).onClick = RechargeBtn_onClick;
        UIEventListener.Get(UpBtn).onClick = UpBtn_onClick;
        UIEventListener.Get(DownBtn).onClick = DownBtn_onClick;
        UIEventListener.Get(H2PBtn).onClick = H2PBtn_onClick;
        UIEventListener.Get(CasinoBtn).onClick = CasinoBtn_onClick;
		UIEventListener.Get(CasinoBackBtn).onClick = CasinoBackBtn_onClick;


		LabelBetNum = GameObject.Find("LabelBetNum").GetComponent<UILabel>();
		CurrentBet = Bet * Rank;
		LabelBetNum.text = CurrentBet.ToString();

		LabelChipsNum = GameObject.Find("LabelChipsNum").GetComponent<UILabel>();
		LabelChipsNum.text = GameManager.user.Gold.ToString();

		LabelCreditNum = GameObject.Find("LabelCreditNum").GetComponent<UILabel>();
		LabelCreditNum.text = Rank.ToString();

		LabelWin.GetComponent<UILabel>().text = "0";
		LabelWin.SetActive(false);

		isUpActive = true;
		isDownActive = true;

		if (Rank == 1)
		{
			isDownActive = false;
			isUpActive = true;
			DownBtn.SetActive(isDownActive);
			UpBtn.SetActive(isUpActive);
		}
		else if(Rank == 5)
		{
			isDownActive = true;
			isUpActive = false;
			DownBtn.SetActive(isDownActive);
			UpBtn.SetActive(isUpActive);
		}

		CheckSpinStatus ();

		int oriWidth = Tutorial1.GetComponent<UISprite> ().width;
		int oriHeight = Tutorial1.GetComponent<UISprite> ().height;
		int desWidth = GameManager.StandardWidth / 4;
		int desHeight = desWidth * oriHeight / oriWidth;
		Debug.Log ("GameManager.StandardWidth = " + GameManager.StandardWidth);
		Debug.Log ("desWidth = " + desWidth);
		Debug.Log ("desHeight = " + desHeight);
		Tutorial1.GetComponent<UISprite> ().width = desWidth;
		Tutorial1.GetComponent<UISprite> ().height = desHeight;
		Tutorial2.GetComponent<UISprite> ().width = desWidth;
		Tutorial2.GetComponent<UISprite> ().height = desHeight;
		Tutorial3.GetComponent<UISprite> ().width = desWidth;
		Tutorial3.GetComponent<UISprite> ().height = desHeight;
		Tutorial4.GetComponent<UISprite> ().width = desWidth;
		Tutorial4.GetComponent<UISprite> ().height = desHeight;

		Tutorial1.transform.localPosition = new Vector3 (-3 * desWidth / 2, 0);
		Tutorial2.transform.localPosition = new Vector3 (-1 * desWidth / 2, 0);
		Tutorial3.transform.localPosition = new Vector3 (desWidth / 2, 0);
		Tutorial4.transform.localPosition = new Vector3 (desWidth * 3 / 2, 0);

		HowToPlayPanel.SetActive(false);
		SpinBtn.SetActive(true);
		AutoSpinBtn.SetActive(false);
    }

	void CheckSpinStatus(){
		if (GameManager.user.Gold <= CurrentBet) {
			SpinBtn.GetComponent<UIButton> ().isEnabled = false;
		} else {
			SpinBtn.GetComponent<UIButton> ().isEnabled = true;
		}
	}

    /// <summary>
    /// When the last scrollslot stops
    /// </summary>
    void ScrollSlot5_onStoppedMoving()
    {
        Audio_Scroll.GetComponent<AudioSource>().Stop();
        Audio_ScrollEnd.GetComponent<AudioSource>().Play();
        //延迟一秒再结算，这一秒等待Scrollview对齐；如果不等待，结果会出错
        //delay 1 second for scrollview so that it aligns and stops totally;if not, there will be some unpridicatable bugs
        Invoke("CalResult", 1.0f);
    }

    void AutoSpinBtn_onClick(GameObject go)
    {
        Audio_Spin.GetComponent<AudioSource>().Play();
        IsAutoSpin = false;
        SpinBtn.SetActive(true);
        AutoSpinBtn.SetActive(false);
    }

    void SpinBtn_onPress(GameObject go, bool isPressed)
    {
        IsSpinPressed = isPressed;
    }

    void RechargeBtn_onClick(GameObject go)
    {
        Debug.Log("RechargeBtn");
		/*
        UserChips += RechargeNum;
        LabelChipsNum.text = UserChips.ToString();
        if (UserChips > 0)
            SpinBtn.GetComponent<UIButton>().isEnabled = true;
            */
    }

    void DownBtn_onClick(GameObject go)
    {
        Debug.Log("DownBtn");
        Audio_Spin.GetComponent<AudioSource>().Play();
        if (Rank > 1)
        {
            Rank--;
            CurrentBet = Rank * Bet;

			CheckSpinStatus ();

            LabelBetNum.text = CurrentBet.ToString();
            LabelCreditNum.text = Rank.ToString();
            if (Rank == 1)
            {
                isDownActive = false;
                DownBtn.SetActive(isDownActive);
            }
            else
            {
                isUpActive = true;
                UpBtn.SetActive(isUpActive);
            }
            PlayerPrefs.SetString("Rank", Rank.ToString());
        }
    }

    void UpBtn_onClick(GameObject go)
    {
        Debug.Log("UpBtn");
        Audio_Spin.GetComponent<AudioSource>().Play();
        if (Rank < 5)
        {
            Rank++;
            CurrentBet = Rank * Bet;

			CheckSpinStatus ();

            LabelBetNum.text = CurrentBet.ToString();
            LabelCreditNum.text = Rank.ToString();
            if (Rank == 5)
            {
                isUpActive = false;
                UpBtn.SetActive(isUpActive);
            }
            else
            {
                isDownActive = true;
                DownBtn.SetActive(isDownActive);
            }
            PlayerPrefs.SetString("Rank", Rank.ToString());
        }
    }

    void H2PBtn_onClick(GameObject go)
    {
        Debug.Log("H2PBtn");
        HowToPlayPanel.SetActive(true);

    }

    void CasinoBtn_onClick(GameObject go)
    {
        Debug.Log("CasinoBtn");
        HowToPlayPanel.SetActive(false);
    }

	void CasinoBackBtn_onClick(GameObject go)
	{
		Debug.Log("CasinoBtn");
		//GameManager.ChangePanel(GameManager.UIS[GameManager.CASINO], GameManager.UIS[GameManager.MAIN],0);
		Formula.UI_IsVisible(GameManager.UIS[GameManager.MAIN],true);
		Destroy (gameObject);
		AudioManager.playMusicByName(AudioManager.MainBG);
	}

    void SpinBtn_onClick(GameObject go)
    {
        Debug.Log("SpinBtn");
		CasinoBackBtn.SetActive (false);
        UpBtn.SetActive(false);
        DownBtn.SetActive(false);
        Audio_Spin.GetComponent<AudioSource>().Play();
        SpinBtn.GetComponent<UIButton>().isEnabled = false;
        LabelWin.SetActive(false);
        for (int i = 1; i <= Rotation.Length; i++)
        {
            for (int j = 1; j <= SlotNum; j++)
            {
                int randomCardIndex = Random.Range(0, cards.Length);
                SlotDic["Slot" + i + j].GetComponent<Slot>().Card = cards[Formula.Spin(i)];
                SlotDic["Slot" + i + j].GetComponent<UISprite>().spriteName = SlotDic["Slot" + i + j].GetComponent<Slot>().Card.ImgName;
                SlotDic["Slot" + i + j].GetComponent<UISprite>().color = SlotDic["Slot" + i + j].GetComponent<Slot>().Color;
                SlotDic["Slot" + i + j].transform.localRotation = new Quaternion(0, 0, 0, 0);
                switch (SlotDic["Slot" + i + j].GetComponent<Slot>().Card.DT)
                {
                    case DirectionType.L:
                        SlotDic["Slot" + i + j].transform.Rotate(0, 0, -45);
                        break;
                    case DirectionType.R:
                        SlotDic["Slot" + i + j].transform.Rotate(0, 0, 45);
                        break;
                    default:
                        break;
                }
            }
            //这里修改过UIScrollView里的代码，使scrollview快速停下来，触发onStoppedMoving委托
            //这里还改过UICenterOnChild，使重新居中的操作在onStoppedMoving时触发一下，保证最后总是转动到居中的位置来保证对齐
            ScrollSlots[i-1].GetComponent<UIScrollView>().Scroll(RotationBegin * i);
        }
    }

    /// <summary>
    /// Calculate result
    /// </summary>
    void CalResult()
    {
        Debug.Log("CalResult");
        for (int i = 1; i <= Rotation.Length; i++)
        {
            for (int l = 1; l <= SlotNum; l++)
            {
                if (SlotDic["Slot" + i + l].activeSelf)
                {
                    if (FindNextActive(i, l, 4))
                    {
                        ResultSlots[i - 1, 0] = FindNext(i, l);
                        ResultSlots[i - 1, 1] = FindNext(i, l + 1);
                        ResultSlots[i - 1, 2] = FindNext(i, l + 2);
                        break;
                    }
                }
            }
        }

        //计算分数
        //三横排：1，2，3号线赢 line 1,2,3 win
        //WinNum += Line1_2_3Win();
        WinNum += Formula.Win1_2_3();

        //右斜排
        //4号线赢 line 4 win
        WinNum += Formula.Win4_8(0, 3, 1, 4);
        //8号线赢 line 8 win
        WinNum += Formula.Win4_8(1, 0, 2, 1);
        //5号线赢 line 5 win
        WinNum += Formula.Win5_6_7(0, 2, 1, 3, 2, 4);
        //6号线赢 line 6 win
        WinNum += Formula.Win5_6_7(0, 1, 1, 2, 2, 3);
        //7号线赢 line 7 win
        WinNum += Formula.Win5_6_7(0, 0, 1, 1, 2, 2);

        //左斜排
        //9号线赢 line 9 win
        WinNum += Formula.Win9_13(1, 4, 2, 3);
        //13号线赢 line 13 win
        WinNum += Formula.Win9_13(0, 1, 1, 0);
        //12号线赢 line 12 win
        WinNum += Formula.Win10_11_12(0, 2, 1, 1, 2, 0);
        //11号线赢 line 11 win
        WinNum += Formula.Win10_11_12(0, 3, 1, 2, 2, 1);
        //10号线赢 line 10 win
        WinNum += Formula.Win10_11_12(0, 4, 1, 3, 2, 2);
        //下折线赢
        //14号线赢 line 14 win
        WinNum += Formula.Win14(0, 0, 1, 1, 2, 2, 1, 3, 0, 4);
        //上折线赢
        //15号线赢 line 15 win
        WinNum += Formula.Win15(2, 0, 1, 1, 0, 2, 1, 3, 2, 4);

        //SkyWheel赢 SkyWheel win
        WinNum += Formula.Win_SkyWheel();
        //汇总结算 conclude
        if(WinNum > 0)
        {
            LabelWin.SetActive(true);
            isTweenText = true;
            Invoke("ExecuteResult", tweenTime * 1.0f / 1000 + 0.5f);
        }
        else ExecuteResult();
    }

    //conclude result
    void ExecuteResult()
    {
        if(WinNum>0)
            Audio_WinSound.GetComponent<AudioSource>().Play();
        isTweenText = false;
        LabelWin.GetComponent<UILabel>().text = WinNum.ToString();
		GameManager.user.Gold = GameManager.user.Gold - CurrentBet + WinNum;
        //存档
		GameManager.SaveData();
		LabelChipsNum.text = GameManager.user.Gold.ToString();

        SpinBtn.GetComponent<UIButton>().isEnabled = true;
		CasinoBackBtn.SetActive (true);

		if (GameManager.user.Gold <= CurrentBet)
        {
            SpinBtn.GetComponent<UIButton>().isEnabled = false;
        }
        else
        {
			if (IsAutoSpin) {
				SpinBtn_onClick(null);
			} 
        }
        tempWinNum = 0;
        WinNum = 0;
        UpBtn.SetActive(isUpActive);
        DownBtn.SetActive(isDownActive);

    }

    bool FindNextActive(int rotation,int index, int checkStep)
    {    
        if(checkStep == 0) return true;

        if (index >= 1 && index < SlotNum)
        {
            if (SlotDic["Slot" + rotation + (index + 1)].activeSelf)
            {
                return FindNextActive(rotation, index + 1,checkStep - 1);
            }
            else return false;  
        }
        if (index >= SlotNum)
        {
            if (SlotDic["Slot" + rotation + (index - SlotNum + 1)].activeSelf)
            {
                return FindNextActive(rotation, (index - SlotNum + 1), checkStep - 1);
            }
            else return false;
        }
        return false;
    }

    GameObject FindNext(int rotation,int index)
    {
        if (index >= 1 && index < SlotNum) return SlotDic["Slot" + rotation + (index + 1)];
        if(index >= SlotNum) return SlotDic["Slot" + rotation + (index - SlotNum + 1)];
        return null;
    }
	
	// Update is called once per frame
	void FixedUpdate() {
        if (IsSpinPressed)
        {
            SpinPressedTime += Time.fixedDeltaTime;
            if (SpinPressedTime >= 1.0f)
            {
                //切换为自动摇奖 switch to auto-spin
                IsAutoSpin = true;
                SpinBtn.SetActive(false);
                AutoSpinBtn.SetActive(true);
                SpinBtn_onClick(null);
                SpinPressedTime = 0.0f;
                IsSpinPressed = false;
            }
        }

        if (isTweenText)
        {
            if (tempWinNum < WinNum)
            {
                tempWinNum += WinNum * Time.fixedDeltaTime / (tweenTime * 1.0f/1000);
                if (tempWinNum > WinNum) tempWinNum = WinNum;
                LabelWin.GetComponent<UILabel>().text = ((int)tempWinNum).ToString();
            }
        }
	}
}
