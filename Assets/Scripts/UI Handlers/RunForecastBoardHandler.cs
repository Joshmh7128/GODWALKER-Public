using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RunForecastBoardHandler : MonoBehaviour
{
    // script will show us the next few rooms in the run based off of run data

    // generation manager
    PlayerGenerationSeedManager seedManager;

    // our sprites
    [SerializeField] Sprite normalCombat, explosiveCombat, energyCombat, bothCombat, freeWeapon;
    [SerializeField] List<Image> imageSlots = new List<Image>(); // a place to hold all of our images

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
        // read our next runs and display the next 3 rooms
        for (int i = 0; i < imageSlots.Count; i++)
        {
            switch ((int)ReadRoom(i))
            {
                // none
                case 0:
                    imageSlots[i].sprite = normalCombat;
                    break;

                // energy
                case 1:
                    imageSlots[i].sprite = energyCombat;
                    break;

                // explosive
                case 2:
                    imageSlots[i].sprite = explosiveCombat;
                    break;

                // both
                case 3:
                    imageSlots[i].sprite = bothCombat;
                    break;

                // energy
                case 4:
                    imageSlots[i].sprite = energyCombat;
                    break;

                // explosive
                case 5:
                    imageSlots[i].sprite = explosiveCombat;
                    break;
            }
        }
    }

    // return a room's type
    PlayerGenerationSeedManager.ElementBiases ReadRoom(int room)
    {
        // read from the generation manager to get our rooms type
        return seedManager.elementBiases[room + seedManager.currentCombatPos];
    }
}
