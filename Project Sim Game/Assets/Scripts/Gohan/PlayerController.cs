using UnityEngine;
using System.Collections;

public class PlayerController : PlayerBackend
{

    protected override void Start()
    {
        base.Start();
    }
    
    protected override void Update()
    {

        base.Update();
    }
    
    protected override void Idle()
    {
        if (Input.GetButtonDown("NormalAttack"))
        {
            State = ActionState.Attacking;
            return;
        }

        else if (Input.GetButtonDown("SpecialAttack"))
        {
            State = ActionState.Blasting;
            return;
        }

        if (InputManager.MovementMagnitude > 0f)
        {
            if(!running)
            {
                State = ActionState.Walking;
            }
            else
            {
                State = ActionState.Running;
            }
        }
        else
        {
            base.Idle();
            rb2d.velocity = Vector2.zero;
        }
    }

    protected override void Walk()
    {
        if (Input.GetButtonDown("NormalAttack"))
        {
            State = ActionState.Attacking;
            return;
        }

        else if (Input.GetButtonDown("SpecialAttack"))
        {
            State = ActionState.Blasting;
            return;
        }

        if (InputManager.MovementMagnitude > 0f)
        {
            base.Walk();
            if (InputManager.MovementMagnitude < 0.618f)
            {
                rb2d.velocity = InputManager.InputDirection * walkSpeed * 0.618f;
            }
            else
            {
                rb2d.velocity = InputManager.InputDirection * walkSpeed;
            }
        }
        else
        {
            State = ActionState.Idle;
        }
    }

    protected override void Run()
    {
        if (Input.GetButtonDown("NormalAttack"))
        {
            State = ActionState.Attacking;
            return;
        }
        else if (Input.GetButtonDown("SpecialAttack"))
        {
            State = ActionState.Blasting;
            return;
        }
        if (InputManager.MovementMagnitude > 0f)
        {
            base.Run();
            rb2d.velocity = InputManager.InputDirection * walkSpeed * 1.618f;
        }
        else
        {
            State = ActionState.Idle;
        }
    }

    protected override void Attack()
    {
        base.Attack();
    }

    protected override void Blast()
    {
        base.Blast();
        if(blastEnd)
        {
            State = ActionState.Idle;
            blastInitiated = false;
        }
    }
}