using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmallChunkScript : MonoBehaviour
{
    [SerializeField] PlayerController playerController; // our player controller
    [SerializeField] GameObject cubePuff; // death particle effect
    [SerializeField] Rigidbody rigidbody;
    [SerializeField] AudioSource ourSource;
    [SerializeField] AudioClip chunkClip;
    [SerializeField] AudioClip scrapClip;
    [SerializeField] SphereCollider sphereCollider;
    [SerializeField] bool isBugPart;
    [SerializeField] float decreaseRate;
    [SerializeField] float speed;
    float interpolationCount = 300; // the amount of frames to lerp within
    float elapsedFrames = 0; // the amount of frames we have elapsed
    float interpolationRatio;

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
        // move faster and faster to the player
        transform.position = Vector3.MoveTowards(transform.position, playerController.gameObject.transform.position, speed*Time.deltaTime);
        speed++;
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
            // play our sound
            switch (chunkType)
            {
                case chunkTypes.ammo:
                    // Debug.Log("player collision");

                    // play a sound
                    ourSource.clip = chunkClip;
                    ourSource.Play();
                    // do the rest
                    if (playerController.powerAmount < playerController.powerMax)
                    {
                        playerController.powerAmount++;
                    }
                    playerController.cameraScript.shakeDuration += 0.06f;
                    Instantiate(cubePuff, transform.position + new Vector3(0, 0.5f, 0), Quaternion.Euler(new Vector3(0, 0, 0)), null);
                    UpgradeSingleton.OnSmallChunkPickup(chunkType.ToString());
                    Destroy(gameObject);

                    break;

                case chunkTypes.mineral:
                    // minerals were removed
                    break;


                case chunkTypes.gem:
                    // play a sound
                    ourSource.clip = chunkClip;
                    ourSource.Play();
                    // do the rest
                    playerController.gemAmount++;
                    playerController.cameraScript.shakeDuration += 0.06f;
                    Instantiate(cubePuff, transform.position + new Vector3(0, 0.5f, 0), Quaternion.Euler(new Vector3(0, 0, 0)), null);
                    UpgradeSingleton.OnSmallChunkPickup(chunkType.ToString());
                    Destroy(gameObject);
                    break;

                case chunkTypes.bug:
                    // play a sound
                    ourSource.clip = scrapClip;
                    ourSource.Play();
                    // do the rest
                    playerController.scrapAmount++;
                    playerController.cameraScript.shakeDuration += 0.06f;
                    Instantiate(cubePuff, transform.position + new Vector3(0, 0.5f, 0), Quaternion.Euler(new Vector3(0, 0, 0)), null);
                    UpgradeSingleton.OnSmallChunkPickup(chunkType.ToString());
                    Destroy(gameObject);
                    break;      

                case chunkTypes.health:
                    // play a sound
                    ourSource.clip = scrapClip;
                    ourSource.Play();
                    // do the hp additional
                    if (playerController.playerHP < playerController.playerMaxHP)
                    {
                        playerController.playerHP++;
                    }
                    playerController.cameraScript.shakeDuration += 0.06f;
                    Instantiate(cubePuff, transform.position + new Vector3(0, 0.5f, 0), Quaternion.Euler(new Vector3(0, 0, 0)), null);
                    UpgradeSingleton.OnSmallChunkPickup(chunkType.ToString());
                    Destroy(gameObject);
                    break;
            }
        }
    }
}
