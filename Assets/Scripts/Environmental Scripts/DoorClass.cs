using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;

public class DoorClass : MonoBehaviour
{
    /// this DoorClass is a new kind of doorclass seperate from the original
    /// this one will handle opening basic doors

    [SerializeField] Animator doorAnimator;
    Player player;
    Transform playerTransform;
    [SerializeField] public bool isOpen, unlocked = false, isChallenge = false; // are we open? are we unlocked? are we a challenge?
    bool addedZone = false;                   // have we added ourselves to our past combat zone?
    [SerializeField] float interactDistance, checkDistance;
    [SerializeField] CombatZone nextCombatZone, pastCombatZone; // our associated combat zone to activate. our past combat zone to see if we can open
    [SerializeField] GameObject openParent, lockedParent, interactionParent; // the parents of our open and closed door parents
    [SerializeField] string targetPastCombatZone; // the combat zone we want to find and add ourselves to
    [SerializeField] MusicController.musicMoods nextMood; // what will the next music mood be?
    [SerializeField] BoxCollider doorCheckTrigger; // our door check trigger

    private void Awake()
    {

    }

    private void Start()
    {
        player = ReInput.players.GetPlayer(0);
        playerTransform = UpgradeSingleton.Instance.player.transform;


        // if are door is unlocked by default and is not affected by combat zones, make sure it is unlocked
        if (unlocked)
        { openParent.SetActive(true); }
        else if (!unlocked)
        { lockedParent.SetActive(true); }

        if (nextCombatZone)
        {
            nextCombatZone.doorClasses.Add(this);
        }

        if (pastCombatZone)
        {
            pastCombatZone.doorClasses.Add(this);
        }
    }

    // Update is called once per frame
    void Update()
    {
        // check if we are near enough to interact with the player
        if (Vector3.Distance(playerTransform.position, transform.position) < checkDistance)
        {
            // if we are near and it is unlocked
            if ((isOpen == false) && (unlocked == true) && Vector3.Distance(playerTransform.position, transform.position) < interactDistance)
            {
                // tell the player that they can open the door
                UpgradeSingleton.Instance.player.InteractableMessageTrigger("Press E to open door", true);
                // activate our interactable
                interactionParent.SetActive(true);

                // if we press E to open the door
                if (player.GetButtonDown("ActionE"))
                {
                    isOpen = true; // we're open
                    doorAnimator.Play("QueueDoor"); // animate!
                    if (nextCombatZone)
                    {
                        nextCombatZone.ActivateZone(); // activate the zone
                    }
                    UpgradeSingleton.Instance.player.InteractableMessageTrigger("Press E to open door", false); // remove interaction text
                    interactionParent.SetActive(false); // disable interaction parent
                }
            }
            else if ((isOpen == false) && (unlocked == true) && Vector3.Distance(playerTransform.position, transform.position) > interactDistance)
            {
                interactionParent.SetActive(false); // disable interaction parent
            }
        }

        // if our targetpastcombatzone is not null, add ourselves to it
        if (targetPastCombatZone != "" && addedZone == false)
        {
            // check if that target past zone exist
            if (GameObject.Find(targetPastCombatZone))
            {
                // add ourselves
                GameObject.Find(targetPastCombatZone).GetComponent<CombatZone>().doorClasses.Add(this);
                addedZone = true;
            }
            else if (!GameObject.Find(targetPastCombatZone))
            {
                // if we cant find that zone throw an error
                Debug.Log(gameObject.name + " DoorClass is unable to find target past combat zone, do you have to set this in the inspector?");
            }
        }
    }

    public void DoorMusicTrigger()
    {
        // choose the next music based on what kind of room it is
            if (GameObject.Find("MusicManager"))
                GameObject.Find("MusicManager").GetComponent<MusicController>().MusicMood(nextMood);
    }

    private void OnTriggerEnter(Collider other)
    {
        // if we're open and the player moves through the door, close it behind them
        if (isOpen && other.transform.tag == "Player")
        {
            // check if we have a next combat zone, if we dont we dont have to lock the door
            if (nextCombatZone && nextCombatZone.combatComplete == false || isChallenge)
            lockedParent.SetActive(true);
            doorCheckTrigger.enabled = false; // disable our trigger so that shots do not get blocked by it
        }
    }

    public void Unlock()
    {
        // unlock everything
        lockedParent.SetActive(false); // set our locked parent to false
        openParent.SetActive(true); // set our open parent to true
        unlocked = true; // set the variable correctly
    }

    private void OnDrawGizmos()
    {
        // show our interaction sphere in the editor
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, interactDistance);
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, checkDistance);
    }
}
