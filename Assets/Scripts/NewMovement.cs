using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class NewMovement : MonoBehaviour
{
    [SerializeField] private float Speed = 5;
    [SerializeField] private float Direction = 0;
    [SerializeField] private float Walkinput;
    Controller controls;
    public Rigidbody2D Ribody;
    [SerializeField] private Controller NewController;
    [SerializeField] private GameObject Player;
    private Vector3 Movedir = Vector3.zero;
    private Vector2 InputVector = Vector2.zero;

    private void Awake()
    {
       
        controls.Enable();

        controls.Players.Movements.performed += context =>
        {
            Direction = context.ReadValue<float>();
        };

        Ribody = GetComponent<Rigidbody2D>();

        NewController = new Controller();
        Player = GetComponent<GameObject>();
    
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //Ribody.velocity = new Vector2(Direction * Speed * Time.deltaTime, Ribody.velocity.y);
        Vector3 moves = new Vector3(InputVector.x, InputVector.y, 0) * Speed * Time.deltaTime;
        transform.Translate(moves);
    
    }

    public void Walk(InputAction.CallbackContext walk) 
    {
        
       Walkinput = walk.ReadValue<float>();
    
    }
    


}
