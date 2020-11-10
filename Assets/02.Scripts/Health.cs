using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Health : MonoBehaviour
{
    [SerializeField]
    protected Slider nowHP;
    // Start is called before the first frame update
    void Start()
    {
        //nowHP.onValueChanged.AddListener((value) => { ChangedHp(); });//hp값이 변경 될 때마다 함수 호출하게 만듬.
        nowHP.maxValue = DataManager.Instance.bodyhp;//hp바 최대값 설정
        nowHP.value = DataManager.Instance.bodyhp;//내 hp바의 값을 시작값으로 변경
    }

    // Update is called once per frame
    void Update()
    {
        ChangedHp();
    }
    public void ChangedHp()
    {
        nowHP.value = PhotonManager.Instance.myHp;//변경된 hp값을 바꿔줌.
        if (PhotonManager.Instance.myHp <= 0)//hp값이 0보다 작을경우 0으로 고정함.
            nowHP.value = 0;
    }
}
