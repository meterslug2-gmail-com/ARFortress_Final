using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public  class Back : MonoBehaviour
{

    public static MeshRenderer renderB;
    private void Start()
    {
        renderB = GetComponent<MeshRenderer>();
        PhotonManager.Instance.isBack = true;
    }
    private void OnTriggerEnter(Collider other)
    {
        //if ((!other.CompareTag("User")) || (!other.CompareTag("Leg")) || (!other.CompareTag("Spider")))
        //{
            //PhotonManager.Instance.isBack = false;//뒤에 뭐가 있으면 뒤로 못간다
        //}
    }
    private void OnTriggerExit(Collider other)
    {
        //if ((!other.CompareTag("User")) || (!other.CompareTag("Leg")) || (!other.CompareTag("Spider")))
        //{
        //Debug.Log("뒤에 안막혀있음");
        PhotonManager.Instance.isBack = true;//뒤에 뭐가 없으면 갈수 있다
        renderB.material.color = Color.white;
       
        //}
    }
    private void OnTriggerStay(Collider other)
    {
        //Debug.Log("뒤에 막혀있음");
        PhotonManager.Instance.isBack = false;//뒤에 뭐가 있으면 뒤로 못간다
        renderB.material.color = Color.red;
       
    }

}

