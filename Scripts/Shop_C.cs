using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Shop_C : MonoBehaviour {

	public GameObject ShopScroll;
    public GameObject ShopGrid;
    public GameObject prefabs_Cell;
    public GameObject Shop_BackBtn;

    ObjectPool<GameObject, IAP_Sheet> OP;

    // Use this for initialization
    void Start () {
        UIEventListener.Get(Shop_BackBtn).onClick = Shop_BackBtn_Click;
        OP = new ObjectPool<GameObject, IAP_Sheet>(10, ResetIAPData, InitIAPData);

		ShopGrid.GetComponent<UIGrid> ().cellWidth = (int)(ShopScroll.GetComponent<UIPanel> ().GetViewSize().x / 5);
		ShopGrid.GetComponent<UIGrid> ().cellHeight = (int)(ShopScroll.GetComponent<UIPanel> ().GetViewSize().y - 20);
		prefabs_Cell.GetComponent<UISprite> ().width = (int)ShopGrid.GetComponent<UIGrid> ().cellWidth;
		prefabs_Cell.GetComponent<UISprite> ().height = (int)ShopGrid.GetComponent<UIGrid> ().cellHeight;
		prefabs_Cell.GetComponent<BoxCollider> ().size = new Vector3 ((int)ShopGrid.GetComponent<UIGrid> ().cellWidth, (int)ShopGrid.GetComponent<UIGrid> ().cellHeight);

		ShopGrid.transform.localPosition = new Vector3 ((ShopScroll.GetComponent<UIPanel> ().GetViewSize().x - ShopGrid.GetComponent<UIGrid> ().cellWidth) / 2 * -1,
			(ShopScroll.GetComponent<UIPanel> ().GetViewSize().y - ShopGrid.GetComponent<UIGrid> ().cellHeight) / 2);
    }

    public void Enter()
    {
        LoadShopData();
    }

    public void LoadShopData()
    {
        //Use object pool to restore objects here
        OP.ObjectSheet = DataManager.IAP_Item;
        //添加数据
        for (int i = 1; i < DataManager.IAP_Item.Count; i++)
        {
            OP.New(prefabs_Cell, i, 0);
        }

        //重排位置
        ShopGrid.GetComponent<UIGrid>().Reposition();
        ShopGrid.GetComponent<UIGrid>().repositionNow = true;
        NGUITools.SetDirty(ShopGrid);
        OP.ResetAll();
    }

    void ResetIAPData(GameObject GO, List<IAP_Sheet> sheet, int i1, int i2)
    {
        //添加配置数据，显示配置数据
        //价格的读取方法
        GO.GetComponent<ShopCell>().LabelIAPName.text = LocalizationEx.LoadLanguageTextName(sheet[i1].PackageName);
        GO.GetComponent<ShopCell>().LabelIAPPrice.text = LocalizationEx.LoadLanguageTextName(sheet[i1].DollarPrice);

        //传递Cell数据
        GO.GetComponent<ShopCell>().CellID = int.Parse(sheet[i1].IAPPackageID);
    }

    void InitIAPData(GameObject GO, List<IAP_Sheet> sheet, int i1, int i2)
    {
        //设定每个cell的相对位置
        Vector3 pos = new Vector3(0, -ShopGrid.GetComponent<UIGrid>().cellHeight * i1, 0);
        GO.transform.localPosition = pos;

        ResetIAPData(GO, sheet, i1, i2);

        //添加为子物体
        OP.ObjectList.Add(NGUITools.AddChild(ShopGrid, GO));

    }

    public void Shop_BackBtn_Click(GameObject b)
    {
        Debug.Log("BackBtn_Click");
        GameManager.ChangePanel(GameManager.UIS[GameManager.SHOP], GameManager.UIS[GameManager.MAIN], 0);
    }
}
