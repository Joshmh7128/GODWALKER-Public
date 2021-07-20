using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmallAmmoChunks : MonoBehaviour
{
    [SerializeField] PlayerController playerController; // our player controller
    [SerializeField] GameObject cubePuff; // death particle effect

    private void Start()
    {
        // make this in to it's own object
        transform.parent = GameObject.Find("Ammo Small Chunks").GetComponent<Transform>();

        if (playerController == null)
        {
            playerController = GameObject.Find("Player").GetComponent<PlayerController>();
        }
    }

    private void OnTriggerEnter(Collider col)
    {
        // Debug.Log("collision");
        // when the player collides with us add ammunition to their inventory
        if (col.CompareTag("Player"))
        {
            // Debug.Log("player collision");
            if (playerController.ammoAmount < playerController.ammoMax)
            {
                playerController.ammoAmount++;
                playerController.cameraScript.shakeDuration += 0.06f;
                Instantiate(cubePuff, transform.position + new Vector3(0, 0.5f, 0), Quaternion.Euler(new Vector3(0,0,0)), null);
                Destroy(gameObject);
            }
        }
    }
}
