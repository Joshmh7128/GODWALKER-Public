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
    public float health, maxHealth;

    // our behaviours
    [HideInInspector] public List<EnemyBehaviour> allBehaviours;
    List<EnemyBehaviour> attackBehaviours = new List<EnemyBehaviour>();
    List<EnemyBehaviour> movementBehaviours = new List<EnemyBehaviour>();
    bool activated = false;
    // our agent
    [HideInInspector] public NavMeshAgent navMeshAgent;

    private void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        // sort our behaviours
        SortBehaviours();
        // setup renderers for getting hurt and vfx
        SetupRenderers();
    }

    // to run our behaviours
    void StartBehaviours()
    {
        // start our behaviours
        StartCoroutine(AttackBehaviourHandler());
        StartCoroutine(MovementBehaviourHandler());
    }

    IEnumerator AttackBehaviourHandler()
    {
        // go through each attack
        foreach (EnemyBehaviour behaviour in attackBehaviours)
        {
            // run the behaviour
            behaviour.RunMain();
            // then wait
            yield return new WaitForSecondsRealtime(behaviour.behaviourTime + Random.Range(-behaviour.behaviourTimeRand, behaviour.behaviourTimeRand));
        }

        StartCoroutine(AttackBehaviourHandler());
    }
    
    IEnumerator MovementBehaviourHandler()
    {
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
        foreach (EnemyBehaviour behaviour in allBehaviours)
        {
            if (behaviour.type == EnemyBehaviour.BehaviourType.attack)
            { attackBehaviours.Add(behaviour); }

            if (behaviour.type == EnemyBehaviour.BehaviourType.movement)
            { movementBehaviours.Add(behaviour); }
        }
    }

    // runs 60 times per second
    private void FixedUpdate()
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

    // getting hurt and dying
    virtual public void GetHurt(float damage)
    {
        health -= (int)damage;
        // flash
        StartCoroutine(HurtFlash());
        // if we are at 0 health, trigger death
        if (health <= 0)
        {
            // die
            OnDeath();
        }

        // run our get hurt extender
        GetHurtExtension();
    }

    // more accessible get hurt class 
    virtual public void GetHurtExtension()
    {
        // this is blank by default - put any additional gethurt aspects into this method
        // by making this extender we can essentially add lines of code which run when GetHurt() runs
    }

    // dying
    virtual public void OnDeath()
    {
        // spawn our on death fx
        Instantiate(OnDeathFX, transform.position, Quaternion.identity, null);
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
        set { displayGroup.alpha = 1; }
    } 

    // run this to display our stats
    void ProcessCanvasDisplay()
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

    // for alpha of our canvas group
    void ProcessCanvasVisibility()
    {
        // always be decreasing our alpha if it is more than 0, so that when it is not being set it is being lowered
        if (displayGroup.alpha > 0)
        {
            displayGroup.alpha -= Time.deltaTime;
        }
    }

    // for death

}
