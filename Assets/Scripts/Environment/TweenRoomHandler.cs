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

    [SerializeField] bool doesAdvance; // does this room advance the current generation position?

    [SerializeField] GameObject backDoor, frontDoor, internalElements; // our doors
    Vector3 frontDoorMove; // where we want our front door to move

    private void Start()
    {
        // make this do not destroy
        DontDestroyOnLoad(gameObject);
        frontDoorMove = frontDoor.transform.position;
        // check if the current position is divisible by 3
        ChooseDestination();
    }

    void ChooseDestination()
    {
        // check if we have a target scene
        switch (PlayerGenerationSeedManager.instance.currentPos)
        {
            case 0:
                targetNextScene = "Stash Reward";
                break;
            case 1:
                targetNextScene = "Area 1 Concept Map 3 no Player";
                break;
            case 2:
                targetNextScene = "Stash Reward";
                break;
            case 3:
                targetNextScene = "Area 1 Concept Map 4 no Player";
                break;
            case 4:
                targetNextScene = "Special Reward";
                break;
            case 5:
                targetNextScene = "Area 1 Concept Map 5 no Player";
                break;            
            case 6:
                targetNextScene = "Finish";
                break;
        }
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
            if (used)
            {
                // prepare this to be unloaded
                SceneManager.MoveGameObjectToScene(gameObject, SceneManager.GetActiveScene());
                // then disable our extraneuous elements
                internalElements.SetActive(false);
            }
        }
    }

    private void FixedUpdate()
    {
        frontDoor.transform.position = Vector3.MoveTowards(frontDoor.transform.position, frontDoorMove, 50 * Time.fixedDeltaTime);
    }

    void MoveToNewScene()
    {
        // advance current pos
        if (doesAdvance)
        PlayerGenerationSeedManager.instance.currentPos++;

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
        StartCoroutine(DelayedLoadMove());
    }

    public IEnumerator DelayedLoadMove()
    {
        yield return new WaitForSecondsRealtime(0.5f);
        // load the new scene
        SceneManager.LoadSceneAsync(targetNextScene);
        yield return new WaitForSecondsRealtime(1f);
        frontDoorMove = frontDoorMove - new Vector3(0, 50, 0);
        // then move this from the scene
        SceneManager.MoveGameObjectToScene(gameObject, SceneManager.GetActiveScene());
    }

}
