using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Move1 : MonoBehaviour
{
    public float Speed = 2.0f;
    private Vector2 moveInput = Vector2.zero;
    [SerializeField] private float walking;


    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        Move();
    }


    public void OnMove(InputAction.CallbackContext move)
    {
        moveInput = move.ReadValue<Vector2>();
    }


    private void Move()
    {





        if (Input.GetKey(KeyCode.LeftArrow))
        {
            transform.Translate(Vector3.left * Speed * Time.deltaTime);
        }
        else if (Input.GetKey(KeyCode.RightArrow))
        {
            transform.Translate(Vector3.right * Speed * Time.deltaTime);
        }
        if (Input.GetKey(KeyCode.UpArrow))
        {
            transform.Translate(Vector3.up * Speed * Time.deltaTime);


        }
        else if (Input.GetKey(KeyCode.DownArrow))
        {

            transform.Translate(Vector3.down * Speed * Time.deltaTime);

        }



    }

}
