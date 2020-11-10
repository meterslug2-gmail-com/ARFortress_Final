using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PowerManager : MonoBehaviour
{
    //private RectTransform rectComponent;
    [SerializeField]
    protected Image imageComp;
    protected float maxDistance;

    // Use this for initialization
    void Start()
    {
        maxDistance = DataManager.Instance.weaponlange;//최대 파워
        Debug.Log("maxDistance : " + maxDistance);
        //rectComponent = GetComponent<RectTransform>();
        //imageComp = rectComponent.GetComponent<Image>();
        imageComp.fillAmount = 0;
    }

    // Update is called once per frame
    void Update()
    {
        imageComp.fillAmount = (PhotonManager.Instance.lange / maxDistance);//현재파워/최대 파워
    }
}
