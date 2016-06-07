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
            if (value != _animation)
            {
                _animation = value;
                //Debug.Log(_animation);//uncomment to print the attempted animation state for debugging purposes
                ani.Play(_animation, 0);
            }
        }
    }

    protected enum ActionState
    {
        Idle, Walking, Running, Attacking, Blasting
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
            if (state != value)
            {
                state = value;
                Act();
            }
        }
    }

    public enum PlayerDirection
    {
        Down, Up, Left, Right
    }
    private PlayerDirection dir;
    protected PlayerDirection Dir
    {
        get
        {
            return dir;
        }
        set
        {
            if (value == PlayerDirection.Right)
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

    GameObject blast;

    // Use this for initialization
    protected virtual void Start()
    {
        rb2d = GetComponent<Rigidbody2D>();
        ani = GetComponent<Animator>();

        blast = (GameObject)Resources.Load("Prefabs/Blast");
    }

    [Range(0.7f, 1f)]
    public float directionSwitch = 0.75f;
    protected bool running = false;

    //public static bool InputManager.isDoubleTap = false;
    protected virtual void Update()
    {
        Act();

        running = InputManager.isDoubleTap || InputManager.isQuickTap;

        switch (Dir)
        {
            case PlayerDirection.Up:
                if (InputManager.InputDirection.y < 0f)
                {
                    Dir = PlayerDirection.Down;
                    break;
                }
                if (InputManager.InputDirection.x > directionSwitch)
                {
                    Dir = PlayerDirection.Right;
                    break;
                }
                if (InputManager.InputDirection.x < -directionSwitch)
                {
                    Dir = PlayerDirection.Left;
                    break;
                }
                break;
            case PlayerDirection.Left:
                if (InputManager.InputDirection.y < -directionSwitch)
                {
                    Dir = PlayerDirection.Down;
                    break;
                }
                if (InputManager.InputDirection.y > directionSwitch)
                {
                    Dir = PlayerDirection.Up;
                    break;
                }
                if (InputManager.InputDirection.x > 0)
                {
                    Dir = PlayerDirection.Right;
                    break;
                }
                break;
            case PlayerDirection.Right:
                if (InputManager.InputDirection.y < -directionSwitch)
                {
                    Dir = PlayerDirection.Down;
                    break;
                }
                if (InputManager.InputDirection.y > directionSwitch)
                {
                    Dir = PlayerDirection.Up;
                    break;
                }
                if (InputManager.InputDirection.x < 0)
                {
                    Dir = PlayerDirection.Left;
                    break;
                }
                break;
            case PlayerDirection.Down:
                if (InputManager.InputDirection.y > 0f)
                {
                    Dir = PlayerDirection.Up;
                    break;
                }
                if (InputManager.InputDirection.x > directionSwitch)
                {
                    Dir = PlayerDirection.Right;
                    break;
                }
                if (InputManager.InputDirection.x < -directionSwitch)
                {
                    Dir = PlayerDirection.Left;
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
            case ActionState.Blasting:
                Blast();
                break;
            default:
                break;
        }
    }

    protected virtual void Idle()
    {
        switch (Dir)
        {
            case PlayerDirection.Up:
                Animation = "IdleBack";
                break;
            case PlayerDirection.Left:
            case PlayerDirection.Right:
                Animation = "IdleSide";
                break;
            case PlayerDirection.Down:
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
            case PlayerDirection.Up:
                Animation = "WalkBack";
                break;
            case PlayerDirection.Left:
            case PlayerDirection.Right:
                Animation = "WalkSide";
                break;
            case PlayerDirection.Down:
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
            case PlayerDirection.Up:
                Animation = "RunBack";
                break;
            case PlayerDirection.Left:
            case PlayerDirection.Right:
                Animation = "RunSide";
                break;
            case PlayerDirection.Down:
                Animation = "RunFront";
                break;
        }
    }

    bool attackInitiated = false;
    string attackName;
    int attackNum = 0;
    protected virtual void Attack()
    {
        if (!attackInitiated)
        {
            if (InputManager.MovementMagnitude > 0f)
            {
                rb2d.velocity = InputManager.InputDirection * walkSpeed * 0.618f;
            }
            attackInitiated = true;
            switch (Dir)
            {
                case PlayerDirection.Up:
                    if (attackNum >= 50)
                    {
                        attackNum = Random.Range(0, 100 - attackNum / 2);
                        attackName = "Attack1Back";
                    }
                    else
                    {
                        attackNum = Random.Range(attackNum / 2, 100 + attackNum / 2);
                        attackName = "Attack2Back";
                    }
                    break;
                case PlayerDirection.Left:
                case PlayerDirection.Right:
                    if (attackNum >= 50)
                    {
                        attackNum = Random.Range(0, 100 - attackNum / 2);
                        attackName = "Attack1Side";
                    }
                    else
                    {
                        attackNum = Random.Range(attackNum / 2, 100 + attackNum / 2);
                        attackName = "Attack2Side";
                    }
                    break;
                case PlayerDirection.Down:
                    if (attackNum >= 50)
                    {
                        attackNum = Random.Range(0, 100 - attackNum / 2);
                        attackName = "Attack1Front";
                    }
                    else
                    {
                        attackNum = Random.Range(attackNum / 2, 100 + attackNum / 2);
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
                if (InputManager.MovementMagnitude > 0f)
                {
                    if (InputManager.isDoubleTap)
                    {
                        rb2d.velocity = InputManager.InputDirection * walkSpeed * 1.618f;
                    }
                    else
                    {
                        rb2d.velocity = InputManager.InputDirection * walkSpeed * 1;
                    }
                }
                if (Input.GetButtonDown("NormalAttack"))
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
    
    protected bool blastInitiated = false;
    string blastName;

    private float blastWait = 0f;
    private float maxBlastWait = 0.35f;

    protected bool blastEnd
    {
        get { return blastWait > maxBlastWait; }
    }

    private float chargeTimer = 0f;
    public float secondsHeldUntilChargeStart = 0.5f;

    protected virtual void Blast()
    {
        if (Input.GetButton("SpecialAttack"))
        {
            blastWait = 0f;
            chargeTimer += Time.deltaTime;
            if (chargeTimer >= secondsHeldUntilChargeStart)
            {
                blastName = "ChargeBlast";
            }
        }
        else if (!Input.GetButtonUp("SpecialAttack"))
        {
            chargeTimer = 0f;
            blastWait += Time.deltaTime;
        }
        if (!blastInitiated)
        {
            rb2d.velocity *= 0f;
            blastName = "ChargeBlast";
            blastInitiated = true;
        }
        else
        {
            if (Input.GetButtonUp("SpecialAttack"))
            {
                if (ani.GetCurrentAnimatorStateInfo(0).ToString().Contains("ChargeBlast") || blastName.Contains("Blast2"))
                {
                    blastName = "Blast1";
                }
                else
                {
                    blastName = "Blast2";
                }
                GameObject blastObject = GameObject.Instantiate(blast);
                blastObject.GetComponent<BlastScript>().Initialize(InputManager.InputDirection,transform.position,Dir);
            }
        }

        string aniString = blastName;
        switch (Dir)
        {
            case PlayerDirection.Up:
                aniString += "Back";
                break;
            case PlayerDirection.Left:
            case PlayerDirection.Right:
                aniString += "Side";
                break;
            case PlayerDirection.Down:
                aniString += "Front";
                break;
        }

        Animation = aniString;
    }
}
