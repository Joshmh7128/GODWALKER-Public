using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorClass : MonoBehaviour
{
    public RoomGenerationManager roomGenerationManager; // our room generation manager
    [SerializeField] Transform roomPlaceTransform; // were will the open door of the next room be?

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
            Instantiate(roomGenerationManager.roomPrefabs[Random.Range(0, roomGenerationManager.roomPrefabs.Count)], roomPlaceTransform);
            // lower the room count
            roomGenerationManager.roomCount--;
        }
    }
}
