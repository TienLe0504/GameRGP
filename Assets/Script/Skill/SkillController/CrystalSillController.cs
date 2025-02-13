using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrystalSillController : MonoBehaviour
{
    private Player player;
    private Animator anim => GetComponent<Animator>();
    private CircleCollider2D cd =>GetComponent<CircleCollider2D>();


    private float crystalExistTimner;
    private bool canExplode;
    private bool canMove;
    private float moveSpeed;

    private bool canGrow;
    private float growSpeed = 5;

    private Transform closestTarget;
    [SerializeField] private LayerMask whatIsEnemy;
   public void SetupScrystal(float _crystalDuration, bool _canExplode, bool _canMove, float _moveSpeed, Transform _closestTarget, Player _player)
    {
        player = _player;
        crystalExistTimner = _crystalDuration;
        canExplode = _canExplode;
        canMove = _canMove;
        moveSpeed = _moveSpeed;
        closestTarget = _closestTarget;
    }


    public void ChooseRandomEnemy()
    {
        float radius = SkillManager.instance.blackHole.GetBlackholeRadius();
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, 25, whatIsEnemy);
        if(colliders.Length > 0 ) 
            closestTarget  = colliders[Random.Range(0,colliders.Length)].transform;

    }


    private void Update()
    {
        Debug.Log("canmove " + canMove);
       
            crystalExistTimner -= Time.deltaTime;
            if (crystalExistTimner < 0)
            {
                FininshCrystal();

            }

            if(canMove)
            {
                if (closestTarget == null)
                    return;
                transform.position = Vector2.MoveTowards (transform.position, closestTarget.position, moveSpeed*Time.deltaTime);
                if (Vector2.Distance(transform.position, closestTarget.position) < 1)
                {
                    FininshCrystal();
                    canMove = false;
                }
            
            }

            if(canGrow)
                transform.localScale = Vector2.Lerp(transform.localScale, new Vector2(3,3), growSpeed*Time.deltaTime);

        
    }

    private void AnimationExplodeEvent()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, cd.radius);
        foreach (var hit in colliders)
        {
            if (hit.GetComponent<Enemy>() != null)
            {
                player.stats.DoMagicalDamage(hit.GetComponent<CharacterStats>());
                ItemData_Equiment equipedAmulet = Inventory.instance.GetEquipment(EquimentType.Amulet);
                if (equipedAmulet != null)
                {
                    equipedAmulet.Effect(hit.transform);
                }
            }
        }
    }

    public void FininshCrystal()
    {
        if (canExplode)
        {

            canGrow = true;
            anim.SetTrigger("Explode");
        }
        else
            selfDestroy();
    }

    public void selfDestroy() => Destroy(gameObject);
}
