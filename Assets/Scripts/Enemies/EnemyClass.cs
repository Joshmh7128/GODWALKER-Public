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

    // our behaviours
    [HideInInspector] public List<EnemyBehaviour> allBehaviours;
    public Transform attackBehaviourParent, movementBehaviourParent;
    [SerializeField] List<EnemyBehaviour> attackBehaviours = new List<EnemyBehaviour>();
    [SerializeField] List<EnemyBehaviour> movementBehaviours = new List<EnemyBehaviour>();
    public bool activated;
    // our agent
    [HideInInspector] public NavMeshAgent navMeshAgent;

    PlayerBodyPartManager playerBodyPartManager;
    private void Awake()
    {
        playerBodyPartManager = PlayerBodyPartManager.instance;
    }

    private void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        // sort our behaviours
        SortBehaviours();
        // setup renderers for getting hurt and vfx
        SetupRenderers();
        // for anything else we want to add in our inherited classes
        StartExtension();
        // set our stats
        SetLevelStats();
        // if we are already active
        if (activated)
        { StartBehaviours(); }
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
            behaviour.RunMain();
            // wait one fixed update
            yield return new WaitForFixedUpdate();
            // then wait
            yield return new WaitForSecondsRealtime(behaviour.behaviourTime + Random.Range(-behaviour.behaviourTimeRand, behaviour.behaviourTimeRand));
            // then set the completion to true
            behaviour.complete = true;
        }
        StartCoroutine(AttackBehaviourHandler());
    }
    
    IEnumerator MovementBehaviourHandler()
    {
        // movementBehaviours.Shuffle();
        // go through each attack
        foreach (EnemyBehaviour behaviour in movementBehaviours)
        {
            // run the behaviour
            behaviour.RunMain();
            // then wait
            yield return new WaitForSecondsRealtime(behaviour.behaviourTime + Random.Range(-behaviour.behaviourTimeRand, behaviour.behaviourTimeRand));
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

    // runs 60 times per second
    public virtual void FixedUpdate()
    {
        // process whether we can run our behaviours
        ProcessBehaviourStart();
        // display our stats
        ProcessCanvasDisplay();
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

    // getting hurt and dying
    virtual public void GetHurt(float damage)
    {
        if (!invincible)
        {
            health -= (int)damage;
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
            // chance to drop a gun?
            int i = Random.Range(0, 100);
            // see if we should drop one
            if (i < lootDropChancePercentage && !dropped)
            {
                dropped = true;
                // access the creator we just built and set its level to our level
                try { Instantiate(dropItem, transform.position, Quaternion.identity, null); } catch { }
                // creator.UpdateItem(); // make sure we update the item since it will not properly show our stats otherwise!
            }

        }
        // destroy the object
        Destroy(gameObject);
    }

    // everything to do with our hurt flash renderer
    List<Renderer> renderers = new List<Renderer> ();
    List<Material> defaultRendererMaterials = new List<Material> ();
    List<GameObject> allChildren = new List<GameObject> ();
    [HeaderAttribute("-- VFX --")]
    [SerializeField] Material hurtMaterial;
    [SerializeField] GameObject OnDeathFX; // our death explosion

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
    virtual public IEnumerator HurtFlash() 
    {
        // set all of our renderers to the hurtflash
        foreach (Renderer renderer in renderers)
        {
            renderer.material = hurtMaterial;
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
        Shock
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
                GameObject obj = Resources.Load<GameObject>("EnemyElementalEffects/ShockZone");
                Instantiate(obj, transform);
                playerBodyPartManager.CallParts("OnShockEffect");
            }

        }
    }

}
