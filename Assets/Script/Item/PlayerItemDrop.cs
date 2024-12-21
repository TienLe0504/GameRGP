using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerItemDrop : ItemDrop
{
    [Header("Player's drop")]
    [SerializeField] private float chanceToLooseItems;
    [SerializeField] private float chanceToLooseMaterials;


    public override void GenerateDrop()
    {
        Inventory inventory = Inventory.instance ;
        List<InventoryItem> itemstoUnequip = new List<InventoryItem> ();
        List<InventoryItem> materialsToLoose = new List<InventoryItem>();
        foreach (InventoryItem item in inventory.GetEquipmentList())
        {
            if(Random.Range(0, 100) <= chanceToLooseItems)
            {
                DropItem(item.data);
                itemstoUnequip.Add(item);
                //inventory.UnEquipment(item.data as ItemData_Equiment);
            }
        }
        for(int i = 0;i< itemstoUnequip.Count; i++)
        {
            inventory.UnEquipment(itemstoUnequip[i].data as ItemData_Equiment);
        }
        foreach(InventoryItem item in inventory.GetStashList())
        {
            if(Random.Range(0,100)<= chanceToLooseMaterials)
            {
                DropItem (item.data);
                materialsToLoose.Add(item);
            }
        }
        for (int i = 0; i < materialsToLoose.Count; i++)
        {
            inventory.RemoveItem(materialsToLoose[i].data);
        }
    }
}
