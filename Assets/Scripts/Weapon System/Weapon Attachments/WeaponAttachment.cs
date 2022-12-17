using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class WeaponAttachment : MonoBehaviour
{
    /// this is the paretn class for all weapon attachments
    /// it holds basic info that we can use to interact with weapons through attachments
    /// 

    protected bool equipped; // are we equipped?
    public WeaponClass weaponClass; // the weapon class we're interacting with

    private void Start()
    {
        foreach (GameObject weapon in PlayerWeaponManager.instance.weapons)
        {
            equipped = false;
            if (weapon == weaponClass.gameObject)
            {
                equipped = true;
                break;
            }
        }
    }

    public void Update()
    {
        if (equipped)
        InputCollection();
    }

    // where we get all our inputs
    void InputCollection()
    {
        // mouse
        if (Input.GetMouseButtonDown(0)) MouseButtonDown0();
        if (Input.GetMouseButtonDown(1)) MouseButtonDown1();
        if (Input.GetMouseButtonUp(0)) MouseButtonUp0();
        if (Input.GetMouseButtonUp(1)) MouseButtonUp1();
        
    }

    // our mouse inputs
    public virtual void MouseButtonDown0() { }
    public virtual void MouseButtonDown1() { }
    public virtual void MouseButtonUp0() { }
    public virtual void MouseButtonUp1() { }
}
