using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Thunder strike effect", menuName = "Data/Thunder strike")]

public class ThunderStrike_Effect : ItemEffect
{

    [SerializeField] private GameObject thunderStrikePrefab;
    public override void ExcuteEffet(Transform _enemyPosition)
    {
        GameObject newThunderStrike = Instantiate(thunderStrikePrefab,_enemyPosition.position,Quaternion.identity);
        newThunderStrike.SetActive(true);
        Destroy(newThunderStrike, 1);
    }
}
