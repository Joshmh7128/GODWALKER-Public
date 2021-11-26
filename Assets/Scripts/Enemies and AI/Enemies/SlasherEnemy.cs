using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlasherEnemy : EnemyClass
{
    [SerializeField] float HP, maxHP; // our current and maximum hp
    [SerializeField] Animator mainAnimator, hurtAnimator; // our animators
    [SerializeField] List<int> attackPattern; // our attack pattern
    [SerializeField] List<AnimationClip> animationClips; // our animation clips
    [SerializeField] List<string> animationClipStrings; // our animation clips
    [SerializeField] LineRenderer targetLineRenderer; // our target line renderers
    bool isActive; // are we active yet?


    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(AttackCoroutine(attackPattern));
    }

    IEnumerator AttackCoroutine(List<int> attackPatterns)
    {
        // do our attacks
        foreach (int i in attackPatterns)
        {
            mainAnimator.Play(animationClipStrings[i]);
            yield return new WaitForSeconds(animationClips[i].length);
        }
    }

    // how we take damage
    public override void TakeDamage(int dmg)
    {
        // play the hurt animation
        hurtAnimator.Play("Hurt");
        // lower HP
        HP -= dmg;
    }
}
