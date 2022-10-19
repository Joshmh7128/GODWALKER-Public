using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BodyPartClass_MagmaCrawler : BodyPartClass
{
    // when this is picked up
    public override void OnBodyPartPickup()
    {
        LavawalkCheck();
        Debug.Log("magma picktup");
    }

    public override void OnDropThisPart()
    {
        Debug.Log("magma ondrop");
        LavawalkCheck();
    }

    void LavawalkCheck()
    {
        bool check = false;
        // check if this part is part of the player
        foreach (BodyPartClass part in PlayerBodyPartManager.instance.bodyParts)
        {
            if (part == this)
            {
                // player can now walk on lava        
                check = true;
                break;
            }
        }

        PlayerStatManager.instance.lavaWalks = check;
    }

}
