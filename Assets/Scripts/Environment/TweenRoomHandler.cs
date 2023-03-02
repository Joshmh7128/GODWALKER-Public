using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TweenRoomHandler : MonoBehaviour
{
    public static TweenRoomHandler instance; // the current instance of this script

    // this script helps to control the tween rooms that carry the player from scene to scene

    public string targetNextScene; // this is the target scene that this will go to

    bool used; // has this been used?
    [SerializeField] bool canClose; // can we close this room?
    [SerializeField] bool reduceRage; // do we weaken the player?
    [SerializeField] bool doesAdvanceCombatPos; // does this room advance the current generation position?
    [SerializeField] bool requestReset; // do we request a reset on the player generation manager when we use this door?

    [SerializeField] GameObject backDoor, frontDoor, frontDoorBlocker, internalElements; // our doors
    Vector3 frontDoorMove; // where we want our front door to move

    private void Start()
    {
        // make this do not destroy
        DontDestroyOnLoad(gameObject);
        frontDoorMove = frontDoor.transform.position;
        // check if the current position is divisible by 3
        Invoke("ChooseDestination", 1f);
    }

    void ChooseDestination()
    {
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
                Debug.LogWarning("Use triggered!");
            }
        }
    }

    public void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            if (canClose)
            {
                // then disable our extraneuous elements
                // internalElements.SetActive(false);
                // prepare this to be unloaded
                // SceneManager.MoveGameObjectToScene(gameObject, SceneManager.GetActiveScene());
            }
        }
    }

    private void FixedUpdate()
    {
        frontDoor.transform.position = Vector3.MoveTowards(frontDoor.transform.position, frontDoorMove, 50 * Time.fixedDeltaTime);

        // check if the player has moved 35 units away from the room
        if (canClose && Vector3.Distance(transform.position, PlayerController.instance.transform.position) > 35)
        {
            // then disable our extraneuous elements
            internalElements.SetActive(false);
            // then enable the other door
            frontDoorBlocker.SetActive(true);
        }
    }

    void MoveToNewScene()
    {
        // only rooms that have combat advance our combat position!
        if (doesAdvanceCombatPos)
        PlayerGenerationSeedManager.instance.currentCombatPos++;
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
        // open front door
        frontDoorMove = frontDoor.transform.position;
        // make the player's weapons worse
        if (reduceRage)
            PlayerWeaponManager.instance.ReduceRageMultiplier();

        // perform a delayed load move of the room
        StartCoroutine(DelayedLoadMove());

        // if we want to reset, request a reset
        if (requestReset)
            PlayerGenerationSeedManager.instance.ResetRun();
    }

    public IEnumerator DelayedLoadMove()
    {
        yield return new WaitForSecondsRealtime(0.1f);
        // load the new scene
        SceneManager.LoadSceneAsync(targetNextScene);
        yield return new WaitUntil(() => SceneManager.GetActiveScene().name == targetNextScene);
        canClose = true;
        yield return new WaitForSecondsRealtime(0.1f);
        frontDoorMove = frontDoorMove - new Vector3(0, 50, 0);
        // then move this from the scene
        SceneManager.MoveGameObjectToScene(gameObject, SceneManager.GetActiveScene());
    }

}
