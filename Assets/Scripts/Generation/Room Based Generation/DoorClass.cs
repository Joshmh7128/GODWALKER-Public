using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorClass : MonoBehaviour
{
    public RoomGenerationManager roomGenerationManager; // our room generation manager
    [SerializeField] Transform roomPlaceTransform; // were will the open door of the next room be?
    [SerializeField] GameObject openableDoor; // part of the door we can actually open
    GameObject ourRoom;

    private void Start()
    {
        // make sure we have our generation manager
        if (roomGenerationManager == null)
        {
            roomGenerationManager = GameObject.Find("Generation Manager").GetComponent<RoomGenerationManager>();
        }

        // place a room
        PlaceRoom();
    }

    // put a new room in the map
    public void PlaceRoom()
    {
        if (roomGenerationManager.roomCount > 0)
        {
            // randomly choose a room from our prefabs to spawn
            if (ourRoom == null)
            {
                ourRoom = Instantiate(roomGenerationManager.roomPrefabs[Random.Range(0, roomGenerationManager.roomPrefabs.Count)], roomPlaceTransform);
                ourRoom.SetActive(false);
                // lower the room count
                roomGenerationManager.roomCount--;
            }
        }
    }

    // open the door
    public void OpenDoor()
    {
        // activate all objects
        ourRoom.SetActive(true);
        // open our door
        openableDoor.SetActive(false);
    }

}
