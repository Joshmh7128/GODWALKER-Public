using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomClass : MonoBehaviour
{
    // this class will hold all the doors of our room, and give us the ability to access them
    public List<DoorClass> doorClasses; // our doors
    [SerializeField] bool isStartRoom; // are we the starting room?

    private void Start()
    {
        
    }
}
