using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class itemObject : MonoBehaviour
{
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private ItemData itemData;



    private void SetupVisuals()
    {
        if (itemData == null)
            return;
        GetComponent<SpriteRenderer>().sprite = itemData.itemIcon;
        gameObject.name = "Item object - " + itemData.itemName;
    }


    public void SetupItem(ItemData _itemData, Vector2 _velocity)
    {
        itemData = _itemData;
        rb.velocity = _velocity;
        SetupVisuals();
    }

    public void PickupItem()
    {
        if (!Inventory.instance.CanAddItem() && itemData.itemType == ItemType.Equiment)
        {
            rb.velocity = new Vector2(0, 7);
            return;
        }
        Inventory.instance.AddItem(itemData);
        Destroy(gameObject);
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.GetComponent<Player>() != null)
        {
            if (collision.gameObject.GetComponent<CharacterStats>().isDead)
                return;
            PlayerManager.instance.player.BecomeIdle();
            PickupItem();
        }
    }
}
