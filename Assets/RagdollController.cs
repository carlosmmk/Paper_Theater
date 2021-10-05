using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.U2D.IK;

public class RagdollController : MonoBehaviour
{
    public GameObject playerSprite;
    public GameObject ragdollPrefab;
    private FollowerSpotLight[] followerSpotLights;
    private PlayerController playerController;
    private Animator anim;
    private IKManager2D ikManager;
    private Rigidbody[] rbs;

    private void Awake()
    {
        followerSpotLights = FindObjectsOfType<FollowerSpotLight>();
        playerController = GetComponent<PlayerController>();
    }

    // Start is called before the first frame update
    void Start()
    {
        // Physics.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("Ground"));
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ActivateRagdoll()
    {
        if (playerController.dead) return;
        
        var ragdollSprite = Instantiate(ragdollPrefab, transform);

        ragdollSprite.transform.localScale = new Vector3(playerController.directionFacing > 0?
            ragdollSprite.transform.localScale.x : -ragdollSprite.transform.localScale.x,
            ragdollSprite.transform.localScale.y, ragdollSprite.transform.localScale.z);

        rbs = ragdollSprite.GetComponentsInChildren<Rigidbody>();
        playerSprite.SetActive(false);
        
        foreach (var spotlight in followerSpotLights)
            spotlight.SetTarget(rbs.First(rb => rb.name.Equals("bone_1")).transform);
        
        foreach (var rb in rbs)
        {
           rb.AddForce(Vector3.right * 25 * playerController.directionFacing, ForceMode.Impulse);
        
            if (rb.name.Equals("bone_1"))
            {
                rb.AddForce(Vector3.right * 25 * playerController.directionFacing, ForceMode.Impulse);
            }
        }
    }
}
