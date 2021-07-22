using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileClass : MonoBehaviour
{
    // our position
    public int xArrayPos, yArrayPos, zArrayPos;
    public int xPos, yPos, zPos;
    public List<TileClass> neighbors;
    public bool isWall;
    public bool isOrigin;
}
