using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Spine.Unity;
using TMPro;

public class Player_Controller_Spine : Unit
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
    public enum Animation
    {
        Idle_1,
        Idle_2,
        Idle_3,
        Attack_1,
        Attack_2,
        Attack_3,
        Attack_4,
        Attack_5,
        Attack_6,
        Attack_7,
        Attack_8,
        Attack_9,
        Attack_10,
        Die_1,
        Die_2,
        Hit,
        Stun,
        Run_NoHand,
        Run_Weapon,
        Walk_NoHand,
        Walk_weapon,
        Win_1,
        Win_2,
        Jump,
        Jump_Landing,
        Jump_Attack
    }
    public enum weapon
    {
        None,
        Angel_Archer_Weapon,
        Devil_Archer_Weapon,
        Fox_Hunter_Weapon,
        Green_Archer_Weapon,
        Holy_Archer_Weapon,
        Indian_Archer_Weapon,
        Oni_Archer_Weapon,
        Poison_Archer_Weapon,
        Royal_Archer_Weapon,
        Steam_Punk_Archer_Weapon,
    }
    public Animation Set_Animation;
    public weapon Set_weapon;
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
    private SkeletonAnimation skeletonAnimation;
    private Coroutine dodge = null;
    private Coroutine charge = null;
    private Coroutine reload = null;
    private Coroutine invincibilityTime = null;
    private Coroutine hit = null;

    public GameObject firePrefab;
    public LayerMask groundLayer;
    public Slider steamiabar;
    public TextMeshProUGUI bulletText;

    protected override void Start()
    {
        base.Start();
        Init();
        skeletonAnimation = GetComponent<SkeletonAnimation>();
        groundcheck = transform.GetChild(0).GetComponent<Transform>();
        attackPoint = transform.GetChild(1).GetComponent<Transform>();
        target = GameObject.FindGameObjectWithTag("Monster");
        ChangeAnimation(0, Animation.Idle_1.ToString(), true);
        SetWeapon(weapon.Angel_Archer_Weapon.ToString());
    }
    private void FixedUpdate()
    {
        if (!isDead)
        {
            Movement();
            Filp();
            rb.velocity = new Vector2(horizon * Time.deltaTime, rb.velocity.y);
        }

        if (hp < 0)
        {
            state = State.Death;
            rb.velocity = Vector2.zero;
        }
    }
    private void ChangeAnimation(int track, string animationName, bool loop)
    {
        skeletonAnimation.AnimationState.SetAnimation(track, animationName, loop);
    }

    private void SetWeapon(string weaponName)
    {
        skeletonAnimation.Skeleton.SetAttachment("Weapon", weaponName);
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
        if (!isDead && !(state == State.Dodge) && !(state == State.Hit) && !(state == State.Invincibility))
        {
            Hp -= damage;
            hpBar.value = hp / maxHp;
            if (hp > 0)
            {
                hit = StartCoroutine(Invincibility_Time(hitpos, hitSpeed, Animation.Hit.ToString(), State.Hit));
            }
        }
    }

    private IEnumerator Invincibility_Time(float position, float speed, string animationName, State changeState)
    {
        bool isRight = true;
        float rightPostion = 0.0f;
        float leftPostion = 0.0f;
        if (changeState == State.Dodge)
        {
            state = State.Dodge;
            invincibilityTimer = 1.0f;
            skeletonAnimation.state.TimeScale = 2.0f;
            if (DodgeRight)
            {
                isRight = true;
            }
            else if (DodgeLeft)
            {
                isRight = false;
            }
        }
        else if (changeState == State.Hit)
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
        invincibilityTime = StartCoroutine(Invincibility_Timer());
        if (isRight)
        {
            rightPostion = transform.position.x + position;
            if (rightPostion > mapEnd)
            {
                rightPostion = mapEnd;
            }
            ChangeAnimation(0, animationName, false);
            while (rightPostion - transform.position.x >= 0)
            {
                moveLeft = false;
                moveRight = false;
                rb.position += Vector2.right * speed * Time.deltaTime;
                yield return null;
            }
            yield return new WaitForSeconds(skeletonAnimation.AnimationState.GetCurrent(0).TrackTime + 0.3f);
            skeletonAnimation.state.TimeScale = 1.0f;
            ChangeAnimation(0, Animation.Idle_1.ToString(), true);
        }
        else
        {
            leftPostion = transform.position.x - position;
            if (leftPostion < -mapEnd)
            {
                leftPostion = -mapEnd;
            }
            ChangeAnimation(0, animationName, false);
            while (leftPostion - transform.position.x <= 0)
            {
                moveLeft = false;
                moveRight = false;
                rb.position += Vector2.left * speed * Time.deltaTime;
                yield return null;
            }
            yield return new WaitForSeconds(skeletonAnimation.AnimationState.GetCurrent(0).TrackTime + 0.3f);
            skeletonAnimation.state.TimeScale = 1.0f;
            ChangeAnimation(0, Animation.Idle_1.ToString(), true);
        }
        state = State.Invincibility;
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
            ChangeAnimation(0, Animation.Walk_weapon.ToString(), true);
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
            ChangeAnimation(0, Animation.Idle_1.ToString(), true);
        }
    }

    public void PointerDownRight()
    {
        if (!(state == State.Attack) && !(state == State.Dodge) && !(state == State.Hit) && !isDead)
        {
            moveRight = true;
            ChangeAnimation(0, Animation.Walk_weapon.ToString(), true);
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
            ChangeAnimation(0, Animation.Idle_1.ToString(), true);
        }
    }
    public void PointerDownJump()
    {
        if (IsGrounded() && !(state == State.Attack) && !(state == State.Dodge) && !(state == State.Hit) && !isDead)
        {
            state = State.Jump;
            StartCoroutine(IdleCheck());
            rb.velocity = Vector2.up * jumpingPower;
        }
    }
    #endregion

    public void PointerDownAttack()
    {
        if ((state == State.Idle || state == State.Move) && IsGrounded())
        {
            if (state == State.Move)
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
    private IEnumerator Invincibility_Timer()
    {
        yield return new WaitForSeconds(invincibilityTimer);
        state = State.Idle;
        invincibilityTime = null;
    }
    public void PointerDownDodgeLeft()
    {
        if (stemia > 0 && IsGrounded() && !(state == State.Attack) && !(state == State.Dodge) && !(state == State.Hit) && !isDead)
        {
            SteamiaChange(-1);
            DodgeLeft = true;
            dodge = StartCoroutine(Invincibility_Time(dodgepos, dodgeSpeed, Animation.Run_Weapon.ToString(), State.Dodge));
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
            dodge = StartCoroutine(Invincibility_Time(dodgepos, dodgeSpeed, Animation.Run_Weapon.ToString(), State.Dodge));
        }
    }
    public void PointerUpDodgeRight()
    {
        DodgeRight = false;
    }
    private IEnumerator SteamiaCharge()
    {
        while (!isDead && !isfullCharge)
        {
            yield return new WaitForSeconds(chargeCoolTime);
            SteamiaChange(GameManager.instance.cha_St_Reg);
        }
    }
    private IEnumerator IdleCheck()
    {
        if (state == State.Attack)
        {
            ChangeAnimation(0, Animation.Attack_1.ToString(), false);
            yield return new WaitForSeconds(0.1f);
            yield return new WaitForSeconds(skeletonAnimation.AnimationState.GetCurrent(0).TrackTime);
        }
        if (state == State.Jump)
        {
            ChangeAnimation(0, Animation.Jump.ToString(), false);
            yield return new WaitForSeconds(0.1f);
            yield return new WaitForSeconds(skeletonAnimation.AnimationState.GetCurrent(0).TrackTime);
        }
        state = State.Idle;
        ChangeAnimation(0, Animation.Idle_1.ToString(), true);
    }

    private void SteamiaChange(float usingstemia)
    {
        Stemia += usingstemia;
        steamiabar.value = stemia / maxSteamia;
        if (stemia >= maxSteamia)
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
        if (isFacingRight && transform.position.x > target.transform.position.x || !isFacingRight && transform.position.x < target.transform.position.x)
        {
            isFacingRight = !isFacingRight;
            transform.Rotate(0f, 180f, 0f);
            bulletText.rectTransform.Rotate(0f, 180f, 0f);
        }
    }
}
