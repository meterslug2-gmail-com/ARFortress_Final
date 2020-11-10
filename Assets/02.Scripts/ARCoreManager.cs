//using Unity.Collections;
//using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
//using GoogleARCore.Examples.CloudAnchors;
using GoogleARCore;
using GoogleARCore.CrossPlatform;
using System.Collections;
using UnityEngine.SceneManagement;


public class ARCoreManager: MonoBehaviourPunCallbacks
{
    
    //Camera ARCamera;
    //public GameObject centerObject;
    //public Text text;
    public GameObject firstTouch;
    public bool firstAnchor = false;
    private TrackableHit hit;
    public Anchor anchor;// 생성한 로컬 앵커
    public string anchorid;//생성된 클라우드 앵커 아이디값
    private AsyncTask<CloudAnchorResult> task;
    private GameObject obj;
    private Pose pos; //  생성할 위치
    private Pose firstpos;//방장이 처음 터치한 위치
    public int nowPlayerCnt;//현재 방에 입장한 플레이어수
    public float DistanceFromCenter; //중심점으로 부터의 거리
    public bool ismakeAnchor = false;
    
    protected float time;
    [SerializeField]
    protected GameObject victoryPanenek;
    [SerializeField]
    protected GameObject gameOverPannel;
    protected bool isGameOver = false;
    [SerializeField]
    protected GameObject pausePannel;
    [SerializeField]
    protected DepthMeshCollider depthMeshCollider;
    [SerializeField]
    protected Text actNum;
    [SerializeField]
    protected Image img;
    [SerializeField]
    protected Text timeText;
    private void Awake()
    {
        DataManager.Instance.ResetmyTime();
    }
    private void Start()
    {
        StartCoroutine(UpdateMeshCollider());
        if(PhotonManager.Instance.isMaster)
        {
            DataManager.Instance.ChangeControlable(true);
        }
        else
        {
            DataManager.Instance.ChangeControlable(false);
        }
        PhotonManager.Instance.aliveCnt = PhotonManager.Instance.localPlayer; //시작시에 살아 있는사람과 방에 입장 유저수는 같음
        if(PhotonManager.Instance.aliveCnt==1)
        {
            Debug.Log("싱글플레이");
            PhotonManager.Instance.isSingle = true;
            isGameOver = true;
        }
        isGameOver = false;
        Debug.Log("isGameOver = " + isGameOver);
        DistanceFromCenter = 0.5f;
        //ARCamera = GameObject.Find("First Person Camera").GetComponent<Camera>();
        //PhotonManager.Instance.aliveCnt = DataManager.Instance.ComputeAliveUser(); //현재 방에유저의 수 = 시작시 살아있는 사람수
        SoundManager.Instance.SetBgmClip("play");
        SoundManager.Instance.SetEffectClip("scenestart");
    }

    private void Update()
    {
        timeText.text = DataManager.Instance.ReturnMyTime().ToString();
        actNum.text = PhotonManager.Instance.myOrder.ToString();
        time += Time.deltaTime;
        if(DataManager.Instance.CheckControlable()==true && DataManager.Instance.ReturnIsGameStart()==true)
        {
            //시간을 체크한다
            DataManager.Instance.SubtractmyTime();
            img.color = Color.red;
        }
        else
        {
            img.color = Color.white;
        }
        //nowHp.text = "현재 체력은 = " + PhotonManager.Instance.myHp.ToString();
        //aliveText.text = "살아있는 인원수 = " + PhotonManager.Instance.aliveCnt.ToString();
        //if(PhotonManager.Instance.isMaster == true)
        //{
            //idreceiveCnt.text = "앵커 아이디 전달받은 플레이어수: "+PhotonManager.Instance.receiveCnt.ToString();
            //resolveCnt.text = "앵커 리졸브한 플레이어수: "+PhotonManager.Instance.readyCnt.ToString();
            //nowPlayer.text = "현재 방에 입장한 플레이어수: " + PhotonManager.Instance.localPlayer.ToString();
            //nowPower.text = "현재 게이지 : " + PhotonManager.Instance.lange.ToString();
            //checkMasterText.text = "내 액터 넘버"+PhotonManager.Instance.myOrder.ToString()+"현재턴은"+PhotonManager.Instance.nowTurn.ToString();
        //}
        //else
        //{
            //checkMasterText.text = "내 엑터 넘버" + PhotonManager.Instance.myOrder.ToString() + " 현재턴은" + PhotonManager.Instance.nowTurn.ToString();
        //}//지울 예정

        
        if ((PhotonManager.Instance.aliveCnt <= 1) && (isGameOver == false))
        {
            Debug.Log("승자 결정");
            isGameOver = true;
            PhotonManager.Instance.Call_ChangeAliveCntOne();
            CheckWinner();
        }

        if (Input.touchCount==0)
        {
            return;
        }

        if(time>4)
        {
        MakePosition();
        }


    }
    public void MakePosition()
    {
        StartCoroutine(MakeCloudAnchorMaster());
    }
    /// <summary>
    /// 방장이 먼저 앵커를 만든다.
    /// 앵커를 호스팅한다.
    /// 다음 사람은 리솔브한다?
    /// </summary>

    public IEnumerator MakeCloudAnchorMaster()
    {
        Touch touch = Input.GetTouch(0);
        TrackableHitFlags racastFilter = TrackableHitFlags.PlaneWithinPolygon; //평면만 검출
        if(PhotonManager.Instance.firstAnchor == false && PhotonManager.Instance.isMaster == true)//처음 앵커는 방장만 찍게 한다.
        {
            if (touch.phase == TouchPhase.Began && Frame.Raycast(touch.position.x, touch.position.y, racastFilter, out hit))
            {

                PhotonManager.Instance.Call_MakeFirstAnchor(); //다른사용자에게 도 firstAnchor의 값을 true로 바꿔줌
                //firstAnchor = true;//값을 true 로 해서 이함수가 2번 실행되지 않게 한다? 터치2번못하게 막는다
                firstpos = hit.Pose;
                pos = hit.Pose;//터치한 지점을 담는다.
                obj = Instantiate(firstTouch, pos.position, Quaternion.identity);
                //anchor = hit.Trackable.CreateAnchor(hit.Pose);//터치한 지점에 앵커를 만든다.
                //localAnchor.Add(hit.Trackable.CreateAnchor(pos));
                //Debug.Log("중심점 설정");
                //유저수 +1 만큼 앵커를 만들고 싶음 유저수 +1만큼 반복문을 돈다
                nowPlayerCnt = PhotonManager.Instance.localPlayer;
                Debug.Log("현재 방안의 플레이어수 : " + nowPlayerCnt);
                for(int i=0; i<nowPlayerCnt+1;i++)
                {
                    ismakeAnchor = false;
                    PhotonManager.Instance.receiveCnt = 0;//몇명이 클라우드 앵커 아이디를 받았는가 0 으로 초기화 하고 시작
                    PhotonManager.Instance.readyCnt = 0; //몇명이 클라우드 앵커를 생성했는가 0으로 초기화 하고 시작
                    if (i == 0) //센터 위치
                    {
                        anchor = hit.Trackable.CreateAnchor(hit.Pose);//터치한 지점에 앵커를 만든다. 
                        Debug.Log("중심점이 될 앵커 생성");
                    }
                    else if(i ==1) //1번 사용자의 위치
                    {
                        pos.position = firstpos.position+new Vector3(DistanceFromCenter, 0, 0);
                        anchor = hit.Trackable.CreateAnchor(pos); //1번 플레이어의 앵커를만든다.
                        Destroy(obj);
                        obj = Instantiate(firstTouch, pos.position, Quaternion.identity);
                        Debug.Log("1번 플레이어의 위치 앵커 생성");
                    }
                    else if(i==2)//2번 사용자의 위치
                    {
                        pos.position = firstpos.position + new Vector3(-DistanceFromCenter, 0, 0);
                        anchor = hit.Trackable.CreateAnchor(pos); //2번 플레이어의 앵커를만든다.
                        Destroy(obj);
                        obj = Instantiate(firstTouch, pos.position, Quaternion.identity);
                        Debug.Log("2번 플레이어의 위치 앵커 생성");
                    }
                    else if(i==3)//3번 사용자의 위치
                    {
                        pos.position = firstpos.position + new Vector3(0, 0, DistanceFromCenter);
                        anchor = hit.Trackable.CreateAnchor(pos); //3번 플레이어의 앵커를만든다.
                        Destroy(obj);
                        obj = Instantiate(firstTouch, pos.position, Quaternion.identity);
                        Debug.Log("3번 플레이어의 위치 앵커 생성");
                    }
                    else //4번사용자의 위치
                    {
                        pos.position = firstpos.position + new Vector3(0, 0, -DistanceFromCenter);
                        anchor = hit.Trackable.CreateAnchor(pos); //4번 플레이어의 앵커를만든다.
                        Destroy(obj);
                        obj = Instantiate(firstTouch, pos.position, Quaternion.identity);
                        Debug.Log("4번 플레이어의 위치 앵커 생성");
                    }
                    //생성한 앵커를 클라우드 앵커로 만들고 다른 플레이어가 앵커를 리졸브 할때까지 대기..
                    StartCoroutine(HostCloudAnchor(anchor));//클라우드 만들고 앵커 아이디를 보냄
                    yield return new WaitUntil(() => ismakeAnchor == true);//방장이 클라우드 앵커 생성할떄까지 대기
                    if (nowPlayerCnt > 1)
                    {
                        yield return new WaitUntil(() => PhotonManager.Instance.receiveCnt == PhotonManager.Instance.localPlayer - 1);//클라우드 앵커아이디가 모든 사용자에게 전달될때까지 대기
                                                                                                                //방장이 아닌 다름 사용자들에게 리졸브하라는 명령을 내림
                        PhotonManager.Instance.Call_ResolveRpc(i);
                        yield return new WaitUntil(() => PhotonManager.Instance.readyCnt == PhotonManager.Instance.localPlayer - 1); //방에 방장을 제외한 다른사용자가 리졸브 할때까지 대기
                    }
                    Debug.Log(i + "번째 반복문 끝남");
                }
                //반복문을 빠져나오면 모든 앵커가 잡히면 각자의 위치에 플레이어를 생성하라는 명령을 알피씨로 내리게함
                Destroy(obj);
                PhotonManager.Instance.Call_InstantePlayer();
                //Debug.Log("방장이 생성한 앵커의 위치 = " + anchor.transform.position);
                //GameObject obj = PhotonNetwork.Instantiate("TestBot", hit.Pose.position, Quaternion.identity);
                //obj = Instantiate(centerObject, anchor.transform.position, Quaternion.identity);
                //obj.transform.SetParent(anchor.transform);
                //Debug.Log("앵커에 자식이된 표시 오브젝트의 위치 = " + obj.transform.position);

            }
        }


    }
   

    /// <summary>
    /// 방장이 생성된 앵커리스트를 보내서 호스팅
    /// 앵커 아이디를 생성해서 다른 유저에게 보냄
    /// </summary>
    /// <param name="anchorlist"></param>
    /// <returns></returns>
    IEnumerator HostCloudAnchor(Anchor anchor)
    {
        //anchor_ = AsyncTask < CloudAnchorResult > CreateCloudAnchor(anchor);
        //task = XPSession.CreateCloudAnchor(anchor);
        //Debug.Log(anchor);
        //Debug.Log(task);
        Debug.Log("응답 머기중.... ");
        task = XPSession.CreateCloudAnchor(anchor);
        yield return new WaitUntil(() => task.IsComplete == true);
        ismakeAnchor = true;
        //PhotonManager.Instance.CheckResolve();//readyCnt 1증가
        //PhotonManager.Instance.anchorIdList.Add(task.Result.Anchor.CloudId);//완료된 클라우드 앵커의 아이디를 리스트에 담는다.
        PhotonManager.Instance.hostingResultList.Add(task);//task를 담는다.
        Debug.Log(task.Result.Response);
        Debug.Log(task.Result.Anchor.CloudId);
        anchorid = task.Result.Anchor.CloudId;
        Debug.Log("클라우드 앵커 생성 상태:" + ismakeAnchor);
        //string id = task.Result.Anchor.CloudId;
        //Debug.Log(id);
        if (nowPlayerCnt > 1 && ismakeAnchor == true) //방장말고 다른 유저가 존재할때
        {
            Debug.Log("다른 사용자에게 앵커 아이디리스트 보내는중...");
            PhotonManager.Instance.SendAnchorId(anchorid);//rpc 가 실행되고 앵커아이디를 전달하고 나도 카운트가 1올라간다
        }
        //플레이어를 해당위치에 생성
        //hostingResultList 매개변수로
        //InstantePlayer(PhotonManager.Instance.hostingResultList);
    }


    /// <summary>
    /// 1명만 살아 있으면 게임 종료하고 종료 관련 문구를띄워준다
    /// </summary>
   public void CheckWinner()
    {
        SoundManager.Instance.SetBgmClip("result");
        if(DataManager.Instance.CheckWinner(PhotonManager.Instance.myOrder))
        {
            //승자 일때
            victoryPanenek.SetActive(true);
        }
        else
        {
            //패자일때
            gameOverPannel.SetActive(true);
        }
    }
    public void BackToRoomLobby()
    {
        SoundManager.Instance.SetEffectClip("movescene");
        SceneManager.LoadScene("05.Room");
    }
    /// <summary>
    /// pause 창을 연다
    /// </summary>
    public void OpenPause()
    {
        SoundManager.Instance.SetEffectClip("click");
        Debug.Log("Pause창을 연다");
        pausePannel.SetActive(true);
    }
    public IEnumerator UpdateMeshCollider()
    {
        while(true)
        {
            //Debug.Log("주변 매쉬 업데이트중");
            yield return null;
            depthMeshCollider.ShootProjectile();
            yield return new WaitForSeconds(1.0f);
        }
    }
}