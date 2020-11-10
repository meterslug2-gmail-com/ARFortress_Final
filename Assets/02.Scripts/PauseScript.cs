using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseScript : MonoBehaviour
{
    
    [SerializeField]
    protected GameObject setting;

    protected SoundManager soundManager;
    // Start is called before the first frame update
    void Start()
    {
        soundManager = SoundManager.GetInstance();
    }

    // Update is called once per frame
    void Update()
    {
    }
    /// <summary>
    /// pause 창을 닫는다
    /// </summary>
    public void ClosePause()
    {
        soundManager.SetEffectClip("click");
        this.gameObject.SetActive(false);
    }
    /// <summary>
    /// 옵션(사운드) 창을 연다
    /// </summary>
    public void OpenSetting()
    {
        soundManager.SetEffectClip("click");
        setting.SetActive(true);
    }
    /// <summary>
    /// 옵션(사운드) 창을 닫는다
    /// </summary>
    public void CloseSetting()
    {
        soundManager.SetEffectClip("click");
        setting.SetActive(false);
    }
    /// <summary>
    /// 게임 나가기 룸 에서 나감
    /// </summary>
    public void ExitGames()
    {
        soundManager.SetEffectClip("movestart");
        PhotonManager.Instance.LeaveRoom();
        //SceneManager.LoadScene("03.Lobby");
    }
}
