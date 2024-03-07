using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public partial class SpecDataManager
{
    public static SpecDataManager Instance = new SpecDataManager();
    
    /// <summary>
    /// 암호키 경로 다시 체크
    /// </summary>
    public static readonly byte[] _key = {38, 37, 64, 111, 92, 69, 80, 101, 93, 60, 44, 56, 101, 103, 70, 42};
}
partial class SpecDataManager
{
    public ISpecData<int, Monster> Monster { get; private set; }
    public ISpecData<int, Stage> Stage { get; private set; }

    /// 파싱 시 i 번호
    ///  0번 - 주소
    ///  1번 - 변수명
    ///  2번 - 데이터 타입
    ///  3번 부터 진행
   
    public void SpecToInnerDatas(string typeName, string[] lines) //InnerDatas jsonData
    {
        switch (typeName)
        {
            case "MonsterEncrypt":
                InnerDataMonster innerDataMonster = new InnerDataMonster();
                innerDataMonster.PreInitialization(lines);
                Monster = innerDataMonster;
                break;
            case "StageEncrypt":
                InnerDataStage innerDataStage = new InnerDataStage();
                innerDataStage.PreInitialization(lines);
                Stage = innerDataStage;
                break;
                
        }
        Debug.Log(typeName + " 데이터 저장 완료");
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

        // 데이터 저장
        public void PreInitialization(string[] lines)
        {
            _dict = new Dictionary<int, Monster>();

            //열 갯수
            for (int i = 3; i < lines.Length; i++)
            {
                //행 갯수
                string[] fields = lines[i].Split(',');
                
                Monster monster = new Monster();
                monster.id = int.Parse(fields[0]);
                monster.hp = int.Parse(fields[1]);
                monster.mp = int.Parse(fields[2]);
                
                _dict.Add(monster.id, monster);    
            }
        }
    }
    
    public class InnerDataStage : ISpecData<int, Stage>
    {
        public IReadOnlyList<Stage> All { get; private set; }

        private Dictionary<int, Stage> _dict;

        public Stage Get(int id)
        {
            if (id == default)
            {
                return default;
            }

            if (_dict.TryGetValue(id, out Stage value))
            {
                return value;
            }

            return default;
        }

        public Stage this[int id] => Get(id);

        // 데이터 저장
        public void PreInitialization(string[] lines)
        {
            _dict = new Dictionary<int, Stage>();
            
            //열 갯수
            for (int i = 3; i < lines.Length; i++)
            {
                //행 갯수
                string[] fields = lines[i].Split(',');
                
                Stage stage = new Stage();
                stage.id = int.Parse(fields[0]);
                stage.stage = int.Parse(fields[1]);;
                stage.enemyCount = int.Parse(fields[2]);;
                stage.clearCount = int.Parse(fields[3]);;
                
                _dict.Add(stage.id, stage);    
            }
        }
    }
}
