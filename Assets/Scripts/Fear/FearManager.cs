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

    private void Start()
    {
        // assign correct values
        // loop by row
        for (int c = 0; c < 3; c++)
        {
            for (int r = 0; r < 7; r++)
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
    }

    // public function to assign values
    public void AssignFear(int row, int column)
    {
        // clear row
        for (int i = 0; i < 3; i++)
        { selectedAmounts[row, i] = false; }
        // set active element in that row
        selectedAmounts[row, column] = true;
        // reload fear amount
        CalculateFear();
    }

    void CalculateFear()
    {
        // start with 0
        finalFear = 0;

        // go column by column
        for (int c = 0; c < 3; c++)
        {
            // then row by row
            for (int r = 0; r < 7; r++)
            {
                if (selectedAmounts[r, c]) finalFear += (float)fearValues[r, c];
            }
        }

        // then divide our fear by 2
        finalFear /= 2;

        finalFearDisplay.text = finalFear.ToString();
        finalFearSlider.value = finalFear;
    }

}
