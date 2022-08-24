using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorScript : MonoBehaviour
{
    bool open = false, canOpen;
    [SerializeField] Animator animator;
    [SerializeField] float interactionDistance = 10f;
    [SerializeField] GameObject openMessage;


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
        if (Vector3.Distance(player.position, transform.position) <= interactionDistance)
        {
            canOpen = true;
            openMessage.SetActive(true);
        }
        else
        {
            canOpen = false;
            openMessage.SetActive(false);
        }
    }

    void ProcessInput()
    {
        if (Input.GetKeyDown(KeyCode.E) && canOpen && !open)
        {
            openMessage.SetActive(false);
            open = true;
            animator.Play("DoorOpening");
            SimpleMusicManager.instance.PlaySong(SimpleMusicManager.MusicMoods.intro);
        }
    }

}
