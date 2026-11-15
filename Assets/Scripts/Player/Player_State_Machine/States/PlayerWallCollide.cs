using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerWallCollide : PlayerState
{
    float stunTime= 0;

    public PlayerWallCollide(PlayerMovement player, PlayerStateMachine playerStateMachine) : base(player, playerStateMachine)
    {

    }

    public override void AnimationTriggerEvent(PlayerMovement.AnimationTriggerType triggerType)
    {
        base.AnimationTriggerEvent(triggerType);
    }

    public override void EnterState()
    {
        base.EnterState();
        player.rb.linearVelocity = Vector2.zero;
        stunTime = player.wallStunTime;
    }

    public override void ExitState()
    {
        base.ExitState();
    }

    public override void FrameUpdate()
    {
        base.FrameUpdate();

        stunTime -= Time.deltaTime;
        if(stunTime < 0 ) {
            player.stateMachine.ChangeState(player.jumpFallState);
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }
}