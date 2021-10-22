using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlasherEnemyScript : MonoBehaviour
{
    // this script manages the large enemy named the Slasher
    [SerializeField] Transform playerTarget; // our target player
    [SerializeField] Transform ourHeadTransform; // our target player
    [SerializeField] Animator animator;
    [SerializeField] Animation aimHoldAnim;
    [SerializeField] Animation aimingAnim;
    [SerializeField] Animation slashAttack001Anim;
    [SerializeField] bool canHeadLookAtPlayer; // can we rotate our head to the player?
    [SerializeField] bool canBodyLookAtPlayer; // can we rotate our body to the player?

    // Start is called before the first frame update
    void OnActivate()
    {
        StartCoroutine(BattleSequence());
    }

    IEnumerator BattleSequence()
    {
        yield return new WaitForSeconds(1f);
        // look at our player
        canHeadLookAtPlayer = true;
        canBodyLookAtPlayer = true;
        // wait for our aiming animation to finish

    }
}
