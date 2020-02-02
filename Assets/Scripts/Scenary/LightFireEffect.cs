using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightFireEffect : MonoBehaviour{
    [SerializeField]private SpriteRenderer[] lightSimulations;
    [SerializeField]private Color lightFire, strongFire, currentColor;

    void Awake(){
        currentColor = lightFire;
    }

    void Update(){
        if (!LevelManager.instance.enableFireEffect){
            foreach (SpriteRenderer light in lightSimulations){
                light.color = Color.clear;
            }
            return;
        }
        currentColor = Color.Lerp(lightFire, strongFire, Mathf.PingPong(Time.time, 1));
        foreach (SpriteRenderer light in lightSimulations){
            light.color = currentColor;
        }
    }
}
