using UnityEngine;
using System.Collections;

public class PlayerBackend : MonoBehaviour
{
    public float walkSpeed = 6f;

    protected Rigidbody2D rb2d;
    protected Animator ani;

    private string _animation;
    protected string Animation
    {
        get { return _animation; }
        set
        {
            if(value != _animation)
            {
                _animation = value;
                ani.Play(_animation);
            }
        }
    }

    protected enum ActionState
    {
        Idle, Walking, Running, Attacking
    }
    private ActionState state;
    protected ActionState State
    {
        get
        {
            return state;
        }
        set
        {
            if(state != value)
            {
                state = value;
                Act();
            }
        }
    }

    protected enum Direction
    {
        Down, Up, Left, Right
    }
    private Direction dir;
    protected Direction Dir
    {
        get
        {
            return dir;
        }
        set
        {
            if(value == Direction.Right)
            {
                transform.localScale = new Vector3(-1, 1, 1);
            }
            else
            {
                transform.localScale = new Vector3(1, 1, 1);
            }

            dir = value;
        }
    }

    // Use this for initialization
    protected virtual void Start()
    {
        rb2d = GetComponent<Rigidbody2D>();
        ani = GetComponent<Animator>();
    }

    [Range (0.7f,1f)]
    public float directionSwitch = 0.75f;

    public static bool running = false;
    protected virtual void Update()
    {
        Act();

        switch (Dir)
        {
            case Direction.Up:
                if (InputManager.MovementDirection.y < 0f)
                {
                    Dir = Direction.Down;
                    break;
                }
                if (InputManager.MovementDirection.x > directionSwitch)
                {
                    Dir = Direction.Right;
                    break;
                }
                if (InputManager.MovementDirection.x < -directionSwitch)
                {
                    Dir = Direction.Left;
                    break;
                }
                break;
            case Direction.Left:
                if (InputManager.MovementDirection.y < -directionSwitch)
                {
                    Dir = Direction.Down;
                    break;
                }
                if (InputManager.MovementDirection.y > directionSwitch)
                {
                    Dir = Direction.Up;
                    break;
                }
                if (InputManager.MovementDirection.x > 0)
                {
                    Dir = Direction.Right;
                    break;
                }
                break;
            case Direction.Right:
                if (InputManager.MovementDirection.y < -directionSwitch)
                {
                    Dir = Direction.Down;
                    break;
                }
                if (InputManager.MovementDirection.y > directionSwitch)
                {
                    Dir = Direction.Up;
                    break;
                }
                if (InputManager.MovementDirection.x < 0)
                {
                    Dir = Direction.Left;
                    break;
                }
                break;
            case Direction.Down:
                if (InputManager.MovementDirection.y > 0f)
                {
                    Dir = Direction.Up;
                    break;
                }
                if (InputManager.MovementDirection.x > directionSwitch)
                {
                    Dir = Direction.Right;
                    break;
                }
                if (InputManager.MovementDirection.x < -directionSwitch)
                {
                    Dir = Direction.Left;
                    break;
                }
                break;
        }
    }

    void Act()
    {
        switch (state)
        {
            case ActionState.Idle:
                Idle();
                break;
            case ActionState.Walking:
                Walk();
                break;
            case ActionState.Running:
                Run();
                break;
            case ActionState.Attacking:
                Attack();
                break;
            default:
                break;
        }
    }

    protected virtual void Idle()
    {
        switch (Dir)
        {
            case Direction.Up:
                Animation = "IdleBack";
                break;
            case Direction.Left:
            case Direction.Right:
                Animation = "IdleSide";
                break;
            case Direction.Down:
                Animation = "IdleFront";
                break;
        }
    }
    protected virtual void Walk()
    {
        if (running)
        {
            State = ActionState.Running;
            return;
        }
        switch (Dir)
        {
            case Direction.Up:
                Animation = "WalkBack";
                break;
            case Direction.Left:
            case Direction.Right:
                Animation = "WalkSide";
                break;
            case Direction.Down:
                Animation = "WalkFront";
                break;
        }
    }
    protected virtual void Run()
    {
        if (!running)
        {
            State = ActionState.Walking;
            return;
        }
        switch (Dir)
        {
            case Direction.Up:
                Animation = "RunBack";
                break;
            case Direction.Left:
            case Direction.Right:
                Animation = "RunSide";
                break;
            case Direction.Down:
                Animation = "RunFront";
                break;
        }
    }

    bool attackInitiated = false;
    string attackName;
    int attackNum = 0;
    protected virtual void Attack()
    {
        if(!attackInitiated)
        {
            if (InputManager.MovementMagnitude > 0f)
            {
                rb2d.velocity = InputManager.MovementDirection * walkSpeed * 0.618f;
            }
            attackInitiated = true;
            switch (Dir)
            {
                case Direction.Up:
                    if (attackNum >= 50)
                    {
                        attackNum = Random.Range(0, 100 - attackNum/2);
                        attackName = "Attack1Back";
                    }
                    else
                    {
                        attackNum = Random.Range(attackNum/2, 100 + attackNum/2);
                        attackName = "Attack2Back";
                    }
                    break;
                case Direction.Left:
                case Direction.Right:
                    if (attackNum >= 50)
                    {
                        attackNum = Random.Range(0, 100 - attackNum/2);
                        attackName = "Attack1Side";
                    }
                    else
                    {
                        attackNum = Random.Range(attackNum/2, 100 + attackNum/2);
                        attackName = "Attack2Side";
                    }
                    break;
                case Direction.Down:
                    if (attackNum >= 50)
                    {
                        attackNum = Random.Range(0, 100 - attackNum/2);
                        attackName = "Attack1Front";
                    }
                    else
                    {
                        attackNum = Random.Range(attackNum/2, 100 + attackNum/2);
                        attackName = "Attack2Front";
                    }
                    break;
            }
            Animation = attackName;
            StartCoroutine(AttackChaining());
        }
    }

    bool attackHold = true;
    bool shouldChainAttack = false;
    IEnumerator AttackChaining()
    {
        float timer = Time.deltaTime;
        float timeHeld = 0f;
        attackHold = true;
        while (ani.GetCurrentAnimatorStateInfo(0).length + timeHeld >= timer + timeHeld / 2f)
        {
            yield return null;
            timer += Time.deltaTime;
            if (timer >= ani.GetCurrentAnimatorStateInfo(0).length / 2)
            {
                ani.speed = 1f;
                if(InputManager.MovementMagnitude > 0f)
                {
                    if(running)
                    {
                        rb2d.velocity = InputManager.MovementDirection * walkSpeed * 1.618f;
                    }
                    else
                    {
                        rb2d.velocity = InputManager.MovementDirection * walkSpeed * 1;
                    }
                }
                if(Input.GetButtonDown("NormalAttack"))
                {
                    shouldChainAttack = true;
                }
            }
            else
            {
                if (timer < ani.GetCurrentAnimatorStateInfo(0).length / 3)
                {
                    if (Input.GetButtonUp("NormalAttack"))
                    {
                        attackHold = false;
                    }
                    if (attackHold)
                    {
                        timeHeld += Time.deltaTime;
                        ani.speed = 0.25f;
                    }
                    else
                    {
                        ani.speed = 1f;
                    }
                }
                else
                {
                    ani.speed = 1f;
                }
                rb2d.velocity = Vector2.Lerp(rb2d.velocity, Vector2.zero, timer / ani.GetCurrentAnimatorStateInfo(0).length);
            }
        }
        attackInitiated = false;
        if (!shouldChainAttack)
        {
            State = ActionState.Idle;
        }
        else
        {
            Attack();
            shouldChainAttack = false;
        }
    }
}
