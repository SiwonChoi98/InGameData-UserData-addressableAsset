using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MakeUI : MonoBehaviour
{
    public void Button_MoveToMain()
    {
        LoadingManager.LoadScene("InGame");
    }
}
