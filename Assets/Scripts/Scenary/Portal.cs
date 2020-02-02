using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Portal : MonoBehaviour{
    public LinkedDirection direction;
    private Area parentArea;

    void Awake(){
        parentArea = transform.parent.GetComponent<Area>();
    }

    private void OnTriggerEnter2D(Collider2D other){
        parentArea.MoveCameraToArea(direction);
    }
}
