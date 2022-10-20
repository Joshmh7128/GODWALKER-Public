using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BatteryHandler : MonoBehaviour
{
    /// this script handles each individual battery
    /// it calculates charge, changes the charge color and scale, and enables and disables the associated elements
    /// 

    [SerializeField] float charge; // current charge
    [SerializeField] float chargeMax; // our maximum charge
    [SerializeField] Transform chargeTransform; // the transform we'll be adjusting the scale of depending on our charge amount
    [SerializeField] Color fullColor, emptyColor; // colors we lerp between
    [SerializeField] Renderer matRend; // our renderer

    [SerializeField] List<GameObject> gameObjects = new List<GameObject>();

    // runs every frame
    private void FixedUpdate()
    {
        ProcessCharge();
    }

    // every frame calculate the charge
    void ProcessCharge()
    {
        // calculate the change in charge
        if (charge >= 0)
        {
            charge -= Time.deltaTime;
        }

        if (charge <= 0)
        {
            // disable our objects
            foreach (GameObject obj in gameObjects)
            { obj.SetActive(false); }
        }

        // set the scale of the charge transform on a scale of 0 to 1 equivalent to the current charge out of its max
        float i = charge / chargeMax;
        chargeTransform.localScale = new Vector3(1, i, 1);

        // lerp our colors
        matRend.material.color = Color.Lerp(emptyColor, fullColor, i);
    }

    // set our charge to its maximum
    public void ChargeBattery()
    {
        Debug.Log("charging battery");
        charge = chargeMax; 

        // enable our objects
        foreach(GameObject obj in gameObjects)
        { obj.SetActive(true); }    
    }
}
