using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class WeaponAttachment : MonoBehaviour
{
    /// this is the paretn class for all weapon attachments
    /// it holds basic info that we can use to interact with weapons through attachments
    /// 

    public WeaponClass weaponClass; // the weapon class we're interacting with

    public void Update()
    {
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
    public abstract void MouseButtonDown0(); 
    public abstract void MouseButtonDown1(); 
    public abstract void MouseButtonUp0(); 
    public abstract void MouseButtonUp1(); 
}
