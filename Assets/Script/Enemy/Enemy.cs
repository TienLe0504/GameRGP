using System.Collections;
using System.Collections.Generic;
using System.Security.Claims;
using UnityEngine;

public class Enemy : Entity
{

    [Header("Stunned info")]
    public float stunDuration;
    public Vector2 stunDirection;
    [SerializeField] protected GameObject counterImage;
    private bool canbeStunned;

    [SerializeField] protected LayerMask whatIsPlayer;
    [Header("Move Info")]
    public float moveSpeed;
    public float idleTime;
    public float battleTime;
    private float defaultMoveSpeed;


    [Header("Attack info")]
    public float attackDistance;
    public float attackCoolDown;
    [HideInInspector]public float lastTimeAttacked;


    public EnemyStateMachine stateMachine { get; private set; }
    public string lastAnimBoolName { get; private set; }

    protected override void Awake()
    {
        base.Awake();
        stateMachine = new EnemyStateMachine();
        defaultMoveSpeed = moveSpeed;
    }
    protected override void Start()
    {
        base.Start();
    }

    protected  override void Update()
    {
        base.Update();
        stateMachine.currentState.Update();
    }

    public virtual void AssignLastAnimName(string _animBoolName)
    {
        lastAnimBoolName = _animBoolName;
    }

    public override void SlowEntityBy(float _slowPercentage, float _slowDuration)
    {
        base.SlowEntityBy(_slowPercentage, _slowDuration);
        moveSpeed = moveSpeed * (1 - _slowPercentage);
        anim.speed = anim.speed * ( 1 - _slowPercentage);
        Invoke("ReturnDefaultSpeed", _slowDuration);

    }
    protected override void ReturnDefaultSpeed()
    {
        base.ReturnDefaultSpeed();
        moveSpeed = defaultMoveSpeed;
    }

    public virtual void FreezeTime(bool _timeFrozen)
    {
        if (_timeFrozen)
        {
            moveSpeed = 0;
            anim.speed = 0;
        }
        else
        {
            moveSpeed = defaultMoveSpeed;
            anim.speed = 1;
        }
    }

    public virtual void FreezeTimeFor(float _duration) => StartCoroutine(FreezeTimeCoroutine(_duration));

    protected virtual IEnumerator FreezeTimeCoroutine(float _seconds)
    {
        FreezeTime(true);
        yield return new WaitForSeconds(_seconds);
        FreezeTime(false);
    }


    public virtual RaycastHit2D IsPlayerDetected() => Physics2D.Raycast(wallCheck.position, Vector2.right * facingDir, 50, whatIsPlayer);
    protected override void OnDrawGizmos()
    {
        base.OnDrawGizmos();
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(transform.position, new Vector3(transform.position.x + attackDistance * facingDir, transform.position.y));
    }


    public virtual bool CanBeStunned()
    {
        if (canbeStunned)
        {
            CloseCounterAttackWindow();
            return true;
        }
        return false;
    }
    #region Counter Attack Window
    public virtual void OpenCounterAttackWindow()
    {
        canbeStunned = true;
        counterImage.SetActive(true);
    }
    public virtual void CloseCounterAttackWindow()
    {
        canbeStunned = false;
        counterImage.SetActive(false);
    }
    #endregion
    public virtual void AnimationFinishTrigger() => stateMachine.currentState.AnimationFinishTrigger();
}
