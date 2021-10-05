using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReactionTrigger : MonoBehaviour
{
    public string soundName;
    public bool destroyOnContact;
    
    private void OnTriggerEnter(Collider other)
    {
        void Start()
        {
            GetComponent<MeshRenderer>().enabled = false;
        }

        if (other.CompareTag("Player"))
        {
            AudioManager.instance.Play(soundName);
            if (destroyOnContact)
            {
                Destroy(gameObject);    
            }
        }
    }

}
