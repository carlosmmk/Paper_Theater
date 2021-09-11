using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Billboard : MonoBehaviour
{
    private Camera target;
    
    void Start()
    {
        target = Camera.main;
    }
    
    void LateUpdate()
    {
        transform.LookAt(target.transform);
        
        //if you want to rotate just in the Y angle
        transform.rotation = Quaternion.Euler(0f, transform.rotation.eulerAngles.y, 0f);
    }
}
