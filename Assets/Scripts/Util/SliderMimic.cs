using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SliderMimic : MonoBehaviour
{
    [SerializeField] Slider mimic, ourSlider;

    private void FixedUpdate()
    {
        ourSlider.value = mimic.value;
    }
}
