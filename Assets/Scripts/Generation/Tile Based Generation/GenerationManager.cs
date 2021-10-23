using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class GenerationManager : MonoBehaviour
{
    // our list of map generation chunks
    [SerializeField] RandomChildSelector[] randomChildSelectors;
    // enemy parent
    [SerializeField] Transform enemyManager;

    // map generation
    [SerializeField] int roomSpace = 50 /* how large rooms are */, targetX, targetY, targetZ;
    int tilesPlaced; // how many tiles we have placed so far
    int minPathDistance = 30;
    static int maxPathDistance = 40; // how long should our generation paths be?
    public int doorAmount; // how many doors do we currently have
    public int doorAmountMax; // how many doors do we currently have
    [SerializeField] TileClass[,,] gridArray = new TileClass[maxPathDistance * 2, maxPathDistance * 2, maxPathDistance * 2]; // our x, y, z array
    [SerializeField] List<TileClass> tileClassList; // the one dimensional list of our tiles
    [SerializeField] List<TileClass> wallTileClassList; // the one dimensional list of our tiles
    [SerializeField] GameObject tileClassObject, tileClassWallObject;
    [SerializeField] TileClass originTile;
    List<Vector3> checkVector3s = new List<Vector3>
    { 
         // new Vector3(0,0,0), dont add ourselves as a neighbor!

         // right side
         new Vector3(1,0,0),
         new Vector3(1,1,0),
         new Vector3(1,-1,0),
         new Vector3(1,0,1),
         new Vector3(1,0,-1),
         new Vector3(1,1,1),
         new Vector3(1,1,-1),
         new Vector3(1,-1,1),
         new Vector3(1,-1,-1),                
         
         // left side
         new Vector3(-1,0,0),
         new Vector3(-1,1,0),
         new Vector3(-1,-1,0),
         new Vector3(-1,0,1),
         new Vector3(-1,0,-1),
         new Vector3(-1,1,1),
         new Vector3(-1,1,-1),
         new Vector3(-1,-1,1),
         new Vector3(-1,-1,-1),        

         // directly in front and behind Z
         new Vector3(0,0,1),   
         new Vector3(0,0,-1),   

         // top middle
         new Vector3(0,1,0),

         // bottom middle
         new Vector3(0,-1,0)
    } ; // our list of all vector3 directions
    [SerializeField] bool playerPlaced = true; // does our player exist? set to true if starting from another scene other than generation
    [SerializeField] bool artifactPlaced = false; // has our asteroid had their upgrade placed? 
    bool multiGen = false; // are we on another generation ?
    [SerializeField] SmallChunkManager smallChunkManager; // our small chunk manager
    [SerializeField] ObjectPooler objectPooler; 
    private void Start()
    {
        // actually make the map first
        MapGeneration();
        // get our upgradesingleton instance
        UpgradeSingleton.Instance.playerPlaced = playerPlaced;
    }

    public void ClearGen()
    {
        // tell the manager that we are now in the multigen stage
        multiGen = true;

        // make sure we can place doors again
        doorAmount = doorAmountMax;

        // set all targets to 0
        targetX = targetY = targetZ = 0;
        Debug.Log("targets  = " + targetX + targetY + targetZ);

        // make sure we clear every tile before making new tiles
        foreach (TileClass tileClass in tileClassList)
        {
            Destroy(tileClass.gameObject);
        }
        
        // make sure we clear every tile before making new tiles
        foreach (TileClass tileClass in wallTileClassList)
        {
            Destroy(tileClass.gameObject);
        }        
        
        // make sure we clear every tile before making new tiles
        foreach (TileClass tileClass in tileClassList)
        {
            gridArray[tileClass.xArrayPos, tileClass.yArrayPos, tileClass.zArrayPos] = null;
        }
        
        // make sure we clear every tile before making new tiles
        foreach (TileClass tileClass in wallTileClassList)
        {
            gridArray[tileClass.xArrayPos, tileClass.yArrayPos, tileClass.zArrayPos] = null;
        }

        // clear out the lists manually
        for (int i = tileClassList.Count - 1; i > -1; i--)
        {
            if (tileClassList[i])
            {
                tileClassList.RemoveAt(i);
            }
        }

        for (int i = wallTileClassList.Count - 1; i > -1; i--)
        {
            if (wallTileClassList[i])
            {
                wallTileClassList.RemoveAt(i);
            }
        }

        Debug.Log("Tile Lists Cleared");

        // kill all enemies
        foreach (GameObject enemyObject in enemyManager.GetComponent<EnemyManager>().enemies)
        {
            Destroy(enemyObject);
        }

        // remove all remaining small chunks
        smallChunkManager.ClearChildren();

        // run map gen
        MapGeneration();
    }

    // full map generation
    public void MapGeneration()
    { 
        int localTilesMax = UnityEngine.Random.Range(minPathDistance, maxPathDistance);

        // create a path by moving one at a time on the X or Z axis
        for (tilesPlaced = 0; tilesPlaced < localTilesMax; tilesPlaced++)
        {
            #region // X,Y,Z movement checks
            // should we move on the x or z axis?
            int u = UnityEngine.Random.Range(0, 4);
            // do we move on the y axis?
            int v = UnityEngine.Random.Range(0, 3);
            // is u 0 or 1?

            // move X
            if (u == 0)
            { targetX++; }

            // move Z
            if (u == 1)
            { targetZ++; }            
            
            // move X-
            if (u == 2)
            { targetX--; }

            // move Z-
            if (u == 3)
            { targetZ--; }
            #endregion

            #region // y check
            /*
            while (true)
            {
                
                if (gridArray[targetX + maxPathDistance, targetY + maxPathDistance, targetZ + maxPathDistance])
                {
                    if (targetY > (maxPathDistance-3))
                    {
                        // targetY--;
                        continue;
                    }
                    else
                    {
                        tilesPlaced--;
                        break;
                    }
                }
                
            }*/

            #endregion

            if (!gridArray[targetX + maxPathDistance, targetY + maxPathDistance, targetZ + maxPathDistance])
            { 
                GameObject ourTile = objectPooler.SpawnFromPool("Tile", new Vector3(targetX * roomSpace, targetY * roomSpace, targetZ * roomSpace), Quaternion.Euler(0, 0, 0));
                TileClass newTileClass = ourTile.GetComponent<TileClass>();
                newTileClass.xArrayPos = targetX + maxPathDistance; newTileClass.yArrayPos = targetY + maxPathDistance; newTileClass.zArrayPos = targetZ + maxPathDistance;
                newTileClass.xPos = targetX; newTileClass.yPos = targetY; newTileClass.zPos = targetZ;
                gridArray[targetX + maxPathDistance, targetY + maxPathDistance, targetZ + maxPathDistance] = newTileClass;
                newTileClass.generationManager = this;
            }

            if (tilesPlaced == localTilesMax - 1)
            {
               //  Debug.Log("Final Tile Placed");
            }
        }

        bool debugged = false;

        // for each tile check it's neighbors, then build connections between them
        foreach (TileClass tileClass in gridArray)
        {
            if (tileClass)
            {
                if (!debugged) { Debug.Log("starting foreach loop"); debugged = true; }

                // check all neighbors around the tile to see if it has any neighbors
                foreach (Vector3 vector in checkVector3s)
                {
                    if (tileClass)
                    {
                        if ((gridArray[tileClass.xArrayPos + (int)vector.x, tileClass.yArrayPos + (int)vector.y, tileClass.zArrayPos + (int)vector.z]) && (!tileClass.isWall))
                        {
                            // find our neighbors
                            tileClass.neighbors.Add((gridArray[tileClass.xArrayPos + (int)vector.x, tileClass.yArrayPos + (int)vector.y, tileClass.zArrayPos + (int)vector.z]));
                        }

                        // check around everything to place walls
                        if ((gridArray[tileClass.xArrayPos + (int)vector.x, tileClass.yArrayPos + (int)vector.y, tileClass.zArrayPos + (int)vector.z] == null) && (!tileClass.isWall))
                        {
                            // do if y is 0 so that we do not make a ceiling (do we want one?)
                            if (vector.y == 0)
                            {
                                GameObject newWall = objectPooler.SpawnFromPool("WallTile", new Vector3((tileClass.xPos * roomSpace) + ((int)vector.x * roomSpace), (tileClass.yPos * roomSpace) + ((int)vector.y * roomSpace), (tileClass.zPos * roomSpace) + ((int)vector.z * roomSpace)), Quaternion.Euler(0, 0, 0));
                                TileClass newWallTileClass = newWall.GetComponent<TileClass>();
                                newWallTileClass.xArrayPos = tileClass.xArrayPos + (int)vector.x; newWallTileClass.yArrayPos = tileClass.yArrayPos + (int)vector.y; newWallTileClass.zArrayPos = tileClass.zArrayPos + (int)vector.z;
                                newWallTileClass.xPos = (tileClass.xPos * roomSpace) + ((int)vector.x * roomSpace); newWallTileClass.yPos = (tileClass.yPos * roomSpace) + ((int)vector.y * roomSpace); newWallTileClass.zPos = (tileClass.zPos * roomSpace) + ((int)vector.z * roomSpace);
                                gridArray[newWallTileClass.xArrayPos, newWallTileClass.yArrayPos, newWallTileClass.zArrayPos] = newWallTileClass;
                                newWallTileClass.generationManager = this;

                                // add it to the list
                                if (!wallTileClassList.Contains(newWall.GetComponent<TileClass>()))
                                wallTileClassList.Add(newWall.GetComponent<TileClass>());
                                // have it check for neighbors
                                foreach (Vector3 wallVector in checkVector3s)
                                {
                                    if ((gridArray[newWallTileClass.xArrayPos + (int)wallVector.x, newWallTileClass.yArrayPos + (int)wallVector.y, newWallTileClass.zArrayPos + (int)wallVector.z]) && (gridArray[newWallTileClass.xArrayPos + (int)wallVector.x, newWallTileClass.yArrayPos + (int)wallVector.y, newWallTileClass.zArrayPos + (int)wallVector.z].isWall == false))
                                    {
                                        newWallTileClass.neighbors.Add(gridArray[newWallTileClass.xArrayPos + (int)wallVector.x, newWallTileClass.yArrayPos + (int)wallVector.y, newWallTileClass.zArrayPos + (int)wallVector.z]);
                                    }
                                }
                            }
                        }
                    }
                }

                // after we check for neighbors add our tile to the tileClassList
                if (!tileClassList.Contains(tileClass))
                {
                    tileClassList.Add(tileClass);
                }
            }
        }

        // once our map is generated, activate the tiles
        ActivateTiles();
    }

    void ActivateTiles()
    {
        Debug.Log("activating tiles...");

        // if we don't have a player yet...
        while (!playerPlaced)
        {
            // i is going to determine which tile has the player character on it
            int i = UnityEngine.Random.Range(0, tileClassList.Count);

            // choose one of the tiles in the list and give it the player
            if (!tileClassList[i].isWall)
            {
                tileClassList[i].isOrigin = true;
                playerPlaced = true;
                Debug.Log("player placed");
            }
        }

        bool emptyChosen = false;
        artifactPlaced = false;

        // if we have already placed the player, create an empty place for them to land 
        if (playerPlaced && multiGen)
        {
            while (!emptyChosen)
            {
                // i is going to determine which tile has the player character on it
                int i = UnityEngine.Random.Range(0, tileClassList.Count);
                if (tileClassList[i].isWall == false)
                {
                    // make one of them empty
                    tileClassList[i].isEmpty = true;
                    emptyChosen = true;
                }
            }

            // if we don't have an Artifact yet...
            while (!artifactPlaced && emptyChosen)
            {
                // i is going to determine which tile has the artifact temple on it
                int i = UnityEngine.Random.Range(0, tileClassList.Count);

                // make sure this is not a wall and is not the origin tile
                if (!tileClassList[i].isWall && !tileClassList[i].isOrigin && !tileClassList[i].isEmpty)
                {
                    tileClassList[i].isArtifact = true;
                    artifactPlaced = true;
                    Debug.Log("Artifact placed");
                }
            }


        }

        // now activate all of non-walls
        foreach (TileClass tileClass in tileClassList)
        {
            if (!tileClass.isWall)
            tileClass.OnGenerate();
        }       
        
        // now activate all of the walls
        foreach (TileClass tileClass in wallTileClassList)
        {


            Debug.Log("generating walls");
            // generate walls
            tileClass.OnGenerate();
        }

        // if we can still place doors, place them until we are good to do
        while (doorAmount > 0)
        {
            // i is going to find a random wall and then turn it in to a door
            int i = UnityEngine.Random.Range(0, wallTileClassList.Count);
            // find the index of i in the wall list and make it in to a door
            if (wallTileClassList[i].isCardinalWall && !wallTileClassList[i].isDoor)
            {
                wallTileClassList[i].isDoor = true;
                wallTileClassList[i].DoorReplace();
                doorAmount--;
            }
            Debug.Log("Door picked");
        }

        Debug.Log("tiles activated");
    }

}
