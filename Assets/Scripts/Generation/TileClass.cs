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
    public bool isActive = true;
    [SerializeField] GameObject generator;
    [SerializeField] GameObject playerPackage;

    private void Start()
    {
        if (!isWall)
        {
            if (!isOrigin)
            {
                if (generator.activeInHierarchy != true)
                generator.SetActive(true);
            }

            if (isOrigin)
            {
                playerPackage.SetActive(true);
            }
        }
    }
}
