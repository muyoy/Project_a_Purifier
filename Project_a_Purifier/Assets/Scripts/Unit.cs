using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Unit : MonoBehaviour
{
    public enum State { Idle, Move, Attack, Hit, Death, Dodge, Jump, Invincibility }
    public State state;
    public bool isDead;
    protected bool isFacingRight = true;
    protected GameObject target = null;
    protected Rigidbody2D rb;
    [SerializeField] protected Slider hpBar;         //hp UI
    protected float maxHp;
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
    }

    public virtual void HpChange(float damage)
    {

    }

    protected virtual void Init()
    {
        hp = GameManager.instance.cha_Hp;
        maxHp = GameManager.instance.cha_Hp;
        moveSpeed = GameManager.instance.move_Speed * 5;
    }

    protected virtual void Movement()
    {

    }

    protected virtual void Dead()
    {
        isDead = true;
    }
}
