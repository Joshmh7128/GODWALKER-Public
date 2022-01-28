using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomGenerationManager : GenerationManager
{
    //we will be using a system of rooms and doors to create our map
    public int roomCount; // how many rooms do we want in the map?
    public List<DoorClassOld> doorClassList; // all the gameObjects of our doors to be accessed
    public List<GameObject> roomPrefabsEasy; // all the easy room prefabs we want to work with in this generation
    public List<GameObject> roomPrefabsHard; // all the hard room prefabs we want to work with in this generation
    public List<GameObject> specialRoomPrefabs; // all the room prefabs we want to work with in this generation
    public GameObject hordeRoom; // the final room in the generation set
    public GameObject minibossRoom; // the final room in the generation set
    public GameObject bossRoom; // the final room in the generation set
    bool primeDeactivation; // have our rooms been deactivated at the start of the run?

    public void DeactivateAllRooms()
    { 
        // deactivate all the doorclass gameobjects
        if (roomCount <= 0 && !primeDeactivation)
        {
            foreach (DoorClassOld doorClass in doorClassList)
            {
                doorClass.gameObject.SetActive(false);
            }
        }
        // then activate the first one
        doorClassList[0].gameObject.SetActive(true);
        // set prime deactivation to true
        primeDeactivation = true;
    }

    public override void MapGeneration()
    {

    }

    public override void ClearGen()
    {

    }

}
