using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorClass : MonoBehaviour
{
    public RoomGenerationManager roomGenerationManager; // our room generation manager
    [SerializeField] bool isSpecialRoom; // is this the door to a special room
    [SerializeField] Transform roomPlaceTransform; // were will the open door of the next room be?
    [SerializeField] GameObject openableDoor; // part of the door we can actually open
    [SerializeField] bool enemiesClear; // have we killed all of the enemies?
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
                if (!isSpecialRoom)
                {
                    ourRoom = Instantiate(roomGenerationManager.roomPrefabs[Random.Range(0, roomGenerationManager.roomPrefabs.Count)], roomPlaceTransform);
                    ourRoom.SetActive(false);
                    // lower the room count
                    roomGenerationManager.roomCount--;
                }

                if (isSpecialRoom)
                {
                    ourRoom = Instantiate(roomGenerationManager.specialRoomPrefabs[Random.Range(0, roomGenerationManager.specialRoomPrefabs.Count)], roomPlaceTransform);
                    ourRoom.SetActive(false);
                }
            }
        }
    }

    // open the door
    public void OpenDoor()
    {
        if (enemiesClear)
        // activate all objects
        ourRoom.SetActive(true);
        // open our door
        openableDoor.SetActive(false);
    }

}
