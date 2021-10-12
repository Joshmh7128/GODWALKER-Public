using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Rendering;
using UnityEngine.Rendering.PostProcessing;
using Rewired;

public class PlayerController : MonoBehaviour
{
    [Header("Controller Values")]
    // controller values
    [SerializeField] CharacterController characterController; // our character controller
    public CameraScript cameraScript;           // our camera script
    [SerializeField] float moveSpeed;           // how fast can we move?
    [SerializeField] float gravity;             // gravity in the environment
    [SerializeField] Transform playerHead;      // for which way we are facing
    [SerializeField] Transform rightGunTip;     // for firing shots
    [SerializeField] Transform leftGunTip;
    [SerializeField] Transform diegeticAimTarget;
    [SerializeField] bool rightArm = true;      // if true, shoot from right arm. if false, shoot from left arm. 
    public int ammoAmount;                      // how much ammo we currently have
    public int mineralAmount;                   // mineral amount
    public int gemAmount;                       // gem amount
    public int ammoMax;                         // how much ammo we can carry at one time
    public int mineralMax;                      // mineral carry space
    public int gemMax;                          // gem carry space
    public int playerHP;                        // the player's health
    public int playerMaxHP;                     // the player's max health
    public int bugPartAmount;                   // how many bug parts do we have?
    public int gemUpgradeCost;
    public int mineralUpgradeCost;
    public int ammoUpgradeCost;
    [SerializeField] Transform treadsParent;    // the parent of our treads
    public bool canMove = true;
    public bool canFire = true;

    // referenced prefabs and objects
    [SerializeField] GameObject playerBullet;   // bullet prefab
    [SerializeField] Slider ammoSlider;         // our ammo slider
    [SerializeField] Text ammoAmountText;       // our ammo amount in text   
    [SerializeField] Text ammoMaxText;          // our ammo amount in text   
    [SerializeField] Slider mineralSlider;      // our mineral slider
    [SerializeField] Text mineralAmountText;    // our mineral amount in text   
    [SerializeField] Text mineralMaxText;       // our mineral amount in text   
    [SerializeField] Slider gemSlider;          // our gem slider
    [SerializeField] Text gemAmountText;        // our gem amount in text   
    [SerializeField] Text gemMaxText;           // our gem amount in text   
    [SerializeField] Slider hpSlider;           // our hp slider
    [SerializeField] Slider hpSliderDiegetic;   // our diegetic hp slider
    [SerializeField] Text hpAmountText;         // our hp amount in text
    [SerializeField] Text hpMaxText;            // our hp max in text
    [SerializeField] Text bugAmountText;        // bug amount text display
    [SerializeField] Text bugMaxText;           // bug amount text display

    // diegetic UI we're modifying in this script
    [SerializeField] CanvasGroup objectiveCanvas; // our objective canvase
    [SerializeField] Text currentObjective; // our current objective
    float objectiveAlphaChange; // how much should our alpha be changing?
    public bool objectiveShowing; // is our objective showing?
    public string objectiveCurrentMessage; // is our objective showing?
    [SerializeField] GameObject tabIndicator; // our tab text indicating you can show the objective

    // non-diegetic UI elements we're modifying
    [SerializeField] CanvasGroup hurtCanvas; // our hurt canvas
    [SerializeField] CanvasGroup deathCanvas; // our death canvas
    [SerializeField] CanvasGroup fadeCanvas; // our death canvas

    // visual effects
    public bool canDistort; // should we distort the image?
    float distortRate = 4; // what rate should we distort the image?
    public PostProcessVolume postProcessVolume; // our post process volume

    // movement and input
    Player player;
    Vector3 move;
    Vector3 moveH;
    Vector3 moveV;

    // interaction spot
    public Transform interactionCameraPos;
    public GameObject interactionMouse;
    public bool canInteract;

    // our drop pod
    [SerializeField] Transform dropPodTransform;
    [SerializeField] DroppodManager dropPodManager;

    public enum weaponTypes
    {
        Pistols,
        Rifle
    }
    [Header("Weapons")]
    public weaponTypes currentWeapon; // what is our current weapon?
    [SerializeField] GameObject pistolCosmeticModel; // pistol cosmetic
    [SerializeField] GameObject rifleCosmeticModel; // rifle cosmetic
    [SerializeField] GameObject rifleReticle; // reticle associated with the rifle
    [SerializeField] GameObject rifleReticleRing; // reticle ring associated with the rifle
    [SerializeField] Transform rifleTip; // where our shots originate
    [SerializeField] GameObject cubePuff; // our bullet cube particle effect
    [SerializeField] GameObject rifleMuzzleFlash; // the rifle muzzle flash
    public float shotCoolDown; // the amount of time until we can fire again

    // artifact upgradess
    bool isInvincible;
    bool isMitoInvincible;
    bool autoShieldCoroutineRunning;
    bool mitoShieldCoroutineRunning;
    float autoShieldTime; // the amount of time our autoshield engages and the amount of time it takes to cool down
    [Header("Artifact Upgrades")]
    [SerializeField] Text artifactInfoText; // our artifact info text
    [SerializeField] GameObject autoShield; // our shield
    [SerializeField] GameObject autoShieldCosmetic; // cosmetic item
    [SerializeField] GameObject enemyCam; // our see through camera
    [SerializeField] GameObject mitoZygoteShield; // our 1 hp shield

    // Start is called before the first frame update
    void Start()
    {
        player = ReInput.players.GetPlayer(0);

        if (currentWeapon == weaponTypes.Pistols)
        {
            // make sure we set the model properly
            pistolCosmeticModel.SetActive(true);
            rifleCosmeticModel.SetActive(false);
            rifleReticle.SetActive(false);
        }        
        
        if (currentWeapon == weaponTypes.Rifle)
        {
            // make sure we set the model properly
            pistolCosmeticModel.SetActive(false);
            rifleCosmeticModel.SetActive(true);
            rifleReticle.SetActive(true);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (canMove)
        {
            // declare our motion
            moveV = playerHead.forward * player.GetAxis("Vertical");
            moveH = playerHead.right * player.GetAxis("Horizontal");
            move = new Vector3(moveH.x, 0f, moveH.z) + new Vector3(moveV.x, 0f, moveV.z);
            // rotate our treads
            Vector3 treadDirection = Vector3.RotateTowards(treadsParent.forward, move, 10 * Time.deltaTime, 0f);
            treadsParent.rotation = Quaternion.LookRotation(treadDirection);
            // apply to the character controller
            characterController.Move(move * Time.deltaTime * moveSpeed);
            characterController.Move(new Vector3(0f, gravity, 0f) * Time.deltaTime);
        }
        // display our ammo amount
        ammoAmountText.text = ammoAmount.ToString(); // in text
        ammoMaxText.text = ammoMax.ToString(); // in text
        ammoSlider.value = (float)ammoAmount / (float)ammoMax;        
        // display our mineral amount
        mineralAmountText.text = mineralAmount.ToString(); // in text
        mineralMaxText.text = mineralMax.ToString(); // in text
        mineralSlider.value = (float)mineralAmount / (float)mineralMax;        
        // display our gem amount
        gemAmountText.text = gemAmount.ToString(); // in text
        gemMaxText.text = gemMax.ToString(); // in text
        gemSlider.value = (float)gemAmount / (float)gemMax;
        // displayer our HP amount
        hpAmountText.text = playerHP.ToString(); // in text
        hpMaxText.text = playerMaxHP.ToString(); // in text
        hpSlider.value = (float)playerHP / (float)playerMaxHP;
        hpSliderDiegetic.value = (float)playerHP / (float)playerMaxHP;
        // display our bug part amount
        bugAmountText.text = bugPartAmount.ToString();
        bugMaxText.text = "900";
        // modify our reticle ring
        if (currentWeapon == weaponTypes.Rifle)
        { rifleReticleRing.transform.localScale = new Vector3(shotCoolDown, shotCoolDown, shotCoolDown); }

        // shoot bullets
        if (canFire)
        {
            if (player.GetButton("Fire"))
            {
                // which weapon are we using?

                // pistols
                if (currentWeapon == weaponTypes.Pistols)
                {
                    // make sure we set the model properly
                    pistolCosmeticModel.SetActive(true);
                    rifleCosmeticModel.SetActive(false);
                    rifleReticle.SetActive(false);
                    // check ammo
                    if (ammoAmount > 0 && shotCoolDown <= 0)
                    {
                        // check arm
                        if (rightArm == true)
                        {
                            // spawn bullet
                            GameObject bullet = Instantiate(playerBullet, rightGunTip.position, Quaternion.identity, null);
                            bullet.GetComponent<PlayerBulletScript>().bulletType = PlayerBulletScript.bulletTypes.Projectile;
                            bullet.GetComponent<PlayerBulletScript>().bulletTarget = diegeticAimTarget;
                            rightArm = false;
                            ammoAmount--;
                            // screenshake
                            cameraScript.shakeDuration += 0.08f;
                            // shot cooldown
                            shotCoolDown = 10f;
                        }
                        else if (rightArm == false)
                        {
                            // spawn bullet
                            GameObject bullet = Instantiate(playerBullet, leftGunTip.position, Quaternion.identity, null);
                            bullet.GetComponent<PlayerBulletScript>().bulletType = PlayerBulletScript.bulletTypes.Hitscan;
                            bullet.GetComponent<PlayerBulletScript>().bulletTarget = diegeticAimTarget;
                            rightArm = true;
                            ammoAmount--;
                            // screenshake
                            cameraScript.shakeDuration += 0.08f;
                            // shot cooldown
                            shotCoolDown = 10f;
                        }
                    }
                }

                // rifle
                if (currentWeapon == weaponTypes.Rifle)
                {
                    // rifle is a hitscan weapon with a slower fire rate
                    pistolCosmeticModel.SetActive(false);
                    rifleCosmeticModel.SetActive(true);
                    rifleReticle.SetActive(true);
                    // check ammo
                    if (ammoAmount > 0 && shotCoolDown <= 0)
                    {
                        // shot cooldown
                        shotCoolDown = 15f;
                        // spawn bullet
                        RaycastHit hit = cameraScript.rifleTargetHit;
                        if (hit.transform != null)
                        {
                            // use ammo
                            ammoAmount--;
                            if (hit.transform.CompareTag("Breakable"))
                            {
                                // anything with the Breakable tag will be a chunk and have a BreakableBreak function
                                hit.transform.gameObject.GetComponent<BreakableChunk>().BreakableBreak();
                                Instantiate(cubePuff, hit.point, Quaternion.Euler(new Vector3(0, 0, 0)), null);
                                // screenshake
                                cameraScript.shakeDuration += 0.08f;
                                // muzzle flash effect
                                rifleMuzzleFlash.SetActive(true);
                                rifleMuzzleFlash.transform.rotation = Quaternion.Euler(0, 0, Random.Range(0, 360));
                            }

                            if (hit.transform.CompareTag("Environment"))
                            {
                                Instantiate(cubePuff, hit.point, Quaternion.Euler(new Vector3(0, 0, 0)), null);
                                // screenshake
                                cameraScript.shakeDuration += 0.08f;
                                // muzzle flash effect
                                rifleMuzzleFlash.SetActive(true);
                                rifleMuzzleFlash.transform.rotation = Quaternion.Euler(0, 0, Random.Range(0, 360));
                            }

                            if (hit.transform.CompareTag("Enemy"))
                            {
                                hit.transform.gameObject.GetComponent<EnemyClass>().TakeDamage(1);
                                Instantiate(cubePuff, hit.point, Quaternion.Euler(new Vector3(0, 0, 0)), null);
                                // screenshake
                                cameraScript.shakeDuration += 0.08f;
                                // muzzle flash effect
                                rifleMuzzleFlash.SetActive(true);
                                rifleMuzzleFlash.transform.rotation = Quaternion.Euler(0, 0, Random.Range(0, 360));
                            }
                        }
                        // if we do not hit anything
                        if (hit.transform != null)
                        {
                            // use ammo
                            ammoAmount--;
                            // screenshake
                            cameraScript.shakeDuration += 0.08f;
                            // muzzle flash effect
                            rifleMuzzleFlash.SetActive(true);
                            rifleMuzzleFlash.transform.rotation = Quaternion.Euler(0, 0, Random.Range(0, 360));
                        }
                    }
                
                    if (ammoAmount <= 0)
                    {
                        StartCoroutine(ObjectivePanelHandler("OUT OF AMMO! COLLECT OR RETRIEVE MORE"));
                    }
                }
            }
        }
        // reload scene for dev purposes
        if (Input.GetKeyDown(KeyCode.F4))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name, LoadSceneMode.Single);
        }

        // tab press to show and update objective panel
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if (!objectiveShowing)
            {
                // run our panel coroutine
                StartCoroutine(ObjectivePanelHandler("Collect Resources. Launch Ship when able."));
            }

            if (artifactInfoText.color.a == 1)
            { artifactInfoText.color = new Color(255, 255, 255, 0); }
            else if (artifactInfoText.color.a == 0)
            { artifactInfoText.color = new Color(255, 255, 255, 1); }
        }

        // can we interact?
        if (canInteract)
        {
            if (player.GetButtonDown("SpacePress"))
            {
                cameraScript.transform.position = interactionCameraPos.position;
                cameraScript.transform.rotation = interactionCameraPos.rotation;
            }
        }

        // lose condition
        if (playerHP <= 0)
        {
            // stop movement 
            canMove = false;
            cameraScript.canLook = false;
            // display death canvas
            deathCanvas.alpha = 1;
            // if we press space, emergency teleport back to base (load primer)
            if (Input.GetKeyDown(KeyCode.Space))
            {
                // display death canvas
                deathCanvas.alpha = 0;
                // drop all resources
                mineralAmount = 0;
                gemAmount = 0;
                bugPartAmount = 0;
                ammoAmount = 0;
                dropPodManager.mineralAmount = 0;
                dropPodManager.gemAmount = 0;
                dropPodManager.ammoAmount = 0;
                dropPodManager.bugPartAmount = 0;
                // fade out
                fadeCanvas.alpha = 1;
                // reset position
                transform.position = new Vector3(0, 3.5f, 0);
                // reset drop pod position. everything else should be handled by the drop pod once it realized we are in the hub.
                dropPodTransform.position = new Vector3(0, 0, 0);
                // set droppod target position to 0 out
                dropPodManager.ourPlatform.targetPos = new Vector3(0, 0, 0);
                // let the player move
                canMove = true;
                cameraScript.canLook = true;
                // load primer
                SceneManager.LoadScene("RePrimer", LoadSceneMode.Single);
                // set player HP to 1
                playerHP = playerMaxHP;
            }

        }

        // artifact upgrades
        if (UpgradeSingleton.Instance.autoShieldDuration > 0 && autoShieldCosmetic.activeInHierarchy == false)
        {
            autoShieldCosmetic.SetActive(true);
        }
    }

    // fixed update is called once per frame
    private void FixedUpdate()
    {
        // update objective panel
        currentObjective.text = objectiveCurrentMessage;

        // set current weapon
        if (currentWeapon == weaponTypes.Pistols)
        {
            // make sure we set the model properly
            pistolCosmeticModel.SetActive(true);
            rifleCosmeticModel.SetActive(false);
            rifleReticle.SetActive(false);
        }

        if (currentWeapon == weaponTypes.Rifle)
        {
            // make sure we set the model properly
            pistolCosmeticModel.SetActive(false);
            rifleCosmeticModel.SetActive(true);
            rifleReticle.SetActive(true);
        }

        // make sure we set our playercontroller properlly
        if (UpgradeSingleton.Instance.player == null)
        {  
            UpgradeSingleton.Instance.player = this;
            UpdateArtifactInfoUI();
        }

        // distortion effect
        if (canDistort)
        {
            if (postProcessVolume.profile.GetSetting<LensDistortion>().intensity.value < 100)
            {
                postProcessVolume.profile.GetSetting<LensDistortion>().intensity.value += distortRate;
            }
        }        
        
        if (!canDistort)
        {
            if (postProcessVolume.profile.GetSetting<LensDistortion>().intensity.value > 0)
            {
                postProcessVolume.profile.GetSetting<LensDistortion>().intensity.value -= distortRate;
            }
        }

        // calculate our costs for upgrading storage
        gemUpgradeCost = (int)Mathf.Round((gemMax / 3) * 1.8f);
        mineralUpgradeCost = (int)Mathf.Round((mineralMax / 3) * 1.8f);
        ammoUpgradeCost = (int)Mathf.Round((ammoMax / 3) * 1.8f);

        // can we decrease our objective alpha?
        objectiveCanvas.alpha += objectiveAlphaChange;

        // decrease hurt alpha
        Mathf.Clamp(hurtCanvas.alpha, 0, 1);
        hurtCanvas.alpha += -0.1f;

        // make sure we aren't out of bounds
        Mathf.Clamp(playerHP, 0, playerMaxHP);

        // muzzle flash effect
        if (rifleMuzzleFlash.activeInHierarchy)
        { rifleMuzzleFlash.SetActive(false); }

        // shot cooldown
        if (shotCoolDown > 0)
        {
            shotCoolDown -= 1;
        }

        // see through camera artifact
        if (UpgradeSingleton.Instance.tetralightVisionDuration > 0)
        {
            UpgradeSingleton.Instance.tetralightVisionDuration--;
            enemyCam.SetActive(true);
        }

        if (UpgradeSingleton.Instance.tetralightVisionDuration <= 0)
        {
            enemyCam.SetActive(false);
        }

        // 1 hp shield from bug part drops
        if (UpgradeSingleton.Instance.mitoZygoteDuration > 0)
        {
            if (!mitoShieldCoroutineRunning)
            {
                StartCoroutine(MitoZygoteShieldTimer());
            }
            mitoZygoteShield.SetActive(true);
            isMitoInvincible = true;
        } else if (UpgradeSingleton.Instance.mitoZygoteDuration <= 0)
        {
            mitoZygoteShield.SetActive(false);
            isMitoInvincible = false;
        }

    }

    // if we gain life, positive number, if we lose life, negative number
    public void AddHP(int HP)
    {
        // is this number positive or negative?
        if ((HP < 0) && !((isInvincible == true) || (isMitoInvincible == true)))
        {
            // we took damage, set hurt canvas to 1
            hurtCanvas.alpha = 1;

            // check for autoshield

            if (UpgradeSingleton.Instance.autoShieldDuration > 0)
            {
                if (!autoShieldCoroutineRunning)
                StartCoroutine(AutoShieldTimer(UpgradeSingleton.Instance.autoShieldDuration));
            }

            // mod it
            playerHP += HP;
        }   // if we have a mitozygote shield deal damage to it (deactivate it)
        
        
        if ((HP < 0) && (isInvincible == false) && (isMitoInvincible == true))
        {
            isMitoInvincible = false;
            mitoZygoteShield.SetActive(false);
            UpgradeSingleton.Instance.mitoZygoteDuration = 0;
        }


    }

    IEnumerator AutoShieldTimer(float shieldTime)
    {
        autoShieldCoroutineRunning = true;
        autoShield.SetActive(true);
        isInvincible = true;
        yield return new WaitForSeconds(shieldTime);
        autoShield.SetActive(false);
        isInvincible = false;
        autoShieldCoroutineRunning = false;
    }

    IEnumerator MitoZygoteShieldTimer()
    {
        mitoShieldCoroutineRunning = true;
        yield return new WaitForSeconds(UpgradeSingleton.Instance.mitoZygoteDuration);
        if (UpgradeSingleton.Instance.mitoZygoteDuration > 0)
        { UpgradeSingleton.Instance.mitoZygoteDuration--;  }
        mitoShieldCoroutineRunning = false;
    }

    public IEnumerator ObjectivePanelHandler(string customText)
    {   // manage our objective related UI
        objectiveCurrentMessage = customText;
        // make in to its own local string so we can modify it in between messages if needed
        currentObjective.text = objectiveCurrentMessage;
        tabIndicator.SetActive(false);
        objectiveShowing = true;
        objectiveAlphaChange = 0;
        objectiveCanvas.alpha = 1;
        yield return new WaitForSeconds(5f);
        objectiveAlphaChange = -0.1f;
        objectiveShowing = false;
        // only show our tab indicator if we are back in the hub
        if (SceneManager.GetActiveScene().name == "Hub")
        {
            tabIndicator.SetActive(true);
        }
    }

    // update our objective UI
    public void UpdateObjectiveUI()
    {
        // what scene are we in?
        if (SceneManager.GetActiveScene().name == "Hub")
        {
            currentObjective.text = "Dropship ready. Board and press Space to launch.";
        }

        // what scene are we in?
        if (SceneManager.GetActiveScene().name == "Advanced Generation")
        {
            currentObjective.text = "Collect Resources. When ready to continue, launch Dropship.";
        }
    }

    // set our artifact info text
    public void UpdateArtifactInfoUI()
    {
        // if there are upgrades
        if (UpgradeSingleton.Instance != null)
        {
            if (UpgradeSingleton.Instance.artifactInfoList.Count > 0)
            {
                // our local string
                string totalInfo = "";
                // for each artifact info we have, add it to the final and a line ending
                foreach (string info in UpgradeSingleton.Instance.artifactInfoList)
                {
                    totalInfo = totalInfo + info + "\n";
                }
                // set info
                artifactInfoText.text = totalInfo;
            } else if (UpgradeSingleton.Instance.artifactInfoList.Count <= 0)
            {
                // our local string
                string totalInfo = "";
                // set info
                artifactInfoText.text = totalInfo;
            }
        }
    }
}
