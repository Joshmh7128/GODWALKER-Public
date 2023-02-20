using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCombatAbilityManager : MonoBehaviour
{
    // script is used to manage the combat abilities, which keys they are bound to
    public PlayerCombatAbility combatAbilitySlotA, combatAbilitySlotB; // our two combat ability slots
    public Transform abilityParent; // the parent of our abilities

    private void Start()
    {
        // RefreshAbilities();
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
            PlayerCombatAbility ability = Instantiate(combatAbilitySlotA, transform);
            ability.buttonAssignment = PlayerCombatAbility.ButtonAssignments.buttonPrimary;
        }

        if (combatAbilitySlotB != null)
        {
            PlayerCombatAbility ability = Instantiate(combatAbilitySlotB, transform);
            ability.buttonAssignment = PlayerCombatAbility.ButtonAssignments.buttonSecondary;
        }
    }
}