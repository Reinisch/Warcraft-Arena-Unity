using System;
using UnityEngine;

public class Unit : MonoBehaviour
{
    public int id;
    public string unitName;
    public bool isHumanPlayer;
    public bool isDead;
    public bool isGrounded;
    public Character character;
    public Transform castPoint;
    public Transform centerPoint;

    public float moveTimer = 0;
    public float castTimer = 0;

    public int Id 
    {
        get { return id; }
        set { id = value; }
    }
    public Vector3 Position
    {
        get { return transform.position; }
        set { transform.position = value; }
    }

    private CapsuleCollider unitCollider;
    private Animator animator;

    public bool IsHumanPlayer
    { 
        get { return isHumanPlayer; }
        set { isHumanPlayer = value; }
    }
    public bool IsDead
    { get { return isDead; }
        set { isDead = value; }
    }
    public bool IsGrounded 
    {
        get { return isGrounded; }
        set
        {
            isGrounded = value;
        }
    }
    public bool IsMovementBlocked
    {
        get
        {
            return Character.states[EntityStateType.Root].InEffect || Character.states[EntityStateType.Stun].InEffect;
        }
    }

    public CapsuleCollider UnitCollider { get { return unitCollider; } }
    public Character Character { get { return character; } }
    public Animator Animator { get { return animator; } }


    void Awake()
    {
        animator = GetComponent<Animator>();
        unitCollider = GetComponent<CapsuleCollider>();

        Character.Initialize(this);
    }

    void Update()
    {
        UpdateUnit(ArenaManager.Instanse);
    }

    public void UpdateUnit(ArenaManager world)
    {
        if (moveTimer > 0)
            moveTimer -= Time.deltaTime;

        Character.Update(this, world);
    }

    public void TriggerInstantCast()
    {
        // Switch leg animation for casting
        if (animator.GetBool("Grounded"))
        {
            if (animator.GetFloat("Speed") > 0.1f)
                animator.Play("Run", 1);
            else
                animator.Play("Cast", 1);
        }
        else
            animator.Play("Air", 1);
    }
}