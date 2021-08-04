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

    int diff = 1;

    void Awake()
    {
        if (isWall)
        {
            // single directions

            // right
            wallObjects[1 + diff, 0 + diff, 0 + diff] = wallObjectList[4];

            // left
            wallObjects[-1 + diff, 0 + diff, 0 + diff] = wallObjectList[3];

            // forward
            wallObjects[0 + diff, 0 + diff, 1 + diff] = wallObjectList[1];

            // backward
            wallObjects[0 + diff, 0 + diff, -1 + diff] = wallObjectList[6];

            // right forward
            wallObjects[1 + diff, 0 + diff, 1 + diff] = wallObjectList[2];

            // left, forward
            wallObjects[-1 + diff, 0 + diff, 1 + diff] = wallObjectList[0];

            // right backward
            wallObjects[1 + diff, 0 + diff, -1 + diff] = wallObjectList[7];

            // left backward
            wallObjects[-1 + diff, 0 + diff, -1 + diff] = wallObjectList[5];
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
            foreach (TileClass tileClass in neighbors)
            {
                int localXArrayPos = tileClass.xArrayPos - xArrayPos;
                int localYArrayPos = tileClass.yArrayPos - yArrayPos;
                int localZArrayPos = tileClass.zArrayPos - zArrayPos;

                if (Mathf.Abs(localXArrayPos) + Mathf.Abs(localYArrayPos) + Mathf.Abs(localZArrayPos) == 1)
                {
                    // right
                    if (localXArrayPos == 1 && localYArrayPos == 0 && localZArrayPos == 0)
                    { wallObjects[1 + diff, localYArrayPos + diff, localZArrayPos + diff].SetActive(true); }

                    // left
                    if (localXArrayPos == -1 && localYArrayPos == 0 && localZArrayPos == 0)
                    { wallObjects[-1 + diff, localYArrayPos + diff, localZArrayPos + diff].SetActive(true); }

                    // forward
                    if (localXArrayPos == 0 && localYArrayPos == 0 && localZArrayPos == 1)
                    { wallObjects[0 + diff, localYArrayPos + diff, localZArrayPos + diff].SetActive(true); }

                    // backward
                    if (localXArrayPos == 0 && localYArrayPos == 0 && localZArrayPos == -1)
                    { wallObjects[0 + diff, localYArrayPos + diff, localZArrayPos + diff].SetActive(true); }
                }
                else
                {
                    // right forward
                    if (localXArrayPos == 1 && localYArrayPos == 0 && localZArrayPos == 1)
                    { wallObjects[1 + diff, localYArrayPos + diff, localZArrayPos + diff].SetActive(true); }

                    // left forward
                    if (localXArrayPos == -1 && localYArrayPos == 0 && localZArrayPos == 1)
                    { wallObjects[-1 + diff, localYArrayPos + diff, localZArrayPos + diff].SetActive(true); }

                    // right backward
                    if (localXArrayPos == 1 && localYArrayPos == 0 && localZArrayPos == -1)
                    { wallObjects[1 + diff, localYArrayPos + diff, localZArrayPos + diff].SetActive(true); }

                    // left backward
                    if (localXArrayPos == -1 && localYArrayPos == 0 && localZArrayPos == -1)
                    { wallObjects[-1 + diff, localYArrayPos + diff, localZArrayPos + diff].SetActive(true); }
                }


            }
            #region // automation attempt
            /*
            foreach (TileClass tileClass in neighbors)
            {
                // compare our position to theirs
                int localXArrayPos = tileClass.xArrayPos - xArrayPos;
                int localYArrayPos = tileClass.yArrayPos - yArrayPos;
                int localZArrayPos = tileClass.zArrayPos - zArrayPos;
                // activate accordingly
                wallObjects[localXArrayPos + diff, localYArrayPos + diff, localZArrayPos + diff].SetActive(true);
            }
            */
            #endregion
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

        if (devDraw && isWall)
        {
            Gizmos.color = Color.magenta;
            Gizmos.DrawWireCube(new Vector3(50, 0, 0), new Vector3(50, 50, 50));
            Gizmos.DrawWireCube(new Vector3(-50, 0, 0), new Vector3(50, 50, 50));
            Gizmos.DrawWireCube(new Vector3(0, 0, 50), new Vector3(50, 50, 50));
            Gizmos.DrawWireCube(new Vector3(0, 0, -50), new Vector3(50, 50, 50));
        }
    }
}
