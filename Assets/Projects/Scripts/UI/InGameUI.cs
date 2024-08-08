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
        
    }

    public void Btn_SaveAppearance()
    {
        PlayfabManager.Instance.SaveAppearance();
    }

    public void Btn_GetAppearance()
    {
        PlayfabManager.Instance.GetAppearance();
    }
}
