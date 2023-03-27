using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : Unit
{
    public float dodgespeed;
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
        else if (DodgeLeft && !isDodge)
        {
            horizon = -dodgespeed;
        }
        else if (DodgeRight)
        {
            horizon = dodgespeed;
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
        Debug.Log("Attack");
        Instantiate(firePrefab, attackPoint.position, attackPoint.rotation);
    }
    public void PointerDownDodgeLeft()
    {
        Debug.Log("DodgeLeft");
        DodgeLeft = true;
        dodge = StartCoroutine("Dodge");
    }
    public void PointerUpDodgeLeft()
    {
        Debug.Log("DodgeLeft");
        DodgeLeft = false;
    }
    public void PointerDownDodgeRight()
    {
        Debug.Log("DodgeRight");
        DodgeRight = true;
    }
    public void PointerUpDodgeRight()
    {
        Debug.Log("DodgeLeft");
        DodgeRight = false;
    }
    private IEnumerator Dodge()
    {
        yield return new WaitForSeconds(0.2f);
        isDodge = true;
        yield return new WaitForSeconds(5f);
        isDodge = false;
        yield return null;
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
