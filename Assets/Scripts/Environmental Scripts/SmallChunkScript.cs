using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmallChunkScript : MonoBehaviour
{
    [SerializeField] PlayerController playerController; // our player controller
    [SerializeField] GameObject cubePuff; // death particle effect
    [SerializeField] Rigidbody rigidbody;
    [SerializeField] SphereCollider sphereCollider;
    [SerializeField] bool isBugPart;
    [SerializeField] float decreaseRate;
    [SerializeField] enum chunkTypes // the types of chunks
    {
        ammo, mineral, gem, health, bug
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

    private void FixedUpdate()
    {
        // if this is a bug part, slowly make the bug part smaller, until it disappears
        if (isBugPart)
        {
            transform.localScale -= new Vector3(decreaseRate,decreaseRate,decreaseRate);

            if (transform.localScale.x <= 0)
            { Destroy(gameObject); }
        }
    }

    private void OnCollisionStay(Collision col)
    {
        if (rigidbody.velocity == Vector3.zero)
        {
            rigidbody.constraints = RigidbodyConstraints.FreezeAll;
            gameObject.GetComponent<Collider>().enabled = false;
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
                    UpgradeSingleton.OnSmallChunkPickup(chunkType.ToString());
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
                    UpgradeSingleton.OnSmallChunkPickup(chunkType.ToString());
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
                        UpgradeSingleton.OnSmallChunkPickup(chunkType.ToString());
                        Destroy(gameObject);
                    }
                    break;

                case chunkTypes.bug:
                    playerController.bugPartAmount++;
                    playerController.cameraScript.shakeDuration += 0.06f;
                    Instantiate(cubePuff, transform.position + new Vector3(0, 0.5f, 0), Quaternion.Euler(new Vector3(0, 0, 0)), null);
                    UpgradeSingleton.OnSmallChunkPickup(chunkType.ToString());
                    Destroy(gameObject);
                    break;
            }
        }
    }
}
