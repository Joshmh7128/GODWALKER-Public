using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering.PostProcessing;
using Rewired;

public class DroppodManager : MonoBehaviour
{
    bool canLaunch = false;
    bool isFlying = false;
    bool atFlyPos = false;
    [SerializeField] Renderer greenZoneRenderer;
    [SerializeField] Material dimGreen;
    [SerializeField] Material brightGreen;
    [SerializeField] GenerationManager generationManager;
    [SerializeField] Transform playerTrans;
    [SerializeField] PlayerController playerController;
    [SerializeField] Vector3 targetPosGround;
    public Vector3 targetPosGroundNew;
    [SerializeField] Vector3 targetPosFly;
    [SerializeField] Vector3 movementDirection;
    [SerializeField] MovingPlatform ourPlatform;
    [SerializeField] GameObject platformWalls;

    bool canDeposit; // can we deposit minerals in to the drop pod's tanks?
    float depositRate = 5;
    [SerializeField] float gemAmount;
    [SerializeField] float gemMax;
    [SerializeField] float mineralAmount;
    [SerializeField] float mineralMax;
    [SerializeField] float ammoAmount;
    [SerializeField] float ammoMax;

    [SerializeField] Slider ammoSlider;         // our ammo slider
    [SerializeField] Text ammoAmountText;       // our ammo amount in text   
    [SerializeField] Slider mineralSlider;      // our mineral slider
    [SerializeField] Text mineralAmountText;    // our mineral amount in text   
    [SerializeField] Slider gemSlider;          // our gem slider
    [SerializeField] Text gemAmountText;        // our gem amount in text   
    [SerializeField] Slider countdownSlider;           // our hp slider
    [SerializeField] Text countdownSliderAmountText;         // our gp amount in text

    private void Start()
    {
        // start the pod where it needs to start
        targetPosGround = transform.position;
        // make sure the pod stays where it starts
        ourPlatform.targetPos = targetPosGround;
        // set a new finishing point for the pod
        targetPosFly = new Vector3(transform.position.x, transform.position.y+100, transform.position.z);

        // make sure we have our generation manager
        if (generationManager == null)
        {
            generationManager = GameObject.Find("Generation Manager").GetComponent<GenerationManager>();
        }
    }

    private void Update()
    {
        if (canLaunch == true)
        {
            // make our green zone green
            greenZoneRenderer.material = brightGreen;
            // launch
            if (ReInput.players.GetPlayer(0).GetButtonDown("SpacePress"))
            {
                // launch the drop pod
                // gameObject.GetComponent<Animator>().Play("Asteroid Hop");
                if (!isFlying)
                {
                    StartCoroutine("LaunchPod");
                }
            }
        }
        else
        {
            greenZoneRenderer.material = dimGreen;
        }
    }

    // <Vector3.Distance(transform.position, targetPosFly) < 0.05f>

    IEnumerator LaunchPod()
    {
        isFlying = true;
        // disable player movement
        playerController.canMove = false;
        // enable the walls
        platformWalls.SetActive(true);
        // start to move the pod up in to the sky
        ourPlatform.targetPos = targetPosFly;
        // reset the player ammo
        playerTrans.gameObject.GetComponent<PlayerController>().ammoAmount = playerTrans.gameObject.GetComponent<PlayerController>().ammoMax;
        // wait until we are high in the air
        yield return new WaitUntil(() => Vector3.Distance(transform.position, targetPosFly) < 0.05f);
        // trigger the visual effect
        playerController.canDistort = true;
        // wait for the visual effect to finish
        yield return new WaitUntil(() => playerController.postProcessVolume.profile.GetSetting<LensDistortion>().intensity == 100f);
        // when we are hanging in the air, generate the new map
        generationManager.ClearGen();
        // wait so that we don't drop the player by accident
        yield return new WaitForSeconds(1f);
        // trigger the visual effect
        playerController.canDistort = false;
        // change the X and Y positions of the drop pod to the new X and Y of the landing pos
        ourPlatform.targetPos = new Vector3(targetPosGroundNew.x, ourPlatform.transform.position.y, targetPosGroundNew.z);
        // wait until we get there
        yield return new WaitUntil(() => Vector3.Distance(transform.position, new Vector3(targetPosGroundNew.x, ourPlatform.transform.position.y, targetPosGroundNew.z)) < 0.05f);
        // then move down
        yield return new WaitForSeconds(3f);
        // open the walls
        platformWalls.SetActive(false);
        // enable player movement
        playerController.canMove = true;
        // set a new movement position
        ourPlatform.targetPos = targetPosGroundNew;
        // set our new flying position
        targetPosFly = new Vector3(targetPosGroundNew.x, targetPosFly.y, targetPosGroundNew.z);
        // we're done
        isFlying = false;
    }

    private void FixedUpdate()
    {
        if (canDeposit)
        {
            if (playerController.gemAmount > 0)
            {
                playerController.gemAmount -= 1;
                gemAmount += 1;
            }

            if (playerController.mineralAmount > 0)
            {
                playerController.mineralAmount -= 1;
                mineralAmount += 1;
            }

            if (playerController.ammoAmount < playerController.ammoMax && ammoAmount > 0)
            {
                playerController.ammoAmount += 1;
                ammoAmount -= 1;
            }
        }

        // display our ammo amount
        ammoAmountText.text = ammoAmount.ToString(); // in text
        ammoSlider.value = (float)ammoAmount / (float)ammoMax;
        // display our mineral amount
        mineralAmountText.text = mineralAmount.ToString(); // in text
        mineralSlider.value = (float)mineralAmount / (float)mineralMax;
        // display our gem amount
        gemAmountText.text = gemAmount.ToString(); // in text
        gemSlider.value = (float)gemAmount / (float)gemMax;
        // displayer our HP amount
        countdownSliderAmountText.text = "0"; // in text
        countdownSlider.value = 0;
    }

    // when the player enters the green zone
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // playerTrans.SetParent(transform);
            canLaunch = true;
            canDeposit = true;
        }
    }    

    // when the player enters the green zone
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // playerTrans.SetParent(null);
            canLaunch = false;
            canDeposit = false;
        }
    }
}
