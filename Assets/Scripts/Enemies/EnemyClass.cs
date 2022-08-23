using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public abstract class EnemyClass : MonoBehaviour
{
    // class exists as the baseline for all of our enemies

    // our behaviours
    [HideInInspector] public List<EnemyBehaviour> allBehaviours;
    List<EnemyBehaviour> attackBehaviours = new List<EnemyBehaviour>();
    List<EnemyBehaviour> movementBehaviours = new List<EnemyBehaviour>();

    // our agent
    [HideInInspector] public NavMeshAgent navMeshAgent;

    private void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        // sort our behaviours
        SortBehaviours();
        // start them 
        StartBehaviours();
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
            yield return new WaitForSecondsRealtime(behaviour.behaviourTime);
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
            yield return new WaitForSecondsRealtime(behaviour.behaviourTime);
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

    abstract public void GetHurt();

    // everything to do with our hurt flash renderer
    List<Renderer> renderers = new List<Renderer> ();
    List<Material> defaultRendererMaterials = new List<Material> ();
    List<GameObject> allChildren = new List<GameObject> ();
    [SerializeField] Material hurtMaterial;
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
}
