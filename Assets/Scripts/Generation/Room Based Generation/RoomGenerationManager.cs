using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomGenerationManager : GenerationManager
{
    //we will be using a system of rooms and doors to create our map
    public int roomCount; // how many rooms do we want in the map?
    public List<RoomClass> roomClassList; // all the gameObjects of our rooms to be accessed
    public List<GameObject> roomPrefabs; // all the room prefabs we want to work with in this generation
    public List<GameObject> specialRoomPrefabs; // all the room prefabs we want to work with in this generation
    public GameObject finalRoom; // the final room in the generation set

    private void Start()
    {
        
    }

    public override void MapGeneration()
    {

    }

    public override void ClearGen()
    {

    }

}
