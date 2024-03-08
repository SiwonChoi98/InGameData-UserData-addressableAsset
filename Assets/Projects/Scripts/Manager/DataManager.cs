using System;
using System.Collections;
using System.Collections.Generic;
using PlayFab.ClientModels;
using UnityEngine;

//유저 데이터 저장 타입
public enum DataType : int
{
    None = 0,
    User = 1,
    ALL = int.MaxValue
}


//유저 데이터 관련
public partial class DataManager : MonoBehaviour
{
    //public---------------------
    public UserData UserData => _user;
    
    //private-------------------
    private UserData _user;

    
}

//유저 데이터 관리
public partial class DataManager : MonoBehaviour
{
    public Action<double, double> CoinValueChanged;
    public Action<double, double> JewelValueChanged;
    //private EncCurrencyData _encCurrencyData = new EncCurrencyData();
    public void AddCurrencyChangedEvent(CurrencyType currencyType, Action<double, double> chnagedAction)
    {
        switch (currencyType)
        {
            case CurrencyType.Coin:
                CoinValueChanged -= chnagedAction;
                CoinValueChanged += chnagedAction;
                break;
            case CurrencyType.Jewel:
                JewelValueChanged -= chnagedAction;
                JewelValueChanged += chnagedAction;
                break;
        }
    }

    public void AddCurrency(CurrencyType type, double value)
    {
        value = Math.Floor(value);
        switch (type)
        {
            case CurrencyType.None:
                break;
            case CurrencyType.Coin:
                break;
            case CurrencyType.Jewel:
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(type), type, null);
        }
    }

    public void UseCurrency(CurrencyType type, double value)
    {
        value = Math.Floor(value);
        switch (type)
        {
            case CurrencyType.None:
                break;
            case CurrencyType.Coin:
                break;
            case CurrencyType.Jewel:
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(type), type, null);
        }
    }

    public double GetCurrency(CurrencyType currencyType)
    {
        switch (currencyType)
        {
            case CurrencyType.Coin:
                break;
                //return _encCurrencyData.coin;
            case CurrencyType.Jewel:
                break;
                //return _encCurrencyData.jewel;
        }

        return 0;
    }
    
    private void AddCoin(double amount)
    {
        //_encCurrencyData.coin += amount;
        //CoinValueChanged?.Invoke(_encCurrencyData.coin, amount);
    }
    
    private void AddJewel(double amount)
    {
        //_encCurrencyData.coin += amount;
        //CoinValueChanged?.Invoke(_encCurrencyData.coin, amount);
    }
    
    private void UseCoin(double amount)
    {
        //_encCurrencyData.coin += amount;
        //CoinValueChanged?.Invoke(_encCurrencyData.coin, amount);
    }
    
    private void UseJewel(double amount)
    {
        //_encCurrencyData.coin += amount;
        //CoinValueChanged?.Invoke(_encCurrencyData.coin, amount);
    }
}