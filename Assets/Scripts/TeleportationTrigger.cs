using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleportationTrigger : MonoBehaviour
{
    public Transform endTopHat;
    private PlayerController player;
    public float teleportationDelay;
    public float maxImpulse;
    public float minImpulse;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        if (player == null)
            player = other.GetComponent<PlayerController>();

        if (player.rb.velocity.y >= 0) return; //if player is on the other end

        StartCoroutine(TeleportPlayer());
    }

    IEnumerator TeleportPlayer()
    {
        player.playerCollider.enabled = false;
        
        yield return new WaitForSeconds(teleportationDelay);

        float ySpeed = -player.rb.velocity.y / 1.75f;
        
        player.transform.position = endTopHat.position;
        player.rb.velocity = new Vector2(0, Mathf.Clamp(ySpeed, minImpulse, maxImpulse));

        yield return new WaitForSeconds(0.1f);

        player.playerCollider.enabled = true;
    }
}
