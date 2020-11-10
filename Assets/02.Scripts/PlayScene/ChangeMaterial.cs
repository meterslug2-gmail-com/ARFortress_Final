using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ChangeMaterial : MonoBehaviour
{
    [SerializeField]
    protected Material occlusionMat;
    [SerializeField]
    protected Material basicMat;
    protected MeshRenderer[] mrs;
    protected SkinnedMeshRenderer[] smrs;
    void Start()
    {
        mrs = transform.GetComponentsInChildren<MeshRenderer>();
        smrs = transform.GetComponentsInChildren<SkinnedMeshRenderer>();
        //랜더러를 가져와서 arcore씬에서는 오클루전 쉐이더 적용 메티리얼로 
        //나머진 씬에선 기본 쉐이더 적용된 메테리얼로 바꿔준다 
        if (SceneManager.GetActiveScene().buildIndex==6)
        {
            //ARcore 씬이면?
            //메테리얼을 occulsion Shader로 
            for (int i = 0; i < mrs.Length; i++)
            {
                mrs[i].material = occlusionMat;
            }
            if(gameObject.tag == "Spider")//스파이더 부품만 매쉬랜더러가 아니라 스캔 매쉬랜더로로 되어 있어서
            {
                for (int i = 0; i < smrs.Length; i++)
                {
                    smrs[i].material = occlusionMat;
                }
            }
        }
        else
        {
            //기본 쉐이더로
            for (int i = 0; i < mrs.Length; i++)
            {
                mrs[i].material = basicMat;
            }

            if (gameObject.tag == "Spider")//스파이더 부품만 매쉬랜더러가 아니라 스캔 매쉬랜더로로 되어 있어서
            {
                for (int i = 0; i < smrs.Length; i++)
                {
                    smrs[i].material = basicMat;
                }
            }
        }
    }

}
