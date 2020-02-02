using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemGoal : MonoBehaviour
{
    private bool playerTouchingItem;
    private PlayerController player;

    void Update(){
        if (playerTouchingItem && player.selectedItem != null && player.selectedItem.CompareTag("WaterCan")){
            player.FillCan();
        }
    }
}
