using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class TileClass : MonoBehaviour
{
    // our position
    public int xArrayPos, yArrayPos, zArrayPos; // our position in the array
    public int xPos, yPos, zPos; // our real world position for walls and our position times 50 for nonwalls
    public List<TileClass> neighbors; // our neighbor tiles
    [SerializeField] List<TileClass> localNeighbors; // filtered neighbor tiles
    public bool isWall = false; // are we a wall?
    public bool isOrigin = false; // are we an origin?
    public bool isEmpty = false; // are we empty?
    [SerializeField] bool devDraw = false; // should we be drawing gizmos?
    [SerializeField] GameObject generator; // what is our generator?
    [SerializeField] GameObject playerPackage; // what is our player package?
    bool[,,] neighborStates = new bool[4, 4, 4]; // all neighbor bool states with 1,1,1 being the unused center

    GameObject[,,] wallObjects = new GameObject[4, 4, 4]; // all objects to be spawned in as walls
    [SerializeField] List<GameObject> wallObjectList; // set in editor to save one million headaches
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
    }; // our list of all vector3 directions
    
    void Start()
    {
        if (isWall)
        {
            int diff = 1;
            // single directions
            wallObjects[-1 + diff, 0 + diff, 1 + diff] = wallObjectList[0];
            wallObjects[0 + diff, 0 + diff, 1 + diff] = wallObjectList[1];
            wallObjects[1 + diff, 0 + diff, 1 + diff] = wallObjectList[2];
            wallObjects[-1 + diff, 0 + diff, 0 + diff] = wallObjectList[3];
            wallObjects[1 + diff, 0 + diff, 0 + diff] = wallObjectList[4];
            wallObjects[-1 + diff, 0 + diff, -1 + diff] = wallObjectList[5];
            wallObjects[0 + diff, 0 + diff, -1 + diff] = wallObjectList[6];
            wallObjects[1 + diff, 0 + diff, -1 + diff] = wallObjectList[7];
        }
    }

    // when we generate the map
    public void OnGenerate()
    {
        if (!isWall)
        {
            if (!isOrigin && !isEmpty)
            {
                generator.SetActive(true);
            }

            if (isOrigin)
            {
                playerPackage.SetActive(true);
            }

            if (isEmpty)
            {
                GameObject.Find("Drop Pod").GetComponent<DroppodManager>().targetPosGroundNew = transform.position;
            }
        }

        if (isWall)
        {
            // check which of our neighbors is a non-wall tile 
            foreach(TileClass tileClass in neighbors)
            {   // if none of these are true, add them to the local list...
                if (!(tileClass.isOrigin || tileClass.isWall || tileClass.isEmpty))
                {   // if the tileclass is not null...
                    if (tileClass)
                    {   // if we do not have it in the list already...
                        if (!localNeighbors.Contains(tileClass))
                        localNeighbors.Add(tileClass);
                    }
                }
            }

            // use this information to place specific wall variations
            foreach (TileClass tileClass in localNeighbors)
            {
                // get our XYZ of one of our neighbor tiles in relation to ourselves
                int xlocalPos = (int)Mathf.Sign(tileClass.xPos - xPos); 
                int ylocalPos = 0; 
                int zlocalPos = (int)Mathf.Sign(tileClass.zPos - zPos); // divide by the form factor of our tile size
                // set our local ints
                int x = xlocalPos+1; int y = ylocalPos+1; int z = zlocalPos+1; // make sure to add 1 so that 1, 1, 1, goes unused as the center
                Debug.Log(x + " " + y + " " + z);
                // set our objects
                neighborStates[x, y, z] = true;
            }

            // set objects to be active if they are active
            foreach (Vector3 vector in checkVector3s)
            {
                if (neighborStates[(int)vector.x+1, (int)vector.y, (int)vector.z+1])
                {
                    if (wallObjects[(int)vector.x + 1, (int)vector.y + 1, (int)vector.z + 1])
                    wallObjects[(int)vector.x + 1, (int)vector.y + 1, (int)vector.z + 1].SetActive(true);
                }
            }
        }
    }

    public void OnDrawGizmos()
    {
        if (devDraw)
        {
            foreach(TileClass tileClass in neighbors)
            {
                Gizmos.color = Color.magenta;
                Gizmos.DrawCube(new Vector3(tileClass.xPos * 50, tileClass.yPos * 50, tileClass.zPos * 50), new Vector3(5, 60, 5));
            }
        }
    }
}
