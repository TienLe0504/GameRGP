using System.Text;
using UnityEngine;

public enum ItemType
{
    Material,
    Equiment
}

[CreateAssetMenu(fileName ="New Item Data", menuName ="Data/Item")]
public class ItemData : ScriptableObject
{
    public ItemType itemType;
    public string itemName;
    public Sprite itemIcon;

    [Range(0,100)]
    public float dopChance;

    protected StringBuilder sb = new StringBuilder();
    public virtual  string GetDescription()
    {
        return "";
    }
}
