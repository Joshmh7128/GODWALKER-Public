using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationTestDisplay : MonoBehaviour
{
    Vector2 animationDirection;

    private void Update()
    {
        animationDirection = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        transform.localPosition = animationDirection * 200;
    }
}
