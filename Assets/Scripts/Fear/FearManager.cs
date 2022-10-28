using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class FearManager : MonoBehaviour
{
    // this script is used to manage the fear levels and changes how the player works when fear is applied
    // it houses all the stats and displays them

    // build and assign instance
    public static FearManager instance;
    private void Awake() => instance = this;

    /// all of our fear stats
    // our fear amounts
    public enum fearAmounts { none, half, one, two = 4}; // we will divide these by 2 at the end result to calculate our fear
    // declare our variables
    public fearAmounts[,] fearValues = new fearAmounts[8, 4];
    public bool[,] selectedAmounts = new bool[8, 4];
    // the amount of fear we have
    public float finalFear;
    [SerializeField] TextMeshProUGUI finalFearDisplay; // the text display in game of our final fear
    [SerializeField] Slider finalFearSlider; // the text display in game of our final fear
    [SerializeField] List<Slider> rowSliderDisplay; // the sliders behind our buttons


    // the player and their stats
    PlayerController playerController; 
    float baseSpeed; // the player's base speed

    public List<fearAmounts> finalAmounts = new List<fearAmounts>(8);

    private void Start()
    {
        // assign correct values
        // loop by row
        for (int c = 0; c < 4; c++)
        {
            for (int r = 0; r < 8; r++)
            {
                switch (c)
                {
                    case 0:
                        fearValues[r,c] = fearAmounts.none;
                        break;

                    case 1:
                        fearValues[r,c] = fearAmounts.half;
                        break;

                    case 2:
                        fearValues[r,c] = fearAmounts.one;
                        break;

                    case 3:
                        fearValues[r,c] = fearAmounts.two;
                        break;
                }
            }
        }

        // setup instance
        playerController = PlayerController.instance;

        // setting up our values
        baseSpeed = playerController.moveSpeed; // the player's base movement speed
    }

    // public function to assign values
    public void AssignFear(int row, int column)
    {
        // clear row
        for (int i = 0; i < 4; i++)
        { selectedAmounts[row, i] = false; }
        // set active element in that row
        selectedAmounts[row, column] = true;
        // then set the readout abilities based on which column we're in
        switch (column)
        {
            case 0:
                finalAmounts[row] = fearAmounts.none;
                break;

            case 1:
                finalAmounts[row] = fearAmounts.half;
                break;

            case 2:
                finalAmounts[row] = fearAmounts.one;
                break;

            case 3:
                finalAmounts[row] = fearAmounts.two;
                break;
        }

        // reload fear amount
        CalculateFear();
    }

    void CalculateFear()
    {
        // start with 0
        finalFear = 0;

        // go column by column
        for (int c = 0; c < 4; c++)
        {
            // then row by row
            for (int r = 0; r < 8; r++)
            {
                if (selectedAmounts[r, c])
                {
                    finalFear += (float)fearValues[r, c];
                    try { rowSliderDisplay[r].value = 0; rowSliderDisplay[r].value = (float)fearValues[r, c]/2 + 0.13f; } catch { }
                }
            }
        }

        // then divide our fear by 2
        finalFear /= 2;

        // then display
        finalFearDisplay.text = finalFear.ToString();
        finalFearSlider.value = finalFear;

        
    }



    // where we are applying our fear, called by our button
    public void ApplyFear()
    {
        // movement speed application, check row 0 and see what it needs to be
        // movement speed
        switch (finalAmounts[0])
        {
            case fearAmounts.none:
                playerController.moveSpeed = baseSpeed;
                break;

            case fearAmounts.half:
                playerController.moveSpeed = baseSpeed / 2;
                break;

            case fearAmounts.one:
                playerController.moveSpeed = baseSpeed / 3;
                break;

            case fearAmounts.two:
                playerController.moveSpeed = baseSpeed / 4;
                break;
        }        
        
        // jump amounts
        switch (finalAmounts[1])
        {
            case fearAmounts.none:
                playerController.maxJumps = 2;
                break;

            case fearAmounts.half:
                playerController.maxJumps = 1;
                break;

            case fearAmounts.one:
                playerController.maxJumps = 0;
                break;

            case fearAmounts.two:
                playerController.maxJumps = 0;
                break;
        }

        // dash amounts
        switch (finalAmounts[2])
        {
            case fearAmounts.none:
                playerController.dashTimeMax = playerController.dashTimeLongMax;
                break;

            case fearAmounts.half:
                playerController.dashTimeMax = playerController.dashTimeShortMax;
                break;

            case fearAmounts.one:
                playerController.canDash = false;
                break;

            case fearAmounts.two:
                playerController.canDash = false;
                break;
        }

        // guns amounts
        switch (finalAmounts[3])
        {
            case fearAmounts.none:
                playerController.weaponManager.currentWeaponInt = 0;
                playerController.weaponManager.UpdateCurrentWeapon();
                break;

            case fearAmounts.half:
                playerController.weaponManager.currentWeaponInt = 1;
                playerController.weaponManager.UpdateCurrentWeapon();
                break;

            case fearAmounts.one:
                playerController.weaponManager.currentWeaponInt = 2;
                playerController.weaponManager.UpdateCurrentWeapon();
                break;

            case fearAmounts.two:
                playerController.weaponManager.currentWeaponInt = 3;
                playerController.weaponManager.UpdateCurrentWeapon();
                break;
        }

        // shield values
        switch (finalAmounts[4])
        {
            case fearAmounts.none:
                playerController.canDeflect = true;
                playerController.shieldRechargeMax = playerController.shieldRechargeMaxFast; // fast recharge
                break;

            case fearAmounts.half:
                playerController.canDeflect = true;
                playerController.shieldRechargeMax = playerController.shieldRechargeMaxSlow; // fast recharge
                break;

            case fearAmounts.one:
                playerController.canDeflect = true;
                playerController.shieldRechargeMax = playerController.shieldRechargeMaxVerySlow; // fast recharge
                break;

            case fearAmounts.two:
                playerController.canDeflect = false;
                break;
        }

        // health values
        switch (finalAmounts[4])
        {
            case fearAmounts.none:
                PlayerStatManager.instance.maxHealth = 10;
                PlayerStatManager.instance.health = 10;
                break;

            case fearAmounts.half:
                PlayerStatManager.instance.maxHealth = 10;
                PlayerStatManager.instance.health = 10;
                break;

            case fearAmounts.one:
                PlayerStatManager.instance.maxHealth = 10;
                PlayerStatManager.instance.health = 10;
                break;

            case fearAmounts.two:
                PlayerStatManager.instance.maxHealth = 10;
                PlayerStatManager.instance.health = 1;
                break;
        }
    }
}
