using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicTrigger : MonoBehaviour
{
    public string soundNamePlay;
    public string soundNameStop;
    public int fade;
    public bool destroyOnContact;

    void Start()
    {
            GetComponent<MeshRenderer>().enabled = false;
    }

    private void OnTriggerEnter(Collider other)
    {
       

        if (other.CompareTag("Player"))
        {
            AudioManager.instance.PlayWithFade(soundNamePlay, fade);
            AudioManager.instance.StopWithFade(soundNameStop, fade);
            if (destroyOnContact)
            {
                Destroy(gameObject);
            }
        }
    }

}
