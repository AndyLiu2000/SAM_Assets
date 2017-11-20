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

    public List<string> DB_u_UnlockedViruses = new List<string>();

    const int unlockedVirus1 = 1;
    const int unlockedVirus2 = 2;
    const int unlockedVirus3 = 3;

    public List<string> DB_u_UnlockedZombies = new List<string>();
    
    const int unlockedZombie1 = 1;
    const int unlockedZombie2 = 2;
    const int unlockedZombie3 = 3;

    public User Init()
    {
        Debug.Log("Init User");
        for(int i = 0; i < DB_u_dna.Length; i++)
        {
            DB_u_dna[i] = new List<U_DNA>();
        }
        //按照表格大小新建用户数据
        for (int i = 1; i < DataManager.DNAUp_Virus.Count; i++)
        {
            DB_u_dna[0].Add(new U_DNA(i, DataManager.DNAUp_Virus));
        }      
        
        for (int i = 1; i < DataManager.DNAUp_Human.Count; i++)
        {
            DB_u_dna[1].Add(new U_DNA(i,DataManager.DNAUp_Human));
        }

        for (int i = 1; i < DataManager.DNAUp_Zombie.Count; i++)
        {
            DB_u_dna[2].Add(new U_DNA(i,DataManager.DNAUp_Zombie));
            //初始解锁一种Zombie
        }

        Gold = INIT_GOLD;
        Gem = INIT_GEM;

        for(int i = 1; i < DataManager.Model_Virus.Count; i++)
        {
            for (int j = 1; j < DataManager.Mission_Parameter.Count; j++)
            {
                DB_u_mf.Add(new U_MissionFlag(i,j));
            }
        }

        DB_u_UnlockedViruses.Add(DataManager.Model_Virus[unlockedVirus1].VirusID);
        //DB_u_UnlockedViruses.Add(DataManager.Model_Virus[unlockedVirus2].VirusID);
        //DB_u_UnlockedViruses.Add(DataManager.Model_Virus[unlockedVirus3].VirusID);

        DB_u_UnlockedZombies.Add(DataManager.Model_Zombie[unlockedZombie1].ZombieID);
        DB_u_UnlockedZombies.Add(DataManager.Model_Zombie[unlockedZombie2].ZombieID);
        DB_u_UnlockedZombies.Add(DataManager.Model_Zombie[unlockedZombie3].ZombieID);

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

        //按照表格大小为数量，进行字段的类型转换
        for (int i = 1; i < DataManager.DNAUp_Virus.Count; i++)
        {
            //防止后期游戏维护时配置数据与存档数据不一致，做一致性处理
            /*
            try { }
            catch { }
            finally { }
            if (f.DB_u_dv[i - 1] != null)
            */

            DB_u_dna[0].Add(new U_DNA(i,DataManager.DNAUp_Virus));
            DB_u_dna[0][i - 1].ID = int.Parse(f.DB_u_dna[0][i - 1].ID);
            DB_u_dna[0][i - 1].Lv = int.Parse(f.DB_u_dna[0][i - 1].Lv);
        }

        for (int i = 1; i < DataManager.DNAUp_Human.Count; i++)
        {
            DB_u_dna[1].Add(new U_DNA(i,DataManager.DNAUp_Human));
            DB_u_dna[1][i - 1].ID = int.Parse(f.DB_u_dna[1][i - 1].ID);
            DB_u_dna[1][i - 1].Lv = int.Parse(f.DB_u_dna[1][i - 1].Lv);
        }

        for (int i = 1; i < DataManager.DNAUp_Zombie.Count; i++)
        {
            DB_u_dna[2].Add(new U_DNA(i,DataManager.DNAUp_Zombie));
            DB_u_dna[2][i - 1].ID = int.Parse(f.DB_u_dna[2][i - 1].ID);
            DB_u_dna[2][i - 1].Lv = int.Parse(f.DB_u_dna[2][i - 1].Lv);
        }

        Gold = long.Parse(f.Gold);
        Gem = long.Parse(f.Gem);

        for (int i = 1; i < DataManager.Model_Virus.Count; i++)
        {
            for (int j = 1; j < DataManager.Mission_Parameter.Count; j++)
            {
                DB_u_mf.Add(new U_MissionFlag(i,j));
                int index = (i - 1) * (DataManager.Mission_Parameter.Count - 1) + j - 1;
                DB_u_mf[index].VirusID = int.Parse(f.DB_u_mf[index].VirusID);
                DB_u_mf[index].MissionID = int.Parse(f.DB_u_mf[index].MissionID);
                DB_u_mf[index].Flag = bool.Parse(f.DB_u_mf[index].Flag);
            }
        }

        foreach(string s in f.DB_u_UnlockedViruses)
        {
            DB_u_UnlockedViruses.Add(s);
        }

        foreach(string s in f.DB_u_UnlockedZombies)
        {
            DB_u_UnlockedZombies.Add(s);
        }

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
        this.VirusID = int.Parse(DataManager.Model_Virus[virusID].VirusID);
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

    public List<string> DB_u_UnlockedViruses = new List<string>();
    public List<string> DB_u_UnlockedZombies = new List<string>();
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
