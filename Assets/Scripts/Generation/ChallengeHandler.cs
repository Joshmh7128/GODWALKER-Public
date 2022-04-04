using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Rewired;

public abstract class ChallengeHandler : MonoBehaviour
{
    // class exists for other challenges to inherit from
    public Text infoText;
    [SerializeField] Slider activationSlider; // our display for activating the challenge
    [SerializeField] float activationDistance, activationValue, activationRate = 0.008f; // how far away can the player activate?
    [SerializeField] PlayerController playerController;
    [SerializeField] Transform playerTransform;
    [SerializeField] Player player;
    [SerializeField] bool activated = false;
    public List<GameObject> activeEnemies; // the active enemies in the room

    private void Start()
    {
        playerController = UpgradeSingleton.Instance.player;
        player = ReInput.players.GetPlayer(0);
        playerTransform = playerController.gameObject.transform;
    }

    private void FixedUpdate()
    {
        // check if the player can activate us
        if (Vector3.Distance(transform.position, playerTransform.position) < activationDistance)
        {
            // if they hold E increase the value of our slider from 0 to 1
            if (player.GetButton("ActionE") && !activated)
            {
                activationValue += activationRate;
                activationSlider.value = activationValue;

                if (activationValue > 1)
                { 
                    Activate();
                    activated = true;
                }
            }

            // if they let go, reset the actionvalue to 0
            if (player.GetButtonUp("ActionE") && !activated)
            {
                activationValue = 0;
                activationSlider.value = activationValue;
            }
        }
    }

    public abstract void Activate();
    public abstract void EndChallenge();

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, activationDistance);
    }
}
