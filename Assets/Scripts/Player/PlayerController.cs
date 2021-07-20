using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Rewired;

public class PlayerController : MonoBehaviour
{
    // controller values
    [SerializeField] CharacterController characterController; // our character controller
    public CameraScript cameraScript; // our camera script
    [SerializeField] float moveSpeed; // how fast can we move?
    [SerializeField] float gravity; // gravity in the environment
    [SerializeField] Transform playerHead; // for which way we are facing
    [SerializeField] Transform rightGunTip; // for firing shots
    [SerializeField] Transform leftGunTip;
    [SerializeField] Transform diegeticAimTarget;
    [SerializeField] bool rightArm = true; // if true, shoot from right arm. if false, shoot from left arm. 
    public int ammoAmount; // how much ammo we currently have
    public int ammoMax; // how much ammo we can carry at one time
    public int playerHP; // the player's health
    public int playerMaxHP; // the player's max health
    [SerializeField] Transform treadsParent; // the parent of our treads

    // referenced prefabs and objects
    [SerializeField] GameObject playerBullet; // bullet prefab
    [SerializeField] Slider ammoSlider; // our ammo slider
    [SerializeField] Text ammoAmountText; // our ammo amount in text   
    [SerializeField] Slider hpSlider; // our ammo slider
    [SerializeField] Text hpAmountText; // our ammo amount in text

    // movement and input
    Player player;
    Vector3 move;
    Vector3 moveH;
    Vector3 moveV;

    // Start is called before the first frame update
    void Start()
    {
        player = ReInput.players.GetPlayer(0);
    }

    // Update is called once per frame
    void Update()
    {
        // move the player
        characterController.Move(new Vector3(0f, gravity, 0f) * Time.deltaTime);
        // declare our motion
        moveV = playerHead.forward * player.GetAxis("Vertical");
        moveH = playerHead.right * player.GetAxis("Horizontal");
        move = new Vector3(moveH.x, 0f, moveH.z) + new Vector3(moveV.x, 0f, moveV.z);
        // apply to the character controller
        characterController.Move(move * Time.deltaTime * moveSpeed);
        // apply to the character controller
        characterController.Move(move * Time.deltaTime * moveSpeed);
        // rotate our treads
        Vector3 treadDirection = Vector3.RotateTowards(treadsParent.forward, move, 10 * Time.deltaTime, 0f);
        treadsParent.rotation = Quaternion.LookRotation(treadDirection);

        // display our ammo amount
        ammoAmountText.text = ammoAmount.ToString(); // in text
        ammoSlider.value = (float)ammoAmount / (float)ammoMax;
        // displayer our HP amount
        hpAmountText.text = playerHP.ToString(); // in text
        hpSlider.value = (float)playerHP / (float)playerMaxHP;

        // shoot bullets
        if (Input.GetMouseButtonDown(0))
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

    // if we gain life, positive number, if we lose life, negative number
    public void AddHP(int HP)
    {
        playerHP += HP;
    }
}
