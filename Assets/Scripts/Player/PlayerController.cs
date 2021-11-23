using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Rendering;
// using UnityEngine.Rendering.PostProcessing;
using Rewired;

public class PlayerController : MonoBehaviour
{
    #region // Main Gameplay Values
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
    [SerializeField] Transform treadYRotationParent; // used to make our treads slightly rock back and forth
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

    #endregion

    #region // Referenced Prefabs
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
    #endregion
    
    #region // Diegetic UI
    // diegetic UI we're modifying in this script
    [SerializeField] CanvasGroup objectiveCanvas; // our objective canvase
    public Text currentObjective; // our current objective
    float objectiveAlphaChange; // how much should our alpha be changing?
    public bool objectiveShowing; // is our objective showing?
    public string objectiveCurrentMessage; // is our objective showing?
    [SerializeField] GameObject tabIndicator; // our tab text indicating you can show the objective
    #endregion

    #region // Non-diegetic UI
    [Header("- Non Diegetic UI -")]
    // non-diegetic UI elements we're modifying
    [SerializeField] CanvasGroup hurtCanvas; // our hurt canvas
    [SerializeField] CanvasGroup deathCanvas; // our death canvas
    [SerializeField] CanvasGroup fadeCanvas; // our death canvas
    // artifact pickup UI elements
    [SerializeField] CanvasGroup popupCanvas; // our popup canvas
    [SerializeField] Text popupTitle; // our popup title
    [SerializeField] Text popupDesc; // our popup description
    [SerializeField] Image popupImage; // our popup image
    float popupAlphaChange; // our popup alphachange
    // inventory related
    [SerializeField] Transform inventoryCameraPos; // position of cam when we access inventory
    [SerializeField] Transform mainCameraContainer; // position of cam when we access inventory
    bool inventoryOpen = false; // is our inventory open?
    [SerializeField] CanvasGroup inventoryCanvas; // the canvas of our inventory
    [SerializeField] CanvasGroup gameplayUICanvas; // the canvas of our inventory
    [SerializeField] AudioSource inventoryAudioSource; // the inventory audio source
    [SerializeField] AudioClip inventoryOpenAudio;
    [SerializeField] AudioClip inventoryCloseAudio;
    Vector3 previousBodyRotation; // used to make opening and closing the inventory panel more comfortable
    [SerializeField] InventoryArtifactUIHandler inventoryArtifactUIHandler;

    // visual effects
    public bool canDistort; // should we distort the image?
    float distortRate = 4; // what rate should we distort the image?
    // public PostProcessVolume postProcessVolume; // our post process volume
    #endregion

    #region // Player Movement Variables
    // movement and input
    Player player;
    Vector3 move;
    Vector3 moveH;
    Vector3 moveV;
    public bool canJump = true;
    [SerializeField] float jumpVelocity; // how fast can we jump
    [SerializeField] float fallMultiplier;  // how quicky we fall in addition to normal gravity
    [SerializeField] float lowJumpMultiplier;  // how quicky we fall in addition to normal gravity
    [SerializeField] float playerJumpVelocity;  // how quicky we fall in addition to normal gravity
    float gravityValue;                        // real time simulated gravity

    #endregion

    // interaction spot
    public Transform interactionCameraPos;
    public GameObject interactionMouse;
    public bool canInteract;

    #region // Droppod variables
    // our drop pod
    [SerializeField] Transform dropPodTransform;
    [SerializeField] DroppodManager dropPodManager;
    #endregion

    #region // Weapon Management
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
    public float rifleDamage; // rifle damage
    public float pistolDamage; // pistol damage
    #endregion

    #region // Artifact Management
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
    #endregion

    #region // General Audio Related
    [Header("General Audio")]
    [SerializeField] AudioSource fireAudioSource;
    [SerializeField] AudioClip pistolsFireAudioClip;
    [SerializeField] AudioClip rifleFireAudioClip;

    #endregion

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

            // vertical velocity variable
            float verticalVelocity;

            // rotate our treads
            Vector3 treadDirection = Vector3.RotateTowards(treadsParent.forward, new Vector3(move.x, 0, move.z), 10 * Time.deltaTime, 0f);
            treadsParent.rotation = Quaternion.LookRotation(treadDirection);

            // gravity modifications
            if (characterController.isGrounded && !player.GetButton("SpacePress"))
            {
                // jump falling
                gravityValue = gravity * 100;
                // move treads
                treadYRotationParent.localRotation = Quaternion.Euler(new Vector3(0, 0, 0f));
            }
            else if (characterController.isGrounded && player.GetButton("SpacePress") && canJump)
            {
                // jump falling
                gravityValue = gravity * lowJumpMultiplier;
            }
            else if (characterController.velocity.y <= 0 && !characterController.isGrounded && canJump)
            {
                // normal falling
                gravityValue = gravity * fallMultiplier;
                // move treads
                float forwardLean = -14;
                treadYRotationParent.localRotation = Quaternion.Euler(new Vector3(forwardLean, 0, 0f));
            } 
            else if (characterController.velocity.y > 0 && canJump)
            {
                // jump falling
                gravityValue = gravity * lowJumpMultiplier;
                // move treads                
                float forwardLean = 14;
                treadYRotationParent.localRotation = Quaternion.Euler(new Vector3(forwardLean, 0,0f));
            }

            if (characterController.isGrounded && playerJumpVelocity < 0)
            {
                playerJumpVelocity = 0f;

            }

            // jumping
            if (canJump)
            {
                if (player.GetButtonDown("SpacePress") && characterController.isGrounded)
                {
                    playerJumpVelocity += Mathf.Sqrt(jumpVelocity * -3.0f * gravity);
                }
            }

            // total move 
            verticalVelocity = playerJumpVelocity += gravityValue * Time.deltaTime;
            move = new Vector3(moveH.x + moveV.x, verticalVelocity / moveSpeed, moveH.z + moveV.z);

            // apply to the character controller
            characterController.Move(move * Time.deltaTime * moveSpeed);
        }

        #region // UI display
        // display our ammo amount
        ammoAmountText.text = ammoAmount.ToString(); // in text
        ammoMaxText.text = ammoMax.ToString(); // in text
        ammoSlider.value = (float)ammoAmount / (float)ammoMax;        
        // display our mineral amount
        mineralAmountText.text = mineralAmount.ToString(); // in text
        mineralMaxText.text = mineralMax.ToString(); // in text    
        // display our gem amount
        gemAmountText.text = gemAmount.ToString(); // in text
        gemMaxText.text = gemMax.ToString(); // in text
        // displayer our HP amount
        hpAmountText.text = playerHP.ToString(); // in text
        hpMaxText.text = playerMaxHP.ToString(); // in text
        hpSlider.value = (float)playerHP / (float)playerMaxHP;
        // display our bug part amount
        bugAmountText.text = bugPartAmount.ToString();
        bugMaxText.text = "900";
        // modify our reticle ring
        if (currentWeapon == weaponTypes.Rifle)
        { rifleReticleRing.transform.localScale = new Vector3(shotCoolDown, shotCoolDown, shotCoolDown); }
        #endregion

        // shoot bullets
        if (canFire)
        {
            // check how much ammo we have, then determine how much damage we will deal based on it
            // ammo amount is out of 150. We want to split this into thirds. 
            // 100 to 150 = 3 dmg
            if (ammoAmount > 100)
            { pistolDamage = 3; }
            // 50 to 100 = 2 dmg
            if (ammoAmount > 50 && ammoAmount < 100)
            { pistolDamage = 2; }
            // 0 to 50 = 1 dmg
            if (ammoAmount > 0 && ammoAmount < 50)
            { pistolDamage = 1; }

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
                    // check cooldown
                    if (shotCoolDown <= 0)
                    {
                        // check arm
                        if (rightArm == true)
                        {
                            // set the sound of our source
                            fireAudioSource.clip = pistolsFireAudioClip;
                            fireAudioSource.Play();
                            // spawn bullet
                            GameObject bullet = Instantiate(playerBullet, rightGunTip.position, Quaternion.identity, null);
                            bullet.GetComponent<PlayerBulletScript>().bulletType = PlayerBulletScript.bulletTypes.Projectile;
                            bullet.GetComponent<PlayerBulletScript>().bulletTarget = diegeticAimTarget;
                            bullet.GetComponent<PlayerBulletScript>().bulletDamage = pistolDamage;
                            rightArm = false;
                            ammoAmount--;
                            // screenshake
                            cameraScript.shakeDuration += 4f;
                            // shot cooldown
                            shotCoolDown = 10f;
                        }
                        else if (rightArm == false)
                        {
                            // set the sound of our source
                            fireAudioSource.clip = pistolsFireAudioClip;
                            fireAudioSource.Play();
                            // spawn bullet
                            GameObject bullet = Instantiate(playerBullet, leftGunTip.position, Quaternion.identity, null);
                            bullet.GetComponent<PlayerBulletScript>().bulletType = PlayerBulletScript.bulletTypes.Hitscan;
                            bullet.GetComponent<PlayerBulletScript>().bulletTarget = diegeticAimTarget;
                            bullet.GetComponent<PlayerBulletScript>().bulletDamage = pistolDamage;
                            rightArm = true;
                            ammoAmount--;
                            // screenshake
                            cameraScript.shakeDuration += 4f;
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
                        // set the sound of our source
                        fireAudioSource.clip = rifleFireAudioClip;
                        fireAudioSource.Play();
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
                                hit.transform.gameObject.GetComponent<EnemyClass>().TakeDamage((int)rifleDamage);
                                Instantiate(cubePuff, hit.point, Quaternion.Euler(new Vector3(0, 0, 0)), null);
                                // screenshake
                                cameraScript.shakeDuration += 0.08f;
                                // muzzle flash effect
                                rifleMuzzleFlash.SetActive(true);
                                rifleMuzzleFlash.transform.rotation = Quaternion.Euler(0, 0, Random.Range(0, 360));
                            }
                        }
                        else // if we do not hit anything
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
        // COMMENTED OUT reload scene for dev purposes
        if (Input.GetKeyDown(KeyCode.F4))
        {
            // SceneManager.LoadScene(SceneManager.GetActiveScene().name, LoadSceneMode.Single);
        }

        // tab press to show and update objective panel
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            // save our previous body rotation
            previousBodyRotation = cameraScript.bodyTransform.rotation.eulerAngles;

            // show / hide our inventory
            if (!inventoryOpen)
            {
                canFire = false;
                // unlock cursor
                Cursor.lockState = CursorLockMode.None;

                // play the noise
                inventoryAudioSource.clip = inventoryOpenAudio;
                inventoryAudioSource.Play();

                // make sure the inventory is seen and not blocked
                gameplayUICanvas.alpha = 0;
                inventoryCanvas.alpha = 1;

                inventoryOpen = true;
                canMove = false;
                cameraScript.canLook = false;

                // zero out the head so that it is not disorienting
                
                cameraScript.bodyTransform.rotation = Quaternion.Euler(new Vector3(0, cameraScript.bodyTransform.rotation.y, 0));
                cameraScript.headTransform.rotation = Quaternion.Euler(new Vector3(cameraScript.headTransform.rotation.x, 0, cameraScript.headTransform.rotation.z));

                // lerping camera now handled in the fixed update
                // mainCameraContainer.position = inventoryCameraPos.position;
                // mainCameraContainer.rotation = inventoryCameraPos.rotation;

            } else
            {
                canFire = true;
                // lock cursor
                Cursor.lockState = CursorLockMode.Locked;
                // play the noise
                inventoryAudioSource.clip = inventoryCloseAudio;
                inventoryAudioSource.Play();

                // turn off the canvas
                gameplayUICanvas.alpha = 1;
                inventoryCanvas.alpha = 0;

                // move camera
                mainCameraContainer.position = Vector3.zero;
                mainCameraContainer.rotation = Quaternion.Euler(Vector3.zero);
                cameraScript.bodyTransform.rotation = Quaternion.Euler(previousBodyRotation);

                inventoryOpen = false;
                canMove = true;
                cameraScript.canLook = true;
            }


            #region // Old Development UI
            /*
            if (!objectiveShowing)
            {
                // run our panel coroutine
                StartCoroutine(ObjectivePanelHandler("Collect Resources. Board Ship and Launch with Space when ready."));
            }

            if (artifactInfoText.color.a == 1)
            { artifactInfoText.color = new Color(255, 255, 255, 0); }
            else if (artifactInfoText.color.a == 0)
            { artifactInfoText.color = new Color(255, 255, 255, 1); }*/
            #endregion
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
            // empty our artifacts
            UpgradeSingleton.DestroySingleton();
            // if we press space, emergency teleport back to base (load primer)
            if (Input.GetKeyDown(KeyCode.F))
            {
                // display death canvas
                deathCanvas.alpha = 0;
                // reset all resources
                gemAmount = 0;
                ammoAmount = 0;
                bugPartAmount = 0;
                // fade out
                fadeCanvas.alpha = 1;
                // reset position
                transform.position = new Vector3(0, 3.5f, 0);
                /*
                // reset drop pod position. everything else should be handled by the drop pod once it realized we are in the hub.
                dropPodTransform.position = new Vector3(0, 0, 0);
                // set droppod target position to 0 out
                dropPodManager.ourPlatform.targetPos = new Vector3(0, 0, 0);
                // make sure our droppod trips are reset
                dropPodManager.remainingTrips = dropPodManager.maxTrips;*/
                // let the player move
                canMove = true;
                cameraScript.canLook = true;
                // load primer
                SceneManager.LoadScene("RePrimer", LoadSceneMode.Single);
                // set player HP to max
                playerHP = playerMaxHP;
                // make sure to reload the upgrade singleton and UI
                // unload the instance
                UpgradeSingleton.DestroySingleton();
                ClearArtifactInfoUI();
                // make sure the player's position is reset on death
                transform.position = new Vector3(0, 5, 0);
            }

            // clear artifacts
            ClearArtifactInfoUI();
        }

        // artifact upgrades
        if (UpgradeSingleton.Instance.autoShieldDuration > 0 && autoShieldCosmetic.activeInHierarchy == false)
        {
            autoShieldCosmetic.SetActive(true);
        }

        if (Input.GetKey(KeyCode.Escape))
        {
            // SceneManager.LoadScene("Main Menu");
        }
    }

    // fixed update is called once per frame
    private void FixedUpdate()
    {
        // check if we are in the room generation scene, then stop showing the loading screen
        if (SceneManager.GetActiveScene().name == "Room Generation")
        {
            // fade out
            fadeCanvas.alpha = 0;
        }

        // check beneath us if we can jump  
        if (Physics.Linecast(new Vector3(transform.position.x, transform.position.y - 1f, transform.position.z), new Vector3(transform.position.x, transform.position.y - 2f, transform.position.z)))
        {

        }
    
        // lerp our camera to the inventory space
        if (inventoryOpen)
        {
            mainCameraContainer.position = Vector3.Lerp(mainCameraContainer.position, inventoryCameraPos.position, 0.75f);
            mainCameraContainer.rotation = Quaternion.Euler(Vector3.Lerp(mainCameraContainer.rotation.eulerAngles, inventoryCameraPos.rotation.eulerAngles, 0.75f));
        }

        // update objective panel
        // currentObjective.text = objectiveCurrentMessage;

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
            // UpdateArtifactInfoUI();
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

        // our popup alpha change
        if (popupCanvas.alpha <= 1 && popupCanvas.alpha >= 0)
        { popupCanvas.alpha += popupAlphaChange; }

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
        yield return new WaitForSeconds(1);
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
    }

    // set our artifact info text
    public void UpdateArtifactInfoUI(string titleText, string infoText, Sprite icon)
    {
        // create a new ui grid object in our inventory
        inventoryArtifactUIHandler.UpdateInventoryGrid(titleText, infoText, icon);
    }

    public void ClearArtifactInfoUI()
    {
        Debug.Log("Clearing Artifact InfoUI PostRun");
        inventoryArtifactUIHandler.ClearInventoryGrid();
    }

    public void UpdateInfoPopupWrapper(string titleText, string descText, Color textColor, Sprite upgradeImage)
    {
        StartCoroutine(UpdateUpgradeInfoPopup(titleText, descText, textColor, upgradeImage));
    }

    // update our Upgrade Info Popup on pickup
    public IEnumerator UpdateUpgradeInfoPopup(string titleText, string descText, Color textColor, Sprite upgradeImage)
    {
        // update our values
        popupTitle.text = titleText;
        popupTitle.color = textColor;
        popupDesc.text = descText;
        popupImage.sprite = upgradeImage;
        // fade in our upgrade info popup alpha
        popupAlphaChange = 0.1f;
        // show for 5 seconds
        yield return new WaitForSeconds(5f);
        // make it go away
        popupAlphaChange = -0.1f;
    }

}
