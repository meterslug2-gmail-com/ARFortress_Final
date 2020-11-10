using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Front : MonoBehaviour
{
    public static MeshRenderer renderF;
    private void Start()
    {
        renderF = GetComponent<MeshRenderer>();
        PhotonManager.Instance.isFornt = true;
    }
    private void OnTriggerEnter(Collider other)
    {
        //if((!other.CompareTag("User"))||(!other.CompareTag("Leg")) ||(!other.CompareTag("Spider")))
        //{
        //PhotonManager.Instance.isFornt = false;//앞에 뭐가 있으면 못간다.
        //}
    }
    private void OnTriggerExit(Collider other)
    {
        //if ((!other.CompareTag("User")) || (!other.CompareTag("Leg")) || (!other.CompareTag("Spider")))
        //{
        //Debug.Log("앞에 안막혀있음");
            PhotonManager.Instance.isFornt = true;//앞에 뭐가 없으면 갈수 있다
        renderF.material.color = Color.white; //true면 갈수 있다

        //}
    }
    private void OnTriggerStay(Collider other)
    {
        //Debug.Log("앞에 막혀있음");
        PhotonManager.Instance.isFornt = false;//앞에 뭐가 있으면 못간다.
        renderF.material.color = Color.red;//false면 못간다

    }

}
