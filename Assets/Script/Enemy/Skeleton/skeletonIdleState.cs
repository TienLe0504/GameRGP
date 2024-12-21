using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class skeletonIdleState : SkeletonGroundedState
{
    public skeletonIdleState(Enemy _enemyBase, EnemyStateMachine _stateMachine, string _boolName, Enemy_Skeleton enemy) : base(_enemyBase, _stateMachine, _boolName, enemy)
    {
    }

    public override void Enter()
    {
        base.Enter();
        stateTimer = enemy.idleTime;
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void Update()
    {
        base.Update();
        if (stateTimer < 0)
        {
            stateMachine.ChangeState(enemy.moveState);
        }
        if (enemy.IsPlayerDetected())
            stateMachine.ChangeState(enemy.battleState);
    }
}
