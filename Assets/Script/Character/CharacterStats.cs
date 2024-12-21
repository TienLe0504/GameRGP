using System.Collections;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;

public enum StatType
{
    strength,
    agility,
    intelegence,
    vitality,
    damage,
    critChance,
    critPower,
    health,
    armor,
    evasion,
    magicRes,
    fireDamage,
    iceDamage,
    lightingDamage
}

public class CharacterStats : MonoBehaviour
{
    private EntityFx fx;


    [Header("Major stats")]
    public Stat strength; // 1 point increase damage by 1 and crit.power by 1%
    public Stat agility; // 1 point increase evasion by 1% and crit.change by 1%
    public Stat intelligence; // 1 point increase magic damgage by 1 and magic resistance by 3
    public Stat vitality; // 1 point increase health by 3 or 5 points

    [Header("Offensive stats")]
    public Stat damage;
    public Stat critChance;
    public Stat critPower;  // default value 150%


    [Header("Defensive stats")]
    public Stat maxHealth;
    public Stat armor;
    public Stat evasion;
    public Stat magicResistance;

    [Header("Magic stats")]
    public Stat fireDamage;
    public Stat iceDamage;
    public Stat lightingDamage;

    public bool isIgnited;
    public bool isChilled;
    public bool isShocked;


    [SerializeField] private float ailmentDuration = 4;
    private float ignitedTimer;
    private float chilledTimer;
    private float shockedTimer;


    private float ignitedDamageCoolDown = .3f;
    private float igniteDamageTimer;
    private int igniteDamage;
    private int shockDamage;
    [SerializeField] private GameObject shockTrikePrefab;

    public bool isDead { get; private set; }
    private bool isVulnerable;
    public int currentHealth;


    public System.Action onHealthChanged;


    // Start is called before the first frame update
    protected virtual void Start()
    {
        UpdateHealthUI();
        fx = GetComponent<EntityFx>();

    }

    private void UpdateHealthUI()
    {
        critPower.SetDefaultValue(150);
        currentHealth = GetMaxHealthValue();
    }

    protected virtual void Update()
    {
        ignitedTimer -= Time.deltaTime;
        chilledTimer -= Time.deltaTime;
        shockedTimer -= Time.deltaTime;
        igniteDamageTimer -= Time.deltaTime;

        if (ignitedTimer < 0)
            isIgnited = false;

        if (chilledTimer < 0)
            isChilled = false;

        if (shockedTimer < 0)
            isShocked = false;

        if(isDead)
            ApplyIgniteDamage();
    }

    public void MakeVulnerableFor(float _duration)
    {
        StartCoroutine(VulnerableCorutine(_duration));
    }

    private IEnumerator VulnerableCorutine(float _duration)
    {
        isVulnerable = true;
        yield return new WaitForSeconds(_duration);
        isVulnerable = false;
    }

    public virtual void IncreaseStatBy(int _modifier, float _duration, Stat _statModify)
    {
        StartCoroutine(StatMoCoroutien(_modifier, _duration, _statModify));
    }
    private IEnumerator StatMoCoroutien(int _modifier, float _duration, Stat _statModify)
    {
        _statModify.AddModifier(_modifier);
        yield return new WaitForSeconds(_duration);
        _statModify.RemoveModifier(_modifier);
    }


    private void ApplyIgniteDamage()
    {
        if (igniteDamageTimer < 0)
        {
            DecreaseHealthBy(igniteDamage);
            if (currentHealth < 0 && !isDead)
                Die();
            igniteDamageTimer = ignitedDamageCoolDown;
        }
    }

    public virtual void IncreaseHealthBy(int _amount)
    {
        currentHealth += _amount;
        Debug.Log("current health" + currentHealth);
        if (currentHealth > GetMaxHealthValue())
            currentHealth = GetMaxHealthValue();
        if (onHealthChanged != null)
            onHealthChanged();
    }

    public virtual void DoDamage(CharacterStats _taragetStats)
    {
        if (TargetCanAvoidAttack(_taragetStats))
            return;
        int totalDamage = damage.GetValue() + strength.GetValue();

        if (CanCrit())
        {
            totalDamage = CalculateCriticalDamage(totalDamage);
        }

        totalDamage = CheckTargetAmor(_taragetStats, totalDamage);
        _taragetStats.TakeDamge(totalDamage);
        DoMagicalDamage(_taragetStats);
    }

    #region Maigical damage and ailements
    public virtual void DoMagicalDamage(CharacterStats _targetStats)
    {
        int _fireDamage = fireDamage.GetValue();
        int _iceDamage = iceDamage.GetValue();
        int _ligthtingDamage = lightingDamage.GetValue();

        int totalMagicalDamage = _fireDamage + _iceDamage + _ligthtingDamage + intelligence.GetValue();
        totalMagicalDamage = CheckTargetReigistance(_targetStats, totalMagicalDamage);
        _targetStats.TakeDamge(totalMagicalDamage);

        if (Mathf.Max(_fireDamage, _iceDamage, _ligthtingDamage) <= 0)
            return;

        AttemptyToApplyAilements(_targetStats, _fireDamage, _iceDamage, _ligthtingDamage);

    }

    private static void AttemptyToApplyAilements(CharacterStats _targetStats, int _fireDamage, int _iceDamage, int _ligthtingDamage)
    {
        bool canApplyIgnite = _fireDamage > _iceDamage && _fireDamage > _ligthtingDamage;
        bool canApplyChill = _iceDamage > _fireDamage && _iceDamage > _ligthtingDamage;
        bool canApplyShock = _ligthtingDamage > _fireDamage && _ligthtingDamage > _iceDamage;


        while (!canApplyIgnite && !canApplyChill && !canApplyShock)
        {
            if (Random.value < .3f && _fireDamage > 0)
            {
                canApplyIgnite = true;
                _targetStats.ApplyAilments(canApplyIgnite, canApplyChill, canApplyShock);
                return;
            }
            if (Random.value < .5f && _iceDamage > 0)
            {
                canApplyChill = true;
                _targetStats.ApplyAilments(canApplyIgnite, canApplyChill, canApplyShock);
                return;
            }
            if (Random.value < .5f && _ligthtingDamage > 0)
            {
                canApplyShock = true;
                _targetStats.ApplyAilments(canApplyIgnite, canApplyChill, canApplyShock);
                return;
            }
        }
        if (canApplyShock)
            _targetStats.SetupShockTrikeDamage(Mathf.RoundToInt(_ligthtingDamage * .1f));

        if (canApplyIgnite)
            _targetStats.SetupIngiteDamage(Mathf.RoundToInt(_fireDamage * .2f));

        _targetStats.ApplyAilments(canApplyIgnite, canApplyChill, canApplyShock);
    }

    private static int CheckTargetReigistance(CharacterStats _targetStats, int totalMagicalDamage)
    {
        totalMagicalDamage -= _targetStats.magicResistance.GetValue() + (_targetStats.intelligence.GetValue() * 3);
        totalMagicalDamage = Mathf.Clamp(totalMagicalDamage, 0, int.MaxValue);
        return totalMagicalDamage;
    }

    public void ApplyAilments(bool _ignite, bool _chill, bool _shock)
    {
        bool canApplyIgnite = !isIgnited && !isChilled && !isShocked;
        bool canApplyChill = !isIgnited && !isChilled && !isShocked;
        bool canApplyShock = !isIgnited && !isChilled;
        if (_ignite && canApplyIgnite)
        {
            isIgnited = _ignite;
            ignitedTimer = ailmentDuration;
            fx.IgniteFxFor(ailmentDuration);
        }
        if (_chill && canApplyChill)
        {
            chilledTimer = ailmentDuration;
            isChilled = _chill;
            float slowPercentage = .2f;
            GetComponent<Entity>().SlowEntityBy(slowPercentage, ailmentDuration);
            fx.ChillFxFor(ailmentDuration);
        }
        if (_shock && canApplyShock)
        {
            if (!isShocked)
            {
                ApplyShock(_shock);

            }
            else
            {
                HitNearestTargetWIthShockStrike();
            }
        }
    }

    public void ApplyShock(bool _shock)
    {
        if (isShocked)
            return;
        shockedTimer = ailmentDuration;
        isShocked = _shock;
        fx.ShockFxFor(ailmentDuration);
    }

    private void HitNearestTargetWIthShockStrike()
    {
        if (GetComponent<Player>() != null)
            return;
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, 25);
        float closestDistance = Mathf.Infinity;
        Transform closestEnemy = null;
        foreach (var hit in colliders)
        {
            if (hit.GetComponent<Enemy>() != null && Vector2.Distance(transform.position, hit.transform.position) > 1)
            {
                float distanceToEnemy = Vector2.Distance(transform.position, hit.transform.position);
                if (distanceToEnemy < closestDistance)
                {
                    closestDistance = distanceToEnemy;
                    closestEnemy = hit.transform;
                }
            }
            if (closestEnemy == null)
                closestEnemy = transform;
        }

        if (closestEnemy != null)
        {
            GameObject newShockStrike = Instantiate(shockTrikePrefab, transform.position, Quaternion.identity);
            if (newShockStrike == null)
                return;
            newShockStrike.SetActive(true);
            newShockStrike.GetComponent<ShockStrike_Controller>().Setup(shockDamage, closestEnemy.GetComponent<CharacterStats>());
        }
    }


    public void SetupIngiteDamage(int _damage) => igniteDamage = _damage;
    public void SetupShockTrikeDamage(int _damage) => shockDamage = _damage;
    #endregion

    protected  int CheckTargetAmor(CharacterStats _taragetStats, int totalDamage)
    {
        if (_taragetStats.isChilled)
            totalDamage -= Mathf.RoundToInt(_taragetStats.armor.GetValue() * .8f);
        else
            totalDamage -= _taragetStats.armor.GetValue();

        totalDamage = Mathf.Clamp(totalDamage, 0, int.MaxValue);
        return totalDamage;
    }

    protected  bool TargetCanAvoidAttack(CharacterStats _taragetStats)
    {
        int totalEvasion = _taragetStats.evasion.GetValue() + _taragetStats.agility.GetValue();
        if (_taragetStats.isShocked)
            totalEvasion += 20;

        if (Random.Range(0, 100) < totalEvasion)
        {
            return true;
        }
        return false;
    }

    public virtual void TakeDamge(int _damgage)
    {
        DecreaseHealthBy(_damgage);
        GetComponent<Entity>().DamageImpact();
        fx.StartCoroutine("FlashFX");
        if (currentHealth < 0 && !isDead)
            Die();
    }


    protected virtual void DecreaseHealthBy(int _damage)
    {
        if (isVulnerable)
            _damage =Mathf.RoundToInt( _damage * 1.1f);

        currentHealth -= _damage;
        if (onHealthChanged != null)
            onHealthChanged();
    }

    protected virtual void Die()
    {
        isDead = true;
    }
    #region Stat calculate
    protected bool CanCrit()
    {
        int totalCriticalChance = critChance.GetValue() + agility.GetValue();
        if (Random.Range(0, 100) <= totalCriticalChance)
        {
            return true;
        }
        return false;
    }

    protected int CalculateCriticalDamage(int _damage)
    {
        float totalCirtPower = (critPower.GetValue() + strength.GetValue()) * .01f;
        float critDamage = _damage * totalCirtPower;
        return Mathf.RoundToInt(critDamage);
    }
    public int GetMaxHealthValue()
    {
        return maxHealth.GetValue() + vitality.GetValue() * 5;
    }
    #endregion



    public Stat GetStats( StatType _statType)
    {
        if (_statType == StatType.strength) return strength;
        else if (_statType == StatType.agility) return agility;
        else if (_statType == StatType.intelegence) return intelligence;
        else if (_statType == StatType.vitality) return vitality;
        else if (_statType == StatType.damage) return damage;
        else if (_statType == StatType.critChance) return critChance;
        else if (_statType == StatType.critPower) return critPower;
        else if (_statType == StatType.health) return maxHealth;
        else if (_statType == StatType.armor) return armor;
        else if (_statType == StatType.evasion) return evasion;
        else if (_statType == StatType.magicRes) return magicResistance;
        else if (_statType == StatType.fireDamage) return fireDamage;
        else if (_statType == StatType.iceDamage) return iceDamage;
        else if (_statType == StatType.lightingDamage) return lightingDamage;
        return null;
    }
}
