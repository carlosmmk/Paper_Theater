using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowerSpotLight : MonoBehaviour
{
    public Transform target;

    public float rotSpeed;

    public float maxAngle;

    private Light light;

    private void Awake()
    {
        light = GetComponent<Light>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Quaternion targetRot = Quaternion.LookRotation(target.position-transform.position, Vector3.up);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, Time.deltaTime*rotSpeed);
        
        light.enabled = Vector3.Angle(transform.position, target.position) > maxAngle ? false : true;
    }
}
