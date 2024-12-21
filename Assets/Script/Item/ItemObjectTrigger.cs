using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemObjectTrigger : MonoBehaviour
{
    private itemObject myItemObject => GetComponentInParent<itemObject>();
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<Player>() != null)
        {
            if (collision.GetComponent<CharacterStats>().isDead)
                return;
            Debug.Log("Picked up item children");
            myItemObject.PickupItem();
        }
    }
    //private void OnCollisionEnter2D(Collision2D collision)
    //{
    //    if (collision.gameObject.GetComponent<Player>() != null)
    //    {
    //        myItemObject.PickupItem();
    //    }
    //}


}
