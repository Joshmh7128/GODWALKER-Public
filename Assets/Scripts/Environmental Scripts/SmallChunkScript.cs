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
        // lerp towards the player
        interpolationRatio = elapsedFrames / interpolationCount;
        transform.position = Vector3.Lerp(transform.position, playerController.gameObject.transform.position, interpolationRatio);
        if (elapsedFrames < interpolationCount)
        { elapsedFrames++; }
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
                    if (playerController.ammoAmount < playerController.ammoMax)
                    {
                        playerController.ammoAmount++;
                    }
                    playerController.cameraScript.shakeDuration += 0.06f;
                    Instantiate(cubePuff, transform.position + new Vector3(0, 0.5f, 0), Quaternion.Euler(new Vector3(0, 0, 0)), null);
                    UpgradeSingleton.OnSmallChunkPickup(chunkType.ToString());
                    Destroy(gameObject);

                    break;

                case chunkTypes.mineral:
                    // Debug.Log("player collision");
                    if (playerController.mineralAmount < playerController.mineralMax)
                    {
                        // play a sound
                        ourSource.clip = chunkClip;
                        ourSource.Play();
                        // do the rest
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
                        // play a sound
                        ourSource.clip = chunkClip;
                        ourSource.Play();
                        // do the rest
                        playerController.gemAmount++;
                        playerController.cameraScript.shakeDuration += 0.06f;
                        Instantiate(cubePuff, transform.position + new Vector3(0, 0.5f, 0), Quaternion.Euler(new Vector3(0, 0, 0)), null);
                        UpgradeSingleton.OnSmallChunkPickup(chunkType.ToString());
                        Destroy(gameObject);
                    }
                    break;

                case chunkTypes.bug:
                    // play a sound
                    ourSource.clip = scrapClip;
                    ourSource.Play();
                    // do the rest
                    playerController.bugPartAmount++;
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
