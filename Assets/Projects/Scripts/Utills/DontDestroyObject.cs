using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DontDestroyObject : Singleton<DontDestroyObject>
{
    [SerializeField] private bool _addComponent = true;
    
    protected void Awake()
    {
        InitManager();
    }
    protected void Start()
    {
        DontDestroyOnLoad(this.gameObject);
    }

    private void InitManager()
    {
        //시작 시 계속 유지되어야 할 매니저들은 여기서 셋팅
        SetSingleton<DataManager>();
        SetSingleton<BattleManager>();
        SetSingleton<PlayfabManager>();
    }

    private void SetSingleton<T>()
    {
        if (_addComponent)
        {
            gameObject.AddComponent(typeof(T));
        }
        else
        {
            var newObject = new GameObject(typeof(T).FullName);
            newObject.transform.SetParent(this.transform);
        }
    }
    
}
