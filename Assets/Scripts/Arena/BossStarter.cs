using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossStarter : MonoBehaviour
{
    bool canStart;

    [SerializeField] GameObject bossEnemy, interactMessage;



    private void FixedUpdate()
    {
        // show message
        interactMessage.SetActive(canStart);
        // starting
        if (canStart && Input.GetKey(KeyCode.E))
        {
            // if we start the fight enable the boss and destory this object
            bossEnemy.SetActive(true);
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.tag == "Player")
        {
            canStart = true;
        }
    }
    
    private void OnTriggerExit(Collider other)
    {
        if (other.transform.tag == "Player")
        {
            canStart = false;
        }
    }

}
