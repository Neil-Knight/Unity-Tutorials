using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WhichFoot : MonoBehaviour
{
    public GameObject LeftFoot;
    public GameObject RightFoot;
    public Animator animator;

    public void RightFootUp()
    {
        animator.SetBool("isRU", true);
        animator.SetBool("isLU", false);
    }

    public void LeftFootUp()
    {
        animator.SetBool("isRU", false);
        animator.SetBool("isLU", true);
    }
}
