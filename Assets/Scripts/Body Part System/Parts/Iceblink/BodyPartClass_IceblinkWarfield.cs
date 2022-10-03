using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BodyPartClass_IceblinkWarfield : BodyPartClass
{
    /// - While Sprinting leave a trail of Explosive Mines
    /// - Whenever an Explosion deals damage while Sprinting your Sprint speed is increased until you stop Sprinting
    /// - Damage is now tied to Movement Speed
    /// - Firerate is now tied to Movement Speed
    /// 

    // our mine prefab
    [SerializeField] GameObject minePrefab;
    bool active, canDrop; // are we active or inactive? can we drop another mine?

    // set active
    public override void OnSprint() => active = true;
    public override void OffSprint() => active = false;

    // function for dropping mines
    void DropMine()
    {
        // drop a mine

        // start the counter

    }


    // counter for being able to drop again
    IEnumerator Counter()
    {
        canDrop = false;
        yield return new WaitForSecondsRealtime(1);
        canDrop = true;
    }

}
