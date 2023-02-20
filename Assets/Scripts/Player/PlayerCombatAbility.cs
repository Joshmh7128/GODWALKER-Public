using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// class which defines all the parameters of a combat ability
public abstract class PlayerCombatAbility : MonoBehaviour
{
    public string abilityName, abilityInfo; // our name and info

    public enum ButtonAssignments { buttonPrimary, buttonSecondary }
    public ButtonAssignments buttonAssignment; // which button are we using?

    // can be overriden if we need it to be, checks the button input assignment
    public virtual void ProcessInput()
    {
        switch (buttonAssignment)
        {
            case ButtonAssignments.buttonPrimary:
                if (Input.GetKey(KeyCode.F))
                    UseMain();
                break;

            case ButtonAssignments.buttonSecondary:
                if (Input.GetKey(KeyCode.Q))
                    UseMain();
                break;
        }
    }

    public virtual void Update()
    {
        ProcessInput();
    }

    public virtual void UseMain()
    {
        // this function is overridable for abilities
    }
}
