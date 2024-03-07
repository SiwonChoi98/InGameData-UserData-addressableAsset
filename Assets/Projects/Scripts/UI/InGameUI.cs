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
        Debug.Log(SpecDataManager.Instance.Monster.Get(1003).mp);
    }
    
    private void Update()
    {
        
    }
}
