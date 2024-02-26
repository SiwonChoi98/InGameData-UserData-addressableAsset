using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class AddressableManager : MonoBehaviour
{
    [SerializeField] private AssetReferenceGameObject _test1Obj;

    //오브젝트를 불러오면 나중에 해제를 했을 때 담아줄 리스트
    private List<GameObject> _GsList = new List<GameObject>();
    private void Start()
    {
        SpawnObj();
    }

    private void Update()
    {
        ReleaseObj();
    }
    
    private void SpawnObj()
    {
        _test1Obj.InstantiateAsync().Completed += (obj) =>
        {
            _GsList.Add(obj.Result);
        };
    }

    private void ReleaseObj()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (_GsList.Count == 0)
                return;

            int index = _GsList.Count - 1;
            
            //InstantiateAsync으로 생성된 오브젝트의 메모리를 해제하고 제거함
            Addressables.ReleaseInstance(_GsList[index]);
            _GsList.RemoveAt(index);
        }
    }
}
