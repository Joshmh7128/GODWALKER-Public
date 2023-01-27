using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TweenRoomHandler : MonoBehaviour
{
    // this script helps to control the tween rooms that carry the player from scene to scene

    public string targetScene; // this is the target scene that this will go to

    bool used; // has this been used?

    [SerializeField] GameObject backDoor, frontDoor; // our doors
    Vector3 frontDoorMove; // where we want our front door to move

    private void Start()
    {
        frontDoorMove = frontDoor.transform.position;
        // make this do not destroy
        DontDestroyOnLoad(gameObject);
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            if (!used)
            {
                used = true;
                MoveToNewScene();
            }
        }
    }

    public void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            if (used)
            {
                
            }
        }
    }

    private void FixedUpdate()
    {
        frontDoor.transform.position = Vector3.MoveTowards(frontDoor.transform.position, frontDoorMove, 50 * Time.fixedDeltaTime);
    }

    void MoveToNewScene()
    {
        // close back door
        backDoor.SetActive(true);
        // get the difference between us and the player
        Vector3 playerDif = transform.position - PlayerController.instance.transform.position;
        // load the new scene
        SceneManager.LoadScene(targetScene);
        // we always start a 0, 0, so go there
        transform.position = Vector3.zero;
        // move to the player to our position minus their position
        PlayerController.instance.Teleport(Vector3.zero - playerDif - Vector3.up); // we subtract up from this to teleport to the EXACT location they are standing
        // open front door
        frontDoorMove = frontDoor.transform.position;
        frontDoorMove = frontDoorMove - new Vector3(0, 50, 0);
    }

}
