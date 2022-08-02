using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WeaponUIHandler : MonoBehaviour
{
    // this script serves as a handler for dynamic weapon UI. it uses weapon class data to display UI information

    WeaponClass weaponClass; // the weaponclass of our weapon

    // the set of UI elements that we can use to represent our weapon
    [SerializeField] Slider ammoSlider; // our weapon's slider
    [SerializeField] Transform reticleLeft, reticleRight, reticleTop, reticleBottom; // all four of our reticle lines 
    Vector3 reticleLeftOrigin, reticleRightOrigin, reticleTopOrigin, reticleBottomOrigin; // the four origin points of our reticles
    [SerializeField] float reticleSpreadResponseMagnitude; // how much our reticles represent the spread

    // run on start to setup our weapon
    void SetupUI()
    {
        // get our weaponClass
        weaponClass = GetComponent<WeaponClass>();
        // setup our reticle points
        reticleLeftOrigin = reticleLeft.position;
        reticleRightOrigin = reticleRight.position;
        reticleTopOrigin = reticleTop.position;
        reticleBottomOrigin = reticleBottom.position;
    }

    // run the UI
    void ProcessUI()
    {
        // ammo slider
        if (ammoSlider != null)
        {
            // set the values of the slider
            ammoSlider.maxValue = weaponClass.maxMagazine;
            ammoSlider.value = weaponClass.currentMagazine;
        }

        // dynamic reticle lerping
        reticleRight.position = new Vector3(reticleRight.position.x + weaponClass.spreadX * reticleSpreadResponseMagnitude, reticleRight.position.y, reticleRight.position.z);
        reticleLeft.position = new Vector3(reticleLeft.position.x + -weaponClass.spreadX * reticleSpreadResponseMagnitude, reticleLeft.position.y, reticleLeft.position.z);

    }

    private void Start()
    {
        SetupUI();
    }

    private void Update()
    {
        // process our UI
        ProcessUI();
    }

}
