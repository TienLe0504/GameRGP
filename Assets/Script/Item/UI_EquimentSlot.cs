using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UI_EquimentSlot : UI_ItemSlot
{
    public EquimentType slotType;
    private void OnValidate()
    {
        gameObject.name = "Equiment slot - " + slotType.ToString();
    }
    public override void OnPointerDown(PointerEventData eventData)
    {
        if (item == null)
        {
            return;
        }

        if (item.data == null)
        {
            return;
        }
        Inventory.instance.UnEquipment(item.data as ItemData_Equiment);
        Inventory.instance.AddItem(item.data as ItemData_Equiment);

        ui.itemToolTip.HideToolTip();

        CleanUpSlot();
    }
}
