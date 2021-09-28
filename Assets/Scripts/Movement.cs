using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Movement : MonoBehaviour
{
    public float speed = 2.0f;
    [SerializeField] private float walk;
    
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    private void Move(InputAction.CallbackContext walk)
    {
        
        
        
        
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            transform.Translate(Vector3.left * speed *Time.deltaTime );
        }
        else if (Input.GetKey(KeyCode.RightArrow)) 
        {
            transform.Translate(Vector3.right * speed * Time.deltaTime);
        }
        if (Input.GetKey(KeyCode.UpArrow)) 
        {
            transform.Translate(Vector3.up * speed * Time.deltaTime);

        
        }
        else if (Input.GetKey(KeyCode.DownArrow)) 
        {

            transform.Translate(Vector3.down * speed * Time.deltaTime);
        
        }


        
    }

   









}
