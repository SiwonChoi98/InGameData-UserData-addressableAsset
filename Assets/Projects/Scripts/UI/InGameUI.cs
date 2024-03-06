using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class InGameUI : MonoBehaviour
{
    private void Awake()
    {
        
    }
    

    private void Start()
    {
        for (int i = 0; i < 20; i++)
        {
            Debug.Log("spec monster " + SpecDataManager.Instance.Monster.Get(i+1000).hp);
        }
        
    }
    
    private void Update()
    {
        
    }
}
