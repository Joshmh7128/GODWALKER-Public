using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BodyPartClass_MagmaCrawler : BodyPartClass
{
    // when this is picked up
    public override void OnBodyPartPickup()
    {
        PlayerStatManager.instance.lavaWalks = true;
    }

    public override void OnDropThisPart()
    {
        PlayerStatManager.instance.lavaWalks = false;
    }
}
