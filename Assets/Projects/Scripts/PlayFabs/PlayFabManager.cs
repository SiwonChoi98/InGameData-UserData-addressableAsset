using PlayFab;
using PlayFab.ClientModels;
using PlayFab.Json;
using PlayFab.ProfilesModels;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using EntityKey = PlayFab.ProfilesModels.EntityKey;

public class PlayfabManager : Singleton<PlayfabManager>
{
    
    //private---------------------------------------------
    private static string _customId = "";
    private static string _playfabId = "";

    private string _entityId;
    private string _entityType;

    private const string _playerPrefKey = "GuestUserID";
    //public-----------------------------------------------
    public string PlayerPrefKey => _playerPrefKey;
    
    //게스트 로그인 버튼
    public void OnClickGuestLogin() 
    {
        string savedUserID = PlayerPrefs.GetString(_playerPrefKey);
        
        if (!PlayerPrefs.HasKey(_playerPrefKey))
            CreateGuestId();
        else
            LoginGuestId(savedUserID);
    }
    
    //저장된 아이디가 없을 경우 새로 생성
    private void CreateGuestId() 
    {
        _customId = GetRandomPassword(16);
       
        //유저 기기에 저장
        PlayerPrefs.SetString(_playerPrefKey, _customId);
        PlayerPrefs.Save();
        
        
        Debug.Log(_customId + " : createId");
        
        PlayFabClientAPI.LoginWithCustomID(new LoginWithCustomIDRequest()
        {
            CustomId = _customId,
            CreateAccount = true
        }, result =>
        {
            OnLoginSuccess(result);
        }, error =>
        {
            Debug.LogError("Login Fail - Guest");
        });
    }

    //랜덤한 16자리 id 생성
    private string GetRandomPassword(int _totLen) 
    {
        string input = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        var chars = Enumerable.Range(0, _totLen)
            .Select(x => input[UnityEngine.Random.Range(0, input.Length)]);
        return new string(chars.ToArray());
    }

    //게스트 로그인
    private void LoginGuestId(string savedUserID) 
    {
        Debug.Log("Guest Login");

        PlayFabClientAPI.LoginWithCustomID(new LoginWithCustomIDRequest()
        {
            CustomId = savedUserID,
            CreateAccount = false
        }, result =>
        {
            OnLoginSuccess(result);
        }, error =>
        {
            Debug.LogError("Login Fail - Guest");
        });
    }

    //로그인 결과
    public void OnLoginSuccess(LoginResult result) 
    {
        Debug.Log("Playfab Login Success");
        
        _playfabId = result.PlayFabId;
        _entityId = result.EntityToken.Entity.Id;
        _entityType = result.EntityToken.Entity.Type;
    }
}