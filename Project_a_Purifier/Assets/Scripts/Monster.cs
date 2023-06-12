using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class Monster : Unit
{
    public GameObject[] longAttacks;
    public GameObject[] dmgTMP;
    public GameObject stun;
    public Slider hpBar;
    public Slider powerlessBar;
    public float bossMaxHp;
    public float bossMaxPowerless = 100.0f;
    private Animator animator;
    private int count = 0;
    private Coroutine chase = null;
    private Coroutine powerlessState = null;
    private bool isCloseAttack = false;
    private const float closeAttackRange = 4.5f;
    private const float longAttackRange = 13.5f;

    [SerializeField] protected float bossPowerless;
    protected virtual float BossPowerless
    {
        get { return bossPowerless; }
        set
        {
            bossPowerless = value;

            if (!isDead && bossPowerless <= 0)
            {
                StopAllCoroutines();
                powerlessState = StartCoroutine(PowerLess());
            }
        }
    }
    protected override void Start()
    {
        base.Start();
        isFacingRight = false;
        bossMaxHp = 1000.0f;
        BossPowerless = bossMaxPowerless;
        hpBar.value = Hp / bossMaxHp;
        powerlessBar.value = BossPowerless / bossMaxPowerless;
        target = GameObject.FindGameObjectWithTag("Player");
        animator = GetComponent<Animator>();
        chase = StartCoroutine(Chase());
    }
    public override void HpChange(float damage)
    {
        Debug.Log(damage);
        if (!isDead)
        {
            Hp -= damage;
            hpBar.value = Hp / bossMaxHp;
        }
        StartCoroutine(dmgTMP[count].GetComponent<DmgEffect>().Dmg(damage));
        ++count;
        if (count >= dmgTMP.Length)
            count = 0;
    }

    public void PowerlessChange(float powerlessFigure)
    {
        if (!isDead)
        {
            BossPowerless -= powerlessFigure;
            powerlessBar.value = BossPowerless / bossMaxPowerless;
        }
    }

    protected override void Dead()
    {
        base.Dead();
        animator.SetBool(HashCode.deadID, true);
    }

    protected override void Movement()
    {
        chase = StartCoroutine(Chase());
    }

    private IEnumerator PowerLess()
    {
        if(state != State.Hit)
        {
            chase = null;
            stun.gameObject.SetActive(true);
            state = State.Hit;
            rb.velocity = Vector3.zero;
        }
        yield return new WaitForSeconds(5.0f);
        stun.gameObject.SetActive(false);
        BossPowerless = bossMaxPowerless;
        Movement();
    }

    private IEnumerator Chase()
    {
        state = State.Move;
        float chasingPoint = target.transform.position.x;
        if (chasingPoint - transform.position.x >= 0)
        {
            horizon = 1;
            animator.SetFloat(HashCode.moveID,Mathf.Abs(horizon));
            Filp();
            while (chasingPoint - transform.position.x >= 0)
            {
                rb.position += Vector2.right * Time.deltaTime;
                yield return null;
            }
        }
        else
        {
            horizon = -1;
            animator.SetFloat(HashCode.moveID, Mathf.Abs(horizon));
            Filp();
            while (chasingPoint - transform.position.x <= 0)
            {
                rb.position += Vector2.left * Time.deltaTime;
                yield return null;
            }
        }
        horizon = 0;
        animator.SetFloat(HashCode.moveID, horizon);
        state = State.Idle;
        Attack(target.transform.position.x);
    }
    private void Filp()
    {
        if (isFacingRight && horizon < 0f || !isFacingRight && horizon > 0f)
        {
            isFacingRight = !isFacingRight;
            transform.Rotate(0f, 180f, 0f);
            for(int i = 0; i < dmgTMP.Length; i++)
            {
                dmgTMP[i].transform.Rotate(0f, 180f, 0f);
            }
        }
    }
    private void Attack(float targetPosition)
    {
        if(Mathf.Abs(targetPosition - transform.position.x) < closeAttackRange)
        {
            isCloseAttack = true;
            state = State.Attack;
            AttackDetecting(target.transform.position.x);
        }
        else if(Mathf.Abs(targetPosition - transform.position.x) > closeAttackRange && Mathf.Abs(targetPosition - transform.position.x) < longAttackRange)
        {
            isCloseAttack = false;
            state = State.Attack;
            AttackDetecting(target.transform.position.x);
        }
        else
        {
            Movement();
        }
    }
    private void AttackDetecting(float targetPosition)
    {
        if (isCloseAttack)
        {
            if (targetPosition > transform.position.x && !isFacingRight)
            {
                isFacingRight = !isFacingRight;
                transform.Rotate(0f, 180f, 0f);
            }
            else if (targetPosition < transform.position.x && isFacingRight)
            {
                isFacingRight = !isFacingRight;
                transform.Rotate(0f, 180f, 0f);
            }
            StartCoroutine(IdleCheck(HashCode.attackID));
        }
        else
        {
            StartCoroutine(IdleCheck(HashCode.longAttackID));
        }
    }

    private IEnumerator IdleCheck(int _attackID)
    {
        if (state == State.Attack && isCloseAttack)
        {
            animator.SetBool(_attackID, true);
            yield return new WaitForSeconds(0.1f);
            yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);
            animator.SetBool(_attackID, false);
        }

        if (state == State.Attack && !isCloseAttack)
        {
            animator.SetBool(_attackID, true);
            yield return new WaitForSeconds(0.1f);
            yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);
            animator.SetBool(_attackID, false);
            if (target.transform.position.x - transform.position.x > 0)
            {
                longAttacks[1].gameObject.SetActive(true);
                longAttacks[1].GetComponent<Monster_LongAttack>().AnimationSetActive();
            }
            else
            {
                longAttacks[0].gameObject.SetActive(true);
                longAttacks[0].GetComponent<Monster_LongAttack>().AnimationSetActive();
            }
        }
        state = State.Idle;
        yield return new WaitForSeconds(1.5f);
        Movement();
    }
}
