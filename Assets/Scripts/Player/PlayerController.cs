using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Rendering;
using Rewired;

public class PlayerController : MonoBehaviour
{
    #region // Main Gameplay Values
    [Header("Controller Values")]
    // controller values
    [SerializeField] CharacterController characterController; // our character controller
    public CameraScript cameraScript;           // our camera script
    [Header("Body Variables")]
    [SerializeField] Transform playerHead;      // for which way we are facing
    [SerializeField] Transform rightGunTip;     // for firing shots
    [SerializeField] Transform leftGunTip;
    [SerializeField] Transform diegeticAimTarget; // moves to our aiming position
    [SerializeField] Transform playerYRotationParent; // used to make our treads slightly rock back and forth
    [SerializeField] bool rightArm = true;      // if true, shoot from right arm. if false, shoot from left arm. 
    public int powerAmount;                      // how much ammo we currently have
    public int gemAmount;                       // gem amount
    public int powerMax;                         // how much ammo we can carry at one time
    public int gemMax;                          // gem carry space
    public int playerHP;                        // the player's health
    public int playerMaxHP;                     // the player's max health
    [SerializeField] Transform playerLegParent;    // the parent of our treads
    public bool canMove = true;
    public bool canFire = true;
    public bool victoryAchieved = false;

    #endregion

    #region // Referenced Prefabs
    // referenced prefabs and objects
    [SerializeField] GameObject playerBulletEffect;   // bullet prefab
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
    [SerializeField] Text scrapAmountText;        // bug amount text display
    [SerializeField] Text bugMaxText;           // bug amount text display
    // hurt particle
    [SerializeField] GameObject hurtParticle;   // the hurt particle prefab
    [SerializeField] GameObject shootParticle;  // the shoot particle prefab
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
    [SerializeField] CanvasGroup victoryCanvas; // our victory canvas
    [SerializeField] CanvasGroup fadeCanvas; // our fade, loading canvas
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
    [Header("Movement Values")]
    // movement and input
    Player player;
    Vector3 move;
    Vector3 moveH;
    Vector3 moveV;
    public bool canJump = true;
    [SerializeField] float moveSpeed;           // how fast can we move?
    [SerializeField] float gravity;             // gravity in the environment
    [SerializeField] float jumpVelocity; // how fast can we jump
    [SerializeField] float fallMultiplier;  // how quicky we fall in addition to normal gravity
    [SerializeField] float normalFallMultiplier;  // how quicky we fall in addition to normal gravity
    [SerializeField] float lowJumpMultiplier;  // how quicky we fall in addition to normal gravity
    [SerializeField] float playerJumpVelocity;  // how quicky we fall in addition to normal gravity
    float gravityValue;                        // real time simulated gravity
    // everything to do with our dash
    [SerializeField] float  dashCoolDownMax, dashCoolDown, dashTime, dashTimeMax, dashIntensity; // how fast we dash
    [SerializeField] Vector3 dashDir;
    [SerializeField] AudioSource dashAudioSource;
    [SerializeField] ParticleSystem dashParticleSystem; // the particles that appear when we dash

    #endregion

    #region // Player animation
    [SerializeField] Animator humanoidPlayerAnimator; // the animator that controls our human animations, mainly for the legs
    [SerializeField] Animator humanoidHandTargetAnimator; // the animator that controls our hands
    [SerializeField] Animator neckTargetAnimator; // the animator that controls the height of our neck
    [SerializeField] float kickIKReduction; // how quickly to reduce our IK kick
    float rightIKArmKickback, leftIKArmKickback; // the amount of kick time on each arm
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
    [SerializeField] GameObject reticleRing; // reticle ring associated with the rifle
    [SerializeField] GameObject cubePuff; // our bullet cube particle effect
    [SerializeField] GameObject rifleMuzzleFlash; // the rifle muzzle flash

    public float rifleDamage; // rifle damage


    // pistol stuff
    public float pistolDamage, pistolMagSize, pistolMagFill; // pistol damage // how many shots per magazine for pistols // how many shots are in our current magazine?
    public float pistolSoundPitch, pistolSoundPitchMin, pistolSoundPitchMax; // our pitch, min, and max
    public float pistolShotSize; // the size the of the pistol spherecast
    public float shotCoolDownRemain, shotCoolDown; // the amount of time until we can fire again
    public float pistolRecoil; // the amount of recoil our pistols give off

    #endregion

    #region // Artifact Management
    // artifact upgradess
    bool isInvincible, isMitoInvincible, autoShieldCoroutineRunning, mitoShieldCoroutineRunning;
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
    [SerializeField] AudioClip[] pistolsFireAudioClips;
    [SerializeField] AudioClip rifleFireAudioClip;

    #endregion

    #region // Visual effect Prefabs
    [Header("FX and Feel")]
    [SerializeField] GameObject pistolMuzzleFlashFX, pistolHitFX;
    [SerializeField] float snapShakeDelta;
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        player = ReInput.players.GetPlayer(0);
    }

    // Update is called once per frame
    void Update()
    {
        #region // movement code
        if (canMove)
        {
            // declare our motion
            float pAxisV = player.GetAxis("Vertical");
            float pAxisH = player.GetAxis("Horizontal");
            moveV = playerHead.forward * pAxisV;
            moveH = playerHead.right * pAxisH;

            // vertical velocity variable
            float verticalVelocity;

            // rotate our treads (will be removed once humanoid animations are complete)
            Vector3 treadDirection = Vector3.RotateTowards(playerLegParent.forward, new Vector3(move.x, 0, move.z), 10 * Time.deltaTime, 0f);
            playerLegParent.rotation = Quaternion.LookRotation(treadDirection);

            // horizontal dash
            if (player.GetButtonDown("DashButton"))
            {
                // make sure we can actually dash
                if (dashCoolDown <= 0 && (Mathf.Abs(pAxisV) > 0.1f) || (Mathf.Abs(pAxisH) > 0.1f))
                {
                    dashCoolDown = dashCoolDownMax;
                    dashTime = dashTimeMax;
                    dashDir = new Vector3((moveV.x + moveH.x) * dashIntensity, 0f, (moveV.z + moveH.z) * dashIntensity);
                    dashAudioSource.Play();
                    dashParticleSystem.Play();
                    humanoidPlayerAnimator.SetLayerWeight(6, 1);
                }
            }

            // if we are moving and not 
            if ((Mathf.Abs(pAxisV) > 0.1f) || (Mathf.Abs(pAxisH) > 0.1f) && (dashCoolDown == 0))
            {
                // leg animation weights
                if (humanoidPlayerAnimator != null)
                {
                    humanoidPlayerAnimator.SetLayerWeight(2, (Mathf.Abs(pAxisV) + Mathf.Abs(pAxisH))); // running layer
                    humanoidPlayerAnimator.SetLayerWeight(1, 0); // idle layer
                }

                // arm animation weights
                if (humanoidHandTargetAnimator != null)
                {
                    // humanoidHandTargetAnimator.SetLayerWeight(1, 0); // idle layer
                    humanoidHandTargetAnimator.SetLayerWeight(2, (Mathf.Abs(pAxisV) + Mathf.Abs(pAxisH))); // running layer
                }

                // neck animation weights
                if (neckTargetAnimator != null)
                {
                    if (characterController.isGrounded)
                    neckTargetAnimator.SetLayerWeight(1, (Mathf.Abs(pAxisV) + Mathf.Abs(pAxisH))); // run layer
                }
            }
            else 
            {
                // leg animation weights
                if (humanoidPlayerAnimator != null)
                {
                    humanoidPlayerAnimator.SetLayerWeight(1, 1); // idle layer
                    humanoidPlayerAnimator.SetLayerWeight(2, 0); // running layer
                }

                // arm animation weights
                if (humanoidHandTargetAnimator != null)
                {
                    humanoidHandTargetAnimator.SetLayerWeight(1, 1); // idle layer
                    humanoidHandTargetAnimator.SetLayerWeight(2, 0); // running layer
                }

                // neck animation weights
                if (neckTargetAnimator != null)
                {
                    neckTargetAnimator.SetLayerWeight(1, 0); // run layer
                }
            }

            // gravity modifications
            if (characterController.isGrounded && !player.GetButtonDown("SpacePress"))
            {
                // jump falling
                gravityValue = gravity*100;
                // jump animation weights
                humanoidPlayerAnimator.SetLayerWeight(6, 0);
                humanoidHandTargetAnimator.SetLayerWeight(5, 0);

            }
            else if (characterController.isGrounded && player.GetButton("SpacePress") && canJump)
            {
                // jump falling
                gravityValue = gravity * lowJumpMultiplier;

                humanoidPlayerAnimator.SetLayerWeight(2, 0);
            }
            else if (characterController.velocity.y <= 0 && !characterController.isGrounded && canJump)
            {
                // normal falling
                gravityValue = gravity * fallMultiplier;
                // jump animation weight
                humanoidPlayerAnimator.SetLayerWeight(6, 1);
                humanoidPlayerAnimator.SetLayerWeight(2, 0);
                // arm animation weights
                humanoidHandTargetAnimator.SetLayerWeight(5, humanoidHandTargetAnimator.GetLayerWeight(5) + 0.1f); // alternate idle layer
                // neck bob animation weight
                neckTargetAnimator.SetLayerWeight(1,0); // run layer
            } 
            else if (characterController.velocity.y > 0 && canJump)
            {
                // jump falling
                gravityValue = gravity * lowJumpMultiplier;
                // jump animation weight
                humanoidPlayerAnimator.SetLayerWeight(6, 1);
                // run weight
                humanoidPlayerAnimator.SetLayerWeight(2, 0);
                // arm animation weights
                humanoidHandTargetAnimator.SetLayerWeight(5, humanoidHandTargetAnimator.GetLayerWeight(5) + 0.1f);
                // neck bob animation weight
                neckTargetAnimator.SetLayerWeight(1, 0); // run layer
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
            move = new Vector3((moveH.x + moveV.x), verticalVelocity / moveSpeed, (moveH.z + moveV.z));
            move += dashDir;
            // apply to the character controller
            characterController.Move(move * Time.deltaTime * moveSpeed);
        }
        #endregion  
        
        #region // UI display
        // display our ammo amount
        ammoAmountText.text = powerAmount.ToString(); // in text
        ammoMaxText.text = powerMax.ToString(); // in text
        ammoSlider.value = (float)powerAmount / (float)powerMax;        
        // display our gem amount
        gemAmountText.text = gemAmount.ToString(); // in text
        gemMaxText.text = gemMax.ToString(); // in text
        // displayer our HP amount
        hpAmountText.text = playerHP.ToString(); // in text
        hpMaxText.text = playerMaxHP.ToString(); // in text
        hpSlider.value = (float)playerHP / (float)playerMaxHP;
        // display our bug part amount
        // scrapAmountText.text = scrapAmount.ToString();
        // modify our reticle ring
        reticleRing.transform.localScale = new Vector3(Mathf.Lerp(0,1f,shotCoolDownRemain / shotCoolDown), Mathf.Lerp(0, 1f, shotCoolDownRemain / shotCoolDown), Mathf.Lerp(0, 1f, shotCoolDownRemain / shotCoolDown));

        /// lets calculate our reload ammunition display for our pistols
        /// for this we are going to want to place the total mag size to the left of our reticle


        #endregion

        #region // shot firing
        // shoot bullets
        if (canFire)
        {
            // check how much ammo we have, then determine how much damage we will deal based on it
            // ammo amount is out of 150. We want to split this into thirds. 
            // 100 to 150 = 3 dmg
            if (powerAmount > 100)
            { pistolDamage = 3; }
            // 50 to 100 = 2 dmg
            if (powerAmount > 50 && powerAmount < 100)
            { pistolDamage = 2; }
            // 0 to 50 = 1 dmg
            if (powerAmount > 0 && powerAmount < 50)
            { pistolDamage = 1; }

            if (player.GetButton("Fire"))
            {
                // which weapon are we using?

                // pistols
                if (currentWeapon == weaponTypes.Pistols)
                {
                    // check cooldown, and make sure we have a bullet in our magazine
                    if (shotCoolDownRemain <= 0 && pistolMagFill > 0)
                    {
                        // check arm
                        if (rightArm == true)
                        {
                            // set the sound of our source
                            fireAudioSource.clip = pistolsFireAudioClips[Random.Range(0, pistolsFireAudioClips.Length)];
                            // change the pitch - lerp from min pitch to max pitch using shots fired / magsize so we can adjust the amount of shots fired and mag size overtime
                            pistolSoundPitch = Random.Range(pistolSoundPitchMin,pistolSoundPitchMax);
                            fireAudioSource.pitch = (float)pistolSoundPitch;

                            // once we have the pitch, play the sound
                            fireAudioSource.Play();

                            // fire muzzle flash when we fire
                            Instantiate(pistolMuzzleFlashFX, rightGunTip.position, rightGunTip.rotation, null);
                            PlayerBulletScriptFX ourBullet = Instantiate(playerBulletEffect, rightGunTip.position, rightGunTip.rotation, null).GetComponent<PlayerBulletScriptFX>();
                            ourBullet.bulletTarget = cameraScript.cameraCenterHit.point;
                            // hitscan and deal damage
                            RaycastHit hit;
                            if (Physics.SphereCast(rightGunTip.position, pistolShotSize, cameraScript.cameraCenterHit.point - rightGunTip.position, out hit, Mathf.Infinity, Physics.AllLayers, QueryTriggerInteraction.Ignore))
                            {
                                // create a visual effect of our shot hitting
                                Instantiate(pistolHitFX, hit.point, Quaternion.Euler(hit.normal));
                                // create a line render to show the path of our bullet in the air

                                // deal damage
                                if (hit.transform.tag == "Enemy")
                                { 
                                    if (hit.transform.gameObject.GetComponent<EnemyClass>() != null)
                                    hit.transform.gameObject.GetComponent<EnemyClass>().TakeDamage((int)pistolDamage); 
                                }
                            }

                            rightArm = false;
                            // fire animation weight amount
                            rightIKArmKickback = 1;
                            // shot cooldown
                            shotCoolDownRemain = shotCoolDown;
                            // camera fov change
                            // shot screen fov lerp
                            // Mathf.Lerp(shotCoolDownRemain, shotCoolDown, cameraScript.GetComponent<Camera>().fieldOfView = cameraScript.GetComponent<Camera>().fieldOfView + (shotCoolDown/8));
                        }
                        else if (rightArm == false)
                        {
                            // set the sound of our source
                            fireAudioSource.clip = pistolsFireAudioClips[Random.Range(0,pistolsFireAudioClips.Length)];
                            // change the pitch - lerp from min pitch to max pitch using shots fired / magsize so we can adjust the amount of shots fired and mag size overtime
                            pistolSoundPitch = Mathf.Lerp(pistolSoundPitchMin, pistolSoundPitchMax, (pistolMagSize - pistolMagFill) / pistolMagSize);
                            fireAudioSource.pitch = (float)pistolSoundPitch;

                            // once we have the pitch, play the sound
                            fireAudioSource.Play();
                            // fire muzzle flash when we fire
                            Instantiate(pistolMuzzleFlashFX, leftGunTip.position, leftGunTip.rotation, null);
                            PlayerBulletScriptFX ourBullet = Instantiate(playerBulletEffect, leftGunTip.position, leftGunTip.rotation, null).GetComponent<PlayerBulletScriptFX>();
                            ourBullet.bulletTarget = cameraScript.cameraCenterHit.point;
                            // hitscan and deal damage
                            RaycastHit hit;
                            if (Physics.SphereCast(leftGunTip.position, pistolShotSize, cameraScript.cameraCenterHit.point - leftGunTip.position, out hit, Mathf.Infinity, Physics.AllLayers, QueryTriggerInteraction.Ignore))
                            {
                                // create a visual effect of our shot hitting
                                Instantiate(pistolHitFX, hit.point, Quaternion.identity, null);
                                // create a line render to show the path of our bullet in the air

                                // deal damage
                                if (hit.transform.tag == "Enemy")
                                { hit.transform.gameObject.GetComponent<EnemyClass>().TakeDamage((int)pistolDamage); }
                            }
                            rightArm = true;
                            // fire animation weight amount
                            leftIKArmKickback = 1;
                            // particle effect
                            Instantiate(shootParticle, leftGunTip.position, leftGunTip.rotation, null);
                            // shot cooldown
                            shotCoolDownRemain = shotCoolDown;
                            // screen fov
                            // Mathf.Lerp(shotCoolDownRemain, shotCoolDown, cameraScript.GetComponent<Camera>().fieldOfView = cameraScript.GetComponent<Camera>().fieldOfView + (shotCoolDown/8));
                        }

                        // screenshake
                        cameraScript.SnapScreenShake(snapShakeDelta);
                        // weapon kick
                        cameraScript.yRotateMod += pistolRecoil;

                        // reduce ammo amount
                        if (powerAmount > 0)
                        {
                            powerAmount--;
                        }

                        // reduce our mag amount
                        if (pistolMagFill > 0)
                        {
                            pistolMagFill--;
                        }


                    }

                    // if our mag is 0, set it to the magsize
                    if (pistolMagFill <= 0)
                    { pistolMagFill = pistolMagSize; }
                }

            }
        }
        #endregion

        // reload scene for dev purposes
        if (Input.GetKeyDown(KeyCode.F4))
        {
            // empty our artifacts
            UpgradeSingleton.DestroySingleton();
            SceneManager.LoadScene("Main Menu");
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
                powerAmount = 0;
                // fade out
                fadeCanvas.alpha = 1;
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

        // check if we achieve victory
        if (victoryAchieved)
        {
            canMove = false;
            victoryCanvas.alpha = 1;
            isInvincible = true;
            if (SceneManager.GetActiveScene().name != "Hub" && Input.GetKeyDown(KeyCode.F))
            {
                transform.position = new Vector3(0, 5f, 0);
                canMove = true;
                victoryCanvas.alpha = 0;
                SceneManager.LoadScene("Hub");
            }
        }
    }

    // fixed update is called once per frame
    private void FixedUpdate()
    {
        // make sure our kick animations weights are counting down properly, so that when we fire the arms go back down
        rightIKArmKickback -= kickIKReduction; leftIKArmKickback -= kickIKReduction;
        humanoidHandTargetAnimator.SetLayerWeight(3, rightIKArmKickback);
        humanoidHandTargetAnimator.SetLayerWeight(4, leftIKArmKickback);

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

        // make sure we set our playercontroller properlly
        if (UpgradeSingleton.Instance.player == null)
        {  
            UpgradeSingleton.Instance.player = this;
            // UpdateArtifactInfoUI();
        }

        // decrease hurt alpha if it is above 0
        if (hurtCanvas.alpha > 0)
        {
            Mathf.Clamp(hurtCanvas.alpha, 0, 1);
            hurtCanvas.alpha += -0.1f;
        }

        // make sure our HP isnt out of bounds
        Mathf.Clamp(playerHP, 0, playerMaxHP);

        // make sure to reduce shot cooldown
        if (shotCoolDownRemain > 0)
        {
            shotCoolDownRemain -= 1;
        }

        // make sure we reduce dash cooldown
        if (dashCoolDown > 0)
        {
            dashCoolDown -= 1;
        }

        if (dashTime > 0)
        { dashTime--; }

        if (dashTime <= 0)
        {
            dashDir = Vector3.zero;
        }

        // dash screen fov lerp
        Mathf.Lerp(dashTime, dashTimeMax, cameraScript.GetComponent<Camera>().fieldOfView = cameraScript.GetComponent<Camera>().fieldOfView + dashTime);

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

            // spawn in a particle effect on the player to show that they got hurt
            Instantiate(hurtParticle, transform, true);

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
