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

    [SerializeField] Sprite abilityIconSpriteA, abilityIconSpriteB;

    // empty combat item
    [SerializeField] GameObject emptyCombatAbilityItem;

    // instance
    public static PlayerCombatAbilityManager instance; // our instance

    private void Awake()
    {
        instance = this;
    }

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
            ability.charge = 0; // set charge to 0 so that the ability doesn't activate
        }
        else
        {
            try
            {
                abilityAIcon.color = new Color(0, 0, 0, 0);
                abilityAIcon.sprite = null; // remove icon
            } catch { }
        }

        if (combatAbilitySlotB != null)
        {
            PlayerCombatAbility ability = Instantiate(combatAbilitySlotB, abilityParent);
            combatAbilitySlotBinstance = ability;
            ability.buttonAssignment = PlayerCombatAbility.ButtonAssignments.buttonSecondary;
            abilityBIcon.sprite = combatAbilitySlotB.abilityIcon;
            abilityBIcon.color = new Color(1, 1, 1, 1);
            ability.charge = 0; // set charge to 0 so that the ability doesn't activate
        }
        else
        {
            try
            {
                abilityBIcon.color = new Color(0, 0, 0, 0);
                abilityBIcon.sprite = null; // remove icon
            }
            catch { }
        }
    }

    private void FixedUpdate()
    {
        // update UI every frame
        UpdateUI();
    }

    // public void for picking up combat abilities
    public void PickupAbility(bool slotA, CombatAbility_Item abilityItem)
    {
        if (slotA)
        {
            // if we have an ability already, drop it as an item and set it to null
            if (combatAbilitySlotAinstance != null)
            {
                Instantiate(emptyCombatAbilityItem, transform.position, Quaternion.identity, null).GetComponent<CombatAbility_Item>().ability = combatAbilitySlotA;
                combatAbilitySlotAinstance = null;
            }

            // set icon
            abilityIconSpriteA = abilityItem.abilityIcon;
            abilityAIcon.sprite = abilityIconSpriteA;
            // then set our ability to the new ability
            combatAbilitySlotA = abilityItem.ability;  
            // refresh our ability to save it
            RefreshAbilities();
            // then destroy the leftover item
            Destroy(abilityItem.gameObject);
        }

        if (!slotA)
        {
            // if we have an ability already, drop it as an item and set it to null
            if (combatAbilitySlotBinstance != null)
            {
                Instantiate(emptyCombatAbilityItem, transform.position, Quaternion.identity, null).GetComponent<CombatAbility_Item>().ability = combatAbilitySlotBinstance;
                combatAbilitySlotBinstance = null;
            }

            // set icon
            abilityIconSpriteB = abilityItem.abilityIcon;
            abilityBIcon.sprite = abilityIconSpriteB;
            // then set our ability to the new ability
            combatAbilitySlotB = abilityItem.ability;
            // refresh our ability to save it
            RefreshAbilities();
            // then destroy the leftover item
            Destroy(abilityItem.gameObject);
        }
    }

    // UI update
    void UpdateUI()
    {
        // set charge values
        if (combatAbilitySlotAinstance != null)
        {
            abilityASlider.value = combatAbilitySlotAinstance.charge / combatAbilitySlotAinstance.chargeMax;
            abilityAIcon.color = new Color(1, 1, 1, 1);
        }
        else
        {
            try
            {
                abilityASliderImage.color = new Color(0, 0, 0, 0);
                abilityAIcon.color = new Color(0, 0, 0, 0);
            } catch { }
        }

        if (combatAbilitySlotBinstance != null)
        {
            abilityBSlider.value = combatAbilitySlotBinstance.charge / combatAbilitySlotBinstance.chargeMax;
            abilityBIcon.color = new Color(1, 1, 1, 1);
        }
        else
        {
            try
            {
                abilityBSliderImage.color = new Color(0, 0, 0, 0);
                abilityBIcon.color = new Color(0, 0, 0, 0);
            } catch {}
        }

        // if they are full, change color

        if (combatAbilitySlotAinstance)
        {
            if (abilityASlider.value == 1)
                abilityASliderImage.color = combatAbilitySlotAinstance.readyColor;

            if (abilityASlider.value != 1)
                abilityASliderImage.color = combatAbilitySlotAinstance.chargingColor;
        }

        if (combatAbilitySlotBinstance)
        {
            if (abilityBSlider.value == 1)
                abilityBSliderImage.color = combatAbilitySlotBinstance.readyColor;

            if (abilityBSlider.value != 1)
                abilityBSliderImage.color = combatAbilitySlotBinstance.chargingColor;
        }
    }
}