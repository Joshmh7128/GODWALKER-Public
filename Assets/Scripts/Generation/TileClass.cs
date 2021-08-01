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
    bool[,,] neighborStates = new bool[5, 5, 5]; // all neighbor bool states with 1,1,1 being the unused center

    public GameObject[,,] wallObjects = new GameObject[5, 5, 5]; // all objects to be spawned in as walls
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
    int diff = 2;

    [SerializeField] int serTempX;
    [SerializeField] int serTempY;
    [SerializeField] int serTempZ;

    void Awake()
    {
        if (isWall)
        {  
            // single directions
            wallObjects[-1 + diff, 0 + diff, 1 + diff] = wallObjectList[0];
            wallObjects[0 + diff, 0 + diff, 1 + diff] = wallObjectList[1];
            wallObjects[1 + diff, 0 + diff, 1 + diff] = wallObjectList[2];
            wallObjects[-1 + diff, 0 + diff, 0 + diff] = wallObjectList[3];
            wallObjects[1 + diff, 0 + diff, 0 + diff] = wallObjectList[4];
            wallObjects[-1 + diff, 0 + diff, -1 + diff] = wallObjectList[5];
            wallObjects[0 + diff, 0 + diff, -1 + diff] = wallObjectList[6];
            wallObjects[1 + diff, 0 + diff, -1 + diff] = wallObjectList[7];
            wallObjects[0 + diff, 0 + diff, 0 + diff] = wallObjectList[8];
            wallObjects[0, 0, 0] = wallObjectList[8];
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
         
            /// attempted to make walls change based on environment, failed spectacularly. 
            // StartCoroutine(LocalNeighborsAssignment());
        }
    }

    IEnumerator LocalNeighborsAssignment()
    {
        yield return new WaitForEndOfFrame();

        if (neighbors.Count == 0)
        {
            Debug.LogError("Tile Has No Neighbors");
        }

        // use this information to place specific wall variations
        foreach (TileClass tileClass in neighbors)
        {
            if (tileClass.xArrayPos - xArrayPos != 0)
            {
                xLocalPos = tileClass.xArrayPos - xArrayPos < 0 ? -1 : 1;
            }
            else
            {
                xLocalPos = 0;
            }

            if (tileClass.yArrayPos - yArrayPos != 0)
            {
                yLocalPos = tileClass.yArrayPos - yArrayPos < 0 ? -1 : 1;
            }
            else
            {
                yLocalPos = 0;
            }

            if (zArrayPos - tileClass.zArrayPos != 0)
            {
                zLocalPos = tileClass.zArrayPos - zArrayPos < 0 ? -1 : 1;
            }
            else
            {
                zLocalPos = 0;
            }

            int tempX = xLocalPos + diff; int tempY = yLocalPos + diff; int tempZ = zLocalPos + diff;

            Debug.Log("diff is: " + diff + " | locals for hash:" + gameObject.GetHashCode() + xLocalPos + " " + yLocalPos + " " + zLocalPos + " | temps: " + tempX + " " + tempY + " " + tempZ);
            // Debug.Log(wallObjects[0, 1, 2]);
            WallObjectControl(tempX, tempY, tempZ);
        }
    }

    void WallObjectControl(int x, int y, int z)
    {
        serTempX = x;
        serTempY = y;
        serTempZ = z;
        wallObjects[x, y, z].SetActive(true);
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
