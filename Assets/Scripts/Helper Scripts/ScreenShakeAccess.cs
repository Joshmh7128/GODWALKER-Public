using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenShakeAccess : MonoBehaviour
{
    [SerializeField] CameraScript mainCamera; // our main camera

    public void ScreenShake(float shakeDuration)
    {
        mainCamera.shakeDuration += shakeDuration;
    }
}
