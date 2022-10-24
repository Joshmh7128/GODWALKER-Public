using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DoorScript : MonoBehaviour
{
    public enum DoorStates
    {
        Unlocked, ToCombat, NeedsKey, Timed, ToNothing, ToCombatUnlocked, ToCombatNeedsKey
    }

    public DoorStates doorState; // the current state of our door

    public bool open = false; //, canOpen, triggerLock, triggerHit, triggerOverride, distanceLock, toCombat, keyLocked; // is this open? can we open it?

    // qualifying bools
    bool playerClose; // is the player close by enough for us to open?
    [SerializeField] bool oneWay; // can the player go back through this door immediately after they pass through it?

    [SerializeField] Animator animator;
    [SerializeField] float interactionDistance = 10f;
    [SerializeField] GameObject openMessage, lockParent, timerCanvasObject, keyMessage;

    [SerializeField] float timeRemaining = 60; // how much time is left?
    [SerializeField] Text timeText; 
    // each door should be associated with 2 rooms at maximum
    [SerializeField] List<ArenaHandler> associatedArenas = new List<ArenaHandler>();

    // handle cosmetics
    [SerializeField] GameObject mainDoorCosmeticParent, specialDoorCosmeticParent;

    private void Start()
    {
        // link ourselves to all associated arenas
        LinkArenas();

        // setup door timed state
        switch (doorState)
        {
            // the basic unlocked door state
            case DoorStates.Unlocked:
                break;

            // doors which lead to the next main combat room
            case DoorStates.ToCombat:
                oneWay = true;
                mainDoorCosmeticParent.SetActive(true);
                break;

            // does this door need a key?
            case DoorStates.NeedsKey:
                keyMessage.SetActive(true);
                specialDoorCosmeticParent.SetActive(true);
                oneWay = false; // we want players to go back from this room
                break;            
                
            // does this door need a key and lead to combat?
            case DoorStates.ToCombatNeedsKey:
                keyMessage.SetActive(true);
                specialDoorCosmeticParent.SetActive(true);
                oneWay = true; // we want players to go back from this room
                break;

            // set up and start out timer
            case DoorStates.Timed:
                timeText.text = timeRemaining.ToString(); // show how much time is left
                specialDoorCosmeticParent.SetActive(true);
                oneWay = false; // we want players to go back from this room
                break;

            // when we want a door to be part of combat, but we don't want it to close
            case DoorStates.ToNothing:
                oneWay = false;
                mainDoorCosmeticParent.SetActive(true);
                break;

            // when a door leads to combat, we want it to be unlocked at the start, and we want it to be one way
            case DoorStates.ToCombatUnlocked:
                oneWay = true;
                doorState = DoorStates.Unlocked;
                mainDoorCosmeticParent.SetActive(true);
                break;
        }
    }

    // link each of our arenas to this door
    void LinkArenas()
    {
        // add ourselves to that arena's list of doors if we are not already in there
        foreach (var arena in associatedArenas)
        {
            if (!arena.doors.Contains(this))
            {
                arena.doors.Add(this);
            }
        }
    }

    private void FixedUpdate()
    {
        ProcessDistance();
    }

    private void Update()
    {
        ProcessInput();
    }

    void ProcessDistance()
    {
        Transform player = PlayerController.instance.transform;
        // can we open?
        if (Vector3.Distance(player.position, transform.position) <= interactionDistance && !open)
        {
            playerClose = true;
            if (doorState == DoorStates.NeedsKey && PlayerStatManager.instance.keyAmount > 0)
                openMessage.SetActive(true);

            if (doorState == DoorStates.Unlocked)
                openMessage.SetActive(true);
            
        }
        else
        {
            playerClose = false;
            if (doorState == DoorStates.NeedsKey && PlayerStatManager.instance.keyAmount > 0)
                openMessage.SetActive(false);

            if (doorState == DoorStates.Unlocked)
                openMessage.SetActive(false);
        }
    }

    // our door-opening input reads
    void ProcessInput()
    {
        // if we can open
        if (Input.GetKeyDown(KeyCode.E) && doorState == DoorStates.Unlocked && open == false && playerClose == true)
        {
            Open();
        }

        // if we're attempting to open with a key
        if (Input.GetKeyDown(KeyCode.E) && doorState == DoorStates.NeedsKey && open == false && playerClose == true && PlayerStatManager.instance.keyAmount > 0)
        {
            PlayerStatManager.instance.keyAmount--;
            Open();
        }

    }

    private void OnTriggerEnter(Collider other)
    {
        // if we have our trigger lock active, lock the door with the trigger
        if (oneWay && other.transform.tag == "Player")
        {
            TriggerLock();
        }
    }

    // when our locking for specifically when the trigger is hit
    void TriggerLock()
    {
        // this happens when the player enters the room
        lockParent.SetActive(true);
    }

    // runs when combat begins
    public void CombatBegin()
    {
        if (doorState == DoorStates.Timed)
        {
            timerCanvasObject.SetActive(true);
            StartCoroutine(Timer());
        }
    }

    IEnumerator Timer()
    {
        yield return new WaitForSecondsRealtime(1f);
        if (timeRemaining > 0)
        {
            timeRemaining -= 1f;
            timeText.text = timeRemaining.ToString(); // show how much time is left
            StartCoroutine(Timer()); // keep counting down
        }
        if (timeRemaining == 0)
        {
            timeText.text = "Locked. You didn't kill them fast enough.";
        }
    }

    // to be run when the door animates and opens
    void Open()
    {
        // open our door
        open = true;
        openMessage.SetActive(false);
        keyMessage.SetActive(false);
        animator.Play("DoorOpening");
        SimpleMusicManager.instance.PlaySong(SimpleMusicManager.MusicMoods.intro);
        // start combat in the correct arena
        foreach (ArenaHandler arena in associatedArenas)
        {
            if (!arena.combatBegun)
            {
                arena.StartCombat();
            }
        }
    }

    // unlock to be used anywhere publicly, this is usually used by an Arena Handler to open all doors after combat
    public void AttemptUnlock()
    {
        // check if this door is timed
        if (doorState == DoorStates.Timed)
        { 
            // did we beat it in time?
            if (timeRemaining > 0)
            {
                doorState = DoorStates.Unlocked;
                lockParent.SetActive(false);
                timerCanvasObject.SetActive(false);
            }
        }   // if this door doesn't need a key, then it is unlocked
        else if (doorState != DoorStates.NeedsKey)
        {
            doorState = DoorStates.Unlocked;
            lockParent.SetActive(false);
        } 
    }

    // locking to be used anywhere publicly
    public void Lock()
    {
        lockParent.SetActive(true);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(transform.position, 1);


        Gizmos.color = Color.magenta;
        if (associatedArenas.Count > 0)
        {
            foreach (ArenaHandler arena in associatedArenas)
            {
                if (arena != null)
                {
                    Gizmos.DrawLine(transform.position, arena.transform.position);
                    Gizmos.color = Color.red;
                    Gizmos.DrawSphere(arena.transform.position, 3);
                }
            }
        }

        if (oneWay)
            Gizmos.DrawCube(transform.position + transform.forward * 5, new Vector3(10, 10, 10));
    }
}
