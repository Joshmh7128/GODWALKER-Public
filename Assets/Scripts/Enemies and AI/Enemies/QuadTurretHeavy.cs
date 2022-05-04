using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class QuadTurretHeavy : EnemyClass
{
    /// the tread turret minion is a small enemy who fires shots at the player
    /// they move to random close points within a thick radius around the player when they are above 50% HP
    /// when they are below 50% HP they will move away from the player, and attempt to run from them
    /// when they shoot they fire three shots at the player, one direct, one slightly left and one slightly right

    // our health point variables are defined in the EnemyClass; HP and MaxHP are defined in the parent abstract class
    [SerializeField] private Transform playerTransform; // our player's transform
    [SerializeField] private float activationDistance, closeRadiusMin, closeRadiusMax, farRadiusMin, farRadiusMax; // our close and far radii
    [SerializeField] private float x, y, z, rx, rz, headHeight, headRotSpeed, playerHeight; // our movement variables
    [SerializeField] private NavMeshAgent navMeshAgent;
    [SerializeField] Transform headJoint, headParent, treadTransform, treadRaycastStart; // our head joint
    [SerializeField] List<Transform> shotPositions; // our list of shot positions
    [SerializeField] GameObject bulletPrefab, bombPrefab;  // what we are firing
    [SerializeField] GameObject deathParticle;  // our death particle
    [SerializeField] GameObject enableParticle;  // our death particle
    [SerializeField] GameObject muzzleFlashParticle;  // our death particle
    public dropTypes dropType;
    [SerializeField] float dropAmount;
    [SerializeField] GameObject powerDrop, healthDrop, naniteDrop;
    bool hasActivated = false; // have we activtated?
    bool canFire; // can we shoot?

    private void Start()
    {
        // get our player transform
        if (playerTransform == null)
        { playerTransform = UpgradeSingleton.Instance.player.transform; }

        // get our nav mesh agent
        if (navMeshAgent == null)
        { navMeshAgent = GetComponent<NavMeshAgent>(); }
    }

    // our movement coroutine
    private IEnumerator NavMeshPositionTarget()
    {
        // if our HP is > 50% choose a position in our close radius, if < 50% choose in our far radius
        // top 50%

        // are we doing local or player based movement?
        int dec = Random.Range(0, 2);
        // calculate movement vectors
        if (HP > maxHP / 2f) { x = Random.Range(closeRadiusMin, closeRadiusMax); z = Random.Range(closeRadiusMin, closeRadiusMax); }
        // bottom 50%
        if (HP < maxHP / 2f)
        { x = Random.Range(farRadiusMin, farRadiusMax); z = Random.Range(farRadiusMin, farRadiusMax); }
        // shuffle those to be positive or negative within the radii
        rx = Random.Range(0, 2); rz = Random.Range(0, 2); if (rx > 0) { x *= -1; }
        if (rz > 0) { z *= -1; }
        // get the X and Z of that position and use the Y of our head so that we can walk up slopes (may need refining?)
        y = headHeight;
        if (dec == 0)
        {
            navMeshAgent.destination = playerTransform.position + new Vector3(x, y, z);
        }
        else if (dec != 0)
        {
            navMeshAgent.destination = transform.position + new Vector3(x, y, z);
        }

        // wait 4 to 6 seconds
        yield return new WaitForSeconds(Random.Range(4f, 6f));
        // repeat this cycle
        StartCoroutine(NavMeshPositionTarget());
    }

    // update runs once per calculated frame
    private void Update()
    {
        // death condition
        if (HP <= 0)
        {
            OnDeath(); // destroy this enemy through our death function
        }
    }

    // the fixed update runs 60 times per second
    private void FixedUpdate()
    {
        // if we can see the player or if our player is within reaction range, activate our enemy
        if (isActive == false)
        { if (!Physics.Linecast(headJoint.transform.position, playerTransform.position) || (Vector3.Distance(transform.position, playerTransform.position)) < activationDistance || HP != maxHP) { Activate(); } }

        if (isActive == true && hasActivated == false) { Activate(); }

        // lets make sure our treads look forward and are level with the ground
        RaycastHit hit;
        // fire a ray downwards
        if (Physics.Raycast(treadRaycastStart.position, Vector3.down, out hit, 3f, Physics.AllLayers, QueryTriggerInteraction.Ignore))
        {
            // make our treads look at it
            treadTransform.LookAt(hit.point);
        }

        // counter our head parent's rotation
        Quaternion nullRotation = Quaternion.Euler(-transform.rotation.eulerAngles);
        headParent.localRotation = nullRotation;

        // rotate our headjoint to look at the player
        Vector3 direction = headJoint.position - (playerTransform.transform.position + new Vector3(0f, playerHeight, 0f)); // get our initial direction from our head to our player
        Quaternion toRotation = Quaternion.FromToRotation(headJoint.forward - headJoint.position, direction); // use our head direction to point at the player
        Quaternion toRotationFixed = Quaternion.Euler(new Vector3(toRotation.eulerAngles.x, toRotation.eulerAngles.y, 0f)); // fix the rotation on the z axis so the head doesnt swing around
        headJoint.localRotation = Quaternion.RotateTowards(headJoint.rotation, toRotationFixed, headRotSpeed * Time.deltaTime);
    }

    // when we are enabled
    private void OnEnable()
    {
        // spawn in our enabled fx
        if (enableParticle)
            Instantiate(enableParticle, transform.position, Quaternion.identity, null);
    }

    public override void Activate()
    {
        // we are now active
        isActive = true; hasActivated = true;
        // start our movement coroutine
        StartCoroutine(NavMeshPositionTarget());
        // play our shooting animation
        GetComponent<Animator>().Play("Shooting Animation");
    }

    // randomizing our animation speed
    public void RandomizeAnimationSpeed()
    {
        GetComponent<Animator>().speed = Random.Range(0.75f, 1.25f);
    }

    public override void Attack() { }

    public void CallAttack(int isPhys)
    {
        if (isPhys == 1)
        {
            Attack(true, 0.17f, 4, true);
        } else if (isPhys == 0)
        {
            Attack(true, 0.05f, 0, false);
        }
    }

    public void StopAttack()
    {
        canFire = false;
    }

    // alternate through our shooting positions
    public void Attack(bool localCanFire, float firerate, int shotpos, bool isPhysics)
    {
        // make sure we can fire
        canFire = localCanFire;

        if (localCanFire)
        {

            if (!isPhysics)
            {
                // start our coroutine
                StartCoroutine(ProjectileFireControl(firerate, shotpos));
            } else if (isPhysics)
            {
                // start our coroutine
                StartCoroutine(PhysicsFireControl(firerate, shotpos));
            }
        }
    }

    public IEnumerator ProjectileFireControl(float firerate, int shotpos)
    {
        yield return new WaitForSeconds(firerate);

        if (canFire)
        {
            // if we are firing projectiles
            if (shotpos < 3)
            {
                // fire the shot
                CustomAttack(shotpos);
                // handle where the shot is coming from
                shotpos++;
            } else if (shotpos >= 3)
            {
                // fire the shot
                CustomAttack(shotpos);
                // handle where the shot is coming from
                shotpos = 0;
            }
            // call the coroutine again
            StartCoroutine(ProjectileFireControl(firerate, shotpos));
        }
    }

    public IEnumerator PhysicsFireControl(float firerate, int shotpos)
    {
        yield return new WaitForSeconds(firerate);

        if (canFire)
        {
            // if we are firing projectiles
            if (shotpos < 6)
            {
                // fire the shot
                CustomAttack(shotpos);
                // handle where the shot is coming from
                shotpos++;
            }
            else if (shotpos == 6)
            {
                // fire the shot
                CustomAttack(shotpos);
                // handle where the shot is coming from
                shotpos = 4;
            }
            // call the coroutine again
            StartCoroutine(PhysicsFireControl(firerate, shotpos));
        }
    }


    // custom attack for our 4 shot positions
    public void CustomAttack(int shotPos)
    {
        if (shotPos <= 3)
        {
            // our shotPos int is handled by animation
            Instantiate(bulletPrefab, shotPositions[shotPos].position, shotPositions[shotPos].rotation, null);
        }
        else
        {
            // our shotPos int is handled by animation
            Instantiate(bombPrefab, shotPositions[shotPos].position, shotPositions[shotPos].rotation, null);
        }

        // and our flash particle too
        Instantiate(muzzleFlashParticle, shotPositions[shotPos].position, shotPositions[shotPos].rotation, null);
    }

    // when we take damage
    public override void TakeDamage(int dmg)
    {
        // apply damage
        HP -= dmg;
        // flicker 
        GetComponent<Animator>().speed = 1;
        GetComponent<Animator>().Play("Damage");
        // perform hurt animation
    }

    // our death function
    public override void OnDeath()
    {
        // make sure we call our combat zone's death
        combatZone.OnDeath();
        // depending on our droptype, drop different amounts of resources
        switch (dropType)
        {
            case (dropTypes.power):
                while (dropAmount > 0)
                {
                    // spawn our desired amount of drops
                    Instantiate(powerDrop, transform.position + new Vector3(Random.Range(-1, 1), Random.Range(-1, 1), Random.Range(-1, 1)), Quaternion.identity);
                    dropAmount--; // deduct our value

                    // break condition
                    if (dropAmount <= 0)
                    { break; }
                }
                break;

            case (dropTypes.nanites):
                while (dropAmount > 0)
                {
                    // spawn our desired amount of drops
                    Instantiate(naniteDrop, transform.position + new Vector3(Random.Range(-1, 1), Random.Range(-1, 1), Random.Range(-1, 1)), Quaternion.identity);
                    dropAmount--; // deduct our value

                    // break condition
                    if (dropAmount <= 0)
                    { break; }
                }
                break;

            case (dropTypes.HP):
                while (dropAmount > 0)
                {
                    // spawn our desired amount of drops
                    Instantiate(healthDrop, transform.position + new Vector3(Random.Range(-1, 1), Random.Range(-1, 1), Random.Range(-1, 1)), Quaternion.identity);
                    dropAmount--; // deduct our value

                    // break condition
                    if (dropAmount <= 0)
                    { break; }
                }
                break;
        }
        // then blow up
        Instantiate(deathParticle, transform.position, Quaternion.identity, null);
        // then destroy ourselves
        Destroy(gameObject);
    }
}
