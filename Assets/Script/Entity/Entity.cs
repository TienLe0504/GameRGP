using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Entity : MonoBehaviour
{
    [Header("Collision info")]
    public Transform attackCheck;
    public float attackCheckRadius;
    [SerializeField] protected Transform groundCheck;
    [SerializeField] protected float groundCheckDistance;
    [SerializeField] protected Transform wallCheck;
    [SerializeField] protected float wallCheckDistance;
    [SerializeField] protected LayerMask whatIsGround;

    public int facingDir { get; private set; } = 1;
    protected bool facingRight = true;

    public System.Action onFlipped;

    #region components
    public Animator anim { get; private set; }
    public Rigidbody2D rb;
    public EntityFx fx {  get; private set; }
    #endregion
    public SpriteRenderer sr { get; private set; }


    public CharacterStats stats { get; private set; }
    public CapsuleCollider2D cd { get; private set; }

    [Header("Knocback info")]
    [SerializeField] protected Vector2 knocbackDirection;
    [SerializeField] protected float knocbackDuration;
    protected bool isKnocked;






    protected virtual void Awake()
    {
        stats = GetComponent<CharacterStats>();

    }
    protected virtual void Start()
    {
        fx = GetComponentInChildren<EntityFx>();
        anim = GetComponentInChildren<Animator>();
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponentInChildren<SpriteRenderer>();
        stats = GetComponent<CharacterStats>();
        cd = GetComponent<CapsuleCollider2D>();
    }

    protected virtual void Update()
    {


    }

    public virtual void SlowEntityBy (float _slowPercentage, float _slowDuration)
    {

    }
    protected virtual void ReturnDefaultSpeed()
    {
        anim.speed = 1;
    }


    public virtual void DamageImpact()
    {
        //fx.StartCoroutine("FlashFX");
        StartCoroutine("HitKnockback");
    }


    protected virtual IEnumerator HitKnockback()
    {
        isKnocked = true;
        rb.velocity = new Vector2(knocbackDirection.x * -facingDir, knocbackDirection.y);
        yield return new WaitForSeconds(knocbackDuration);
        isKnocked=false;
    }


    #region Collision

    protected virtual void OnDrawGizmos()
    {
        Gizmos.DrawLine(groundCheck.position, new Vector3(groundCheck.position.x, groundCheck.position.y - groundCheckDistance));

        Gizmos.DrawLine(wallCheck.position, new Vector3(wallCheck.position.x + wallCheckDistance, wallCheck.position.y));
        Gizmos.DrawWireSphere(attackCheck.position, attackCheckRadius);
    }

    public virtual bool IsGroundDetected() => Physics2D.Raycast(groundCheck.position, Vector2.down, groundCheckDistance, whatIsGround);

    public virtual bool IsWallDetected() => Physics2D.Raycast(wallCheck.position, Vector2.right * facingDir, wallCheckDistance, whatIsGround);

    #endregion

    #region Flip

    public virtual void Flip()
    {
        facingDir *= -1;
        facingRight = !facingRight;
        transform.Rotate(0, 180, 0);
        if(onFlipped !=null)
            onFlipped();
    }
    public virtual void FlipController(float _x)
    {
        if (_x > 0 && !facingRight)
        {
            Flip();
        }
        else if (_x < 0 && facingRight)
        {
            Flip();
        }
    }
    #endregion



    #region Velocity

    public  void SetZeroVelocity()
    {
        if (isKnocked)
            return;
        rb.velocity = new Vector2(0, 0);
    }

   public virtual void SetVelocity(float _xVelocity, float _yVelocity)
    {
        if (isKnocked)
            return;
        rb.velocity = new Vector2(_xVelocity, _yVelocity);
        FlipController(_xVelocity);
    }

    #endregion

    public void MakeTransprent(bool _transrent)
    {
        if (_transrent)
            sr.color = Color.clear;
        else
            sr.color = Color.white;
    }


    public virtual void Die()
    {

    }


}
