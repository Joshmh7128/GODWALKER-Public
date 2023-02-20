using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerCombatAbilityManager : MonoBehaviour
{
    // script is used to manage the combat abilities, which keys they are bound to
    public PlayerCombatAbility combatAbilitySlotA, combatAbilitySlotB; // our two combat ability slots
    public Transform abilityParent; // the parent of our abilities
    [SerializeField] Slider abilityASlider, abilityBSlider; // our sliders showcasing our ability usability
    [SerializeField] Image abilityASliderImage, abilityBSliderImage, abilityAIcon, abilityBIcon; // our slider images

    private void Start()
    {
        RefreshAbilities();
    }

    // public function to refresh out combat abilities
    public void RefreshAbilities()
    {
        // delete all children
        foreach (Transform child in abilityParent)
        {
            Destroy(child.gameObject);
        }

        // check and build out abilities into slots
        if (combatAbilitySlotA != null)
        {
            PlayerCombatAbility ability = Instantiate(combatAbilitySlotA, abilityParent);
            ability.buttonAssignment = PlayerCombatAbility.ButtonAssignments.buttonPrimary;
            abilityASliderImage = combatAbilitySlotA.abilityIcon;
        }
        else
        {
            abilityASliderImage = null; // remove icon
        }

        if (combatAbilitySlotB != null)
        {
            PlayerCombatAbility ability = Instantiate(combatAbilitySlotB, abilityParent);
            ability.buttonAssignment = PlayerCombatAbility.ButtonAssignments.buttonSecondary;
            abilityBSliderImage = combatAbilitySlotB.abilityIcon;
        }
        else
        {
            abilityASliderImage = null; // remove icon
        }
    }

    private void FixedUpdate()
    {
        // update UI every frame
        UpdateUI();
    }

    // UI update
    void UpdateUI()
    {
        // set charge values
        abilityASlider.value = combatAbilitySlotA.charge / combatAbilitySlotA.chargeMax;
        abilityBSlider.value = combatAbilitySlotB.charge / combatAbilitySlotB.chargeMax;

        // if they are full, change color
        if (abilityASlider.value == 1)
        abilityASliderImage.color = Color.white;
        if (abilityBSlider.value == 1)
        abilityBSliderImage.color = Color.white;

        if (abilityASlider.value != 1)
        abilityASliderImage.color = Color.grey;
        if (abilityBSlider.value != 1)
        abilityBSliderImage.color = Color.grey;
    }
}