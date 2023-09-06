using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fallzone : MonoBehaviour
{
    /// if the player touches our trigger we send them up above us and they fall back on the map
    [SerializeField] float dropHeight; // what height does the player get restored at?

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            // teleport
            PlayerController.instance.Teleport(new Vector3(PlayerController.instance.transform.position.x, dropHeight, PlayerController.instance.transform.position.z));
        }
    }
}
