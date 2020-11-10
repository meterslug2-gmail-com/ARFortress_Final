﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;//이벤트 시스템 사용

/// <summary>
/// 포탑 조종해서 조준하는 기능
/// </summary>
public class AimingButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public enum ButtonRoll
    {
        up,//0
        down,//1
        left,//2
        right,//3
        shoot//4
    }
    [SerializeField]
    protected ButtonRoll roll;
    [SerializeField]
    protected Aiming aiming;
    public void OnPointerDown(PointerEventData eventData)
    {
        if (PhotonManager.Instance.isMaster == true) //방장일때만 조작이 가능하게
        {
            if (DataManager.Instance.CheckControlable() == false)
            {
                return; //제어권이 false 이면 true 될때까지 못쏘게 막는다
            }
            //버튼을 누르고 있을때
            switch (roll)
            {
                case ButtonRoll.up:
                    //위버튼을 눌렀을때 
                    aiming.isRotateUpDown = true;
                    aiming.x = 1;
                    break;
                case ButtonRoll.down:
                    //아래 버튼을 눌렀을때
                    aiming.isRotateUpDown = true;
                    aiming.x = -1;
                    break;
                case ButtonRoll.left:
                    //왼쪽 버튼을 눌렀을때
                    aiming.isRotateLeftRight = true;
                    aiming.y = 1;
                    break;
                case ButtonRoll.right:
                    //오른쪽 버튼을 눌렀을때
                    aiming.isRotateLeftRight = true;
                    aiming.y = -1;
                    break;
                case ButtonRoll.shoot:
                    aiming.isControlPower = true;
                    PhotonManager.Instance.isFireCheck = false;
                    break;
            }
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        
        aiming.isRotateUpDown = false;
            aiming.isRotateLeftRight = false;
            aiming.isControlPower = false;
        if (PhotonManager.Instance.isMaster == true )
        {
            if (roll == ButtonRoll.shoot)
            {
                if (DataManager.Instance.CheckControlable() == false)
                {
                    return; //제어권이 false 이면 true 될때까지 못쏘게 막는다
                }

                //버튼에서 떼고 있을때
                DataManager.Instance.ChangeControlable(false);//쏘고나서 조종 불가 상태로 만든다
                PhotonManager.Instance.isFireCheck = true;
                //손떼면 손을뗏다는것을 알리는 불값을 전송한다 다른사용자에게
                PhotonManager.Instance.Call_SendShootValue(PhotonManager.Instance.isFireCheck, PhotonManager.Instance.lange,
                    PhotonManager.Instance.myOrder);
                //if(PhotonManager.Instance.isSingle==false)
                //{
                //    DataManager.Instance.ChangeControlable(false);//쏘고나서 조종 불가 상태로 만든다
                //}
            }
        }
    }
    /// <summary>
    /// 미사일 발사 버튼이 눌리면 그이후로 못눌리게 막는다
    /// </summary>
    public void TurnOffShotMissilButton()
    {
        DataManager.Instance.ChangeControlable(false);//쏘고나서 조종 불가 상태로 만든다
    }
}

