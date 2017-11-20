using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class User
{
    const long INIT_GOLD = 10000;
    const long INIT_GEM = 100;
    //用户属性字段 user attribute
    public List<U_DNA>[] DB_u_dna = new List<U_DNA>[3]; //Index 0:Virus 1:Human 2:Zombie

    public long Gold;
    public long Gem;

    public List<U_MissionFlag> DB_u_mf = new List<U_MissionFlag>();

    public User Init()
    {
        Debug.Log("Init User");
        for(int i = 0; i < DB_u_dna.Length; i++)
        {
            DB_u_dna[i] = new List<U_DNA>();
        }

        Gold = INIT_GOLD;
        Gem = INIT_GEM;

        return this;
    }

    //因为Json反序列化后所有字段都为string类型，所以这里先用一个临时类存放刚刚序列化后的数据
    // after deserializing ,all fields turn into string, then use a temporary class to get all deserialized data
    //再用下面类进行类型转换
    public User Deserialize(F_User f)
    {
        Debug.Log("DeSerialize User");

        for (int i = 0; i < DB_u_dna.Length; i++)
        {
            DB_u_dna[i] = new List<U_DNA>();
        }

        Gold = long.Parse(f.Gold);
        Gem = long.Parse(f.Gem);

        return this;
    }
}

[Serializable]
public class U_DNA
{
    //存储的字段
    public int ID;
    public int Lv;

    public U_DNA()
    {

    }

    public U_DNA(int row,List<DNAUp_Sheet> dnaSheet)
    {
        //把每项可升级属性的等级设为1
        this.ID = int.Parse(dnaSheet[row].ID);
        this.Lv = 1;
    }
}

[Serializable]
public class U_MissionFlag
{
    //存储的字段
    public int VirusID;
    public int MissionID;
    public bool Flag;

    public U_MissionFlag()
    {

    }

    public U_MissionFlag(int virusID, int row)
    {
        //把每项可升级属性的等级设为1
        this.MissionID = int.Parse(DataManager.Mission_Parameter[row].MissionID);
        this.Flag = false;
    }
}

public class F_User
{
    //用户属性字段
    public List<F_U_DNA>[] DB_u_dna = new List<F_U_DNA>[3];

    public string Gold;
    public string Gem;

    public List<F_U_MissionFlag> DB_u_mf = new List<F_U_MissionFlag>();

}

public class F_U_DNA
{
    //存储的字段
    public string ID;
    public string Lv;
}

public class F_U_MissionFlag
{
    //存储的字段
    public string VirusID;
    public string MissionID;
    public string Flag;
}
