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
    // weapon manager instance
    PlayerWeaponManager playerWeaponManager;
    // our buff as updated
    float buffAmount;

    public override void PartStart()
    {
        // get our instance
        playerController = PlayerController.instance;
        playerWeaponManager = PlayerWeaponManager.instance;
    }

    // set active
    public override void OnSprint()
    { 
        active = true; 
        // only start the coroutine again if we can drop another mine
        if (!coroutineRunning) StartCoroutine(Counter()); 
    }

    public override void OffSprint()
    {
        // we are no longer active
        active = false;
        // reset the sprint move mod
        playerController.sprintMoveMod = 0;
        // reset the damage buff
        ManageBuff(buffAmount);
    }

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

    // whenever we deal explosive damage increase sprint speed by 10%
    public override void OnExplosionDamage()
    {
        if (active)
        {
            // make our player move faster
            playerController.sprintMoveMod += 0.1f;
            // damage mod
            ManageBuff(buffAmount, playerController.sprintMoveMod);
        }
    }

    // use this ability: fire 5 bombs
    public override void OnUseAbility()
    {
        GameObject g;
        Debug.Log("using ability");
        for (int i = 0; i < 5; i++)
        {
            g = Instantiate(minePrefab, playerController.transform.position + playerController.animationRigParent.forward, Quaternion.identity, null);
            g.GetComponent<Rigidbody>().AddForce(new Vector3(Random.Range(-1, 1), 0, Random.Range(-1, 1)));
        }
    }

    void ManageBuff(float lastBuff, float newBuff)
    {
        // remove our previous buff
        playerWeaponManager.currentWeapon.damageMods.Remove(lastBuff);
        // then update our damage mod to match
        playerWeaponManager.currentWeapon.damageMods.Add(newBuff);
        // then set to match
        buffAmount = newBuff;
    }

    void ManageBuff(float lastBuff)
    {
        // remove our previous buff
        if (playerWeaponManager.currentWeapon.damageMods.Contains(lastBuff))
        playerWeaponManager.currentWeapon.damageMods.Remove(lastBuff);
        // then set to match
        buffAmount = 0;
    }
}
