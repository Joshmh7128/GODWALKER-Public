using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BodyPartClass_MagmaCrawler : BodyPartClass
{
    // Start is called before the first frame update
    public override void PartStart()
    {
        LavawalkCheck();
    }

    public override void OnBodyPartPickup()
    {
        StartCoroutine(Buffer());
    }

    IEnumerator Buffer()
    {
        yield return new WaitForFixedUpdate();
        yield return new WaitForFixedUpdate();
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
