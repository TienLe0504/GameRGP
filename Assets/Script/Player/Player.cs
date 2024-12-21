using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Entity
{
    [Header("Attack Detail")]
    public Vector2[] attackMovement;
    [SerializeField]public float counterAttackDuration = .2f;
    public bool isBusy {  get; private set; }
    public float swordReturnImpact;

    [Header("Move Info")]
    public float moveSpeed = 8f;
    public float jumpForce;
    private float defaultMoveSpeed;
    private float defaultJumpForce;

    [Header("Dash Info")]
    public float dashSpeed;
    public float dashDuration;
    private float defaultDashSpeed;
    public float dashDir {  get; private set; }


    public SkillManager skill {  get; private set; }
    public GameObject sword { get; private set; }

 



    #region states
    public PlayerStateMachine stateMachine { get; private set; }

    public PlayerIdleState idleState { get; private set; }  
    public PlayerMoveState moveState { get; private set; }
    public PlayerAirState airState { get; private set; }
    public PlayerJumpState jumpState { get; private set; }
    public PlayerDashState dashState { get; private set; }
    public PlayerWallSlideState wallSlideState { get; private set; }
    public PlayerWallJumpState wallJump { get; private set; }
    public PlayerPrimaryAttackState primaryAttack { get; private set; }
    public PlayerCounterAttackState counterAttack { get; private set; }

    public PlayerAimSwordState aimSwordState { get; private set; }
    public PlayerCatchSwordState catchSwordState { get; private set; }

    public PlayerBlackHoleState blackHoleState { get; private set; }

    public PlayerDeadState deadState { get; private set; }

    #endregion
    protected override void Awake()
    {
        base.Awake();
        stateMachine = new PlayerStateMachine();
        idleState = new PlayerIdleState(this, stateMachine, "Idle");
        moveState = new PlayerMoveState(this, stateMachine, "Move");
        jumpState = new PlayerJumpState(this, stateMachine, "Jump");
        airState = new PlayerAirState(this, stateMachine, "Jump");
        dashState = new PlayerDashState(this, stateMachine, "Dash");
        wallSlideState = new PlayerWallSlideState(this, stateMachine, "WallSlide");
        wallJump = new PlayerWallJumpState(this, stateMachine, "Jump");
        primaryAttack = new PlayerPrimaryAttackState(this, stateMachine, "Attack");
        counterAttack = new PlayerCounterAttackState(this, stateMachine, "CounterAttack");

        aimSwordState = new PlayerAimSwordState(this, stateMachine, "AimSword");
        catchSwordState = new PlayerCatchSwordState(this, stateMachine, "CatchSword");
        blackHoleState = new PlayerBlackHoleState(this, stateMachine, "Jump");
        deadState = new PlayerDeadState(this, stateMachine, "Die");
    }

    protected override void Start()
    {
        base.Start();
        skill = SkillManager.instance;
        stateMachine.Initialize(idleState);
        defaultMoveSpeed = moveSpeed;
        defaultJumpForce = jumpForce;
        defaultDashSpeed = dashSpeed;
    }

    public void BecomeIdle()
    {
        stateMachine.ChangeState(idleState);

    }

    protected override void Update()
    {
        base.Update();
        stateMachine.currentState.Update();
        CheckForDashInput();
        if (Input.GetKeyDown(KeyCode.F)  && skill.crystal.crystalUnlocked)
            skill.crystal.CanUseSkill();
        if (Input.GetKeyDown(KeyCode.Alpha1))
            Inventory.instance.UseFlask();
    }

    public void AssignNewSword(GameObject _newSword)
    {
        sword = _newSword;
    }


    public void ExitBlackHoleAbility()
    {
        stateMachine.ChangeState(airState);
    }

    public void CatchTheSword()
    {
        stateMachine.ChangeState(catchSwordState);
        Destroy(sword);
    }

    public void CheckForDashInput()
    {
        if (IsWallDetected())
        {
            return;
        }

        if (skill.dash.dashUnlocked == false)
            return;

        //dashUserageTimer -= Time.deltaTime;
        if (Input.GetKeyDown(KeyCode.LeftShift) && SkillManager.instance.dash.CanUseSkill())
        {
            //dashUserageTimer = dashCoolDown;
            dashDir = Input.GetAxisRaw("Horizontal");
            if (dashDir == 0)
            {
                dashDir = facingDir;
            }
            stateMachine.ChangeState(dashState);
        }
    }

    public override void SlowEntityBy(float _slowPercentage, float _slowDuration)
    {
        base.SlowEntityBy(_slowPercentage, _slowDuration);
        moveSpeed = moveSpeed * (1 - _slowPercentage);
        jumpForce = jumpForce * (1 - _slowPercentage);
        dashSpeed = dashSpeed * (1 - _slowPercentage);
        anim.speed = anim.speed * (1 - _slowPercentage);
        Invoke("ReturnDefaultSpeed",_slowDuration);
    }
    protected override void ReturnDefaultSpeed()
    {
        base.ReturnDefaultSpeed();
        moveSpeed = defaultMoveSpeed;
        jumpForce = defaultJumpForce;
        dashSpeed = defaultDashSpeed;
    }

    public IEnumerable BusyFor(float _seconds)
    {
        isBusy = true;
        yield return new WaitForSeconds(_seconds);
        isBusy = false;
    }


    public void AnimationTrigger() => stateMachine.currentState.AnimationFinishTrigger();



    public override void Die()
    {
        base.Die();
        stateMachine.ChangeState(deadState);
    }



}
