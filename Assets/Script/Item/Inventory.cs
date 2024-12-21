using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEditor.Timeline.Actions.MenuPriority;

public class Inventory : MonoBehaviour
{
    public static Inventory instance;

    public List<InventoryItem> equipment;
    public Dictionary<ItemData_Equiment, InventoryItem> equimentDictionary;
    public List<ItemData> startingItem;

    public List<InventoryItem> inventory;
    public Dictionary<ItemData, InventoryItem> inventoryDictionanry;
    public List<InventoryItem> stash;
    public Dictionary<ItemData, InventoryItem> stashDictionanry;

    [Header("Inventory UI")]
    [SerializeField] private Transform inventorySlotParent;
    [SerializeField] private Transform stashSlotParent;
    [SerializeField] private Transform equimentSlotParent;
    [SerializeField] private Transform statSlotParent;



    private UI_ItemSlot[] inventoryItemSlot;
    private UI_ItemSlot[] statshItemSlot;
    private UI_EquimentSlot[] equimentSlot;
    private UI_StatSlot[] statSlot;

    [Header("Items cooldown")]
    private float lastTimeUsedFask;
    private float lastIimeUsedArmor;

    private float flaskCooldown;
    private float armorCooldown;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }
    void Start()
    {
        inventory = new List<InventoryItem>();
        inventoryDictionanry = new Dictionary<ItemData, InventoryItem>();

        stash = new List<InventoryItem>();
        stashDictionanry = new Dictionary<ItemData, InventoryItem>();


        equipment = new List<InventoryItem>();
        equimentDictionary = new Dictionary<ItemData_Equiment, InventoryItem>();

        inventoryItemSlot = inventorySlotParent.GetComponentsInChildren<UI_ItemSlot>();
        statshItemSlot = stashSlotParent.GetComponentsInChildren<UI_ItemSlot>();
        equimentSlot = equimentSlotParent.GetComponentsInChildren<UI_EquimentSlot>();
        statSlot = statSlotParent.GetComponentsInChildren<UI_StatSlot>();
        AddStartingItem();
    }

    private void AddStartingItem()
    {
        for (int i = 0; i < startingItem.Count; i++)
        {
            if(startingItem!=null)
                AddItem(startingItem[i]);
        }
    }

    public void EquipItem(ItemData _item)
    {
        ItemData_Equiment newEquiment = _item as ItemData_Equiment;
        InventoryItem newItem = new InventoryItem(newEquiment);
        ItemData_Equiment oldEquiment = null;
        foreach(KeyValuePair<ItemData_Equiment, InventoryItem> item in equimentDictionary)
        {
            if(item.Key.equimentType == newEquiment.equimentType)
            {
                oldEquiment = item.Key;
            }
        }
        if (oldEquiment != null)
        {
            UnEquipment(oldEquiment);
            AddItem(oldEquiment);
        }

        equipment.Add(newItem);
        equimentDictionary.Add(newEquiment, newItem);
        newEquiment.AddModifiers();
        RemoveItem(_item);
        UpdateSlotUI();
    }

    public void UnEquipment(ItemData_Equiment itemToRemove)
    {
        if (equimentDictionary.TryGetValue(itemToRemove, out InventoryItem value))
        {
            equipment.Remove(value);
            equimentDictionary.Remove(itemToRemove);
            itemToRemove.RemoveModifiers();
        }
    }

    private void UpdateSlotUI()
    {

        for(int i = 0; i < equimentSlot.Length; i++)
        {
            foreach (KeyValuePair<ItemData_Equiment, InventoryItem> item in equimentDictionary)
            {
                if (item.Key.equimentType == equimentSlot[i].slotType)
                {
                    equimentSlot[i].UpdateSlot(item.Value);
                }
            }
        }

        for(int i =0; i< inventoryItemSlot.Length; i++)
        {
            inventoryItemSlot[i].CleanUpSlot();
        }

        for(int i =0;i< statshItemSlot.Length; i++)
        {
            statshItemSlot[i].CleanUpSlot();
        }

        for(int i =0;i<inventory.Count; i++)
        {
            inventoryItemSlot[i].UpdateSlot(inventory[i]);
        }

        for(int i =0; i < stash.Count; i++)
        {
            statshItemSlot[i].UpdateSlot(stash[i]);
        }

        for (int i =0;i<statSlot.Length; i++)
        {
            statSlot[i].UpdateStatValueUI();
        }
    }

    // Update is called once per frame
   
    public void AddItem(ItemData _item)
    {

        if (_item.itemType == ItemType.Equiment && CanAddItem())
        {
            AddToInventory(_item);

        }
        else if(_item.itemType == ItemType.Material)
        {
            AddToStash(_item);

        }
        UpdateSlotUI();
    }

    private void AddToStash(ItemData _item)
    {
        if (stashDictionanry.TryGetValue(_item, out InventoryItem value))
        {
            value.AddStack();
        }
        else
        {
            InventoryItem newItem = new InventoryItem(_item);
            stash.Add(newItem);
            stashDictionanry.Add(_item, newItem);
        }
    }

    private void AddToInventory(ItemData _item)
    {
        if (inventoryDictionanry.TryGetValue(_item, out InventoryItem value))
        {
            value.AddStack();
        }
        else
        {
            InventoryItem newItem = new InventoryItem(_item);
            inventory.Add(newItem);
            inventoryDictionanry.Add(_item, newItem);
        }
    }

    public void RemoveItem(ItemData _item)
    {
        if(inventoryDictionanry.TryGetValue(_item, out InventoryItem value))
        {
            if(value.stackSize<= 1)
            {
                inventory.Remove(value);
                inventoryDictionanry.Remove(_item);
            }
            else
            {
                value.RemoveStack();
            }
        }
        if(stashDictionanry.TryGetValue(_item, out InventoryItem stashValue))
        {
            if(stashValue.stackSize<= 1)
            {
                stash.Remove(stashValue);
                stashDictionanry.Remove(_item);
            }
            else
            {
                stashValue.RemoveStack();
            }
        }
        UpdateSlotUI();
    }

    public bool CanCraft(ItemData_Equiment _itemToCraft, List<InventoryItem> _requiredMaterials)
    {
        List<InventoryItem> materialsToMove = new List<InventoryItem>();
        for(int i = 0; i<  _requiredMaterials.Count; i++)
        {
            if (stashDictionanry.TryGetValue(_requiredMaterials[i].data, out InventoryItem stashValue))
            {
                if(stashValue.stackSize < _requiredMaterials[i].stackSize)
                {
                    return false;
                }
                else
                {
                    materialsToMove.Add(stashValue);
                }
            }
            else
            {
                return false;
            }
        }
        for(int i = 0; i< materialsToMove.Count; i++)
        {
            RemoveItem(materialsToMove[i].data);
        }
        AddItem(_itemToCraft);
        Debug.Log("Here is your item"+_itemToCraft.name);
        return true;
    }
    public List<InventoryItem> GetEquipmentList() => equipment;
    public List<InventoryItem> GetStashList() => stash;
    public ItemData_Equiment GetEquipment(EquimentType _type)
    {
        ItemData_Equiment equipedItem = null;
        foreach (KeyValuePair<ItemData_Equiment, InventoryItem> item in equimentDictionary)
        {
            if (item.Key.equimentType ==_type)
            {
                equipedItem = item.Key;
            }
        }
        return equipedItem;
    }

    public void UseFlask()
    {
        ItemData_Equiment currentFlask = GetEquipment(EquimentType.Flask);
        if (currentFlask == null)
            return;
        bool canUseFlask = Time.time > lastTimeUsedFask + flaskCooldown;
        if (canUseFlask)
        {
            flaskCooldown = currentFlask.itemCooldown;
            currentFlask.Effect(null);
            lastTimeUsedFask = Time.time;
        }
        else
        {
            Debug.Log("Flask on cooldown");
        }
    }
    public bool CanUseArmor()
    {
        ItemData_Equiment currentArmor = GetEquipment(EquimentType.Armor);
        if(Time.time>lastIimeUsedArmor + armorCooldown)
        {
            armorCooldown = currentArmor.itemCooldown;
            lastIimeUsedArmor = Time.time;
            return true;
        }
        return false;
    }
    public bool CanAddItem()
    {
        if (inventory.Count >= inventoryItemSlot.Length)
        {
            return false;
        }
        return true;
    }
 
}
