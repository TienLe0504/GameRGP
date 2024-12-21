using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyState {


    protected EnemyStateMachine stateMachine;
    protected Enemy enemyBase;
    protected bool triggerCalled;
    private string animBoolName;
    protected float stateTimer;
    protected Rigidbody2D rb;
    public EnemyState(Enemy _enemyBase, EnemyStateMachine _stateMachine, string _boolName) {
        this.enemyBase = _enemyBase;
        this.stateMachine = _stateMachine;
        this.animBoolName = _boolName;
    
    }
    public virtual void Enter()
    {
        triggerCalled = false;
        rb = enemyBase.rb;
        enemyBase.anim.SetBool(animBoolName, true);
    }


    public virtual void Update()
    {
        stateTimer -= Time.deltaTime;
    }

    public virtual void Exit()
    {
        enemyBase.anim.SetBool(animBoolName, false);
        enemyBase.AssignLastAnimName(animBoolName);
    }

    public virtual void AnimationFinishTrigger()
    {
        triggerCalled = true;
    }



}
