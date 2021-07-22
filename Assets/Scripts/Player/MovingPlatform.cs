using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    Rigidbody ourRigidbody;
    public Vector3 targetPos; 
    [SerializeField] Vector3 movementDirection; 
    [SerializeField] CharacterController characterController;
    [SerializeField] float platformSpeed;

    private void Start()
    {

    }

    private void LateUpdate()
    {
        if ( Mathf.Abs(Vector3.Distance(targetPos, transform.position)) > 1f)
        {
            movementDirection = targetPos - transform.position;
        }
        else
        {
            movementDirection = Vector3.zero;
        }
        // make sure we only work in direction units of 1
        Vector3 normalizedMovementDirection;
        normalizedMovementDirection = movementDirection.normalized;
        Vector3 finalizedMovementDirection;
        finalizedMovementDirection = new Vector3(normalizedMovementDirection.x * platformSpeed, normalizedMovementDirection.y * platformSpeed, normalizedMovementDirection.z * platformSpeed);

        transform.Translate(finalizedMovementDirection * Time.fixedDeltaTime);
        characterController.Move(finalizedMovementDirection * Time.fixedDeltaTime);
    }
}