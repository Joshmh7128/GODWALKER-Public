using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public abstract class EnemyClass : MonoBehaviour
{
    // class exists as the baseline for all of our enemies
    // stats
    [HeaderAttribute("-- Stats --")]
    public string enemyName;
    public int level;
    public float health;
    public float maxHealth;
    public float damage; // how much damage do we deal?
    public float rageAmount; // how much rage does killing us give to the player
    public float rageModifier = 1; // when we get hit how much rage does the shot add when it hits us, based off of its own rage amount?
    public bool invincible; // is this invincible?
    [SerializeField] float lootDropChancePercentage;
    [SerializeField] GameObject dropItem;
    bool dead; // are we dead?
    bool canKickMovement; // can we kick movement?
    float kickReset; // count down to kick reset
    [SerializeField] Light fxLight; // our vfx light

    // our debug linerenderer
    [Header("Debug Line Renderer")]
    public LineRenderer lineRenderer;

    // elemental shields
    public enum ElementalProtection
    {
        none, explosiveShield, energyShield
    }

    [Header("-- Elemental Stats --")]
    // do we spawn with any shield types?
    public ElementalProtection activeElementalProtection;

    // armor list
    [SerializeField] List<GameObject> armorPlates;
    [SerializeField] List<GameObject> energyShields;

    // armor values
    [SerializeField] float setExplosiveArmorHP, setEnergyShieldHP;
    [SerializeField] public float explosiveArmorHP, energyShieldHP;

    // our behaviours
    [HideInInspector] public List<EnemyBehaviour> allBehaviours;
    [Header("-- Behaviours --")]
    public Transform attackBehaviourParent;
    public Transform movementBehaviourParent;
    [SerializeField] List<EnemyBehaviour> attackBehaviours = new List<EnemyBehaviour>();
    [SerializeField] List<EnemyBehaviour> movementBehaviours = new List<EnemyBehaviour>();
    public bool activated;
    // our agent
    public NavMeshAgent navMeshAgent;

    PlayerBodyPartManager playerBodyPartManager;

    // our spawn point management
    public enum SpawnPointRequirements
    {
        groundRandom, // anywhere that is on the ground
        airRandom, // randomly in the air
        groundFarFromPlayer, // chooses a grounded point that is deliberately far from the player
        airFarFromPlayer, // anywhere in the air far from the player
        centralGrounded, // chooses a random point in the central of the map
        centralAir, // chooses a random point in the central of the map
    }

    public SpawnPointRequirements spawnPointRequirement; // the spawn point that this enemy wants

    [SerializeField] List<SpawnPointRequirements> possibleSpawnPointRequirements; // a list of possible spawn point requirements for this enemy

    private void Awake()
    {
        playerBodyPartManager = PlayerBodyPartManager.instance;
    }

    private void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        // sort our behaviours
        SortBehaviours();
        // choose our spawn point
        ChooseSpawnPointRequirement();
        // setup renderers for getting hurt and vfx
        SetupRenderers();
        // for anything else we want to add in our inherited classes
        StartExtension();
        // set our stats
        SetLevelStats();
        // check our elemental protections
        ElementalProtectionCheck();
        // if we are already active
        if (activated)
        { StartBehaviours(); }
    }

    private void OnEnable()
    {
        ElementalProtectionCheck();
    }

    // run to generate spawn point requirements
    void ChooseSpawnPointRequirement()
    {
        try
        {
            if (possibleSpawnPointRequirements.Count > 0)
            {
                spawnPointRequirement = possibleSpawnPointRequirements[Random.Range(0, possibleSpawnPointRequirements.Count)];
            }
        }
        catch { Debug.Log("No spawn point requirements to choose from. Using default selection."); }
    }

    // to run our behaviours
    void StartBehaviours()
    {
        // start our behaviours
        if (attackBehaviours.Count > 0)
        StartCoroutine(AttackBehaviourHandler());

        if (movementBehaviours.Count > 0)
        StartCoroutine(MovementBehaviourHandler());
    }

    // we call this in the start of this class so that we can add functionality on the start event of inherited classes
    virtual public void StartExtension() { }

    // we must determine our values from our level
    virtual public void SetLevelStats() { } // every enemyclass must set its own stats

    IEnumerator AttackBehaviourHandler()
    {
        // attackBehaviours.Shuffle();
        // go through each attack
        foreach (EnemyBehaviour behaviour in attackBehaviours)
        {
            behaviour.complete = false;
            // run the behaviour
            if (behaviour.gameObject.activeInHierarchy)
                behaviour.RunMain();
            // wait one fixed update
            yield return new WaitForFixedUpdate();
            // then wait
            yield return new WaitForSecondsRealtime(behaviour.behaviourTime);
            // then set the completion to true
            behaviour.complete = true;
        }
        StartCoroutine(AttackBehaviourHandler());
    }
    

    IEnumerator MovementBehaviourHandler()
    {
        // go through each attack
        foreach (EnemyBehaviour behaviour in movementBehaviours)
        {
            Debug.Log("Running Movement Behaviour");
            if (behaviour.gameObject.activeInHierarchy)
                // run the behaviour
                behaviour.RunMain();
            // then wait
            yield return new WaitForSecondsRealtime(behaviour.behaviourTime);
            
        }
        StartCoroutine(MovementBehaviourHandler());
    }

    // to sort the behaviours that our body uses
    void SortBehaviours()
    {
        if (attackBehaviourParent)
        foreach (Transform behaviour in attackBehaviourParent)
        {
            // add attacks
            attackBehaviours.Add(behaviour.GetComponent<EnemyBehaviour>());
        }

        if (movementBehaviourParent)
        foreach (Transform behaviour in movementBehaviourParent)
        {
            // add attacks
            movementBehaviours.Add(behaviour.GetComponent<EnemyBehaviour>());
        }
    }

    // do we have any elemental protections?
    void ElementalProtectionCheck()
    {
        if (activeElementalProtection == ElementalProtection.explosiveShield)
        {
            // make sure to set our armor amount
            explosiveArmorHP = setExplosiveArmorHP;

            // set armor plating to active
            if (armorPlates.Count > 0)
                foreach (GameObject plate in armorPlates)
                    plate.SetActive(true);
        }

        if (activeElementalProtection == ElementalProtection.energyShield)
        {
            // make sure to set our armor amount
            energyShieldHP = setEnergyShieldHP;
            // set armor plating to active
            if (armorPlates.Count > 0)
                foreach (GameObject plate in energyShields)
                    plate.SetActive(true);
        }
    }

    // runs 60 times per second
    public virtual void FixedUpdate()
    {
        // debug line render code
        if (lineRenderer)
        {
            NavMeshPath path = navMeshAgent.path;
            Vector3[] corners = path.corners;
            // draw lines to show our path
            lineRenderer.SetPositions(corners);
        }
        // process whether we can run our behaviours
        ProcessBehaviourStart();
        // display our stats
        ProcessCanvasDisplay();

        if (kickReset > 0)
            kickReset -= 1;

        if (kickReset <= 0)
            canKickMovement = true;

        // make sure we manage our velocity
        SlowVelocity();
    }

    // run in fixed update to see if we can see the player
    void ProcessBehaviourStart()
    {
        if (!activated)
        {
            RaycastHit hit;
            // draw a linecast from us to the player and see if we can see them
            if (Physics.Linecast(transform.position, PlayerController.instance.transform.position, out hit, Physics.AllLayers, QueryTriggerInteraction.Ignore))
            {
                // if we don't hit anything and we're not activated
                if (hit.transform.tag == "Player" && !activated)
                {   // set ourselves to activated and start our behaviours
                    activated = true;
                    StartBehaviours();
                }
            }
        }
    }

    // called by arena handler
    public void ManualBehaviourStart()
    {
        activated = true;
        StartBehaviours();
    }

    // for when an elemental attack hits us
    virtual public void GetHurt(float damage, ElementalProtection element)
    {
        if (!invincible)
        {
            // add to our stats
            PlayerRunStatTracker.instance.damageDealt += (int)damage;

            // before dealing damage, check to ensure we have no armors
            if (explosiveArmorHP <= 0 && energyShieldHP <= 0)
            {
                health -= (int)damage;
            }

            if (explosiveArmorHP > 0 || energyShieldHP > 0)
            {
                if (element == ElementalProtection.none)
                {
                    explosiveArmorHP -= damage/2;
                    energyShieldHP -= damage/2;
                } 
                
                // damage to non-types
                if (element == ElementalProtection.energyShield)
                {
                    explosiveArmorHP -= damage/3;
                }

                if (element == ElementalProtection.explosiveShield)
                {
                    energyShieldHP -= damage/3;
                }
            }

            if (element == ElementalProtection.explosiveShield)
            {
                explosiveArmorHP -= (int)damage * 4;
                // drop some shield parts to represent how much HP we've lots in the explosion
            }

            // explosion FX

            if (explosiveArmorHP > 0)
            {
                // the shields we're throwing off of the enemy
                List<GameObject> blownPlates = new List<GameObject>();
                // get 3 shields to pop off the enemy
                for (int i = 0; i <= 1; i++)
                {
                    blownPlates.Add(armorPlates[Random.Range(0, armorPlates.Count)]);
                }

                // then
                try
                {
                    foreach (GameObject plate in blownPlates)
                    {
                        // turn on collider
                        plate.GetComponent<Collider>().enabled = true;
                        // unparent and throw the plate
                        plate.transform.Unparent();
                        plate.GetComponent<Rigidbody>().useGravity = true;
                       if (plate.transform.parent == null && Mathf.Abs(plate.GetComponent<Rigidbody>().velocity.x) < 0.001f) 
                            plate.GetComponent<Rigidbody>().AddForce(new Vector3(Random.Range(-1, 2), Random.Range(-1, 2), Random.Range(-1, 2)) * 20f, ForceMode.Impulse);

                    }
                }
                catch
                {
                    // we're out of shields, pop them all off! 
                    foreach (GameObject plate in armorPlates)
                    {
                        // turn on collider
                        plate.GetComponent<Collider>().enabled = true;
                        // unparent and throw the plate
                        plate.transform.Unparent();
                        if (plate.transform.parent == null && Mathf.Abs(plate.GetComponent<Rigidbody>().velocity.x) < 0.001f) 
                            plate.GetComponent<Rigidbody>().AddForce(new Vector3(Random.Range(-1, 2), Random.Range(-1, 2), Random.Range(-1, 2)) * 20f, ForceMode.Impulse);

                    }
                }

            }

            if (explosiveArmorHP <= 0)
            {
                if (armorPlates.Count > 0)
                // we're out of shields, pop them all off! 
                foreach (GameObject plate in armorPlates)
                {
                    if (plate != null)
                    {
                        try // in case this enemy has no shields
                        {
                            // turn on collider
                            plate.GetComponent<Collider>().enabled = true;
                            // unparent and throw the plate
                            plate.transform.Unparent();
                            plate.GetComponent<Rigidbody>().useGravity = true;
                            if (plate.transform.parent == null && Mathf.Abs(plate.GetComponent<Rigidbody>().velocity.x) < 0.001f)
                                plate.GetComponent<Rigidbody>().AddForce(new Vector3(Random.Range(-1, 2), Random.Range(-1, 2), Random.Range(-1, 2)) * 20f, ForceMode.Impulse);
                        }
                        catch { }
                    }
                }
            }

            // energy fx

            if (energyShieldHP <= 0)
            {
                try // in case this enemy has no shields
                {
                    if (energyShields.Count > 0)
                        if (energyShields[0].activeInHierarchy == true)
                            Instantiate(Resources.Load("EnemyElementalEffects/ShockExplosionNoDamage") as GameObject, transform);

                    foreach (GameObject plate in energyShields)
                    {
                        if (plate != null)
                        {
                            plate.SetActive(false);
                        }
                    }
                } catch { }
            }

            if (element == ElementalProtection.energyShield)
            {
                energyShieldHP -= (int)damage * 4;
            }

            // set the intensity of our energy shields to the inverse % of our remaining HP
            try
            {
                foreach (GameObject plate in energyShields)
                {
                    plate.GetComponent<Renderer>().material.SetColor("_EmissionColor", plate.GetComponent<Renderer>().material.GetColor("_EmissionColor") * (2f));
                    // 10% chance to spawn a shield vfx
                    int c = Random.Range(0, 10);
                    if (c > 8)
                    {
                        if (plate.transform.childCount == 0)
                            Instantiate(Resources.Load("EnemyElementalEffects/ShieldBreakingVFX") as GameObject, plate.transform);
                    }
                }
            } catch { }

            // flash
            StartCoroutine(HurtFlash());
            // if we are at 0 health, trigger death
            if (health <= 0)
            {
                // die
                if (!dead)
                    OnDeath();
            }

            // run our get hurt extender
            GetHurtExtension();
        }
    }

    // more accessible get hurt class 
    virtual public void GetHurtExtension()
    {
        // this is blank by default - put any additional gethurt aspects into this method
        // by making this extender we can essentially add lines of code which run when GetHurt() runs
    }

    // dying
    bool dropped; // have we dropped?
    virtual public void OnDeath()
    {
        if (!dead)
        {
            dead = true;
            // spawn our on death fx
            Instantiate(OnDeathFX, transform.position, Quaternion.identity, null);
            // drop our currency 
            try { if (PlayerRageManager.instance.godmoding) Instantiate(dropItem, transform.position, Quaternion.identity, null); } catch { }
            // add our rage to the manager on death
            PlayerRageManager.instance.AddRage(rageAmount);
            // kick our rage 
            if (PlayerRageManager.instance.godmoding == true)
                PlayerGodfeelManager.instance.KickFeel();
            // add our stat
            PlayerRunStatTracker.instance.kills++;
        }
        // destroy the object
        Destroy(gameObject);
    }

    // everything to do with our hurt flash renderer
    [SerializeField] List<Renderer> renderers = new List<Renderer> ();
    List<Material> defaultRendererMaterials = new List<Material> ();
    List<GameObject> allChildren = new List<GameObject> ();
    [HeaderAttribute("-- VFX --")]
    [SerializeField] Material hurtMaterial;
    public GameObject OnDeathFX; // our death explosion

    // full function
    void SetupRenderers()
    {
        // setup our mesh renderers
        GetChildRecursive(gameObject);
        GetAllRenderers();
    }

    // function to get all of our renderers
    void GetAllRenderers()
    {
        // clear renderers
        renderers.Clear();
        defaultRendererMaterials.Clear();
        // get renderers
        for (int i = 0; i < allChildren.Count; i++)
        {
            // add everything to our list of renderers
            if (allChildren[i].GetComponent<MeshRenderer>())
            {
                renderers.Add(allChildren[i].GetComponent<MeshRenderer>());
                defaultRendererMaterials.Add(allChildren[i].GetComponent<MeshRenderer>().material);
            }
        }
    }
    // getting all children recursively
    void GetChildRecursive(GameObject obj)
    {
        // if there is no object, return
        if (null == obj)
            return;

        // for each child in this object, add this object
        foreach (Transform child in obj.transform)
        {
            if (null == child)
                continue;
            //child.gameobject contains the current child you can do whatever you want like add it to an array
            allChildren.Add(child.gameObject);
            GetChildRecursive(child.gameObject);
        }

    }

    // function to be called within all GetHurt() functions to make us flicker
    public IEnumerator HurtFlash() 
    {
        // set all of our renderers to the hurtflash
        foreach (Renderer renderer in renderers)
        {
            // if (renderer.gameObject.transform.root == transform)
            renderer.material = Resources.Load("Hurt Flash", typeof(Material)) as Material;
        }
        // wait about 0.25 of a second
        yield return new WaitForSecondsRealtime(0.06f);
        // set all renderers back to normal
        for (int i = 0; i < renderers.Count; i++)
        {
            renderers[i].material = defaultRendererMaterials[i];
        }

    }

    [HeaderAttribute("-- Canvas Display --")]
    // everything to do with our diegetic canvas
    [SerializeField] CanvasGroup displayGroup;
    [SerializeField] Slider healthSlider; 
    [SerializeField] Slider lerpHealthSlider; 
    [SerializeField] Text healthDisplay, nameDisplay;
    [SerializeField] float lerpSliderSpeed;
    public bool showDisplay // can we show our display?
    {   // when our display is shown, set the alpha to 1
        set { if (displayGroup) displayGroup.alpha = 1; }
    } 

    // run this to display our stats
    void ProcessCanvasDisplay()
    {
        if (displayGroup)
        {
            // make sure we can be visible first
            ProcessCanvasVisibility();
            // manage our sliders
            healthSlider.value = health / maxHealth;
            lerpHealthSlider.value = Mathf.Lerp(lerpHealthSlider.value, healthSlider.value, lerpSliderSpeed * Time.deltaTime);
            // manage our text displays
            healthDisplay.text = "HP " + health + " / " + maxHealth;
            nameDisplay.text = "Lvl " + level + " " + enemyName;
        }
    }

    // for alpha of our canvas group
    void ProcessCanvasVisibility()
    {
        // always be decreasing our alpha if it is more than 0, so that when it is not being set it is being lowered
        if (displayGroup && displayGroup.alpha > 0)
        {
            displayGroup.alpha -= Time.deltaTime;
        }
    }

    // enum for the different effects we can apply
    public enum Effects
    {
        None, 
        Shock, // enemies take damage overtime when shocked
        Slag, // enemies generate more rage when slagged
    }

    public List<Effects> activeEffects = new List<Effects>();

    // apply effects
    internal void ApplyEffect(Effects effect)
    {
        if (!invincible)
        {
            foreach (Effects ef in activeEffects)
            {
                if (ef == effect)
                {
                    return;
                }
            }

            activeEffects.Add(effect);

            // empty statement
            if (effect == Effects.None)
            {

            }

            // shock
            if (effect == Effects.Shock)
            {
                // spawn in the shock zone prefab on us
                // GameObject obj = Resources.Load<GameObject>("EnemyElementalEffects/ShockZone");
                // Instantiate(obj, transform);
            }

            // slag
            if (effect == Effects.Slag)
            {
                // raise the amount of rage we generate
                if (CheckEffect(Effects.Slag) == true)
                {
                    rageModifier *= 4;
                }

                // spawn in the slag zone prefab on us
                GameObject obj = Resources.Load<GameObject>("EnemyElementalEffects/SlagEffect");
                Instantiate(obj, transform.position, Quaternion.identity, gameObject.transform);

            }
        }
    }

    // check for active effects
    public bool CheckEffect(Effects effect)
    {
        if (activeEffects.Contains(effect)) return true;
        else return false;
    }

    // what happens when we get hit?   // how much power do we apply the kick?
    public void ApplyKickMovement(float distancePower, float movementPower, Vector3 forceOrigin)
    {
        if (canKickMovement)
        {

            canKickMovement = false;
            kickReset = 10; // set our reset to 10 frames so that we must kick that frame
            Debug.Log("applying kick with " + distancePower + " " + movementPower + " at " + forceOrigin);

            // apply our kick in the direction from the forceOrigin minus our position by the power
            Vector3 forceDir = (transform.position - forceOrigin).normalized;
            // if we have a movement agent or not
            // if we have a nav mesh agent, set a new movement direction
            if (navMeshAgent)
            {
                /*
                // make sure we setup the reset so that we don't have weird movement functionality
                StartCoroutine(ResetNavMeshStats(navMeshAgent.acceleration, navMeshAgent.speed));
                // where is this coming from?
                Vector3 relativePos = new Vector3(forceDir.x, 0, forceDir.z);

                // fire a raycast in the relative direction until we hit something. set our destination to the X and Z of this point.
                RaycastHit navkickhit; // our hit info
                Physics.Raycast(transform.position + relativePos*2, transform.position - relativePos, out navkickhit, Mathf.Infinity); 

                // our final relative movement point
                Vector3 finalRelaMove = new Vector3(navkickhit.point.x, transform.position.y, navkickhit.point.z);

                // move to our final pos
                navMeshAgent.SetDestination(finalRelaMove + transform.position);
                */

                // set our high accel so that it is instant
                navMeshAgent.acceleration = 10f;
                navMeshAgent.speed = 10f;
                navMeshAgent.velocity = forceDir * distancePower;
            }

            // if we don't have a nav mesh agent, use physics to move this enemy
            if (!navMeshAgent)
            {
                GetComponent<Rigidbody>().AddForce(forceDir * movementPower);
            }
        }
    }

    // make sure we don't fly across the map like crazy
    void SlowVelocity()
    {
        if (navMeshAgent && Mathf.Abs(navMeshAgent.velocity.magnitude) > 0.05f)
        {
            // move to a target zero
            Vector3 targetVel = navMeshAgent.velocity;
            targetVel = Vector3.MoveTowards(targetVel, Vector3.zero, Time.deltaTime * 3f);
            // set our new velocity
            navMeshAgent.velocity = targetVel;
        }
    }

    // coroutine to reset our navmesh stats
    IEnumerator ResetNavMeshStats(float acceleration, float speed)
    {
        yield return new WaitForSeconds(0.5f);
        navMeshAgent.speed = speed;
        navMeshAgent.acceleration = acceleration;   

    }

    // draw our gizmos
    private void OnDrawGizmos()
    {
        // set color to cyan
        Gizmos.color = Color.cyan;
        // draw our navigation mesh destination
        Gizmos.DrawSphere(navMeshAgent.destination, 1f);
    }

}
