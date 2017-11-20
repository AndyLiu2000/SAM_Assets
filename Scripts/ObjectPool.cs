using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool<T1,T2> where T1:class, new()
{
    public List<T1> ObjectList;
    public List<T2> ObjectSheet;
    public Action<T1, List<T2>, int, int> ResetAction;
    public Action<T1, List<T2>, int, int> InitAction;

    private int nextAvailableIndex = 0;

    public ObjectPool(int initialBufferSize, Action<T1, List<T2>, int,int> resetAction = null, Action<T1, List<T2>, int,int> initAction = null)
    {
        ObjectList = new List<T1>(initialBufferSize);
        ResetAction = resetAction;
        InitAction = initAction;
    }

    public T1 New(T1 go, int i1,int i2)
    {
        if (nextAvailableIndex < ObjectList.Count)
        {
            // an allocated object is already available; just reset it
            T1 t = ObjectList[nextAvailableIndex];
            //(t as GameObject).SetActive(true);
            Formula.Btn_IsVisible(t as GameObject, true);
            nextAvailableIndex++;

            if (ResetAction != null)
                ResetAction(t, ObjectSheet, i1,i2);

            return t;
        }
        else
        {
            // no allocated object is available
            nextAvailableIndex++;

            if (InitAction != null)
                InitAction(go, ObjectSheet, i1,i2);

            return go;
        }
    }

    public void ResetAll()
    {
        //重置索引
        nextAvailableIndex = 0;
    }
}
