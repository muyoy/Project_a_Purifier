using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerController : Unit
{
    [SerializeField] protected float stemia;
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
    private float maxSteamia;
    private float reloadCount = 10;
    private float currnetReloadCount;
    private float reloadTime = 2.0f;
    private float chargeCoolTime = 5.0f;
    private const float jumpingPower = 12.5f;
    private float mapEnd = 19.0f;

    private const float dodgepos = 3.6f;
    public float dodgeSpeed;
    private const float hitpos = 1.8f;
    public float hitSpeed;

    private float invincibilityTimer = 1.0f;

    private bool moveLeft = false;
    private bool moveRight = false;
    private bool DodgeLeft = false;
    private bool DodgeRight = false;
    private bool isfullCharge = true;

    private Transform attackPoint;
    private Transform groundcheck;
    private Animator animator;
    private Coroutine dodge = null;
    private Coroutine charge = null;
    private Coroutine reload = null;
    private Coroutine hit = null;

    public GameObject firePrefab;
    public LayerMask groundLayer;
    public Slider steamiabar;
    public TextMeshProUGUI bulletText;

    protected override void Start()
    {
        base.Start();
        Init();
        animator = GetComponent<Animator>();
        groundcheck = transform.GetChild(0).GetComponent<Transform>();
        attackPoint = transform.GetChild(1).GetComponent<Transform>();
        target = GameObject.FindGameObjectWithTag("Monster");
    }
    private void FixedUpdate()
    {
        if (!isDead)
        {
            Movement();
            Filp();
            rb.velocity = new Vector2(horizon * Time.deltaTime, rb.velocity.y);
            animator.SetFloat(HashCode.moveID, Mathf.Abs(horizon));
            animator.SetFloat(HashCode.jumpID, rb.velocity.y);
            animator.SetBool(HashCode.dropID, IsGrounded());
        }

        if(hp < 0)
        {
            state = State.Death;
            rb.velocity = Vector2.zero;
        }
    }
    /// <summary>
    /// 테스트용
    /// </summary>
    private void Update()
    {

        if(Input.GetKeyDown(KeyCode.Space))
        {
            hit = StartCoroutine(Invincibility_Time(hitpos, hitSpeed, HashCode.hitID, State.Hit));
        }
    }

    protected override void Init()
    {
        base.Init();
        maxSteamia = GameManager.instance.cha_St;
        stemia = GameManager.instance.cha_St;
        hpBar.value = hp / maxHp;
        steamiabar.value = stemia / maxSteamia;
        currnetReloadCount = reloadCount;
        bulletText.text = currnetReloadCount.ToString();
    }
    public override void HpChange(float damage)
    {
        if (!isDead && !(state == State.Dodge) && !(state == State.Hit))
        {
            Hp -= damage;
            hpBar.value = hp / maxHp;
            if(hp > 0)
            {
                hit = StartCoroutine(Invincibility_Time(hitpos, hitSpeed, HashCode.hitID, State.Hit));
            }
        }
    }

    protected override void Dead()
    {
        base.Dead();
        animator.SetBool(HashCode.deadID, true);
    }

    private IEnumerator Invincibility_Time(float position, float speed, int animationHashCode, State changeState)
    {
        bool isRight = true;
        float rightPostion = 0.0f;
        float leftPostion = 0.0f;
        if (changeState == State.Dodge)
        {
            state = State.Dodge;
            invincibilityTimer = 1.0f;
            if (DodgeRight)
            {
                isRight = true;
                //if (!isFacingRight)
                //{
                //    transform.Rotate(0f, 180f, 0f);
                //    isFacingRight = true;
                //    isRight = true;
                //}
                //else
                //{
                //    isRight = true;
                //}
            }
            else if (DodgeLeft)
            {
                isRight = false;
                //if (isFacingRight)
                //{
                //    transform.Rotate(0f, 180f, 0f);
                //    isFacingRight = false;
                //    isRight = false;
                //}
                //else
                //{
                //    isRight = false;
                //}
            }
        }
        else if(changeState == State.Hit)
        {
            state = State.Hit;
            invincibilityTimer = 1.5f;
            if (!isFacingRight)
            {
                isRight = true;
            }
            else
            {
                isRight = false;
            }

            if (!IsGrounded())
            {
                rb.velocity = Vector2.zero;
            }
        }
        if (isRight)
        {
            rightPostion = transform.position.x + position;
            if (rightPostion > mapEnd)
            {
                rightPostion = mapEnd;
            }
            animator.SetBool(animationHashCode, true);
            while (rightPostion - transform.position.x >= 0)
            {
                moveLeft = false;
                moveRight = false;
                rb.position += Vector2.right * speed * Time.deltaTime;
                yield return null;
            }
            yield return new WaitForSeconds(invincibilityTimer);
            animator.SetBool(animationHashCode, false);
        }
        else
        {
            leftPostion = transform.position.x - position;
            if (leftPostion < -mapEnd)
            {
                leftPostion = -mapEnd;
            }
            animator.SetBool(animationHashCode, true);
            while (leftPostion - transform.position.x <= 0)
            {
                moveLeft = false;
                moveRight = false;
                rb.position += Vector2.left * speed * Time.deltaTime;
                yield return null;
            }
            yield return new WaitForSeconds(invincibilityTimer);
            animator.SetBool(animationHashCode, false);
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
            Attack();
        }
    }
    private void Attack()
    {
        if (currnetReloadCount > 0)
        {
            state = State.Attack;
            StartCoroutine(IdleCheck());
            currnetReloadCount -= 1;
            bulletText.text = currnetReloadCount.ToString();
            firePrefab = ObjectPool.GetObject();
            firePrefab.transform.position = attackPoint.position;
            firePrefab.GetComponent<Rigidbody2D>().velocity = transform.right * 20.0f;
            firePrefab.GetComponent<Bullet>().Shoot(0.4f);

            if (currnetReloadCount == 0)
            {
                bulletText.text = "Reload..";
                if (reload == null)
                {
                    reload = StartCoroutine(ReloadTime());
                }
            }
        }
    }

    private IEnumerator ReloadTime()
    {
        yield return new WaitForSeconds(reloadTime);
        currnetReloadCount = reloadCount;
        bulletText.text = currnetReloadCount.ToString();
        reload = null;
    }
    public void PointerDownDodgeLeft()
    {
        if (stemia > 0 && IsGrounded() && !(state == State.Attack) && !(state == State.Dodge) && !(state == State.Hit) && !isDead)
        {
            SteamiaChange(-1);
            DodgeLeft = true;
            dodge = StartCoroutine(Invincibility_Time(dodgepos, dodgeSpeed, HashCode.dodgeID, State.Dodge));
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
            dodge = StartCoroutine(Invincibility_Time(dodgepos, dodgeSpeed, HashCode.dodgeID, State.Dodge));
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
            SteamiaChange(GameManager.instance.cha_St_Reg);
        }
    }
    private IEnumerator IdleCheck()
    {
        if (state == State.Attack)
        {
            animator.SetBool(HashCode.attackID, true);
            yield return new WaitForSeconds(0.1f);
            yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);
            animator.SetBool(HashCode.attackID, false);
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
        if(isFacingRight && transform.position.x > target.transform.position.x || !isFacingRight && transform.position.x < target.transform.position.x)
        {
            isFacingRight = !isFacingRight;
            transform.Rotate(0f, 180f, 0f);
            bulletText.rectTransform.Rotate(0f, 180f, 0f);
        }
        //if (isFacingRight && horizon < 0f || !isFacingRight && horizon > 0f)
        //{
        //    isFacingRight = !isFacingRight;
        //    transform.Rotate(0f, 180f, 0f);
        //    bulletText.rectTransform.Rotate(0f, 180f, 0f);
        //}
    }
}
