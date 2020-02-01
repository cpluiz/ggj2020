using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour{
    [Header("Itens in inventory")]
    [SerializeField] private ItemResource _resource;
    [SerializeField] private ItemSeed _seed;

    [Header("Inventory Selector")] [SerializeField]
    private int _selectedSlot;
}
