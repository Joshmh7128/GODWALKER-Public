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

    int[,] primeNumberArray = new int[3, 3] { { 13, 17, 23 }, { 11, 0, 2 }, { 7, 5, 3 } };
    // int[,] primeNumberArray = new int[3, 3] { { 1, 17, 1 }, { 11, 0, 2 }, { 1, 5, 1 } };

    void Awake()
    {
        if (isWall)
        {
            // single directions

            // default
            wallObjects[0 + diff, 0 + diff, 0 + diff] = wallObjectList[8];

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
            int primeResult = 1;

            int localXArrayPos;
            int localYArrayPos;
            int localZArrayPos;


            foreach (TileClass tileClass in neighbors)
            {
                localXArrayPos = tileClass.xArrayPos - xArrayPos;
                localYArrayPos = tileClass.yArrayPos - yArrayPos;
                localZArrayPos = tileClass.zArrayPos - zArrayPos;
                primeResult *= primeNumberArray[localXArrayPos + diff, localZArrayPos + diff];
            }

            Debug.Log(primeResult);

            if (primeResult == 2 || primeResult == 6 || primeResult == 46 ||primeResult == 138)
            {   // to the forward
                wallObjects[0 + diff, 0 + diff, 1 + diff].SetActive(true);
            } 
            else if (primeResult == 11 || primeResult == 77 || primeResult == 143 || primeResult == 1001)
            {   // to backward
                wallObjects[0 + diff, 0 + diff, -1 + diff].SetActive(true);
            }            
            else if (primeResult == 5 || primeResult == 15 || primeResult == 35 || primeResult == 105)
            {   // right
                wallObjects[1 + diff, 0 + diff, 0 + diff].SetActive(true);
            }            
            else if (primeResult == 17 || primeResult == 221 || primeResult == 391 || primeResult == 5083)
            {   // left
                wallObjects[-1 + diff, 0 + diff, 0 + diff].SetActive(true);
            }           
            else if (primeResult == 3)
            {   // forward right
                wallObjects[1 + diff, 0 + diff, 1 + diff].SetActive(true);
            }           
            else if (primeResult == 23)
            {   // forward left
                wallObjects[-1 + diff, 0 + diff, 1 + diff].SetActive(true);
            }           
            else if (primeResult == 7)
            {   // backward right
                wallObjects[1 + diff, 0 + diff, -1 + diff].SetActive(true);
            }
            else if (primeResult == 13)
            {   // backward left
                wallObjects[-1 + diff, 0 + diff, -1 + diff].SetActive(true);
            }
            else if (primeResult == 30 || primeResult == 10 || primeResult == 690 || primeResult == 230 || primeResult == 70 || primeResult == 210 || primeResult == 1610 || primeResult == 4830)
            {   // forward right corner
                wallObjectList[11].SetActive(true);
            }
            else if (primeResult == 34 || primeResult == 782 || primeResult == 10166 || primeResult == 2346 || primeResult == 442 || primeResult == 102 || primeResult == 1326 || primeResult == 30498)
            {   // forward left corner
                wallObjectList[12].SetActive(true);
            }
            else if (primeResult == 55 || primeResult == 385 || primeResult == 165 || primeResult == 715 || primeResult == 1155 || primeResult == 2145 || primeResult == 5005 || primeResult == 15015)
            {   // backward right corner
                wallObjectList[9].SetActive(true);
            }
            else if (primeResult == 187 || primeResult == 2431 || primeResult == 1309 || primeResult == 4301 || primeResult == 17017 || primeResult == 30107 || primeResult == 55913 || primeResult == 391391)
            {   // backward left corner
                wallObjectList[10].SetActive(true);
            }            
            else if (primeResult % 1870 == 0)
            {   // cardinal open
                wallObjectList[13].SetActive(true);
            }           
            else if (primeResult == 69)
            {   // forward T
                wallObjectList[14].SetActive(true);
            }           
            else if (primeResult == 21)
            {   // right T
                wallObjectList[15].SetActive(true);
            }          
            else if (primeResult == 299)
            {   // left T
                wallObjectList[16].SetActive(true);
            }         
            else if (primeResult == 91)
            {   // backwards T
                wallObjectList[17].SetActive(true);
            }        
            else if (primeResult == 11730 || primeResult == 152490 || primeResult == 82110 || primeResult == 1067430 || primeResult == 3910 || primeResult == 50830 || primeResult == 27370 || primeResult == 255810 || primeResult == 510 || primeResult == 6630 || primeResult == 3570 || primeResult == 46410)
            {   // forward end nub
                wallObjectList[18].SetActive(true);
            }            
            else if (primeResult == 85085 || primeResult == 1956955 || primeResult == 255255 || primeResult == 5870865 || primeResult == 6545 || primeResult == 150535 || primeResult == 19635 || primeResult == 451605 || primeResult == 12155 || primeResult == 279565 || primeResult == 36465 || primeResult == 838695)
            {   // backward end nub
                wallObjectList[19].SetActive(true);
            }            
            else if (primeResult == 111826 || primeResult == 335478 || primeResult == 782782 || primeResult == 2348346 || primeResult == 4862 || primeResult == 14586 || primeResult == 34034 || primeResult == 102102 || primeResult == 8602 || primeResult == 25806 || primeResult == 60214 || primeResult == 180642 )
            {   // left end nub
                wallObjectList[20].SetActive(true);
            }            
            else if (primeResult == 2310 || primeResult == 30030 || primeResult == 53130 || primeResult == 690690 || primeResult == 330 || primeResult == 7590 || primeResult == 4290 || primeResult == 98670 || primeResult == 23100 || primeResult == 300300 || primeResult == 531300 || primeResult == 6906900 )
            {   // right end nub
                wallObjectList[21].SetActive(true);
            }
            else
            {
                wallObjects[0 + diff, 0 + diff, 0 + diff].SetActive(true);
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
