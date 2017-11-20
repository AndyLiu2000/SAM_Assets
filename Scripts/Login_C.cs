using UnityEngine;
using System.Collections;

public class Login_C : MonoBehaviour {

    public GameObject Login_StartBtn;

    private void Start()
    {
        UIEventListener.Get(Login_StartBtn).onClick = Login_StartBtn_Click;

    }

    //在界面的Enter方法里写界面文字的显示处理
    public void enter()
    {
		Login_StartBtn.GetComponentInChildren<UILabel>().text = LocalizationEx.LoadLanguageTextName("Start");
    }

    public void Login_StartBtn_Click(GameObject button)
    {
        Debug.Log("StartBTn_Click");
        GameManager.ChangePanel(GameManager.UIS[GameManager.LOGIN], GameManager.UIS[GameManager.MAIN],0);
    }
}
