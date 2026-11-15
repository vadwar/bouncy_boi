using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerGroundCollide : PlayerState
{
    float stunTime = 0;

    public PlayerGroundCollide(PlayerMovement player, PlayerStateMachine playerStateMachine) : base(player, playerStateMachine)
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
        stunTime = player.floorStunTime;
    }

    public override void ExitState()
    {
        base.ExitState();
    }

    public override void FrameUpdate()
    {
        base.FrameUpdate();

        stunTime -= Time.deltaTime;
        if (stunTime < 0)
        {
            player.stateMachine.ChangeState(player.walkState);
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }
}