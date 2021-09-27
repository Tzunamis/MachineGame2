using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class NewMovement : MonoBehaviour
{
    [SerializeField]
    private float Speed = 5;
    [SerializeField]
    private float Direction = 0; 
    [SerializeField]
    private float Walkinput;
    Controller controls;
    public Rigidbody2D Ribody;

    private void Awake()
    {
        controls = new Controller();
        controls.Enable();

        controls.Players.Movements.performed += context =>
        {
            Direction = context.ReadValue<float>();
        };
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Ribody.velocity = new Vector2(Direction * Speed * Time.deltaTime, Ribody.velocity.y);
    }

    //public void Walk(InputAction.CallbackContext walk) 
    //{
        
    //   Walkinput = walk.ReadValue<float>();
    
   // }
    


}
