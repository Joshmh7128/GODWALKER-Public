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
                        fearValues[r,c] = fearAmounts.two; Debug.Log("setting 2");
                        break;
                }
            }
        }
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

}
