using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Item Data", menuName = "Data/Item effect")]

public class ItemEffect : ScriptableObject
{
    public virtual void ExcuteEffet(Transform _enemyPosition)
    {
        Debug.Log("Effect executed!");
    }
}
