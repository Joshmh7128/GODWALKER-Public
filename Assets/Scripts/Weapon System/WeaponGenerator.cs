using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponGenerator : MonoBehaviour
{
    /// 
    /// this script is used to generate a weapon based off of a pre-made weapon part body
    /// attach it to a pre-made weapon prefab and it will generate a weapon
    /// to make a new weapon, use the WeaponCreator Script
    /// this script is built dynamically so that weapons can be customized
    /// you will not need to have all parts of the weapon attached to it. for example, a pistol does not have to have a scope,
    /// or an SMG does not need to have a foregrip. 
    /// 

    [SerializeField] Transform partRoot; // the root of each of our parts

    private void OnEnable()
    {
        // on enable, build our weapon
        GenerateWeapon();
    }

    void GenerateWeapon()
    {
        // first grab our part root and make a list of lists
        List<Transform> roots = new List<Transform>();
        // add the part parents to that list
        foreach (Transform child in partRoot)
        { roots.Add(child); }
        // then for each root enable one object
        foreach (Transform root in roots)
        {
            if (root.childCount > 0)
            {
                // check if this root has any active objects
                bool preActive = false;
                for (int i = 0; i < root.childCount; i++)
                {
                    if (root.GetChild(i).gameObject.activeInHierarchy)
                    {
                        preActive = true;
                    }
                }

                // if we did not preactivate parts on this root, activate one
                if (!preActive)
                {
                    // enable a random child part
                    int r = Random.Range(0, root.childCount);
                    root.GetChild(r).gameObject.SetActive(true);
                    // then clean the extras
                    foreach (Transform child in root)
                    {
                        if (!child.gameObject.activeInHierarchy)
                        {
                            Destroy(child.gameObject);
                        }
                    }
                }
            }
        }



    }
}
