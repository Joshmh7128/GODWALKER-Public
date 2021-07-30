using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileClass : MonoBehaviour
{
    // our position
    public int xArrayPos, yArrayPos, zArrayPos;
    public int xPos, yPos, zPos;
    public List<TileClass> neighbors;
    public bool isWall = false;
    public bool isOrigin = false;
    public bool isEmpty = false;
    [SerializeField] GameObject generator;
    [SerializeField] GameObject playerPackage;

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
    }
}
