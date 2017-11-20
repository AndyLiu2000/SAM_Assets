using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopCell : MonoBehaviour {

    public UILabel LabelIAPName;
    public UILabel LabelIAPPrice;
    //ShopCell cell;

    //数据相关
	public int CellID;

    public void Cell_Click()
    {
		//Purchaser.BuyProductID(DataManager.IAP_Item[CellID].IAPSlot);
    }
}
