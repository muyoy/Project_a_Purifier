using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monster : Unit
{
    private Coroutine chase = null;
    private float closeAttackRange = 4.5f;
    private float longAttackRange = 13.5f;
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
            if(targetPosition < transform.position.x)
            {
                isFacingRight = !isFacingRight;
                transform.Rotate(0f, 180f, 0f);
            }
            state = State.Attack;
            StartCoroutine(IdleCheck());
        }
        else if(Mathf.Abs(targetPosition - transform.position.x) > closeAttackRange && Mathf.Abs(targetPosition - transform.position.x) < longAttackRange)
        {
            Debug.Log("원거리 공격");
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
}
