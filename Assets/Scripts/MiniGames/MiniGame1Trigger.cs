using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniGame1Trigger : MonoBehaviour{
    public bool isCarnivourMinigame;
    [SerializeField]private bool insideTrigger;
    private void OnTriggerEnter2D(Collider2D other){
        if (other.gameObject.CompareTag("Player")){
            insideTrigger = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other){
        if (other.gameObject.CompareTag("Player"))
            insideTrigger = false;
    }

    void Update(){
        if (insideTrigger && Input.GetButtonDown("Action1") && !LevelManager.instance.miniGameIsOpened){
            LevelManager.instance.StartMinigame1(isCarnivourMinigame);
        }
    }
}
