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
    bool[,,] neighborStates = new bool[3, 3, 3]; // all neighbor bool states with 1,1,1 being the unused center

    public GameObject[,,] wallObjects = new GameObject[3, 3, 3]; // all objects to be spawned in as walls
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

    int xLocalPos;
    int yLocalPos;
    int zLocalPos;

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

        for (int i = 0; i < wallObjects.Length; i++)
        {
            for (int j = 0; j < wallObjects.Length; j++)
            {
                for (int v = 0; v < wallObjects.Length; v++)
                {
                    Debug.Log(wallObjects[i, j, v]);
                }
            }
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
                if (xPos - tileClass.xPos != 0)
                {
                    xLocalPos = (int)Mathf.Sign(xPos - tileClass.xPos);
                }

                if (yPos - tileClass.yPos != 0)
                {
                    yLocalPos = (int)Mathf.Sign(yPos - tileClass.yPos);
                }

                if (zPos - tileClass.zPos != 0)
                {
                    zLocalPos = (int)Mathf.Sign(zPos - tileClass.zPos);
                }

                int tempX = xLocalPos+1; int tempY = yLocalPos + 1; int tempZ = zLocalPos + 1;

                Debug.Log("temps " + tempX + " " + tempY + " " + tempZ);
                Debug.Log(wallObjects[0, 1, 2]);
            }


            // wallObjects[xLocalPos + 1, yLocalPos + 1, zLocalPos + 1].SetActive(true);
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
