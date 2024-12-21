using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : CharacterStats
{
    private Player player;
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        player = GetComponent<Player>();
    }
    public override void TakeDamge(int _damgage)
    {
        base.TakeDamge(_damgage);
        player.DamageImpact();
    }
    protected override void Die()
    {
        base.Die();
        player.Die();
        GetComponent<PlayerItemDrop>()?.GenerateDrop();
    }
    public override void DoDamage(CharacterStats _taragetStats)
    {
        base.DoDamage(_taragetStats);
    }
    protected override void DecreaseHealthBy(int _damage)
    {
        base.DecreaseHealthBy(_damage);
        ItemData_Equiment currentArmor = Inventory.instance.GetEquipment(EquimentType.Armor);
        if(currentArmor != null)
        {
            currentArmor.Effect(player.transform);
        }
    }

    public void CloneDoDamage(CharacterStats _taragetStats, float _multiplier)
    {
        if (TargetCanAvoidAttack(_taragetStats))
            return;
        int totalDamage = damage.GetValue() + strength.GetValue();

        if (_multiplier > 0)
            totalDamage = Mathf.RoundToInt(totalDamage * _multiplier);

        if (CanCrit())
        {
            totalDamage = CalculateCriticalDamage(totalDamage);
        }

        totalDamage = CheckTargetAmor(_taragetStats, totalDamage);
        _taragetStats.TakeDamge(totalDamage);
        DoMagicalDamage(_taragetStats);
    }
}
