using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour{
    
    [Range(0.1f, 20f)]
    public float movementSpeed;

    //Local References of player properties
    private Rigidbody2D rb;
    private Animator anim;
    private SpriteRenderer _renderer;
    private Item[] inventory = new Item[2];
    private int selectedSlot = 0;
    public Item selectedItem{
        get{ return inventory[selectedSlot]; }
    }

    private bool canHasWater;

    void Awake(){
        rb = gameObject.GetComponent<Rigidbody2D>();
        anim = gameObject.GetComponent<Animator>();
        _renderer = gameObject.GetComponent<SpriteRenderer>();
    }
    
    void Update(){
        if (LevelManager.instance.miniGameIsOpened){
            rb.velocity = Vector2.zero;
            return;
        }
        if (Time.timeScale > 0 && (Mathf.Abs(Input.GetAxisRaw("Horizontal")) == 1 || Mathf.Abs(Input.GetAxisRaw("Vertical")) == 1))
            rb.velocity = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")) * movementSpeed;
        else
            rb.velocity = Vector2.zero;
        if (Input.GetButtonDown("Action3")){
            selectedSlot = selectedSlot == 0 ? 1 : 0;
        }
    }

    public void SetStartPosition(Vector2 position){
        transform.position = position;
    }

    public void EnterInArea(LinkedDirection fromDirection){
        switch (fromDirection){
            case LinkedDirection.up:
                transform.position += (Vector3) (Vector2.up * _renderer.bounds.extents) * 3.5f;
                break;
            case LinkedDirection.right:
                transform.position += (Vector3) (Vector2.right * _renderer.bounds.extents) * 3.5f;
                break;
            case LinkedDirection.down:
                transform.position += (Vector3) (Vector2.down * _renderer.bounds.extents) * 3.5f;
                break;
            case LinkedDirection.left:
                transform.position += (Vector3) (Vector2.left * _renderer.bounds.extents) * 3.5f;
                break;
        }
    }

    //Retorna se há água no regador - retorna true padrão para testes
    public bool CanHasWater(){
        return canHasWater;
    }

    public void FillCan(){
        canHasWater = true;
    }

    public void EmptyCan(){
        canHasWater = false;
    }

    public void GetItem(Item item){
        if (inventory[selectedSlot] != null){
            inventory[selectedSlot].transform.parent = item.transform.parent;
            inventory[selectedSlot].transform.position = item.transform.position;
            inventory[selectedSlot].gameObject.SetActive(true);
        }
        inventory[selectedSlot] = item;
        inventory[selectedSlot].transform.parent = this.transform;
        inventory[selectedSlot].gameObject.SetActive(false);
    }
}
