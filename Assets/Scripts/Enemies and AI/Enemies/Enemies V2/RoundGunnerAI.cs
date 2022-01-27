using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoundGunnerAI : EnemyClass
{
    [SerializeField] PlayerController playerController; // our player's transform
    // all our movement stuff
    [SerializeField] float movementSpeed, rotationSpeed;
    // animation control
    [SerializeField] Animator ourAnimator;

    // Start is called before the first frame update
    void Start()
    {
        if (UpgradeSingleton.Instance)
        { playerController = UpgradeSingleton.Instance.player; }
        else
        { Debug.LogError("A Round Gunner AI is attempting to access the UpgradeSingleton - has it been created yet?");  }
    }

    // fixed update is called 60 times per second
    private void FixedUpdate()
    {
        // rotate at our rotation speed to face the player
        Quaternion lookRot = Quaternion.LookRotation(playerController.gameObject.transform.position - transform.position);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRot, Time.deltaTime * rotationSpeed);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // when we take damage
    public override void TakeDamage(int dmg)
    {

    }
}
