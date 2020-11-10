﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissileShoot : MonoBehaviour
{
    public bool isFire = false;
    public Vector3 dir;//포신이 향하고 있는 방향
    public float speed;// 속도
    protected bool isHit = false;
    [SerializeField]
    protected Rigidbody rb;
    [SerializeField]
    protected GameObject explosion;
    [SerializeField]
    protected GameObject startEffecf;
    public bool isMasterMisill;//미사일이 2개가 발사될수도 있고 1개가 나갈수도 있다 하나만 골라서 폭파시 턴전환이 되게 한다
    protected GameObject obj;
    protected float time;
    protected int ownerActnum;
    private void Awake()
    {
        //미사일이 생성되면 더이상의 조종은  턴이 넘어갈때까지 불가능하다
        DataManager.Instance.ChangeControlable(false);
    }
    private void Start()
    {
        Debug.Log("isMasterMisill = " + isMasterMisill);
        isHit = false;
        rb.velocity = (dir * speed*1.2f);
        Instantiate(startEffecf,transform.position, Quaternion.identity);//발사시 이펙트 발생
        SoundManager.Instance.SetEffectClip("shot");
       
    }
    private void FixedUpdate()
    {
        time += Time.deltaTime; //7초가 지나도 안터지면 터지게 한다
        if(time >7)
        {
            if ((PhotonManager.Instance.isMaster==true)&& (isMasterMisill==true))
            {
                //미사일이 충돌했을때 현재 씬이 마스터면 미사일 폭파 명령을 호출
                PhotonManager.Instance.Call_DestroyMisillInScene(); //나도 파괴 다른사람도 파괴하라는 명령을 내린다
            }
        }
    }
    private void Update()
    {
        if(!isHit)
        {
        transform.forward = rb.velocity;
        }

    }
    private void OnCollisionEnter(Collision collision)
    {
        isHit = true;
        if ((PhotonManager.Instance.isMaster) && (isMasterMisill==true))
        {
            //미사일이 충돌했을때 현재 씬이 마스터면 미사일 폭파 명령을 호출
            PhotonManager.Instance.Call_DestroyMisillInScene(); //나도 파괴 다른사람도 파괴하라는 명령을 내린다
        }
    }
    //오브젝트가 파괴될때 호출됨
    private void OnDestroy()
    {
        SoundManager.Instance.SetEffectClip("explosion");
        obj = Instantiate(explosion, transform.position, Quaternion.identity); //사라지기전 이펙트를 남긴다
        obj.GetComponent<MisillEffect>().IsMasterEffect(isMasterMisill);
        obj.GetComponent<MisillEffect>().OwnerActNum(ownerActnum);//이펙트에 내 엑터 넘버를 부여한다
    }
    public void OwnerActNum(int actNum)
    {
        ownerActnum = actNum;
    }
}
