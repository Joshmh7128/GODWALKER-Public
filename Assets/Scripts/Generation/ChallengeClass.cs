using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Rewired;

public abstract class ChallengeClass : MonoBehaviour
{
    // class exists for other challenges to inherit from
    [SerializeField] Text infoText;
    [SerializeField] Slider activationSlider; // our display for activating the challenge
    [SerializeField] float activationDistance, activationValue, activationRate = 0.008f; // how far away can the player activate?
    PlayerController playerController;
    Transform playerTransform;
    Player player;

    private void Start()
    {
        playerController = UpgradeSingleton.Instance.player;
        player = ReInput.players.GetPlayer(0);
    }

    private void FixedUpdate()
    {
        // check if the player can activate us
        if (Vector3.Distance(transform.position, playerTransform.position) < activationDistance)
        {
            // if they hold E increase the value of our slider from 0 to 1
            if (player.GetButton("ActionE"))
            {
                activationValue += activationRate;
                activationSlider.value = activationValue;

                if (activationValue > 1)
                {
                    Activate();
                }

            }
        }
    }

    public abstract void Activate();
    public abstract void EndChallenge();
}
