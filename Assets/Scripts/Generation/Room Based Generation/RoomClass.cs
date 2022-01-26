using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomClass : MonoBehaviour
{
    // this class will hold all the doors of our room, and give us the ability to access them
    public List<DoorClass> doorClasses; // our doors
    [SerializeField] bool isStartRoom; // are we the starting room?
    public List<EnemyClassOld> enemyClasses; // the list of enemies in this room
    Transform playerTransform; // our player's transform
    int counter;
    [SerializeField] List<Renderer> renderers;
    public bool isActiveRoom; // is this the active room?
         
    private void Start()
    {
        // get our player
        playerTransform = GameObject.Find("Player").transform;

        if (isStartRoom)
        {
            // make sure the dropship exists before we try to set it's new position
            if (GameObject.Find("Drop Pod"))
            {   // then set it
                GameObject.Find("Drop Pod").GetComponent<DroppodManager>().targetPosGroundNew = transform.position;
            }
        }
    }

    private void FixedUpdate()
    {
        // count up
        counter++;

        // check distance every other frame
        if (counter >= 2f)
        {
            if (Vector3.Distance(transform.position, playerTransform.position) > 1400f)
            {
                gameObject.SetActive(false);
            }

            if (Vector3.Distance(transform.position, playerTransform.position) < 1400f)
            {
                gameObject.SetActive(true);
            }

            counter = 0;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // if the player enters
        if (other.gameObject.tag == "Player")
        {
            isActiveRoom = true;
        }
    }    
    
    private void OnTriggerExit(Collider other)
    {
        // if the player enters
        if (other.gameObject.tag == "Player")
        {
            isActiveRoom = false;
        }
    }
}
