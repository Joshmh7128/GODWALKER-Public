using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMenuController : MonoBehaviour
{
    // this script is used to 

    [SerializeField] GameObject playerObject, menuObject; // our player and our menu

    bool playerActive; // is the player active

    private void Start()
    {
        playerActive = true;

        // set active
        SetActive();
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            playerActive = !playerActive;
            SetActive();
        }
    }

    void SetActive()
    {
        playerObject.SetActive(playerActive);
        menuObject.SetActive(!playerActive);

        if (!playerActive) Cursor.lockState = CursorLockMode.None;
        if (playerActive) Cursor.lockState = CursorLockMode.Locked;
    }

}
