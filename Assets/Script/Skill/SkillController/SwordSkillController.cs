using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using UnityEngine;

public class SwordSkillController : MonoBehaviour
{
    private Animator anim;
    private Rigidbody2D rb;
    private CircleCollider2D cd;
    private bool canRotate = true;
    private Player player;
    private bool isReturning = false;

    private float freezeTimeDuration;
    private float returnSpeed = 12;

    [Header("PierceInfo")]
    private float piereceAmount;

    [Header("Bounce info")]
    private float bounceSpeed;
    private bool isBouncing;
    private int amountOfBounce;
    public List<Transform> enemyTarget;
    private int targetIndex;

    [Header("Spin info")]
    private float maxTravelDistance;
    private float spinDuration;
    private float spinTimer;
    private bool wasStopped;
    private bool isSpinning;

    private float hitTimer;
    private float hitCoolDown;

    private float spinDirection;

    private void Awake()
    {
        anim = GetComponentInChildren<Animator>();
        rb = GetComponent<Rigidbody2D>();
        cd = GetComponent<CircleCollider2D>();
    }
    private void DestroyMe()
    {
        Destroy(gameObject);
    }
    public void SetupSword(Vector2 _dir, float _gravityScale, Player _player, float _freeTimeDuration, float _returnSpeed)
    {
        returnSpeed = _returnSpeed;
        player = _player;
        freezeTimeDuration = _freeTimeDuration; 
        rb.velocity = _dir;
        rb.gravityScale = _gravityScale;
        if(piereceAmount<=0)
            anim.SetBool("Rotation", true);

        spinDirection = Mathf.Clamp(rb.velocity.x, -1, 1);
        Invoke("DestroyMe", 7);

    }

    public void SetUpBounce(bool _isBouncing, int _amountOfBounces,float _bounceSpeed)
    {
        isBouncing = _isBouncing;
        amountOfBounce = _amountOfBounces;
        enemyTarget = new List<Transform>();
        bounceSpeed = _bounceSpeed;
    }
    public void SetUpPierce(int _pierceAmount)
    {
        piereceAmount = _pierceAmount;
    }
    public void SetupSpin(bool _isSpinning, float _maxTravelDistance, float _spinDuration, float _hitCoolDown)
    {
        isSpinning = _isSpinning;
        spinDuration = _spinDuration;
        maxTravelDistance = _maxTravelDistance;
        hitCoolDown = _hitCoolDown;
    }

    public void ReturnSword()
    {
        rb.constraints = RigidbodyConstraints2D.FreezeAll;
        //rb.isKinematic = false;
        transform.parent = null;
        isReturning = true;
    }

    private void Update()
    {
        if (canRotate)
        {

            transform.right = rb.velocity.normalized;
        }
        if (isReturning)
        {
            transform.position = Vector2.MoveTowards(transform.position, player.transform.position, returnSpeed * Time.deltaTime);
            if (Vector2.Distance(transform.position, player.transform.position) < 1)
            {
                player.CatchTheSword();
            }

        }
        BounceLogic();
        SpinLogic();

    }

    private void SpinLogic()
    {
        if (isSpinning)
        {
            if (Vector2.Distance(player.transform.position, transform.position) > maxTravelDistance && !wasStopped)
            {
                StopWhenSpinning();
            }
            if (wasStopped)
            {
                spinTimer -= Time.deltaTime;
                transform.position = Vector2.MoveTowards(transform.position, new Vector2(transform.position.x + spinDirection, transform.position.y), 1.5f * Time.deltaTime);
                if (spinTimer < 0)
                {
                    isReturning = true;
                    isSpinning = false;
                }
                hitTimer -= Time.deltaTime;
                if (hitTimer < 0)
                {
                    hitTimer = hitCoolDown;
                    Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, 1);
                    foreach (var hit in colliders)
                    {
                        if (hit.GetComponent<Enemy>() != null)
                        {
                            SwordSkillDamage(hit.GetComponent<Enemy>());
                        }
                    }
                }
            }
        }
    }

    private void StopWhenSpinning()
    {
        wasStopped = true;
        rb.constraints = RigidbodyConstraints2D.FreezeAll;
        spinTimer = spinDuration;
    }

    private void BounceLogic()
    {
        if (isBouncing && enemyTarget.Count > 0)
        {
            transform.position = Vector2.MoveTowards(transform.position, enemyTarget[targetIndex].position, bounceSpeed * Time.deltaTime);
            if (Vector2.Distance(transform.position, enemyTarget[targetIndex].position) < .1f)
            {
                SwordSkillDamage(enemyTarget[targetIndex].GetComponent<Enemy>());
                targetIndex++;
                amountOfBounce--;
                if (amountOfBounce <= 0)
                {
                    isBouncing = false;
                    isReturning = true;
                }
                if (targetIndex >= enemyTarget.Count)
                    targetIndex = 0;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isReturning)
        {

            return;
        }
        if (collision.GetComponent<Enemy>() != null)
        {
            Enemy enemy = collision.GetComponent<Enemy>();
            SwordSkillDamage(enemy);
        }
        collision.GetComponent<Enemy>()?.DamageImpact();

        SetupStargetsForBounce(collision);
        StuckInto(collision);

    }

    private void SwordSkillDamage(Enemy enemy)
    {
        EnemyStats enemyStats = enemy.GetComponent<EnemyStats>();

        player.stats.DoDamage(enemyStats);
        if (player.skill.sword.timeStopUnlocked)
            enemy.FreezeTimeFor(freezeTimeDuration);

        if (player.skill.sword.vulnerablocked)
            enemyStats.MakeVulnerableFor(freezeTimeDuration);

        enemy.FreezeTimeFor(freezeTimeDuration);
        ItemData_Equiment equipedAmulet = Inventory.instance.GetEquipment(EquimentType.Amulet);
        if (equipedAmulet != null)
        {
            equipedAmulet.Effect(enemy.transform);
        }
    }

    private void SetupStargetsForBounce(Collider2D collision)
    {
        if (collision.GetComponent<Enemy>() != null)
        {

            if (isBouncing && enemyTarget.Count <= 0)
            {
                Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, 10);
                foreach (var hit in colliders)
                {
                    if (hit.GetComponent<Enemy>() != null)
                    {
                        enemyTarget.Add(hit.transform);
                    }
                }
            }

        }
    }

    private void StuckInto(Collider2D collision)
    {
        if(piereceAmount>0 && collision.GetComponent<Enemy>() != null)
        {
            piereceAmount--;
            return;
        }

        if (isSpinning)
        {
            StopWhenSpinning();
            return;
        }

        canRotate = false;
        cd.enabled = false;

        rb.isKinematic = true;
        rb.constraints = RigidbodyConstraints2D.FreezeAll;

        if (isBouncing && enemyTarget.Count > 0)
            return;

        anim.SetBool("Rotation", false);
        transform.parent = collision.transform;
    }
}
