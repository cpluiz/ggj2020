using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniGame1Trigger : MonoBehaviour{
    public bool isCarnivourMinigame, correctIsCarnivo;
    [SerializeField]private bool insideTrigger;
    private PlayerController player;
    private void OnTriggerEnter2D(Collider2D other){
        if (other.gameObject.CompareTag("Player")){
            player = other.GetComponent<PlayerController>();
            insideTrigger = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other){
        if (other.gameObject.CompareTag("Player"))
            insideTrigger = false;
    }

    void Update(){
        if (insideTrigger && Input.GetButtonDown("Action1") && !LevelManager.instance.miniGameIsOpened){
//            if(player.CanHasWater() && player.selectedItem.CompareTag("Seed"))
                LevelManager.instance.StartMinigame1(isCarnivourMinigame, correctIsCarnivo);
        }
    }
}
