using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class UserData
{
    public int level = 1;

    /// <summary>
    /// 유저가 캐릭터를 생성 했는지
    /// </summary>
    public bool isClass;
    
    /// <summary>
    /// 마지막으로 플레이한 스테이지 ID
    /// </summary>
    public int lastStageID;
    
    /// <summary>
    /// 현재 진행중인 스테이지 ID
    /// </summary>
    public int currentStageID;

    /// <summary>
    /// 클리어한 스테이지 ID
    /// </summary>
    public int bestStageID;

    
    
}
