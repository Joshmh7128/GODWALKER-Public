using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerCurrencyManager : MonoBehaviour
{
    /// 
    /// this script serves to manage the player's currency balance
    /// there is no maximum to the player's currency. 
    /// 

    public int playerCurrencyAmount; // how much currency do we currently have?

    [SerializeField] TextMeshProUGUI currencyDisplayAmount;

    public static PlayerCurrencyManager instance;

    private void Awake() => instance = this;

    // adding currency
    public void AddCurrency(int amount)
    {
        playerCurrencyAmount += amount;

        // after adding currency update the UI
        UpdateUI();
    }

    // subtracting currency
    public void SubCurrency(int amount)
    {
        if (playerCurrencyAmount - amount >= 0)
        {
            playerCurrencyAmount -= amount;
        }
        else
        {
            playerCurrencyAmount = 0;
        }

        // after subtracting currency update the UI
        UpdateUI();
    }

    // run our UI
    void UpdateUI()
    {
        currencyDisplayAmount.text = playerCurrencyAmount.ToString();
    }



}
