using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomClass : MonoBehaviour
{
    // this class will hold all the doors of our room, and give us the ability to access them
    public List<DoorClass> doorClasses; // our doors
    [SerializeField] bool isStartRoom; // are we the starting room?
    public List<EnemyClass> enemyClasses; // the list of enemies in this room

    private void Start()
    {
        if (isStartRoom)
        { GameObject.Find("Drop Pod").GetComponent<DroppodManager>().targetPosGroundNew = transform.position; }
    }
}
