using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    //회피알고리즘 다시 짜야함
    public enum State
    {
        Idle, Move, Attack, Dodge, Jump, Hit, Death
    }
    public State state;
    public float speed;
    public float dodgespeed;
    public float jumpingPower;

    private float horizontal;
    private bool isDodge = false;
    private bool isFacingRight = true;
    private bool moveLeft = false;
    private bool moveRight = false;
    private bool DodgeLeft = false;
    private bool DodgeRight = false;

    private Rigidbody2D rb;
    private Animator animator;
    private Transform groundcheck;

    public LayerMask groundLayer;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        groundcheck = transform.GetChild(0).GetComponent<Transform>();
    }
    private void FixedUpdate()
    {
        rb.velocity = new Vector2(horizontal * Time.deltaTime, rb.velocity.y);
        animator.SetFloat("Speed", Mathf.Abs(horizontal));
    }
    private void Update()
    {
        Movement();
        Filp();
    }

    private void Movement()
    {
        if (moveLeft)
        {
            horizontal = -speed;
        }
        else if (moveRight)
        {
            horizontal = speed;
        }
        else if (DodgeLeft && !isDodge)
        {
            horizontal = -dodgespeed;
        }
        else if (DodgeRight)
        {
            horizontal = dodgespeed;
        }
        else
        {
            horizontal = 0;
        }
    }
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
    public void PointerDownAttack()
    {
        Debug.Log("Attack");
    }
    public void PointerDownDodgeLeft()
    {
        Debug.Log("DodgeLeft");
        DodgeLeft = true;
        StartCoroutine("Dodge");
    }
    public void PointerUpDodgeLeft()
    {
        Debug.Log("DodgeLeft");
        StopCoroutine("Dodge");
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
        isDodge = true;
        yield return new WaitForSeconds(10f);
        isDodge = false;
    }
    private bool IsGrounded()
    {
        return Physics2D.OverlapCircle(groundcheck.position, 0.2f, groundLayer);
    }
    private void Filp()
    {
        if (isFacingRight && horizontal < 0f || !isFacingRight && horizontal > 0f)
        {
            isFacingRight = !isFacingRight;
            transform.Rotate(0f, 180f, 0f);
        }
    }
}
