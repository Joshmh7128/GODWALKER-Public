using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorClass : MonoBehaviour
{
    public RoomGenerationManager roomGenerationManager; // our room generation manager
    [SerializeField] bool isSpecialRoom; // is this the door to a special room
    [SerializeField] Transform roomPlaceTransform; // were will the open door of the next room be?
    [SerializeField] GameObject openableDoor; // part of the door we can actually open
    [SerializeField] GameObject finalRoomWarning; // the warning that the final room is up ahead
    [SerializeField] bool enemiesClear; // have we killed all of the enemies?
    GameObject ourRoom;
    [SerializeField] int doorID; // the id number of our odor

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
        if (isSpecialRoom)
        {
            ourRoom = Instantiate(roomGenerationManager.specialRoomPrefabs[Random.Range(0, roomGenerationManager.specialRoomPrefabs.Count)], roomPlaceTransform);
            ourRoom.SetActive(false);
        }

        if (!isSpecialRoom)
        {
            if (roomGenerationManager)
            {
                // add ourselves to the roomclass list
                roomGenerationManager.doorClassList.Add(this);
            }

            if (!roomGenerationManager)
            {
                Debug.LogError("Room Generation Manager Not Found");
            }

            // set our doorID
            doorID = roomGenerationManager.doorClassList.FindIndex((DoorClass dc) => { return dc == this; });

            // if we are in the first 3 rooms spawn an easy room
            if (roomGenerationManager.roomCount >= 3)
            {
                // randomly choose a room from our prefabs to spawn
                if (ourRoom == null)
                {
                    if (!isSpecialRoom)
                    {
                        ourRoom = Instantiate(roomGenerationManager.roomPrefabsEasy[Random.Range(0, roomGenerationManager.roomPrefabsEasy.Count)], roomPlaceTransform);
                        // ourRoom.SetActive(false);
                        // lower the room count
                        roomGenerationManager.roomCount--;
                    }
                }
            }

            // if we are in the last 3 rooms spawn a hard room
            if (roomGenerationManager.roomCount <= 3 && roomGenerationManager.roomCount != 0)
            {
                // randomly choose a room from our prefabs to spawn
                if (ourRoom == null)
                {
                    if (!isSpecialRoom)
                    {
                        ourRoom = Instantiate(roomGenerationManager.roomPrefabsHard[Random.Range(0, roomGenerationManager.roomPrefabsHard.Count)], roomPlaceTransform);
                        // ourRoom.SetActive(false);
                        // lower the room count
                        roomGenerationManager.roomCount--;
                    }
                }
            }

            // place the final room
            if (roomGenerationManager.roomCount <= 0)
            {
                // choose the final room to spawn
                if (ourRoom == null)
                {
                    ourRoom = Instantiate(roomGenerationManager.hordeRoom, roomPlaceTransform);
                    roomGenerationManager.roomCount--;
                }

                if (roomGenerationManager.roomCount < 0)
                {
                    finalRoomWarning.SetActive(true);
                }
            }
        }

        roomGenerationManager.DeactivateAllRooms();
    }

    // open the door
    public void OpenDoor()
    {
        // activate all objects
        ourRoom.SetActive(true);
        // activate the next door
        roomGenerationManager.doorClassList[doorID + 1].gameObject.SetActive(true);
        // open our door
        openableDoor.SetActive(false);
    }

}
