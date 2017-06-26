using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimatorControl : MonoBehaviour {

    private const string bIsJump = "bIsJump";
   
    public Animator SelfAnimator;

    void Start()
    {
       
       
    }

    void Update()
    {
        
    }


    public void PlayJumpAnimation(bool enable)
    {
       
        SelfAnimator.SetBool(bIsJump, enable);
       
    }

    public void PlayDeathAnimation()
    {
       
    }
}
