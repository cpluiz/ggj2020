using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniGame1Player : MonoBehaviour{
    
    
    [SerializeField] private GameObject waterArea;
    [SerializeField] private Transform waterCanImage;
    [SerializeField] private float wateringTime;
    [SerializeField] private MiniGame1Manager miniGameManager;
    private bool isRotating;

    void Awake(){
        waterCanImage.localRotation = Quaternion.identity;
    }

    void Update(){
        if(isRotating) return;
        if(Mathf.Abs(Input.GetAxisRaw("Horizontal")) == 1)
            transform.localScale = new Vector3(Input.GetAxisRaw("Horizontal") * -1, 1, 1);

        if (Input.GetButtonDown("Action1") && !isRotating && miniGameManager.charges > 0)
            StartCoroutine(RotateWaterCan());
    }

    //type 0 = error
    //type 1 = plant
    //type 2 = carnivour
    public void CheckValidity(int type){
        if (type == 0 || (type == 1 && miniGameManager.correctIsCarnivour) || (type == 2 && !miniGameManager.correctIsCarnivour)){
            miniGameManager.errors++;
        }else{
            miniGameManager.corrects++;
        }
    }

    private IEnumerator RotateWaterCan(){
        miniGameManager.charges--;
        isRotating = true;
        waterCanImage.Rotate(Vector3.forward, 43 * transform.localScale.x);
        waterArea.gameObject.SetActive(true);
        yield return new WaitForSecondsRealtime(wateringTime);
        waterArea.gameObject.SetActive(false);
        waterCanImage.Rotate(Vector3.forward, -43 * transform.localScale.x);
        yield return new WaitForSecondsRealtime(wateringTime);
        isRotating = false;
    }
}
