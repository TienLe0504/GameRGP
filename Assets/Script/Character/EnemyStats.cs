using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStats : CharacterStats
{
    [Header("Level details")]
    [SerializeField] private int level = 1;
    [Range(0f, 1f)]
    [SerializeField] private float percantageModifier = 0.4f;



    private ItemDrop myDropSystem;
    private Enemy enemy;
    // Start is called before the first frame update
    protected override void Start()
    {
        ApplyLevelModifiers();
        base.Start();
        enemy = GetComponent<Enemy>();
        myDropSystem = GetComponent<ItemDrop>();
    }

    private void ApplyLevelModifiers()
    {
        Modify(strength);
        Modify(agility);
        Modify(intelligence);
        Modify(vitality);
      
        Modify(damage);
        Modify(critChance);
        Modify(critPower);

        Modify(maxHealth);
        Modify(armor);
        Modify(evasion);
        Modify(magicResistance);

        Modify(fireDamage);
        Modify(iceDamage);
        Modify(lightingDamage);
    }

    private void Modify(Stat _stat)
    {
        for(int i = 0; i < level; i++)
        {
            float modifier = _stat.GetValue() * percantageModifier;
            _stat.AddModifier(Mathf.RoundToInt(modifier));
        }
    }

    public override void TakeDamge(int _damgage)
    {
        base.TakeDamge(_damgage);
        //enemy.DamageEffect();
    }
    protected override void Die()
    {
        base.Die();
        enemy.Die();
        myDropSystem.GenerateDrop();
    }
}
