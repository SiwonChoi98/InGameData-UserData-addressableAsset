using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class SpecDataManager
{
    public static SpecDataManager Instance = new SpecDataManager();
}

partial class SpecDataManager
{
    /*[Serializable]
    public static class InnerDatas //private
    {
        public static List<Monster> Monster = default;
    }*/
}

partial class SpecDataManager
{
    public ISpecData<int, Monster> Monster { get; private set; }
    
    //spec 데이터 저장
    public bool Load() //IReadOnlyList
    {
        try
        {
            //InnerDatas jsonData = UnityEngine.JsonUtility.FromJson<InnerDatas>(json);
            //OnDeserialize(jsonData);
            OnDeserialize();
        }
        catch (Exception e)
        {
            Debug.LogError(e.ToString());
            return false;
        }

        return true;
    }
    
    private void OnDeserialize() //InnerDatas jsonData
    {
        InnerDataMonster innerDataMonster = new InnerDataMonster();
        innerDataMonster.PreInitialization();
        Monster = innerDataMonster;

        Debug.Log("스펙 데이터 저장 완료");
    }
}

partial class SpecDataManager
{
    public class InnerDataMonster : ISpecData<int, Monster>
    {
        public IReadOnlyList<Monster> All { get; private set; }

        private Dictionary<int, Monster> _dict;

        public Monster Get(int id)
        {
            if (id == default)
            {
                return default;
            }

            if (_dict.TryGetValue(id, out Monster value))
            {
                return value;
            }

            return default;
        }

        public Monster this[int id] => Get(id);

        // 초기화
        /*public void PreInitialization(IReadOnlyList<monster> datas)
        {
            All = datas;
            _dict = new Dictionary<int, monster>(datas.Count);
            foreach (monster data in datas)
            {
                _dict.Add(data.id, data);
            }
        }*/
        
        public void PreInitialization()
        {
            
            _dict = new Dictionary<int, Monster>();

            for (int i = 0; i < 20; i++)
            {
                Monster monster = new Monster();
                monster.id = i;
                monster.hp = i+20;
                monster.mp = i+30;
                
                _dict.Add(i+1000, monster);
            }
            
           
        }
    }
}