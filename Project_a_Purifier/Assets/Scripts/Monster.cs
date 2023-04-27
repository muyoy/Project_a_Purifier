using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monster : Unit
{
    //public GameObject[] longAttacks;
    [SerializeField] public List<GameObject[]> longAttacks = new List<GameObject[]>();
    private Coroutine chase = null;
    private bool isCloseAttack = false;
    private const float closeAttackRange = 4.5f;
    private const float longAttackRange = 13.5f;
    protected override void Start()
    {
        base.Start();
        isFacingRight = false;
        target = GameObject.FindGameObjectWithTag("Player");
        chase = StartCoroutine(Chase());
    }
    public override void HpChange(float damage)
    {
        Debug.Log(damage);
        if (!isDead)
        {
            Hp -= damage;
        }
    }

    protected override void Movement()
    {
        chase = StartCoroutine(Chase());
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
            if (targetPosition - transform.position.x > 0)
            {
                //longAttacks[1].gameObject.SetActive(true);
                //longAttacks[1].GetComponentInChildren<Monster_CloseAttack>().AnimationSetActive();
            }
            else
            {
                //longAttacks[0].gameObject.SetActive(true);
                //longAttacks[0].GetComponentInChildren<Monster_CloseAttack>().AnimationSetActive();
            }
        }
    }

    private IEnumerator IdleCheck(int _attackID)
    {
        if (state == State.Attack)
        {
            animator.SetBool(_attackID, true);
            yield return new WaitForSeconds(0.1f);
            yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);
            animator.SetBool(_attackID, false);
        }
        state = State.Idle;
    }
}
