using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;

public class TEST : MonoBehaviour
{
    //protected Rigidbody[] rbs;
    //protected BoxCollider[] boxcs;
    void Start()
    {
        //rbs = GetComponentsInChildren<Rigidbody>();
        //boxcs = GetComponentsInChildren<BoxCollider>();
        //for (int i=0; i<rbs.Length;i++)
        //{
        //    rbs[i].isKinematic = false;
        //    boxcs[i].isTrigger = false;

        //}
        //for(int i=0;i<rbs.Length;i++)
        //{
        //    rbs[i].AddExplosionForce(1, new Vector3(0, 1, 0), 1);
        //}
        
    }
    private void Update()
    {
        //transform.Rotate(Vector3.up * 10);
        transform.position += transform.forward * Time.deltaTime*2;
    }

}
