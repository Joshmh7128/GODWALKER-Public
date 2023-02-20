using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerCombatAbilityManager : MonoBehaviour
{
    // script is used to manage the combat abilities, which keys they are bound to
    public PlayerCombatAbility combatAbilitySlotA, combatAbilitySlotB; // our two combat ability slots
    public PlayerCombatAbility combatAbilitySlotAinstance, combatAbilitySlotBinstance; // our two combat ability slots
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
            combatAbilitySlotAinstance = ability;
            ability.buttonAssignment = PlayerCombatAbility.ButtonAssignments.buttonPrimary;
            abilityAIcon.sprite = combatAbilitySlotA.abilityIcon;
            abilityAIcon.color = new Color(1, 1, 1, 1);
        }
        else
        {
            abilityAIcon = null; // remove icon
            abilityAIcon.color = new Color(0, 0, 0, 0);
        }

        if (combatAbilitySlotB != null)
        {
            PlayerCombatAbility ability = Instantiate(combatAbilitySlotB, abilityParent);
            combatAbilitySlotBinstance = ability;
            ability.buttonAssignment = PlayerCombatAbility.ButtonAssignments.buttonSecondary;
            abilityAIcon.sprite = combatAbilitySlotB.abilityIcon;
            abilityAIcon.color = new Color(1, 1, 1, 1);
        }
        else
        {
            abilityAIcon = null; // remove icon
            abilityAIcon.color = new Color(0, 0, 0, 0);
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
        abilityASlider.value = combatAbilitySlotAinstance.charge / combatAbilitySlotAinstance.chargeMax;
        abilityBSlider.value = combatAbilitySlotBinstance.charge / combatAbilitySlotBinstance.chargeMax;

        // if they are full, change color
        if (abilityASlider.value == 1)
        abilityASliderImage.color = combatAbilitySlotAinstance.readyColor;
        if (abilityBSlider.value == 1)
        abilityBSliderImage.color = combatAbilitySlotBinstance.readyColor;

        if (abilityASlider.value != 1)
        abilityASliderImage.color = combatAbilitySlotAinstance.chargingColor;
        if (abilityBSlider.value != 1)
        abilityBSliderImage.color = combatAbilitySlotBinstance.chargingColor;
    }
}