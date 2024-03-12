using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MakeUI : MonoBehaviour
{
    //1. 현재 계정 아이디에 직업 정보 넣기
    
    //2. 직업 정보가 재대로 넣어졌다면, 인게임 이동

    public void Start()
    {
    }

    public void Button_MoveToMain()
    {
        LoadingManager.LoadScene("InGame");
    }
}
