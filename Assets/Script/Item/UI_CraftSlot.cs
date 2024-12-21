using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UI_CraftSlot : UI_ItemSlot
{
    protected override void Start()
    {
        base.Start();

    }
    public void SetupCraftSlot(ItemData_Equiment _data)
    {
        if (_data == null)
            return;
        item.data = _data;
        itemImage.sprite = _data.itemIcon;
        itemText.text = _data.itemName;
        if (itemText.text.Length > 12)
            itemText.fontSize = itemText.fontSize * .7f;
        else
            itemText.fontSize = 24;

    }


    public override void OnPointerDown(PointerEventData eventData)
    {
        //inventory craft item data
        //ItemData_Equiment craftData = item.data as ItemData_Equiment;
        //Inventory.instance.CanCraft(craftData, craftData.craftingMaterials);
        ui.craftWindow.SetupCraftIndow(item.data as ItemData_Equiment);
    }
}
