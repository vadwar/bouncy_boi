using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerTrampolineCollide : PlayerState
{
    float stunTime = 0;
    Vector2 initialVelocity;

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
        player.rb.linearVelocity = new Vector2(initialVelocity.y, initialVelocity.x);
    }

    public override void FrameUpdate()
    {
        base.FrameUpdate();

        stunTime -= Time.deltaTime;
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