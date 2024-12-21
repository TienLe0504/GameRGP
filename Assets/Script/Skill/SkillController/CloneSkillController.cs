using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloneSkillController : MonoBehaviour
{
    private Player player;
    private SpriteRenderer rs;
    private Animator anim;
    [SerializeField] private float colorLoosingSpeed;
    [SerializeField] private Transform attackCheck;
    [SerializeField] private float attackCheckRadius = .8f;
    private float cloneTimer;
    private float attackMultiplier;
    private bool canDuplicateClone;
    private int facingDir = 1;
    private float changeToDuplicate;

    private Transform closestEnemy;
    private void Awake()
    {
        rs = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
    }
    private void Update()
    {
        cloneTimer -= Time.deltaTime;
        if (cloneTimer < 0)
        {
            rs.color = new Color(1, 1, 1, rs.color.a - (Time.deltaTime*colorLoosingSpeed));
            if (rs.color.a < 0)
            {
                Destroy(gameObject);
            }
        }

    }
    public void SetupClone(Transform _newTransform, float _cloneDuration, bool _canAttack, Vector3 _offset, Transform _closestEenmy, bool _canDuplicate, float _changeToDuplicate, Player _player, float _attackMultiplier)
    {
        if (_canAttack)
            anim.SetInteger("AttackNumber", Random.Range(1, 3));


        attackMultiplier = _attackMultiplier;
        player = _player;
        transform.position = _newTransform.position+_offset;
        cloneTimer = _cloneDuration;
        closestEnemy = _closestEenmy;
        canDuplicateClone = _canDuplicate;
        changeToDuplicate = _changeToDuplicate;
        FaceClosesTarget();
    }



    private void AnimationTrigger()
    {
        cloneTimer -= .1f;
    }
    private void AttackTrigger()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(attackCheck.position, attackCheckRadius);
        foreach (var hit in colliders)
        {
            if (hit.GetComponent<Enemy>() != null)
            {
                PlayerStats playerStats = player.GetComponent<PlayerStats>();
                EnemyStats enenmyStats = hit.GetComponent<EnemyStats>();

               // player.stats.DoDamage(hit.GetComponent<CharacterStats>());


                playerStats.CloneDoDamage(enenmyStats,attackMultiplier);

                if (player.skill.clone.canApplyOnHitEffect)
                {

                    ItemData_Equiment weaponData = Inventory.instance.GetEquipment(EquimentType.Weapon);
                    if (weaponData != null)
                    {
                        weaponData.Effect(hit.transform); 
                    }
                }


                if (canDuplicateClone)
                {
                    if (Random.Range(0, 100) < changeToDuplicate)
                    {
                        SkillManager.instance.clone.CreateClone(hit.transform, new Vector3(.5f * facingDir, 0));
                    }
                }
            }
        }
    }
    private void FaceClosesTarget()
    {
        //Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, 25);
        //float closesDistance = Mathf.Infinity;
        //foreach(var hit in colliders)
        //{
        //    if(hit.GetComponent<Enemy>() != null)
        //    {
        //        float distanceToEnemy = Vector2.Distance(transform.position,hit.transform.position);
        //        if (distanceToEnemy < closesDistance)
        //        {
        //            closestEnemy = hit.transform;
        //        }
        //    }
        //}
        if(closestEnemy != null)
        {
            if (transform.position.x > closestEnemy.position.x)
                facingDir = -1;
                transform.Rotate(0, 180, 0);
        }
    }
}
