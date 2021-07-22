using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerationManager : MonoBehaviour
{
    // our list of map generation chunks
    [SerializeField] RandomChildSelector[] randomChildSelectors;
    // bug parent
    [SerializeField] Transform enemyManager;

    // map generation
    int pathDistanceMinimum = 50;
    int roomSpace /* how large rooms are */, pathDistance, targetX, targetY, targetZ;
    int minTargetY = 3;
    int tilesPlaced; // how many tiles we have placed so far
    static int maxPathDistance = 50; // how long should our generation paths be?
    [SerializeField] TileClass[,,] gridArray = new TileClass[maxPathDistance * 2, maxPathDistance * 2, maxPathDistance * 2]; // our x, y, z array
    [SerializeField] GameObject tileClassObject;
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

         // top middle
         new Vector3(0,1,0),

         // bottom middle
         new Vector3(0,-1,0)
    } ; // our list of all vector3 directions

    private void Start()
    {
        MapGeneration();
    }

    public void MapRegen()
    {
        StartCoroutine("BasicMapRegenCo");
    }

    // basic map regeneration in which all tile randomizers are reset
    // trigger regeneration
    IEnumerator BasicMapRegenCo()
    {
        // generate while drop pod is up in the air before it comes down
        foreach (RandomChildSelector selector in randomChildSelectors)
        {
            selector.Regen();
        }

        // kill all the enemies
        foreach (Transform ourGameObject in enemyManager)
        {
            Destroy(ourGameObject.gameObject);
        }

        yield return new WaitForSeconds(1f);
    }

    // fill our array with NULL entities before we begin adding them
    void ClearArray()
    {
        foreach (TileClass tileClass in gridArray)
        {
            
        }
    }

    // full map generation
    void MapGeneration()
    {
        int localTilesMax = Random.Range(pathDistanceMinimum, maxPathDistance);

        // create a path by moving one at a time on the X or Z axis
        for (tilesPlaced = 0; tilesPlaced < localTilesMax; tilesPlaced++)
        {
            #region // X,Y,Z movement checks
            // should we move on the x or z axis?
            int u = Random.Range(0, 4);
            // do we move on the y axis?
            int v = Random.Range(0, 3); 
            // is u 0 or 1?

            // move X
            if (u == 0)
            {
                targetX++;
            }

            // move Z
            if (u == 1)
            {
                targetZ++;
            }            
            
            // move X-
            if (u == 2)
            {
                targetX--;
            }

            // move Z-
            if (u == 3)
            {
                targetZ--;
            }
            /*
            // move Y+
            if (v == 1)
            {
                // targetY++;
            }

            // move Y-
            if (v == 2)
            {
                targetY--;
            }*/
            #endregion

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

                TileClass newTileClass = Instantiate(tileClassObject, new Vector3(targetX, targetY, targetZ), Quaternion.Euler(0, 0, 0),null).GetComponent<TileClass>();
                newTileClass.xArrayPos = targetX + maxPathDistance; newTileClass.yArrayPos = targetY + maxPathDistance; newTileClass.zArrayPos = targetZ + maxPathDistance;
                gridArray[targetX + maxPathDistance, targetY + maxPathDistance, targetZ + maxPathDistance] = newTileClass;
                break;
            }
            
            if (tilesPlaced == localTilesMax - 1)
            {
                Debug.Log("Final Tile Placed");
            }
        }

        bool debugged = false;

        // for each tile check it's neighbors, then build connections between them
        foreach (TileClass tileClass in gridArray)
        {
            if (!debugged) { Debug.Log("starting foreach loop"); debugged = true; }
            // Debug.Log("Starting Neighbor check");
            // check all neighbors around the tile to see if it has any neighbors
            foreach (Vector3 vector in checkVector3s)
            {
                if (tileClass)
                {
                    if (gridArray[tileClass.xArrayPos + (int)vector.x, tileClass.yArrayPos + (int)vector.y, tileClass.zArrayPos + (int)vector.z])
                    {
                        tileClass.neighbors.Add((gridArray[tileClass.xArrayPos + (int)vector.x, tileClass.yArrayPos + (int)vector.y, tileClass.zArrayPos + (int)vector.z]));
                    }
                }
            }
        }
    }
}
