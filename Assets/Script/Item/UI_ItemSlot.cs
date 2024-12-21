using TMPro;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.EventSystems;

public class UI_ItemSlot : MonoBehaviour, IPointerDownHandler, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] protected Image itemImage;
    [SerializeField] protected TextMeshProUGUI itemText;
    public InventoryItem item;
    protected UI ui;

    public void UpdateSlot(InventoryItem _newItem)
    {
        item = _newItem;
        itemImage.color = Color.white;
        if (item != null)
        {
            itemImage.sprite = item.data.itemIcon;
            if (item.stackSize > 1)
            {
                itemText.text = item.stackSize.ToString();
            }
            else
            {
                itemText.text = "";
            }
        }
        else
        {
            itemImage.sprite = null;
            itemText.text = "";
        }
    }

    protected virtual void Start()
    {
        ui = GetComponentInParent<UI>();
        if (ui == null)
        {

        }
    }

    public virtual void OnPointerDown(PointerEventData eventData)
    {
        if (item == null)
        {
            return;
        }

        if (item.data == null)
        {
            return;
        }

        if (Input.GetKey(KeyCode.LeftControl))
        {
            Inventory.instance.RemoveItem(item.data);
            return;
        }

        if (item.data.itemType == ItemType.Equiment)
        {
            Inventory.instance.EquipItem(item.data);
        }
        ui.itemToolTip.HideToolTip();
    }

    public void CleanUpSlot()
    {
        item = null;
        itemImage.sprite = null;
        itemImage.color = Color.clear;
        itemText.text = "";
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (ui == null)
        {
            return;
        }

        if (item == null)
        {
            return;
        }
        Vector2 mousePostition = Input.mousePosition;
        float xOffset = 0;
        float yOffset = 0;
        if (mousePostition.x > 380)
            xOffset = -150;
        else
            xOffset = 150;
        if (mousePostition.y > 320)
            yOffset = -150;
        else
        {
            yOffset = 150;
        }


        ItemData_Equiment equimentItem = item.data as ItemData_Equiment;
        if (equimentItem == null)
        {
            return;
        }

        ui.itemToolTip.ShowToolTip(equimentItem);
        ui.itemToolTip.transform.position = new Vector2(mousePostition.x + xOffset, mousePostition.y + yOffset);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (ui == null)
        {
            return;
        }

        ui.itemToolTip.HideToolTip();
    }
}
