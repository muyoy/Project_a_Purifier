using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : Unit
{
    [SerializeField] protected float stemia;    //점프중과 어택이 끝나고 idle 상태로 돌아오게 하기
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
    public float MaxSteamia;
    public float chargeCoolTime;
    public float dodgepos;
    public float dodgeSpeed;
    public float rightDodgepos;
    public float leftDodgepos;
    public float jumpingPower;

    private bool moveLeft = false;
    private bool moveRight = false;
    private bool DodgeLeft = false;
    private bool DodgeRight = false;
    private bool isfullCharge = true;

    private Transform attackPoint;
    private Transform groundcheck;
    private Coroutine dodge = null;
    private Coroutine charge = null;
    public GameObject firePrefab;
    public LayerMask groundLayer;
    public Slider steamiabar;

    private const int hash = -1736918096; // HashCode만 모아놓은 클라스 생성
                                          //Debug.Log(Animator.StringToHash("isDodge"));

    protected override void Start()
    {
        base.Start();
        hpBar.value = hp / maxHp;
        steamiabar.value = stemia / MaxSteamia;
        groundcheck = transform.GetChild(0).GetComponent<Transform>();
        attackPoint = transform.GetChild(1).GetComponent<Transform>();
    }
    private void FixedUpdate()
    {
        rb.velocity = new Vector2(horizon * Time.deltaTime, rb.velocity.y);
        animator.SetFloat("Speed", Mathf.Abs(horizon));
        animator.SetFloat("jumpSpeed", rb.velocity.y);
        animator.SetBool("isGround", IsGrounded());
    }
    private void Update()
    {
        Movement();
        Filp();
    }

    protected override void Movement()
    {
        if (moveLeft)
        {
            horizon = -moveSpeed;
        }
        else if (moveRight)
        {
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
        if (!(state == State.Attack))
        {
            state = State.Move;
            moveLeft = true;
        }
    }
    public void PointerUpLeft()
    {
        if (state == State.Move)
        {
            state = State.Idle;
            moveLeft = false;
        }
    }

    public void PointerDownRight()
    {
        if (!(state == State.Attack))
        {
            state = State.Move;
            moveRight = true;
        }
    }
    public void PointerUpRight()
    {
        if (state == State.Move)
        {
            state = State.Idle;
            moveRight = false;
        }
    }
    public void PointerDownJump()
    {
        if (IsGrounded())
        {
            state = State.Jump;
            rb.velocity = Vector2.up * jumpingPower;
        }   
    }
    #endregion

    public void PointerDownAttack()
    {
        state = State.Attack;
        Instantiate(firePrefab, attackPoint.position, attackPoint.rotation);
    }
    public void PointerDownDodgeLeft()
    {
        if (stemia > 0 && !(state == State.Dodge))
        {
            SteamiaChange(-1);
            DodgeLeft = true;
            dodge = StartCoroutine("Dodge");
        }
    }
    public void PointerUpDodgeLeft()
    {
        DodgeLeft = false;
    }
    public void PointerDownDodgeRight()
    {
        if (stemia > 0 && !(state == State.Dodge))
        {
            SteamiaChange(-1);
            DodgeRight = true;
            dodge = StartCoroutine("Dodge");
        }
    }
    public void PointerUpDodgeRight()
    {
        DodgeRight = false;
    }
    private IEnumerator SteamiaCharge()
    {
        while(!(state == State.Death) && !isfullCharge)
        {
            yield return new WaitForSeconds(chargeCoolTime);
            SteamiaChange(1);
        }
    }
    private IEnumerator Dodge()
    {
        state = State.Dodge;
        if (DodgeRight && !animator.GetBool(hash))
        {
            rightDodgepos = transform.position.x + dodgepos;
            if(!isFacingRight)
            {
                transform.Rotate(0f, 180f, 0f);
                isFacingRight = true;
            }
            animator.SetBool(hash, true);
            while (rightDodgepos - transform.position.x >= 0)
            {
                rb.position += Vector2.right * dodgeSpeed * Time.deltaTime;
                yield return null;
            }
            yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);
            animator.SetBool("isDodge", false);
        }
        if (DodgeLeft && !animator.GetBool(hash))
        {
            leftDodgepos = transform.position.x - dodgepos;
            if (isFacingRight)
            {
                transform.Rotate(0f, 180f, 0f);
                isFacingRight = false;
            }

            animator.SetBool("isDodge", true);
            while (leftDodgepos - transform.position.x <= 0)
            {
                rb.position += Vector2.left * dodgeSpeed * Time.deltaTime;
                yield return null;
            }
            yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);
            animator.SetBool("isDodge", false);
        }
        state = State.Idle;
    }
    private void SteamiaChange(float usingstemia)
    {
        Stemia += usingstemia;
        steamiabar.value = stemia / MaxSteamia;
        if(stemia >= MaxSteamia)
        {
            isfullCharge = true;
            stemia = MaxSteamia;
            StopCoroutine(charge);
        }
        else
        {
            isfullCharge = false;
            charge = StartCoroutine("SteamiaCharge");
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
