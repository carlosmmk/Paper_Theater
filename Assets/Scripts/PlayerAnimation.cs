using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    private Animator anim;
    private PlayerController playerController;
    private PushAndPull pushAndPull;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        playerController = GetComponentInParent<PlayerController>();
        pushAndPull = GetComponentInParent<PushAndPull>();
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        anim.SetBool("walking", Input.GetAxisRaw("Horizontal") != 0 && Mathf.Abs(playerController.rb.velocity.x) > 0.1f);
        anim.SetBool("grounded", playerController.IsGrounded());
        anim.SetBool("pushing", pushAndPull.pushing);
        
        anim.SetFloat("pushSpeed", (playerController.rb.velocity.x * playerController.directionFacing) / playerController.maxSpeed);
    }

    public void SetTrigger(string trigger)
    {
        ResetAllTriggers();
        anim.SetTrigger(trigger);
    }

    private void ResetAllTriggers()
    {
        foreach (var param in anim.parameters)
        {
            if (param.type == AnimatorControllerParameterType.Trigger)
            {
                anim.ResetTrigger(param.name);
            }
        }
    }

    public bool IsPlayerLanding()
    {
        // return anim.GetCurrentAnimatorStateInfo(0).IsName("Jump") || anim.GetCurrentAnimatorStateInfo(0).IsName("Landing");
        return anim.GetCurrentAnimatorStateInfo(0).IsName("Landing");
    }

    public bool IsPlayerInAir()
    {
        return anim.GetCurrentAnimatorStateInfo(0).IsName("Jump") || anim.GetCurrentAnimatorStateInfo(0).IsName("InAir");
    }

    public void PlayAnimation(string animName)
    {
        anim.CrossFade(animName, 0.1f);
    }

    public void PlayFootstepSound()
    {
        AudioManager.instance.PlayRandomBetweenSounds(new []{"Footstep01", "Footstep02", "Footstep03", "Footstep04", "Footstep05", "Footstep06"
        , "Footstep07", "Footstep08", "Footstep09", "Footstep10"});
    }

    public void PlayLandingSound()
    {
        AudioManager.instance.PlayRandomBetweenSounds(new[] {"Landing01", "Landing02", "Landing03"});
    }
}
