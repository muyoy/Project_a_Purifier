using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class PlayerController : Unit
{
    [SerializeField] protected float stemia; //피격 벽박치기 예외처리, 최적화, 공격 오브젝트풀링 제작
    protected virtual float Stemia
    {
        get { return stemia; }
        set
        {
            stemia = value;
            if (stemia <= 0)
            {
                stemia = 0;
            }
        }
    }
    private float maxSteamia = 10;
    public float chargeCoolTime;    //test
    public float jumpingPower;      //test
    public float maprightend;      //test

    public float dodgepos;
    public float dodgeSpeed;
    public float hitSpeed;
    public float rightDodgepos;
    public float leftDodgepos;

    private float dodgeTimer = 1.0f;
    private float hitTimer = 1.0f;

    private bool moveLeft = false;
    private bool moveRight = false;
    private bool DodgeLeft = false;
    private bool DodgeRight = false;
    private bool isfullCharge = true;

    private Transform attackPoint;
    private Transform groundcheck;
    private Coroutine dodge = null;
    private Coroutine charge = null;
    private Coroutine hit = null;

    public GameObject firePrefab;
    public LayerMask groundLayer;
    public Slider steamiabar;

    private const int hash = -1736918096; // HashCode만 모아놓은 클라스 생성
                                          //Debug.Log(Animator.StringToHash("isDodge"));

    protected override void Start()
    {
        base.Start();
        hpBar.value = hp / maxHp;
        steamiabar.value = stemia / maxSteamia;
        groundcheck = transform.GetChild(0).GetComponent<Transform>();
        attackPoint = transform.GetChild(1).GetComponent<Transform>();
    }
    private void FixedUpdate()
    {
        if (!isDead)
        {
            Movement();
            Filp();
            rb.velocity = new Vector2(horizon * Time.deltaTime, rb.velocity.y);
            animator.SetFloat("Speed", Mathf.Abs(horizon));
            animator.SetFloat("jumpSpeed", rb.velocity.y);
            animator.SetBool("isGround", IsGrounded());
        }

        if(hp < 0)
        {
            state = State.Death;
            rb.velocity = Vector2.zero;
        }
    }
    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            hit = StartCoroutine(HitKnock_Back());
        }
    }
    public override void HpChange(float damage)
    {
        if (!isDead && !(state == State.Dodge) && !(state == State.Hit))
        {
            Hp -= damage;
            hpBar.value = hp / maxHp;
            if(hp > 0)
            {
                hit = StartCoroutine(HitKnock_Back());
            }
        }
    }
    private IEnumerator HitKnock_Back()
    {
        float hitKnock_Back = 0.0f;
        float HitKnock_Back_pos = 1.0f;
        float hitKnock_Back_speed = 30.0f;
        if(!IsGrounded())
        {
            rb.velocity = Vector2.zero;
        }
        state = State.Hit;
        if (!isFacingRight)
        {
            hitKnock_Back = transform.position.x + HitKnock_Back_pos;
            animator.SetBool("isHit", true);
            while (hitKnock_Back - transform.position.x >= 0)
            {
                moveLeft = false;
                moveRight = false;
                rb.position += Vector2.right * hitKnock_Back_speed * Time.deltaTime;
                yield return null;
            }
            yield return new WaitForSeconds(hitTimer);
            animator.SetBool("isHit", false);
        }
        else
        {
            hitKnock_Back = transform.position.x - HitKnock_Back_pos;
            animator.SetBool("isHit", true);
            while (hitKnock_Back - transform.position.x <= 0)
            {
                moveLeft = false;
                moveRight = false;
                rb.position += Vector2.left * hitKnock_Back_speed * Time.deltaTime;
                yield return null;
            }
            yield return new WaitForSeconds(hitTimer);
            animator.SetBool("isHit", false);
        }
        state = State.Idle;
    }

    protected override void Movement()
    {
        if (moveLeft)
        {
            state = State.Move;
            horizon = -moveSpeed;
        }
        else if (moveRight)
        {
            state = State.Move;
            horizon = moveSpeed;
        }
        else
        {
            horizon = 0;
        }
    }
    #region UIButtonMove

    public void PointerDownLeft()
    {
        if (!(state == State.Attack) && !(state == State.Dodge) && !(state == State.Hit) && !isDead)
        {
            moveLeft = true;
        }
    }
    public void PointerUpLeft()
    {
        if (!(state == State.Move))
        {
            return;
        }
        else
        {
            state = State.Idle;
            moveLeft = false;
        }
    }

    public void PointerDownRight()
    {
        if (!(state == State.Attack) && !(state == State.Dodge) && !(state == State.Hit) && !isDead)
        {
            moveRight = true;
        }
    }
    public void PointerUpRight()
    {
        if (!(state == State.Move))
        {
            return;
        }
        else
        {
            state = State.Idle;
            moveRight = false;
        }
    }
    public void PointerDownJump()
    {
        if (IsGrounded() && !(state == State.Attack) && !(state == State.Dodge) && !(state == State.Hit) && !isDead)
        {
            rb.velocity = Vector2.up * jumpingPower;
        }   
    }
    #endregion

    public void PointerDownAttack()
    {
        if ((state == State.Idle || state == State.Move) && IsGrounded())
        {
            if(state == State.Move)
            {
                moveLeft = false;
                moveRight = false;              
            }
            state = State.Attack;
            Instantiate(firePrefab, attackPoint.position, attackPoint.rotation);
            StartCoroutine(IdleCheck());
        }
    }
    public void PointerDownDodgeLeft()
    {
        if (stemia > 0 && IsGrounded() && !(state == State.Attack) && !(state == State.Dodge) && !(state == State.Hit) && !isDead)
        {
            SteamiaChange(-1);
            DodgeLeft = true;
            dodge = StartCoroutine(Dodge());
        }
    }
    public void PointerUpDodgeLeft()
    {
        DodgeLeft = false;
    }
    public void PointerDownDodgeRight()
    {
        if (stemia > 0 && IsGrounded() && !(state == State.Attack) && !(state == State.Dodge) && !(state == State.Hit) && !isDead)
        {
            SteamiaChange(-1);
            DodgeRight = true;
            dodge = StartCoroutine(Dodge());
        }
    }
    public void PointerUpDodgeRight()
    {
        DodgeRight = false;
    }
    private IEnumerator SteamiaCharge()
    {
        while(!isDead && !isfullCharge)
        {
            yield return new WaitForSeconds(chargeCoolTime);
            SteamiaChange(1);
        }
    }
    private IEnumerator IdleCheck()
    {
        if (!IsGrounded())
        {
            while (rb.velocity.y != 0)
            {
                yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);
            }
        }
        if (state == State.Attack)
        {
            animator.SetBool("isAttack", true);
            yield return new WaitForSeconds(0.1f);
            yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);
            animator.SetBool("isAttack", false);
        }
        state = State.Idle;
    }
    private IEnumerator Dodge()
    {
        state = State.Dodge;
        if (DodgeRight && !animator.GetBool(hash))
        {
            rightDodgepos = transform.position.x + dodgepos;
            if(rightDodgepos > maprightend)
            {
                rightDodgepos = maprightend;
            }
            if(!isFacingRight)
            {
                transform.Rotate(0f, 180f, 0f);
                isFacingRight = true;
            }
            animator.SetBool(hash, true);
            while (rightDodgepos - transform.position.x >= 0)
            {
                moveLeft = false;
                moveRight = false;
                rb.position += Vector2.right * dodgeSpeed * Time.deltaTime;
                yield return null;
            }
            yield return new WaitForSeconds(dodgeTimer);
            animator.SetBool("isDodge", false);
        }
        if (DodgeLeft && !animator.GetBool(hash))
        {
            leftDodgepos = transform.position.x - dodgepos;
            if (leftDodgepos < -maprightend)
            {
                leftDodgepos = -maprightend;
            }
            if (isFacingRight)
            {
                transform.Rotate(0f, 180f, 0f);
                isFacingRight = false;
            }

            animator.SetBool("isDodge", true);
            while (leftDodgepos - transform.position.x <= 0)
            {
                moveLeft = false;
                moveRight = false;
                rb.position += Vector2.left * dodgeSpeed * Time.deltaTime;
                yield return null;
            }
            yield return new WaitForSeconds(dodgeTimer);
            animator.SetBool("isDodge", false);
        }
        state = State.Idle;
    }
    private void SteamiaChange(float usingstemia)
    {
        Stemia += usingstemia;
        steamiabar.value = stemia / maxSteamia;
        if(stemia >= maxSteamia)
        {
            isfullCharge = true;
            stemia = maxSteamia;
            StopCoroutine(charge);
            charge = null;
        }
        else
        {
            isfullCharge = false;
            if (charge == null)
            {
                charge = StartCoroutine(SteamiaCharge());
            }
        }
    }
    private bool IsGrounded()
    {
        return Physics2D.OverlapCircle(groundcheck.position, 0.2f, groundLayer);
    }
    private void Filp()
    {
        if (isFacingRight && horizon < 0f || !isFacingRight && horizon > 0f)
        {
            isFacingRight = !isFacingRight;
            transform.Rotate(0f, 180f, 0f);
        }
    }
}
