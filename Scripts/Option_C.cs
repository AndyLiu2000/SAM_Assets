using UnityEngine;
using System.Collections;

public class Option_C : MonoBehaviour {

    public UILabel LabelOption;
    public UILabel LabelLanguageName;
    public UILabel LabelLanguage;
    public GameObject MusicBar;
    public GameObject SoundSwitch;
    public GameObject Option_BackBtn;
    public GameObject LanguageRight;
    public GameObject LanguageLeft;

    private UISlider musicBarSlider;
    private UIToggle soundToggle;

    void Start() {
        musicBarSlider = MusicBar.GetComponent<UISlider>();
        soundToggle = SoundSwitch.GetComponent<UIToggle>();

        UIEventListener.Get(Option_BackBtn).onClick = Option_BackBtn_Click;
        UIEventListener.Get(LanguageRight).onClick = LanguageRight_Click;
        UIEventListener.Get(LanguageLeft).onClick = LanguageLeft_Click;

        //musicBarSlider.value = AudioManager.BgVolume;
        //soundToggle.value = AudioManager.IsSoundOn;

        //处理从存档中读取音量的操作，若无存档，则读取默认数值
    }

    public void Enter()
    {
        musicBarSlider.value = AudioManager.BgVolume;
        soundToggle.value = AudioManager.IsSoundOn;

        LabelOption.text = LocalizationEx.LoadLanguageTextName("Option");
        LabelLanguageName.text = LocalizationEx.LoadLanguageTextName("LanguageName");
        LabelLanguage.text = LocalizationEx.LoadLanguageTextName("Language");
    }

	public void UpdateMusic(GameObject b){
		AudioManager.ChangeBGVolumeTo(b.GetComponent<UISlider>().value);
	}

	public void UpdateME(GameObject b){
		AudioManager.ChangeMEToggle(b.GetComponent<UIToggle>().value);
	}

    public void Option_BackBtn_Click(GameObject b)
    {
		Debug.Log ("BackBtn_Click");

        //处理存储音量

        //GameManager.ChangePanel(GameManager.UIS[GameManager.OPTION], GameManager.UIS[GameManager.MAIN], 0);
    }

    public void LanguageRight_Click(GameObject b)
    {
        LocalizationEx.SaveLanguage(LanguageChange.right);
        Enter();
    }

    public void LanguageLeft_Click(GameObject b)
    {
        LocalizationEx.SaveLanguage(LanguageChange.left);
        Enter();
    }


}
