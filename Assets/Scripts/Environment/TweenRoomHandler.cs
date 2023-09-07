using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TweenRoomHandler : MonoBehaviour
{
    public static TweenRoomHandler instance; // the current instance of this script

    // this script helps to control the tween rooms that carry the player from scene to scene

    public string targetNextScene; // this is the target scene that this will go to

    bool used; // has this been used?
    [SerializeField] bool canClose, closed; // can we close this room?
    [SerializeField] bool reduceRage; // do we weaken the player?
    [SerializeField] bool doesAdvanceCombatPos; // does this room advance the current generation position?
    [SerializeField] bool requestReset; // do we request a reset on the player generation manager when we use this door?
    public bool ready, moves; // ready?
    [SerializeField] Vector3 startPos, lowerPos; // our start and lower positions

    [SerializeField] GameObject backDoor, frontDoor, frontDoorBlocker, internalElements; // our doors
    [SerializeField] Vector3 frontDoorStart, frontDoorMove; // where we want our front door to move

    private void Start()
    {
        // make this do not destroy
        DontDestroyOnLoad(gameObject);
        // check if the current position is divisible by 3
        Invoke("ChooseDestination", 1f);

        // calculate our positions
        startPos = transform.position;
        lowerPos = startPos - Vector3.up * 100;

    }


    void ChooseDestination()
    {
        // check for a reset
        if (requestReset)
        {
            PlayerGenerationSeedManager.instance.currentRunPos = 0;
            PlayerGenerationSeedManager.instance.currentCombatPos = 0;
        }

        // just get the next room name from the list
        targetNextScene = PlayerGenerationSeedManager.instance.roomNames[PlayerGenerationSeedManager.instance.currentRunPos];

        #region // old switch
        /*
        // check if we have a target scene
        switch (PlayerGenerationSeedManager.instance.currentRunPos)
        {
            case 0:
                targetNextScene = PullRoom();
                break;
            case 1:
                targetNextScene = "Stash Reward no Player";
                break;
            case 2:
                targetNextScene = PullRoom();
                break;
            case 3:
                targetNextScene = "Stash Reward no Player";
                break;
            case 4:
                targetNextScene = PullRoom();
                break;
            case 5:
                targetNextScene = "Shop no Player";
                break;
            case 6:
                targetNextScene = PullRoom();
                break;
            case 7:
                targetNextScene = "Special Reward no Player";
                break;
            case 8:
                targetNextScene = PullRoom();
                break;
            case 9:
                targetNextScene = "Special Reward no Player";
                break;
            case 10:
                targetNextScene = PullRoom();
                break;
            case 11:
                targetNextScene = "Finish";
                break;
            case 12:
                targetNextScene = "Permanent Reward no Player";
                break;

        }
    */
        #endregion
    }

    // pull a room from the room list
    string PullRoom()
    {
        try
        {
            // we pull the 0th index as that will be randomized in the roomnames list for us to work with
            string r = PlayerGenerationSeedManager.instance.roomNames[0];
            PlayerGenerationSeedManager.instance.roomNames.Remove(r);
            return r;
        }
        catch
        {
            // wait and try again
            StartCoroutine(WaitPullRoom());
            return null;
        }
    }

    IEnumerator WaitPullRoom()
    {
        yield return new WaitForSecondsRealtime(0.25f);
        PullRoom();
        yield return true;
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            if (!used)
            {
                used = true;
                gameObject.GetComponent<BoxCollider>().enabled = false;
                MoveToNewScene();
            }

            if (used)
            {                
                // Debug.LogWarning("Use triggered!");
            }
        }
    }

    private void FixedUpdate()
    {
        frontDoor.transform.position = Vector3.MoveTowards(frontDoor.transform.position, frontDoor.transform.position + frontDoorMove, 50 * Time.fixedDeltaTime);

        if (ready && moves)
            transform.position = Vector3.MoveTowards(transform.position, startPos, 10 * Time.fixedDeltaTime);
        if (!ready && moves)
            transform.position = Vector3.MoveTowards(transform.position, lowerPos, 10 * Time.fixedDeltaTime);

        if (!closed) TryClose();
    }

    void TryClose()
    {

        // check if the player has moved 35 units away from the room
        try
        {
            if (canClose && Vector3.Distance(transform.position, PlayerController.instance.transform.position) > 35)
            {
                // then disable our extraneuous elements
                internalElements.SetActive(false);
                // then enable the other door
                frontDoorBlocker.SetActive(true);
                // remove ourselves from the scene
                SceneManager.MoveGameObjectToScene(gameObject, SceneManager.GetActiveScene());
                // close
                closed = true;
            }
        }
        catch { //Debug.Log("No Player");
                }
    }

    void MoveToNewScene()
    {
        // only rooms that have combat advance our combat position!
        if (doesAdvanceCombatPos)
        PlayerGenerationSeedManager.instance.currentCombatPos++;
        PlayerRunStatTracker.instance.roomsCompleted++;
        // then always advance our run pos
        PlayerGenerationSeedManager.instance.currentRunPos++;
        // do not destroy this door!
        DontDestroyOnLoad(gameObject);
        // close back door
        backDoor.SetActive(true);
        // get the difference between us and the player
        Vector3 playerDif = transform.position - PlayerController.instance.transform.position;
        // we always start a 0, 0, so go there
        transform.position = Vector3.zero;
        // move to the player to our position minus their position
        PlayerController.instance.Teleport(Vector3.zero - playerDif - Vector3.up); // we subtract up from this to teleport to the EXACT location they are standing

        // make the player's weapons worse
        if (reduceRage)
            PlayerWeaponManager.instance.ReduceRageMultiplier();

        // perform a delayed load move of the room
        if (!requestReset)
        StartCoroutine(DelayedLoadMove());



        // turn off all tool tips if they are on
        TooltipHandler.instance.SetTooltip(TooltipHandler.Tooltips.none);

        // show our low rage tooltip if any weapon is below 1 rage
        foreach (GameObject weapon in PlayerWeaponManager.instance.weapons)
        {
            // if the weapon is weak
            if (weapon.GetComponent<WeaponClass>().rageMultiplier < 0.7)
            {
                // if we havent shown the tooltip yet
                if (!WeaponQuickInfoHandler.instance.shownHotnessTooltip)
                {
                    WeaponQuickInfoHandler.instance.shownHotnessTooltip = true;
                    // run the tooltip
                    TooltipHandler.instance.SetTooltip(TooltipHandler.Tooltips.juicePop);
                }
            }
        }


        // if we want to reset, request a reset
        if (requestReset)
        {
            targetNextScene = "Permanent Reward no Player";
            StartCoroutine(DelayedLoadMove());
        }
    }

    public IEnumerator DelayedLoadMove()
    {
        yield return new WaitForSecondsRealtime(0.1f);
        // load the new scene
        SceneManager.LoadSceneAsync(targetNextScene, LoadSceneMode.Single);
        yield return new WaitUntil(() => SceneManager.GetActiveScene().name == targetNextScene);
        canClose = true;
        yield return new WaitForSecondsRealtime(0f);        
        // open front door
        frontDoorMove = Vector3.down;
        WeaponQuickInfoHandler.instance.EffectivenessLowered();
        // then move this from the scene
        SceneManager.MoveGameObjectToScene(gameObject, SceneManager.GetActiveScene());
    }

}
