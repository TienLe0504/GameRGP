using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UI_ItemToolTip : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI itemNameText;
    [SerializeField] private TextMeshProUGUI itemTypeText;
    [SerializeField] private TextMeshProUGUI itemDescription;
    [SerializeField] private int deafaultFontSize = 32;
    void Start()
    {
        
    }

    public void ShowToolTip(ItemData_Equiment item)
    {
        if (item == null)
        {
            return;
        }

        if (itemNameText == null)
        {
            return;
        }

        if (itemTypeText == null)
        {
            return;
        }

        itemNameText.text = item.itemName;
        itemTypeText.text = item.equimentType.ToString();
        itemDescription.text = item.GetDescription();

        if (itemNameText.text.Length > 12)
        {
            itemNameText.fontSize = itemNameText.fontSize * .7f;
        }
        else
        {
            itemNameText.fontSize = deafaultFontSize;
        }
        gameObject.SetActive(true);
    }

    public void HideToolTip() 
    {
        itemNameText.fontSize = deafaultFontSize;
        gameObject.SetActive(false);
    }
}
