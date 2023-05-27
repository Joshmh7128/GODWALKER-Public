using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyClass_Helion : EnemyClass
{
    /// the helion is a huge enemy that fires noisy shots out of the tips all over its body
    /// it alternates between noise mode (orange) and homing mode (purple)
    [SerializeField] float baseHealth; // health we set in-editor, this is a beefier enemy so we want it to be higher
    [SerializeField] bool rotates = true;

    public override void FixedUpdate()
    {
        base.FixedUpdate();
        // rotate our body slowly to make us look more dramatic
        if (rotates)
        transform.eulerAngles += new Vector3(0.5f, 0.5f, 0);

        if (Vector3.Distance(transform.position, PlayerController.instance.transform.position) > 400)
        {
            Destroy(gameObject);
        }
    }


}
