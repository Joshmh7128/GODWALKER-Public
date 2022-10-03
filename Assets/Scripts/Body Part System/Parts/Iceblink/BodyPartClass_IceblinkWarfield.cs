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
    bool active, canDrop, coroutineRunning; // are we active or inactive? can we drop another mine?

    // player instance
    PlayerController playerController;

    public override void PartStart()
    {
        // get our instance
        playerController = PlayerController.instance;
    }

    // set active
    public override void OnSprint()
    { 
        active = true; 
        // only start the coroutine again if we can drop another mine
        if (!coroutineRunning) StartCoroutine(Counter()); 
    }

    public override void OffSprint() => active = false;

    // function for dropping mines
    void DropMine()
    {
        // drop a mine
        if (canDrop)
        Instantiate(minePrefab, playerController.transform.position + playerController.animationRigParent.forward, Quaternion.identity, null);
    }

    // check every second to see if we can drop another mine
    IEnumerator Counter()
    {
        coroutineRunning = true;
        // if we are sprinting
        if (active)
        DropMine();
        // after the drop, we cannot drop another mine until the timer runs our
        canDrop = false;
        yield return new WaitForSecondsRealtime(1);
        canDrop = true;
        // if we are still sprinting drop another mine
        if (active)
        {
            StartCoroutine(Counter());
        }
        else if (!active)
        {
            coroutineRunning = false;
        }
    }

}
