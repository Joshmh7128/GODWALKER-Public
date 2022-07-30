using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerProjectileScript : MonoBehaviour
{
    [SerializeField] float speed;

    // Update is called once per frame
    void Update()
    {
        // move the bullet
        ProcessMovement();
    }

    void ProcessMovement()
    {
        // go forward, I guess?
        transform.position = transform.position + transform.forward * speed * Time.deltaTime;
    }
}
