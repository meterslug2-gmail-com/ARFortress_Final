using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DieAction : MonoBehaviour
{
    [SerializeField]
    protected Rigidbody rb;
    [SerializeField]
    protected Vector3 dir;
    protected BoxCollider boxcoll;
    void Start()
    {
        boxcoll = GetComponent<BoxCollider>();
        rb = GetComponent<Rigidbody>();
        dir = new Vector3(Random.Range(0, 1f), Random.Range(0, 1f), Random.Range(0, 1f));
    }
    public void DestroyAction()
    {
        boxcoll.isTrigger = false;
        rb.isKinematic = false;
        //rb.AddExplosionForce(20f, transform.position, 180f,1f);
        rb.AddForce(dir*45f);

    }
   
}
