using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{
    public enum State { Idle, Move, Attack, Hit, Death, Dodge, Jump }
    public State state;
    public bool isDead;
    public bool isMove;
    public bool isHit;
    protected bool isFacingRight = true;
    protected GameObject target = null;
    protected Rigidbody2D rb;
    protected Animator animator;
    protected float maxHp = 0.0f;
    protected float horizon; 
    [SerializeField] protected float moveSpeed = 0.0f; 

    [SerializeField] protected float hp;
    protected virtual float Hp
    {
        get { return hp; }
        set
        {
            hp = value;

            if(!isDead && hp <= 0)
            {
                hp = -1;
                Dead();
            }
        }
    }

    protected virtual void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    protected virtual void Init()
    {

    }

    protected virtual void Movement()
    {

    }

    protected virtual void Dead()
    {

    }
}
