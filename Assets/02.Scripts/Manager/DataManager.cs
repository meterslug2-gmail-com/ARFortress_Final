using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayFab.ClientModels;
using Photon.Pun;

public class DataManager : MonoBehaviour
{
    protected static DataManager instance = null;
    //--------------------------------임시 추가
    public static DataManager Instance
    {
        get
        {
            if (null == instance)
            {
                return null;
            }
            return instance;
        }
    }
    //------------------------------------------------
    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }
        DontDestroyOnLoad(this.gameObject);
    }
    public static DataManager GetInstance()
    {
           if (instance == null)
            {
                GameObject obj = new GameObject();
                obj.name = "DataManager";
                instance = obj.AddComponent<DataManager>();
            }
            return instance;
    }


    public int myMoney;//현재 내가 가진돈
    public List<CatalogItem> itemList = new List<CatalogItem>();
    //무기 리스트
    public List<CatalogItem> weaponList = new List<CatalogItem>();
    //몸통 리스트
    public List<CatalogItem> bodyList = new List<CatalogItem>();
    //다리 리스트
    public List<CatalogItem> legList = new List<CatalogItem>();

    [Header("무기 상세정보 담은 리스트")]
    public List<WeaponInfo> weaponInfo_List = new List<WeaponInfo>();    //무기의 상세 정보를 담을 리스트

    [Header("몸통 상세정보 담은 리스트")]
    public List<BodyInfo> bodyInfo_List = new List<BodyInfo>();    //몸통의 상세 정보를 담을 리스트

    [Header("다리 상세정보 담은 리스트")]
    public List<LegInfo> legInfo_List = new List<LegInfo>();    //다리의 상세 정보를 담을 리스트

    [Header("보유한 아이템 리스트")]
    public List<string> ownedItem_List = new List<string>();  //보유 아이템 리스트

    [Header("인트로에서 무기 Prefab정보 담을 배열")]
    public GameObject[] weaponPartsArr;

    [Header("인트로에서 몸통 Prefab정보 담을 배열")]
    public GameObject[] bodyPartsArr;

    [Header("인트로에서 다리 Prefab정보 담을 배열")]
    public GameObject[] legPartsArr;

    [Header("상점에서 사용할 스프라이트 이미지")]
    public Sprite[] imgArr;

    [Header("랩실에서 조립할려고 현재 선택한 파츠")]
    public GameObject selectLeg;//선택한 다리
    public GameObject selectBody;//선택한 몸통
    public GameObject selectWeapon;//선택한 무기

    [Header("DB에 저장될 유저의 정보")]
    public UserInfo userinfo;//타이틀 데이터에 저장될 플레이어 정보

    [Header("현재 조립된 무기의 프리팹")]
    public GameObject weaponPrefab;
    public GameObject bodyPrefab;
    public GameObject legPrefab;

    //클릭하고 파츠의 교체가 될때마다 이전의 정보를 기억한다.(최종 사용할 프리팹 정보)
    [Header("이전에 선택한 파츠프리팹")]
    public GameObject beforeLeg; //이전 다리
    public GameObject beforeBody; //이전 몸체
    public GameObject beforeWeapon; //이전 무기

    //CurrentStats() 함수에서 현재 조립된 부품의 인덱스 정보를 기억한다.
    [Header("현재 조립된 부품의 인덱스 정보")]
    public int legindex;
    public int bodyindex;
    public int weaponindex;

    [Header("다리 능력치")]
    public int legtotalweight;
    public float legspeed;
    public int legamor;

    [Header("몸통 능력치")]
    public int bodyhp;
    public int bodyamor;
    public string bodytype;
    public int bodyweight;

    [Header("무기 능력치")]
    public int weaponweight;
    public string weapontype;
    public int weaponlange;
    public int weaponattack;

    [Header("조립가능 여부")]
    public bool isloadOver = false;//하중 초과여부
    public bool istypeNoeSame = false;//타입 불일치 여부
    [Header("이전 부품의 이름")]
    public string beforeLegName;
    public string beforeBodyName;
    public string beforeWeaponName;

    [Header("메세지")]
    public string Msg;
    [Header("방에 생성된 로봇들")]
    protected Dictionary<int, GameObject> robots = new Dictionary<int, GameObject>();

    [Header("방에 살아 있는 사람")]
    protected Dictionary<int, bool> aliveMember = new Dictionary<int, bool>();

    [Header("내가 조종 가능한가")]
    protected bool isControl;

    protected bool isMove;

    protected float myTime;//내 남은 시간
    protected bool isGameStart;
    /// <summary>
    /// 내 로봇이 생성되면 게임 시작이라는것을 알려주기 위해서 isGmaeStart 를 true로 바꾸고 리턴
    /// </summary>
    /// <returns></returns>
    public void GameStart(bool start)
    {
        isGameStart = start;    
    }
    public bool ReturnIsGameStart()
    {
        return isGameStart;
    }
    /// <summary>
    /// 다리 부분 애니메이션 동작 바꾸기 위한 불값변경
    /// </summary>
    /// <param name="motion"></param>
    public void ChangeAnimation(bool motion)
    {
        isMove = motion;
    }
    /// <summary>
    /// 애니메니션 상태 리턴
    /// </summary>
    /// <returns></returns>
    public bool ReturnIsMove()
    {
        return isMove;
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    /// <summary>
    /// 딕셔너리에 키값을 엑터넘버로 해서 로봇을 생성함
    /// </summary>
    /// <param name="actNum"></param>
    /// <param name="robot"></param>
    public void AddUser(int actNum, GameObject robot)
    {
        robots.Add(actNum, robot);
        Debug.Log(actNum + "번 유저 로봇 딕셔너리 저장");
    }
    /// <summary>
    /// 죽은 유저 처리 콜라이더 비활성화 죽은후 시체 처리
    /// </summary>
    /// <param name="actorNum"></param>
    public void DestroyUser(int actorNum)
    {
        if(robots.ContainsKey(actorNum))
        {
            //더이상 피격안되게 콜라이더 비활성화
            robots[actorNum].GetComponent<BoxCollider>().enabled = false;
            Debug.Log(actorNum+"번 유저의 콜라이더 해제");
            //시체 처리
            robots[actorNum].GetComponent<Player>().ActionBodyDie();
            robots[actorNum].GetComponent<Player>().ActionWeaponDie();
        }
    }
    /// <summary>
    /// 유저가 방에 들어올때 플레이 가능한 상태인 true로 해서 엑터 넘버를 키값으로 값을 저장한다
    /// </summary>
    /// <param name="actNum"></param>
    /// <param name="alive"></param>
    public void UserState(int actNum, bool alive)
    {
        aliveMember.Add(actNum, alive);
    }
    /// <summary>
    /// 사용자가 플레이 불가 상태 죽거나 방에서 나갔을떄 false로 해준다
    /// </summary>
    /// <param name="actNum"></param>
    /// <param name="alive"></param>
    public void ChanegeAliveUserState(int actNum, bool alive)
    {
        aliveMember[actNum] = alive;
    }
/// <summary>
/// 플레이 가능한 사람이 몇명인지 알아본다 매개변수는 포톤 현재 방입장인원의 정보를 가지고 있는 배열을 가지고온다
/// </summary>
    protected int aliveCnt;
    public int ComputeAliveUser(Photon.Realtime.Player[] playerArr)
    {
        Debug.Log(aliveMember.Values);
        aliveCnt = 0;
        for (int i=0; i< playerArr.Length; i++)
        {
            if(aliveMember[playerArr[i].ActorNumber]==true)
            {
                aliveCnt++;
            }
        }
        return aliveCnt;
    }
    /// <summary>
    /// aliveMember 딕셔너리 초기화해주는거 Room에 들어오면 딕셔너리가 전부 비워져 있어야한다
    /// </summary>
    public void ResetDictionary()
    {
        aliveMember.Clear();
        Debug.Log("aliveMember.Count = " + aliveMember.Count);
        robots.Clear();
        Debug.Log("robots.Count = " + robots.Count);
    }
    /// <summary>
    /// 내가 승자인지 패자인제 체크
    /// </summary>
    /// <param name="actNum"></param>
    public bool CheckWinner(int actNum)
    {
        if(aliveMember[actNum]==true)
        {
            Debug.Log("승리");
            return true;
        }
        else
        {
            Debug.Log("패배");
            return false;
        }
    }
    /// <summary>
    /// 내가 조종 가능한지 여부를 확인하기위한 불값을 리턴
    /// </summary>
    /// <returns></returns>
    public bool CheckControlable()
    {
        return isControl;
    }
    /// <summary>
    /// 내 조종 상태를 변경하기 위해서
    /// </summary>
    /// <param name="control"></param>
    public void ChangeControlable(bool control)
    {
        Debug.Log("내 제어권 = " + control +"로 변경");
        isControl = control;
    }
    public void ResetmyTime()
    {
        myTime = 20.0f;
    }
    /// <summary>
    /// 내시간 체크해서 20초가 넘어가면 턴을넘기게 한다
    /// </summary>
    public void SubtractmyTime()
    {
        if (myTime < 0)//시간이 다되면?
        {
            if(CheckControlable()==true)//시간이 다됬는데 제어권이 나한테 있다면?
            {
                ChangeControlable(false);//제어권 뻇고
                PhotonManager.Instance.Call_ChangeMasterClient();//턴을 넘긴다
            }
            return;
        }
        else
        {
        myTime -= Time.deltaTime;
        }
    }
    /// <summary>
    /// 내 남은 시간을 리턴한다
    /// </summary>
    /// <returns></returns>
    public float ReturnMyTime()
    {
        return myTime;
    }
}
