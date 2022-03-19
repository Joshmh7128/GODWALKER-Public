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
    [SerializeField] public bool isOpen, unlocked = false; // are we open? are we unlocked?
    [SerializeField] float interactDistance;
    [SerializeField] CombatZone nextCombatZone, pastCombatZone; // our associated combat zone to activate. our past combat zone to see if we can open
    [SerializeField] GameObject openParent, lockedParent, interactionParent; // the parents of our open and closed door parents
    [SerializeField] string targetPastCombatZone; // the combat zone we want to find and add ourselves to
    [SerializeField] MusicController.musicMoods nextMood; // what will the next music mood be?

    private void Awake()
    {
        // if our targetpastcombatzone is not null, add ourselves to it
        if (targetPastCombatZone != "")
        {
            // check if that target past zone exist
            if (GameObject.Find(targetPastCombatZone))
            {
                // add ourselves
                GameObject.Find(targetPastCombatZone).GetComponent<CombatZone>().doorClasses.Add(this);
            } else if (!GameObject.Find(targetPastCombatZone))
            {
                // if we cant find that zone throw an error
                Debug.Log(gameObject.name + " DoorClass is unable to find target past combat zone, do you have to set this in the inspector?");
            }
        }
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
    }

    // Update is called once per frame
    void Update()
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
        } else if ((isOpen == false) && (unlocked == true) && Vector3.Distance(playerTransform.position, transform.position) > interactDistance)
        {
            // remove interaction text
            UpgradeSingleton.Instance.player.InteractableMessageTrigger("Press E to open door", false);
            interactionParent.SetActive(false); // disable interaction parent
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
        if (isOpen && nextCombatZone.combatComplete == false && other.transform.tag == "Player")
        {
            lockedParent.SetActive(true);
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
        Gizmos.DrawWireSphere(transform.position, interactDistance);
    }
}
