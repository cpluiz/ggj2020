using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour{
    
    [Range(0.1f, 20f)]
    public float movementSpeed;

    //Local References of player properties
    private Rigidbody2D rb;
    private Animator anim;

    void Awake(){
        rb = gameObject.GetComponent<Rigidbody2D>();
        anim = gameObject.GetComponent<Animator>();
    }
    
    void Update(){
        if (Time.timeScale > 0 && (Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0))
            rb.velocity = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical")) * movementSpeed;
        else
            rb.velocity = Vector2.zero;
        Debug.Log(Input.GetAxis("Horizontal"));
    }
    
}
