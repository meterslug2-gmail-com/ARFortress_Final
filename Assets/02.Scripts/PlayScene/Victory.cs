using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Victory : MonoBehaviour
{
    protected DBManager dbManager;
    void Start()
    {
        //승리 패널이 켜지면 돈을 넣어준다
        dbManager = DBManager.GetInstance();
        dbManager.GetMoney(1000);
    }


}
