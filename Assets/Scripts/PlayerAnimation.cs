using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    private Animator anim;
    private PlayerController playerController;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        playerController = GetComponentInParent<PlayerController>();
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        anim.SetBool("walking", Input.GetAxisRaw("Horizontal") != 0);
        anim.SetBool("grounded", playerController.IsGrounded());
        anim.SetBool("pushing", playerController.pushing);
        
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
        return anim.GetCurrentAnimatorStateInfo(0).IsName("Jump") || anim.GetCurrentAnimatorStateInfo(0).IsName("Landing");
    }
}
