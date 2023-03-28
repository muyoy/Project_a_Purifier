using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : Unit
{
    public float stemia;
    public float dodgepos;
    public float dodgeSpeed;
    public float rightDodgepos;
    public float leftDodgepos;
    public float jumpingPower;

    private bool isDodge = false;
    private bool moveLeft = false;
    private bool moveRight = false;
    private bool DodgeLeft = false;
    private bool DodgeRight = false;


    private Transform attackPoint;
    private Transform groundcheck;
    private Coroutine dodge = null;
    public GameObject firePrefab;
    public LayerMask groundLayer;

    protected override void Start()
    {
        base.Start();
        groundcheck = transform.GetChild(0).GetComponent<Transform>();
        attackPoint = transform.GetChild(1).GetComponent<Transform>();
    }
    private void FixedUpdate()
    {
        rb.velocity = new Vector2(horizon * Time.deltaTime, rb.velocity.y);
        animator.SetFloat("Speed", Mathf.Abs(horizon));
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
        moveLeft = true;
    }
    public void PointerUpLeft()
    {
        moveLeft = false;
    }

    public void PointerDownRight()
    {
        moveRight = true;
    }
    public void PointerUpRight()
    {
        moveRight = false;
    }
    public void PointerDownJump()
    {
        if (IsGrounded())
        {
            rb.velocity = Vector2.up * jumpingPower;
        }
    }
    #endregion

    public void PointerDownAttack()
    {
        Instantiate(firePrefab, attackPoint.position, attackPoint.rotation);
    }
    public void PointerDownDodgeLeft()
    {
        DodgeLeft = true;
        dodge = StartCoroutine("Dodge");
    }
    public void PointerUpDodgeLeft()
    {
        DodgeLeft = false;
    }
    public void PointerDownDodgeRight()
    {
        DodgeRight = true;
        dodge = StartCoroutine("Dodge");
    }
    public void PointerUpDodgeRight()
    {
        DodgeRight = false;
    }
    private IEnumerator Dodge()
    {
        if (DodgeRight)
        {
            rightDodgepos = transform.position.x + dodgepos;
            while (rightDodgepos - transform.position.x >= 0)
            {
                rb.position += Vector2.right * dodgeSpeed * Time.deltaTime;
                yield return null;
            }
        }

        if (DodgeLeft)
        {
            leftDodgepos = transform.position.x - dodgepos;
            while (leftDodgepos - transform.position.x <= 0)
            {
                rb.position += Vector2.left * dodgeSpeed * Time.deltaTime;
                yield return null;
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
