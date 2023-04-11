using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RunForecastBoardHandler : MonoBehaviour
{
    // script will show us the next few rooms in the run based off of run data

    // generation manager
    PlayerGenerationSeedManager seedManager;

    // our sprites
    [SerializeField] Sprite normalCombat, explosiveCombat, energyCombat, bothCombat, freeWeapon, shop, finish;
    [SerializeField] List<Image> imageSlots = new List<Image>(); // a place to hold all of our images
    [SerializeField] List<TextMeshProUGUI> infoText = new List<TextMeshProUGUI>();

    private void Start()
    {
        // get our instance
        seedManager = PlayerGenerationSeedManager.instance;
        StartCoroutine(LateStartBuffer());
    }

    IEnumerator LateStartBuffer()
    {
        yield return new WaitForSeconds(0.25f);
        LateStart();
    }

    private void LateStart()
    {
        DisplayRooms();
    }

    // this script displays what will be on the board
    public void DisplayRooms()
    {
        bool elementalUpNext = false; // are there elemental enemies up next?

        // read our next runs and display the next 3 rooms
        for (int i = 0; i < imageSlots.Count; i++)
        {
            switch ((int)ReadRoom(i))
            {
                // none
                case (int)PlayerGenerationSeedManager.ElementBiases.none:
                    imageSlots[i].sprite = normalCombat;
                    infoText[i].text = "Normal Enemies";
                    break;

                // energy
                case (int)PlayerGenerationSeedManager.ElementBiases.partialEnergy:
                    imageSlots[i].sprite = energyCombat;
                    infoText[i].text = "Energy Weak Enemies";
                    elementalUpNext = true;
                    break;

                // explosive
                case (int)PlayerGenerationSeedManager.ElementBiases.partialExplosive:
                    imageSlots[i].sprite = explosiveCombat;
                    infoText[i].text = "Explosive Weak Enemies";
                    elementalUpNext = true;
                    break;

                // both
                case (int)PlayerGenerationSeedManager.ElementBiases.partialMixed:
                    imageSlots[i].sprite = bothCombat;
                    infoText[i].text = "Energy & Explosive Weak Enemies";
                    elementalUpNext = true;
                    break;

                // energy
                case (int)PlayerGenerationSeedManager.ElementBiases.allEnergy:
                    imageSlots[i].sprite = energyCombat;
                    infoText[i].text = "Energy Weak Enemies";
                    elementalUpNext = true;
                    break;

                // explosive
                case (int)PlayerGenerationSeedManager.ElementBiases.allExplosive:
                    elementalUpNext = true;
                    imageSlots[i].sprite = explosiveCombat;
                    infoText[i].text = "Explosive Weak Enemies";
                    break;

                // shop
                case (int)PlayerGenerationSeedManager.ElementBiases.shop:
                    imageSlots[i].sprite = shop;
                    infoText[i].text = "Weapon Shop";
                    break;

                // free gun
                case (int)PlayerGenerationSeedManager.ElementBiases.freeGun:
                    imageSlots[i].sprite = freeWeapon;
                    infoText[i].text = "Free Weapons";
                    break;

                // finish
                case (int)PlayerGenerationSeedManager.ElementBiases.finish:
                    imageSlots[i].sprite = finish;
                    infoText[i].text = "Finish";
                    break;
            }
        }

        // send a tooltip to the player
        if (elementalUpNext)
            TooltipHandler.instance.SetTooltip(TooltipHandler.Tooltips.elementalPop);
    }

    // return a room's type
    PlayerGenerationSeedManager.ElementBiases ReadRoom(int room)
    {
        // read from the generation manager to get our rooms type
        return seedManager.elementBiases[room + seedManager.currentCombatPos];
    }
}
