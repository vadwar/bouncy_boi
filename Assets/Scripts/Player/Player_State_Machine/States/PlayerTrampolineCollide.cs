using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerTrampolineCollide : PlayerState
{
    private float stunTime = 0;
    private Vector2 initialVelocity;
    private int yVelocityBoost = 100;
    private int xVelocityBoost = 20;
    private float direction; // follows NESW as 0123 (given by TrampolineTrigger script)

    public void SetDirection(float newDirection)
    {
        direction = newDirection;
    }


    public PlayerTrampolineCollide(PlayerMovement player, PlayerStateMachine playerStateMachine) : base(player, playerStateMachine)
    {

    }

    public override void AnimationTriggerEvent(PlayerMovement.AnimationTriggerType triggerType)
    {
        base.AnimationTriggerEvent(triggerType);
    }

    public override void EnterState()
    {
        base.EnterState();
        initialVelocity = player.rb.linearVelocity;
        player.rb.linearVelocity = Vector2.zero;
        stunTime = player.trampolineDelay;
    }

    public override void ExitState()
    {
        base.ExitState();

        // North
        if (direction == 0)
        {
            player.rb.linearVelocity = new Vector2(initialVelocity.x, initialVelocity.y);
            player.VerticalVelocity = yVelocityBoost;
        }
        // East
        if (direction == 1)
        {
            player.rb.linearVelocity = new Vector2(-initialVelocity.x - xVelocityBoost, initialVelocity.y);
            Debug.Log(player.rb.linearVelocityX);
        }

        // South
        if (direction == 2)
        {
            player.rb.linearVelocity = new Vector2(initialVelocity.x, initialVelocity.y);
            player.VerticalVelocity = -yVelocityBoost;
        }

        // West
        if (direction == 3)
        {
            player.rb.linearVelocity = new Vector2(-initialVelocity.x + xVelocityBoost, initialVelocity.y);
            Debug.Log(player.rb.linearVelocityX);
        }

    }

    public override void FrameUpdate()
    {
        base.FrameUpdate();

        stunTime -= Time.fixedDeltaTime;
        if (stunTime < 0)
        {
            player.stateMachine.ChangeState(player.jumpFallState);
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }
}