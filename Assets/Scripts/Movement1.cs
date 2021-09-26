using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class Movement1 : MonoBehaviour
{
  
    private CharacterController controller;
    private Vector3 playerVelocity;
    private bool groundedPlayer;
    [SerializeField]
    private float playerSpeed = 2.0f;
    [SerializeField]
    private float jumpHeight = 1.0f;
    [SerializeField]
    private float gravityValue = -9.81f;

    private Vector2 MoveInput = Vector2.zero;
    private bool Jump = false;
    
    
    
    private void Start()
    {
        controller = gameObject.GetComponent<CharacterController>();
    }

    public void OnMovements(InputAction.CallbackContext moves)
    {
        MoveInput = moves.ReadValue<Vector2>();
    }

    public void OnJump(InputAction.CallbackContext jump) 
    {
        Jump = jump.ReadValue<bool>();
        Jump = jump.action.triggered;
    
    }


    void Update()
    {
        groundedPlayer = controller.isGrounded;
        if (groundedPlayer && playerVelocity.y < 0)
        {
            playerVelocity.y = 0f;
        }

        Vector3 move = new Vector3(MoveInput.x, 0,MoveInput.y);
        controller.Move(move * Time.deltaTime * playerSpeed);

        //if (move != Vector3.zero)
        //{
        //    gameObject.transform.forward = move;
        //}

        // Changes the height position of the player..
        if (Jump && groundedPlayer)
        {
            playerVelocity.y += Mathf.Sqrt(jumpHeight * -3.0f * gravityValue);
        }

        playerVelocity.y += gravityValue * Time.deltaTime;
        controller.Move(playerVelocity * Time.deltaTime);
    }
}

