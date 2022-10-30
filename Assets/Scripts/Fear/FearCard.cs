using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FearCard : MonoBehaviour
{
    /// this card is used to display the current level of fear on the player and then either add a new 
    /// detriment or advanced a current one
    /// 

    // our fear manager
    FearManager fearManager;

    // all the choices we can make for our fear
    public enum fearChoices
    {
        movementSpeed, jumpAmount, 
        dashAmount, gunValue, 
        shieldValues, healthValues, 
        bulletSpeed, 

        Count
    }

    public fearChoices firstChoice, secondChoice;

    [SerializeField] string fearMessage, subInfo; 

    private void Start()
    {
        fearManager = FearManager.instance;
    }

    // make our choices
    void ChooseFear()
    {
        // list our current choicese
        List<int> choices = new List<int>();
        // add all our fear choices to the list
        for (int i = 0; i < (int)fearChoices.Count; i++)
        {
            choices.Add(i);
        }
        // assign and remove our first choice
        firstChoice = (fearChoices)choices[Random.Range(0,choices.Count)];
        choices.Remove((int)firstChoice);
        // assign our second choice
        secondChoice = (fearChoices)choices[Random.Range(0, choices.Count)];

        // now that we have our categories, change our values based off of what our fear manager has
        
    }

    

}
