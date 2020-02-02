using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniGame1Item : MonoBehaviour{
    public float speed = 0.5f;
    public int type;

    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector2.down * Time.unscaledDeltaTime * speed);
    }

    private void OnTriggerEnter2D(Collider2D other){
        if(other.name == "KillZone")
            Destroy(gameObject);
        else if (other.transform.parent.name == "WateringCan"){
            MiniGame1Player player = other.transform.parent.GetComponent<MiniGame1Player>();
            player.CheckValidity(type);
            Destroy(gameObject);
        }
    }
}
