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
    [SerializeField] Slider mineralSlider;      // our mineral slider
    [SerializeField] Text mineralAmountText;    // our mineral amount in text   
    [SerializeField] Slider gemSlider;          // our gem slider
    [SerializeField] Text gemAmountText;        // our gem amount in text   
    [SerializeField] Slider hpSlider;           // our hp slider
    [SerializeField] Text hpAmountText;         // our gp amount in text
    [SerializeField] Text bugAmountText;        // bug amount text display

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

    // Start is called before the first frame update
    void Start()
    {
        player = ReInput.players.GetPlayer(0);
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
        ammoAmountText.text = ammoAmount.ToString() + "/" + ammoMax.ToString(); // in text
        ammoSlider.value = (float)ammoAmount / (float)ammoMax;        
        // display our mineral amount
        mineralAmountText.text = mineralAmount.ToString() + "/" + mineralMax.ToString(); // in text
        mineralSlider.value = (float)mineralAmount / (float)mineralMax;        
        // display our gem amount
        gemAmountText.text = gemAmount.ToString() + "/" + gemMax.ToString(); // in text
        gemSlider.value = (float)gemAmount / (float)gemMax;
        // displayer our HP amount
        hpAmountText.text = playerHP.ToString() + "/" + playerMaxHP.ToString(); // in text
        hpSlider.value = (float)playerHP / (float)playerMaxHP;
        // display our bug part amount
        bugAmountText.text = bugPartAmount.ToString();

        // shoot bullets
        if (canFire)
        {
            if (player.GetButtonDown("Fire"))
            {
                // check ammo
                if (ammoAmount > 0)
                {
                    // check arm
                    if (rightArm == true)
                    {
                        // spawn bullet
                        GameObject bullet = Instantiate(playerBullet, rightGunTip.position, Quaternion.identity, null);
                        bullet.GetComponent<PlayerBulletScript>().bulletTarget = diegeticAimTarget;
                        rightArm = false;
                        ammoAmount--;
                        // screenshake
                        cameraScript.shakeDuration += 0.08f;
                    }
                    else if (rightArm == false)
                    {
                        // spawn bullet
                        GameObject bullet = Instantiate(playerBullet, leftGunTip.position, Quaternion.identity, null);
                        bullet.GetComponent<PlayerBulletScript>().bulletTarget = diegeticAimTarget;
                        rightArm = true;
                        ammoAmount--;
                        // screenshake
                        cameraScript.shakeDuration += 0.08f;
                    }
                }
            }
        }
        // reload scene for dev purposes
        if (Input.GetKeyDown(KeyCode.F4))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name, LoadSceneMode.Single);
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
    }

    // fixed update is called once per frame
    private void FixedUpdate()
    {
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
    }

    // if we gain life, positive number, if we lose life, negative number
    public void AddHP(int HP)
    {
        playerHP += HP;
    }
}
