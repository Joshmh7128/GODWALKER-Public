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
    public bool activated = false, complete = false;
    public List<GameObject> activeEnemies; // the active enemies in the room
    [SerializeField] Transform challengeBubble; // grow this on activation and shrink this on end
    public float bubbleGrowthRate, bubbleTargetSize, bubbleMaxSize; 

    [SerializeField] string challengeType, difficultyLevel, reward, fullInfo; // our info strings

    private void Start()
    {
        // set our info text correctly for this challenge
        fullInfo = challengeType + "\n" + difficultyLevel + "\n" + reward;
        // set that to the text on the canvas
        infoText.text = fullInfo;
    }

    private void FixedUpdate()
    {
        if (playerController == null) { playerController = UpgradeSingleton.Instance.player; }
        if (playerTransform == null) { playerTransform = playerController.gameObject.transform; }
        if (player == null) { player = ReInput.players.GetPlayer(0); }

        // check if the player can activate us
        if (Vector3.Distance(playerTransform.position, transform.position) < activationDistance)
        {
            // if they hold E increase the value of our slider from 0 to 1
            if (player.GetButton("ActionE") && !activated)
            {
                activationValue += activationRate;
                activationSlider.value = activationValue;

                if (activationValue >= 1)
                { 
                    Activate();
                    activated = true;
                }
            } else if (!player.GetButton("ActionE") && !activated && activationValue > 0)
            {
                activationValue -= activationRate;
                activationSlider.value = activationValue;
            }
        }

        // scale our challenge bubble accordingly
        if (challengeBubble.localScale.x != bubbleTargetSize)
        {
            if (challengeBubble.localScale.x < bubbleTargetSize)
            { challengeBubble.localScale += new Vector3(bubbleGrowthRate, bubbleGrowthRate, bubbleGrowthRate); }

            if (challengeBubble.localScale.x > bubbleTargetSize)
            { challengeBubble.localScale -= new Vector3(bubbleGrowthRate, bubbleGrowthRate, bubbleGrowthRate); }
        }
    }

    public abstract void Activate();
    public abstract void UpdateInfo(string optionalInfo);
    public abstract void EndChallenge();

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, activationDistance/2);
    }
}
