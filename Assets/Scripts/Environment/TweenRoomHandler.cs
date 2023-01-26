using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TweenRoomHandler : MonoBehaviour
{
    // this script helps to control the tween rooms that carry the player from scene to scene

    public string targetScene; // this is the target scene that this will go to

    [SerializeField] GameObject backDoor, frontDoor; // our doors

    private void Start()
    {
        // make this do not destroy
        DontDestroyOnLoad(gameObject);
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            MoveToNewScene();
        }
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
        PlayerController.instance.Teleport(Vector3.zero + playerDif);
        // open front door
        frontDoor.SetActive(false);
    }

}
