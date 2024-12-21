using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySkeletonAnimationTrigger : MonoBehaviour
{
    private Enemy_Skeleton enemy => GetComponentInParent<Enemy_Skeleton>();
    private void AnimationTrigger()
    {
        enemy.AnimationFinishTrigger();
    }
    private void AttackTriger()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(enemy.attackCheck.position, enemy.attackCheckRadius);
        foreach(Collider2D hit in colliders)
        {
            if (hit.GetComponent<Player>() != null)
            {
                PlayerStats starget = hit.GetComponent<PlayerStats>();
                enemy.stats.DoDamage(starget);
                //hit.GetComponent<Player>().Damage();
            }
        }
    }
    private void OpenCounterWindow() => enemy.OpenCounterAttackWindow();
    private void CloseCounterWindow() => enemy.CloseCounterAttackWindow();
}
