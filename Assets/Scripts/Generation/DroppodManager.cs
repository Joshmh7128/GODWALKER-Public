using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
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
    [SerializeField] float bugPartAmount;
    [SerializeField] float bugPartMax;

    [SerializeField] Slider ammoSlider;         // our ammo slider
    [SerializeField] Text ammoAmountText;       // our ammo amount in text   
    [SerializeField] Slider mineralSlider;      // our mineral slider
    [SerializeField] Text mineralAmountText;    // our mineral amount in text   
    [SerializeField] Slider gemSlider;          // our gem slider
    [SerializeField] Text gemAmountText;        // our gem amount in text       
    [SerializeField] Slider bugSlider;          // our gem slider
    [SerializeField] Text bugPartAmountText;        // our gem amount in text   
    [SerializeField] Slider countdownSlider;           // our hp slider
    [SerializeField] Text countdownSliderAmountText;         // our gp amount in text

    // hub management
    [SerializeField] bool inHub; // are we in the hub?
    [SerializeField] int maxTrips; // how many trips total
    [SerializeField] int remainingTrips; // how many trips do we have left?

    // fade ui
    [SerializeField] CanvasGroup fadeCanvasGroup; // our fade canvas group
    [SerializeField] float fadeAmount; // how much are we fading?
    [SerializeField] GameObject hubWarp; 
    [SerializeField] GameObject clusterWarp;

    // hub
    [SerializeField] HubManager hubManager;

    private void Start()
    {
        // start the pod where it needs to start
        targetPosGround = transform.position;
        // make sure the pod stays where it starts
        ourPlatform.targetPos = targetPosGround;
        // set a new finishing point for the pod
        targetPosFly = new Vector3(transform.position.x, transform.position.y+100, transform.position.z);

        // make sure we have our generation manager
        if (generationManager == null && inHub == false)
        {
            generationManager = GameObject.Find("Generation Manager").GetComponent<GenerationManager>();
        }
    }

    private void Update()
    {
        if (hubManager == null)
        {
            hubManager = GameObject.Find("Hub Manager").GetComponent<HubManager>();
        }

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

    IEnumerator LaunchPod()
    {
        isFlying = true;
        // disable player movement
        playerController.canMove = false;
        // enable the walls
        platformWalls.SetActive(true);
        // set a new finishing point for the pod
        targetPosFly = new Vector3(transform.position.x, transform.position.y + 250, transform.position.z);
        // start to move the pod up in to the sky
        ourPlatform.targetPos = targetPosFly;
        // reset the player ammo
        playerTrans.gameObject.GetComponent<PlayerController>().ammoAmount = playerTrans.gameObject.GetComponent<PlayerController>().ammoMax;
        // wait until we are high in the air
        yield return new WaitUntil(() => Vector3.Distance(transform.position, targetPosFly) < 1f);
        // trigger the visual effect
        playerController.canDistort = true;
        // wait for the visual effect to finish
        yield return new WaitUntil(() => playerController.postProcessVolume.profile.GetSetting<LensDistortion>().intensity == 100f);
        // have we loaded in?
        bool loaded = false;
        // check if we are in the hub or not
        if (SceneManager.GetActiveScene().name == "Advanced Generation")
        { inHub = false; }
        // if we are not in the hub, check to make sure we have trips left
        if (remainingTrips < 1)
        {
            // fade and enable hub warp
            hubWarp.SetActive(true);
            clusterWarp.SetActive(false);
            fadeAmount = 0.1f;
            yield return new WaitUntil(() => fadeCanvasGroup.alpha >= 1);
            // load in to the advanced generation scene
            SceneManager.LoadScene("Hub", LoadSceneMode.Single);
            // reset remainingTrips
            remainingTrips = maxTrips;
            // wait until the hub has been loaded 
            yield return new WaitUntil(() => SceneManager.GetActiveScene().name == "Hub");
            // disengage the visual effect
            playerController.canDistort = false;
            // fade out
            hubWarp.SetActive(false);
            clusterWarp.SetActive(false);
            fadeAmount = -0.1f;
            yield return new WaitUntil(() => fadeCanvasGroup.alpha <= 0);
            // move our drop pod up to the top of the spire
            ourPlatform.targetPos = new Vector3(ourPlatform.transform.position.x, 250, ourPlatform.transform.position.z);
            // wait until we are high in the air
            yield return new WaitUntil(() => Vector3.Distance(ourPlatform.transform.position, ourPlatform.targetPos) < 1f);
            // move our player to the top of the spire
            ourPlatform.targetPos = new Vector3(0, 250, 0);
            // wait until we above the spire
            yield return new WaitUntil(() => Vector3.Distance(ourPlatform.transform.position, ourPlatform.targetPos) < 1f);
            // move them down to the ground
            ourPlatform.targetPos = new Vector3(0, 0, 0);
            // wait until we are on the ground
            yield return new WaitUntil(() => Vector3.Distance(ourPlatform.transform.position, new Vector3(0, 0, 0)) < 1f);
            // set inHub
            inHub = true;
            // open the walls
            platformWalls.SetActive(false);
            // enable player movement
            playerController.canMove = true;
            // we're done
            isFlying = false;
            // update our values
            hubManager.hubGemAmount += gemAmount;
            gemAmount = 0;
            hubManager.hubMineralAmount += mineralAmount;
            mineralAmount = 0;
            hubManager.hubBugPartAmount += bugPartAmount;
            bugPartAmount = 0;
            hubManager.dropPodAmmoAmount = ammoAmount;
            // save our progress
            hubManager.SaveProgress();
            // exit coroutine
            yield break;
        }

        // respond accordingly
        if (inHub == true)
        {
            // enable can deposit
            canDeposit = true;
            // fade and enable hub warp
            hubWarp.SetActive(false);
            clusterWarp.SetActive(true);
            fadeAmount = 0.1f;
            yield return new WaitUntil(() => fadeCanvasGroup.alpha >= 1);
            // load in to the advanced generation scene
            SceneManager.LoadScene("Advanced Generation", LoadSceneMode.Single);
            yield return new WaitUntil(() => SceneManager.GetActiveScene().name == "Advanced Generation");
            // get our generation manager
            if (generationManager == null)
            {
                generationManager = GameObject.Find("Generation Manager").GetComponent<GenerationManager>();
            }
            yield return new WaitUntil(() => generationManager != null);
            loaded = true;
        }
        else
        {
            loaded = true;
        }

        // wait until we have loaded
        yield return new WaitUntil(() => loaded == true);
        // when we are hanging in the air, generate the new map
        generationManager.ClearGen();
        // fade out
        hubWarp.SetActive(false);
        clusterWarp.SetActive(false);
        fadeAmount = -0.1f;
        yield return new WaitUntil(() => fadeCanvasGroup.alpha <= 0);
        // trigger the visual effect
        playerController.canDistort = false;
        // change the X and Y positions of the drop pod to the new X and Y of the landing pos
        ourPlatform.targetPos = new Vector3(targetPosGroundNew.x, ourPlatform.transform.position.y, targetPosGroundNew.z);
        // wait until we get there
        yield return new WaitUntil(() => Vector3.Distance(transform.position, new Vector3(targetPosGroundNew.x, ourPlatform.transform.position.y, targetPosGroundNew.z)) < 1f);
        // then move down
        yield return new WaitForSeconds(1f);
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
        // lower remaining trips by 1
        remainingTrips--;
    }

    private void FixedUpdate()
    {
        // player depositing in to drop ship
        if (canDeposit)
        {
            if (playerController.gemAmount > 0)
            {
                playerController.gemAmount--;
                gemAmount++;
            }

            if (playerController.mineralAmount > 0)
            {
                playerController.mineralAmount--;
                mineralAmount++;
            }            
            
            if (playerController.bugPartAmount > 0)
            {
                playerController.bugPartAmount--;
                bugPartAmount++;
            }

            if (playerController.ammoAmount < playerController.ammoMax && ammoAmount > 0)
            {
                playerController.ammoAmount++;
                ammoAmount--;
            }
        }

        // update fade canvas
        fadeCanvasGroup.alpha += fadeAmount;
        // clamp
        Mathf.Clamp(fadeCanvasGroup.alpha, 0, 1);

        // drop ship depositing in to hub
        if ((canDeposit) && (SceneManager.GetActiveScene().name == "Hub"))
        {
            if (gemAmount > 0)
            {
                gemAmount--;
                hubManager.hubGemAmount++;
            }

            if (mineralAmount > 0)
            {
                mineralAmount--;
                hubManager.hubMineralAmount++;
            }
        }
        // display our ammo amount
        ammoAmountText.text = ammoAmount.ToString(); // in text
        ammoSlider.value = ammoAmount / ammoMax;
        // display our mineral amount
        mineralAmountText.text = mineralAmount.ToString(); // in text
        mineralSlider.value = mineralAmount / mineralMax;
        // display our gem amount
        gemAmountText.text = gemAmount.ToString(); // in text
        gemSlider.value = gemAmount / gemMax;        
        // display our bug part amount
        bugPartAmountText.text = bugPartAmount.ToString(); // in text
        bugSlider.value = bugPartAmount / bugPartMax;
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
