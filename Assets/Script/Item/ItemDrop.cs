using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemDrop : MonoBehaviour
{
    [SerializeField] private int possibleItemDrop;
    [SerializeField] private ItemData[] possibleDrop;
    private List<ItemData> dropList = new List<ItemData>(); 

    [SerializeField] private GameObject dropPrefab;

    //public virtual void GenerateDrop()
    //{
    //    for(int i = 0;i < possibleDrop.Length;i++)
    //    {
    //        if(Random.Range(0,100)<= possibleDrop[i].dopChance)
    //            dropList.Add(possibleDrop[i]);
    //    }

    //    for(int i = 0;i< possibleItemDrop; i++)
    //    {
    //        ItemData randomItem = dropList[Random.Range(0,dropList.Count-1)];
    //        dropList.Remove(randomItem);
    //        DropItem(randomItem);
    //    }
    //}
    public virtual void GenerateDrop()
    {
        for (int i = 0; i < possibleDrop.Length; i++)
        {
            if (Random.Range(0, 100) <= possibleDrop[i].dopChance)
                dropList.Add(possibleDrop[i]);
        }

        if (dropList.Count == 0)
        {
            return;
        }

        for (int i = 0; i < possibleItemDrop; i++)
        {
            if (dropList.Count == 0)
                break; 

            ItemData randomItem = dropList[Random.Range(0, dropList.Count)];
            dropList.Remove(randomItem);
            DropItem(randomItem);
        }
    }

    protected void DropItem(ItemData _itemData)
    {
        GameObject newDrop = Instantiate(dropPrefab, transform.position,Quaternion.identity);
        newDrop.SetActive(true);
        Vector2 randomVelocity = new Vector2(Random.Range(-5, 5), Random.Range(15, 20));
        newDrop.GetComponent<itemObject>().SetupItem(_itemData, randomVelocity);
    }
}
