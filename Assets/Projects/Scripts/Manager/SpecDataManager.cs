using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Security.Cryptography;
using System.Text;
using System.Linq;
public interface ISpecData<K, T> where T : class
{
    IReadOnlyList<T> All { get; }

    T Get(K id);

    T this[K id] { get; }
}

public partial class SpecDataManager : MonoBehaviour
{
    public ISpecData<int, GameConfig> GameConfig { get; private set; }
    public ISpecData<int, Skill> Skill { get; private set; }
    public ISpecData<int, SkillLevel> SkillLevel { get; private set; }
    public ISpecData<int, InGameExp> InGameExp { get; private set; }
    public ISpecData<int, Stage> Stage { get; private set; }
    public ISpecData<int, Monster> Monster { get; private set; }
    public ISpecData<int, RewardGroup> RewardGroup { get; private set; }
    private void Awake()
    {
        Load(LoadSpecData());
    }

    private static readonly byte[] _key = {38, 37, 64, 111, 92, 69, 80, 101, 93, 60, 44, 56, 101, 103, 70, 42};
    public static string LoadSpecData()
    {
        var byteKey = _key.Clone() as byte[];
        for (byte i = 0; i < byteKey!.Length; i++)
        {
            byte b = byteKey[i];
            int v = b ^ i;
            byteKey[i] = (byte)v;
        }

        byte[] path =
        {
            83,
            112,
            101,
            99,
            68,
            97,
            116,
            97
        }; // SpecData
        var data = Resources.Load<TextAsset>(Encoding.UTF8.GetString(path));
        if (data == default)
        {
            Resources.UnloadAsset(data);
            return default;
        }

        var bytes = data.bytes;
        byte[] dataArray;
        try
        {
            byte[] keyArray1 = byteKey;
            byte[] ikArray = byteKey.Reverse().ToArray();
            RijndaelManaged rijndaelManaged1 = new RijndaelManaged();
            rijndaelManaged1.Mode = CipherMode.CBC;
            rijndaelManaged1.Padding = PaddingMode.PKCS7;
            rijndaelManaged1.Key = keyArray1;
            rijndaelManaged1.IV = ikArray;
            ICryptoTransform cryptoTransform1 = rijndaelManaged1.CreateDecryptor();
            byte[] resultArray = cryptoTransform1.TransformFinalBlock(bytes, 0, bytes.Length);
            dataArray = resultArray;
        }
        catch
        {
            dataArray = default;
        }

        Resources.UnloadAsset(data);
        return dataArray == default ? default : Encoding.UTF8.GetString(dataArray);
    }
    
    public bool Load(string json)
    {
        try
        {
            InnerDatas jsonData = UnityEngine.JsonUtility.FromJson<InnerDatas>(json);
            OnDeserialize(jsonData);
            //OnLoad();
            //IsLoaded = true;
        }
        catch (Exception e)
        {
            UnityEngine.Debug.LogError(e.ToString());
            return false;
        }

        return true;
    }
    
    private void OnDeserialize(InnerDatas jsonData)
    {
        // instance, PreInitialization
        InnerDataGameConfig innerDataGameConfig = new InnerDataGameConfig();
        innerDataGameConfig.PreInitialization(jsonData.GameConfig);
        GameConfig = innerDataGameConfig;
        InnerDataSkill innerDataSkill = new InnerDataSkill();
        innerDataSkill.PreInitialization(jsonData.Skill);
        Skill = innerDataSkill;
        InnerDataSkillLevel innerDataSkillLevel = new InnerDataSkillLevel();
        innerDataSkillLevel.PreInitialization(jsonData.SkillLevel);
        SkillLevel = innerDataSkillLevel;
        InnerDataInGameExp innerDataInGameExp = new InnerDataInGameExp();
        innerDataInGameExp.PreInitialization(jsonData.InGameExp);
        InGameExp = innerDataInGameExp;
        InnerDataStage innerDataStage = new InnerDataStage();
        innerDataStage.PreInitialization(jsonData.Stage);
        Stage = innerDataStage;
        InnerDataMonster innerDataMonster = new InnerDataMonster();
        innerDataMonster.PreInitialization(jsonData.Monster);
        Monster = innerDataMonster;
        InnerDataRewardGroup innerDataRewardGroup = new InnerDataRewardGroup();
        innerDataRewardGroup.PreInitialization(jsonData.RewardGroup);
        RewardGroup = innerDataRewardGroup;
        // ReferenceInitialization (다른 Data 참조 설정)
    }
}

public class GameConfig
{
    public int id;
    public string key;
    public float value;
}
public class Skill
{
    public int id;
    public string name;
    public int skill_type;
}
public class SkillLevel
{
    public int id;
    public int skill_id;
    public int level;
    public string projectile_name;
    public string folder_name;
    public string sprite;
    public string desc;
    public int next_level_id;
    public int prj_type;
    public float skill_cooltime;
    public int base_obj_count;
    public float base_damage_rate;
    public float prj_scale;
    public float desc_valueType;
    public float value1;
    public float scale_valueType;
    public float value2;
}
public class InGameExp
{
    public int id;
    public int lv;
    public int exp_min;
    public int exp_max;
    public int need;  
}
public class Stage
{
    public int id;
    public int stage;
    public string name;
}
public class Monster
{
    public int id;
    public string name;
    public string prefab_name;
    public string sprite_name;
    public int monster_type;
    public float atk_type;
    public float move_type;
    public float spawn_type;
    public float move_speed;
    public int atk;
    public int hp;
    public float atk_range;
    public float atk_speed;
    public float prj_speed;
    public float multiple;
    public float cool_time;
    public float frequency;
    public int monster_count;
    public int reward_group_id;
}
public class RewardGroup
{
    public int id;
    public int reward_type1;
    public int reward_value1;
    public int reward_type2;
    public int reward_value2;
    public int reward_type3;
    public int reward_value3;
    public int reward_type4;
    public int reward_value4;
    
}

partial class SpecDataManager
{
    [Serializable]
    private class InnerDatas
    {
        public List<GameConfig> GameConfig = default;
        public List<Skill> Skill = default;
        public List<SkillLevel> SkillLevel = default;
        public List<InGameExp> InGameExp = default;
        public List<Stage> Stage = default;
        public List<Monster> Monster = default;
        public List<RewardGroup> RewardGroup = default;
    }
}
partial class SpecDataManager
{
    private class InnerDataGameConfig : ISpecData<int, GameConfig>
    {
        public IReadOnlyList<GameConfig> All { get; private set; }

        private Dictionary<int, GameConfig> _dict;
        public GameConfig Get(int id)
        {
            if (id == default)
            {
                throw new ArgumentException("잘못된 GameConfig id : default 값 참조");
            }

            if (_dict.TryGetValue(id, out GameConfig value))
            {
                return value;
            }

            return default;
        }

        public GameConfig this[int id] => Get(id);
        // 초기화
        public void PreInitialization(IReadOnlyList<GameConfig> datas)
        {
            All = datas;
            _dict = new Dictionary<int, GameConfig>(datas.Count);
            foreach (GameConfig data in datas)
            {
                _dict.Add(data.id, data);
            }
        }
    }

    private class InnerDataSkill : ISpecData<int, Skill>
    {
        public IReadOnlyList<Skill> All { get; private set; }

        private Dictionary<int, Skill> _dict;
        public Skill Get(int id)
        {
            if (id == default)
            {
                throw new ArgumentException("잘못된 Skill id : default 값 참조");
            }

            if (_dict.TryGetValue(id, out Skill value))
            {
                return value;
            }

            return default;
        }

        public Skill this[int id] => Get(id);
        // 초기화
        public void PreInitialization(IReadOnlyList<Skill> datas)
        {
            All = datas;
            _dict = new Dictionary<int, Skill>(datas.Count);
            foreach (Skill data in datas)
            {
                _dict.Add(data.id, data);
            }
        }
    }

    private class InnerDataSkillLevel : ISpecData<int, SkillLevel>
    {
        public IReadOnlyList<SkillLevel> All { get; private set; }

        private Dictionary<int, SkillLevel> _dict;
        public SkillLevel Get(int id)
        {
            if (id == default)
            {
                throw new ArgumentException("잘못된 SkillLevel id : default 값 참조");
            }

            if (_dict.TryGetValue(id, out SkillLevel value))
            {
                return value;
            }

            return default;
        }

        public SkillLevel this[int id] => Get(id);
        // 초기화
        public void PreInitialization(IReadOnlyList<SkillLevel> datas)
        {
            All = datas;
            _dict = new Dictionary<int, SkillLevel>(datas.Count);
            foreach (SkillLevel data in datas)
            {
                _dict.Add(data.id, data);
            }
        }
    }

    private class InnerDataInGameExp : ISpecData<int, InGameExp>
    {
        public IReadOnlyList<InGameExp> All { get; private set; }

        private Dictionary<int, InGameExp> _dict;
        public InGameExp Get(int id)
        {
            if (id == default)
            {
                throw new ArgumentException("잘못된 InGameExp id : default 값 참조");
            }

            if (_dict.TryGetValue(id, out InGameExp value))
            {
                return value;
            }

            return default;
        }

        public InGameExp this[int id] => Get(id);
        // 초기화
        public void PreInitialization(IReadOnlyList<InGameExp> datas)
        {
            All = datas;
            _dict = new Dictionary<int, InGameExp>(datas.Count);
            foreach (InGameExp data in datas)
            {
                _dict.Add(data.id, data);
            }
        }
    }

    private class InnerDataStage : ISpecData<int, Stage>
    {
        public IReadOnlyList<Stage> All { get; private set; }

        private Dictionary<int, Stage> _dict;
        public Stage Get(int id)
        {
            if (id == default)
            {
                throw new ArgumentException("잘못된 Stage id : default 값 참조");
            }

            if (_dict.TryGetValue(id, out Stage value))
            {
                return value;
            }

            return default;
        }

        public Stage this[int id] => Get(id);
        // 초기화
        public void PreInitialization(IReadOnlyList<Stage> datas)
        {
            All = datas;
            _dict = new Dictionary<int, Stage>(datas.Count);
            foreach (Stage data in datas)
            {
                _dict.Add(data.id, data);
            }
        }
    }

    private class InnerDataMonster : ISpecData<int, Monster>
    {
        public IReadOnlyList<Monster> All { get; private set; }

        private Dictionary<int, Monster> _dict;
        public Monster Get(int id)
        {
            if (id == default)
            {
                throw new ArgumentException("잘못된 Monster id : default 값 참조");
            }

            if (_dict.TryGetValue(id, out Monster value))
            {
                return value;
            }

            return default;
        }

        public Monster this[int id] => Get(id);
        // 초기화
        public void PreInitialization(IReadOnlyList<Monster> datas)
        {
            All = datas;
            _dict = new Dictionary<int, Monster>(datas.Count);
            foreach (Monster data in datas)
            {
                _dict.Add(data.id, data);
            }
        }
    }

    private class InnerDataRewardGroup : ISpecData<int, RewardGroup>
    {
        public IReadOnlyList<RewardGroup> All { get; private set; }

        private Dictionary<int, RewardGroup> _dict;
        public RewardGroup Get(int id)
        {
            if (id == default)
            {
                throw new ArgumentException("잘못된 RewardGroup id : default 값 참조");
            }

            if (_dict.TryGetValue(id, out RewardGroup value))
            {
                return value;
            }

            return default;
        }

        public RewardGroup this[int id] => Get(id);
        // 초기화
        public void PreInitialization(IReadOnlyList<RewardGroup> datas)
        {
            All = datas;
            _dict = new Dictionary<int, RewardGroup>(datas.Count);
            foreach (RewardGroup data in datas)
            {
                _dict.Add(data.id, data);
            }
        }
    }
}
