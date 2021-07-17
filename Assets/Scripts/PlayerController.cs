using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // controller values
    [SerializeField] float moveSpeed;
    [SerializeField] float gravity;
    [SerializeField] Transform playerHead;
    [SerializeField] CharacterController characterController;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        characterController.Move(new Vector3(0f, gravity, 0f) * Time.deltaTime);
        // declare our motion
        Vector3 moveV = playerHead.forward*Input.GetAxis("Vertical");
        Vector3 moveH = playerHead.right*Input.GetAxis("Horizontal");
        Vector3 move = new Vector3(moveH.x, 0f, moveH.z) + new Vector3(moveV.x, 0f, moveV.z);
        // apply to the character controller
        characterController.Move(move * Time.deltaTime * moveSpeed);
    }
}
