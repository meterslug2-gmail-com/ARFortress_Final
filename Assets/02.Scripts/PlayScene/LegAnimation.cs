
using UnityEngine;

public class LegAnimation : MonoBehaviour
{
    [SerializeField]
    protected Animator anim;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        anim.SetBool("isMove" , DataManager.Instance.ReturnIsMove());
    }
}
