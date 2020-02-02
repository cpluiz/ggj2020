using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour{
    private bool playerTouchingItem;
    private PlayerController player;



    void Update(){
        if (playerTouchingItem && Input.GetButton("Action2")){
            player.GetItem(this);
        }
    }

    private void OnTriggerEnter2D(Collider2D other){
        if (other.CompareTag("Player")){
            player = other.GetComponent<PlayerController>();
            playerTouchingItem = true;
        }
            
    }
    
    
    private void OnTriggerExit2D(Collider2D other){
        if (other.CompareTag("Player")){
            player = null;
            playerTouchingItem = false;
        }
            
    }
}
