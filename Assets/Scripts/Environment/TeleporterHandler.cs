using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TeleporterHandler : MonoBehaviour
{
    // script exists to teleport player from room to room
    public string nextRoom; // the room we are going to next
    [SerializeField] GameObject visuals; // the parent object of all our teleporter visuals
    string localStartScene; // the scene we started in

    bool used; // have we been used? 

    private void Start()
    {
        // store the scene we just started in
        localStartScene = SceneManager.GetActiveScene().name;

        // make sure we dont destroy this script on load
        DontDestroyOnLoad(this);
        
    }

    // call this when we activate the teleporter
    // BY DEFAULT the teleporter's collider should be off an the visual should be off if we don't want the player to leave!
    public void ActivateTeleporter()
    {
        visuals.SetActive(true);
        gameObject.GetComponent<Collider>().enabled = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            used = true;
            // load the next scene
            UnityEngine.SceneManagement.SceneManager.LoadScene(nextRoom);
            // start our coroutine
            StartCoroutine(SceneCheck());

        }
    }

    // runs every physics update
    private void FixedUpdate()
    {
        SceneChangeCheck();
    }


    void SceneChangeCheck()
    {
        // if we have been used dont run this check
        if (used) return;

        // if at any point the scene changes destroy ourselves
        if (localStartScene != SceneManager.GetActiveScene().name)
            Destroy(gameObject);

        // if at any point the player is null destroy ourselves
        if (PlayerController.instance == null)
            Destroy(gameObject);
    }

    // teleports the player to 0,0,0 and makes sure we don't destroy until we are in the next scene
    IEnumerator SceneCheck()
    {
        // wait until the scene loads
        yield return new WaitUntil(() => UnityEngine.SceneManagement.SceneManager.GetActiveScene().name == nextRoom);

        PlayerUIManager.instance.fadeCanvasGroup.alpha = 1;

        // teleport the player
        PlayerController.instance.Teleport(Vector3.zero, Vector3.zero);

        // lower effectiveness
        PlayerWeaponManager.instance.ReduceEffectiveness();
        
        // destroy ourselves
        Destroy(gameObject);
    }

}
