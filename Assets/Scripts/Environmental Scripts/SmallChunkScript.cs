using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmallChunkScript : MonoBehaviour
{
    [SerializeField] PlayerController playerController; // our player controller
    [SerializeField] GameObject cubePuff; // death particle effect
    [SerializeField] Rigidbody rigidbody;
    [SerializeField] SphereCollider sphereCollider;
    [SerializeField] enum chunkTypes // the types of chunks
    {
        ammo, mineral, gem, health
    }
    [SerializeField] chunkTypes chunkType;

    private void Start()
    {
        // make this in to it's own object
        transform.parent = GameObject.Find("Small Chunks").GetComponent<Transform>();

        if (rigidbody == null)
        { rigidbody = gameObject.GetComponent<Rigidbody>(); }

        if (playerController == null)
        { playerController = GameObject.Find("Player").GetComponent<PlayerController>(); }
    }

    private void OnCollisionEnter(Collision col)
    {
        // freeze our small chunk if we touch the environment
        if (col.gameObject.tag == "Environment")
        {
            rigidbody.constraints = RigidbodyConstraints.FreezeAll;
            // sphereCollider.enabled = false;
        }
    }

    private void OnTriggerEnter(Collider col)
    {
        // when the player collides with us add ammunition to their inventory
        if (col.CompareTag("Player"))
        {
            switch (chunkType)
            {
                case chunkTypes.ammo:
                // Debug.Log("player collision");
                if (playerController.ammoAmount < playerController.ammoMax)
                {
                    playerController.ammoAmount++;
                    playerController.cameraScript.shakeDuration += 0.06f;
                    Instantiate(cubePuff, transform.position + new Vector3(0, 0.5f, 0), Quaternion.Euler(new Vector3(0, 0, 0)), null);
                    Destroy(gameObject);
                }
                    break;

                case chunkTypes.mineral:
                // Debug.Log("player collision");
                if (playerController.mineralAmount < playerController.mineralMax)
                {
                    playerController.mineralAmount++;
                    playerController.cameraScript.shakeDuration += 0.06f;
                    Instantiate(cubePuff, transform.position + new Vector3(0, 0.5f, 0), Quaternion.Euler(new Vector3(0, 0, 0)), null);
                    Destroy(gameObject);
                }
                    break;


                case chunkTypes.gem:
                    // Debug.Log("player collision");
                    if (playerController.gemAmount < playerController.gemMax)
                    {
                        playerController.gemAmount++;
                        playerController.cameraScript.shakeDuration += 0.06f;
                        Instantiate(cubePuff, transform.position + new Vector3(0, 0.5f, 0), Quaternion.Euler(new Vector3(0, 0, 0)), null);
                        Destroy(gameObject);
                    }
                    break;
            }
        }
    }
}
