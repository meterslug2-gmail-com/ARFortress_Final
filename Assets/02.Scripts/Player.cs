using Photon.Pun;
using UnityEngine;
using Photon.Realtime;
using PlayFab.GroupsModels;

public class Player : MonoBehaviourPunCallbacks
{
    //[SerializeField]
    //protected GameObject bullet;//날아갈 미사일
    [SerializeField]
    protected GameObject depthMissile; //뎁스 API 적용 미사일
    public GameObject isMine;
    public string legname;//다리 이름
    public string bodyname;//몸체 이름
    public string weaponname; //무기 이름
    public int actnum; //이객체가 누구의 소유인지 체크

    public int attack;//공격력
    public int hp;//체력
    public int amor;//방어력
    public float speed;//이동속도
    public int lange;//사거리

    private GameObject legObj;
    private GameObject bodyObj;
    private GameObject weaponObj; //좌우회전때 회전시킬 대상
    private Transform barrel; //포신 위아래로 움직이는 대상

    public bool isFireCheck = false;
    protected DieAction bodyAction;
    protected DieAction weaponAction;
    [SerializeField]
    protected Rigidbody rb;
    void Start()
    {
        attack = DataManager.Instance.weaponattack;
        hp = DataManager.Instance.bodyhp;
        amor = DataManager.Instance.legamor + DataManager.Instance.bodyamor;
        speed = DataManager.Instance.legspeed;
        lange = DataManager.Instance.weaponlange;
        //다리 정보 찾아서 생성
        for(int i=0; i<DataManager.Instance.legPartsArr.Length;i++)
        {
            if(DataManager.Instance.legPartsArr[i].name==legname)
            {
                legObj = Instantiate(DataManager.Instance.legPartsArr[i], transform.position, Quaternion.identity, transform);
            }
        }
        //몸체 정보 찾아서 생성
        for(int i=0;i<DataManager.Instance.bodyPartsArr.Length;i++)
        {
            if(DataManager.Instance.bodyPartsArr[i].name == bodyname)
            {
                bodyObj = Instantiate(DataManager.Instance.bodyPartsArr[i], legObj.GetComponent<LegParts>().bodyPos.transform.position, Quaternion.identity, transform);
            }
        }
        //무기 정보 찾아서 생성
        for(int i=0; i<DataManager.Instance.weaponPartsArr.Length;i++)
        {
            if(DataManager.Instance.weaponPartsArr[i].name == weaponname)
            {
                weaponObj = Instantiate(DataManager.Instance.weaponPartsArr[i], bodyObj.GetComponent<BodyParts>().weaponPos.transform.position, Quaternion.identity, transform);
            }
        }
        transform.forward = PhotonManager.Instance.hostingResultList[0].Result.Anchor.transform.position - transform.position;
        if(actnum == PhotonManager.Instance.myOrder)
        {
            isMine.SetActive(true);
        }
        barrel = weaponObj.GetComponent<WeaponParts>().ReturnTransform();

        legObj.GetComponent<LegParts>().OnDepthMateiral();
        bodyObj.GetComponent<BodyParts>().OnDepthMateiral();
        weaponObj.GetComponent<WeaponParts>().OnDepthMateiral();
        //죽엇을때 모습을위함 메소드가 들어 있음
        bodyAction = bodyObj.GetComponent<DieAction>();
        weaponAction = weaponObj.GetComponent<DieAction>();

        if(PhotonManager.Instance.myOrder == actnum)
        {
            PhotonManager.Instance.myHp = hp;
            PhotonManager.Instance.myAmor = amor;
            PhotonManager.Instance.myDmg = attack;
            DataManager.Instance.ResetmyTime();
            DataManager.Instance.GameStart(true);//내로봇이 생성되면 게임이 사작된것이다
            //if(PhotonManager.Instance.isMaster==true)
            //{
            //    DataManager.Instance.ChangeControlable(true);
            //}
        }
    }

    
    void Update()
    {
        if(!PhotonManager.Instance.isMaster)//마스터고 나한테 제어권이 있으면?
        {
            //내차례가 아니면 로봇들의 리지드 바디 꺼놓는다
            rb.isKinematic = true;
           
        }
        else
        {
            //내차례면 로봇들의 리지드 바디를 켜놓는다
            rb.isKinematic = false;
        }

        //이로봇 객체의 엑터 넘버와 현재 턴이 일치할때 움직임값을 받아서 움직인다
        if(PhotonManager.Instance.nowTurn == actnum) //이로봇의 엑터 넘버와 현재 턴이 일치하면?
        {

            if (PhotonManager.Instance.isInput == true)
            {
            MovePlayer();
            }
            else
            {
                DataManager.Instance.ChangeAnimation(false);
            }
            //현재 턴과 이 로봇 객체의 엑터넘버가 일치하고 위아래 회전중일때
            if (PhotonManager.Instance.isRotateUpDown == true) RotateUpDown();
            //현재턴과 이 로봇 객체의 엑터넘버가 일치하고 좌우 회전중일때
            if (PhotonManager.Instance.isRotateLeftRight == true) RotateLeftRight();

            if(PhotonManager.Instance.isFireCheck == true)
            {
                PhotonManager.Instance.isFireCheck = false;
                //FireBullet(PhotonManager.Instance.lange); //그냥 총알
                FireMissile(PhotonManager.Instance.lange); //뎁스 적용해본것
            }
        }
    }

    /// <summary>
    /// NetWork의 x값은 컨트롤러의 좌우 값을, z는 앞뒤값을 변경하는데 사용한다
    /// </summary>
    public void MovePlayer()
    {
        //좌우 회전에 관한 x 값의 변화가 있을때
        if(PhotonManager.Instance.x !=0)
        {
            float dir = PhotonManager.Instance.x;
            if (PhotonManager.Instance.z < 0) dir *= -1;
            transform.Rotate(Vector3.up * dir);
            //전진중일때
            //if (PhotonManager.Instance.z > 0)
            //{
            //    //Debug.Log("회전중");
            //    transform.Rotate(Vector3.up * PhotonManager.Instance.x);
            //}
            ////후진중일때
            //if (PhotonManager.Instance.z < 0)
            //{
            //    transform.Rotate(Vector3.up * -PhotonManager.Instance.x);

            //}

        }
        //전진 후진 에 대한 z축값에 변화가 있을때
        if (PhotonManager.Instance.z !=0)
        {
            PhotonManager.Instance.hight = transform.position.y;
            Vector3 dir = transform.forward * PhotonManager.Instance.z;
            dir.Normalize();
            transform.position += dir * (speed / 600) * Time.deltaTime;
            transform.position.Set(transform.position.x, PhotonManager.Instance.hight, transform.position.z);
        }
        DataManager.Instance.ChangeAnimation(true);
    }
    /// <summary>
    /// 좌우 회전할때
    /// </summary>
    public void RotateLeftRight()
    {
        weaponObj.transform.Rotate(Vector3.up * -PhotonManager.Instance.rotateY);
    }
    /// <summary>
    /// 포신 위아래 회전할때
    /// </summary>
    public void RotateUpDown()
    {
        barrel.Rotate(Vector3.right * -PhotonManager.Instance.rotateX);
    }
    /// <summary>
    /// 게이지 모은 만큼 미사일 발사
    /// </summary>
    /// <param name="lange"></param>
    //public void FireBullet(float lange)
    //{
    //    Transform firePos1 = weaponObj.GetComponent<WeaponParts>().ReturnFirePos1();
    //    Transform firePos2 = weaponObj.GetComponent<WeaponParts>().ReturnFirePos2();
    //    if(firePos1 !=null)
    //    {
    //        GameObject obj = Instantiate(bullet, firePos1.transform.position, Quaternion.identity);
    //        obj.GetComponent<TestBall>().dir = firePos1.transform.forward;
    //        obj.GetComponent<TestBall>().speed = lange*0.05f;
    //    }

    //    if(firePos2 !=null)
    //    {
    //        GameObject obj = Instantiate(bullet, firePos2.transform.position, Quaternion.identity);
    //        obj.GetComponent<TestBall>().dir = firePos2.transform.forward;
    //        obj.GetComponent<TestBall>().speed = lange*0.05f;
    //    }
    //    PhotonManager.Instance.lange = 0; //쏘고나서 게이지 초기화
    //}

    public void FireMissile(float lange)
    {
        Transform firePos1 = weaponObj.GetComponent<WeaponParts>().ReturnFirePos1();
        Transform firePos2 = weaponObj.GetComponent<WeaponParts>().ReturnFirePos2();
        if (firePos1 != null)
        {
            GameObject obj = Instantiate(depthMissile, firePos1.transform.position, Quaternion.identity);
            obj.GetComponent<MissileShoot>().dir = firePos1.transform.forward;
            obj.GetComponent<MissileShoot>().speed = lange * 0.05f;
            obj.GetComponent<MissileShoot>().isMasterMisill = true;
            obj.GetComponent<MissileShoot>().OwnerActNum(PhotonManager.Instance.shooter);//미사일에 내 엑터 넘버를 부여한다
            PhotonManager.Instance.misillListInScent.Add(obj); //발사된 미사일은 리스트에 추가
        }

        if (firePos2 != null)
        {
            GameObject obj = Instantiate(depthMissile, firePos2.transform.position, Quaternion.identity);
            obj.GetComponent<MissileShoot>().dir = firePos2.transform.forward;
            obj.GetComponent<MissileShoot>().speed = lange * 0.05f;
            obj.GetComponent<MissileShoot>().isMasterMisill = false;
            PhotonManager.Instance.misillListInScent.Add(obj);
        }
        PhotonManager.Instance.lange = 0; //쏘고나서 게이지 초기화
    }
    //죽엇을때 몸체 날아감
    public void ActionBodyDie()
    {
        bodyAction.DestroyAction();
    }
    //죽었을때 무기 날아감
    public void ActionWeaponDie()
    {
        weaponAction.DestroyAction();
    }
}
