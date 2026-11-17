using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerIdle : PlayerState
{
    public PlayerIdle(PlayerMovement player, PlayerStateMachine playerStateMachine) : base(player, playerStateMachine)
    {

    }

    public override void AnimationTriggerEvent(PlayerMovement.AnimationTriggerType triggerType)
    {
        base.AnimationTriggerEvent(triggerType);
    }

    public override void EnterState()
    {
        base.EnterState();
        player.animator.SetTrigger("Still");
    }

    public override void ExitState()
    {
        base.ExitState();
    }

    public override void FrameUpdate()
    {
        base.FrameUpdate();

        if (InputManager.instance.dashInput && player.dashCD <= 0)
        {
            player.stateMachine.ChangeState(player.dashState);
        }

        if (InputManager.instance.jumpJustPressed || player._isJumping)
        {
            player.stateMachine.ChangeState(player.jumpFallState);
        }
        if (InputManager.instance.moveInput.x != 0)
        {
            player.stateMachine.ChangeState(player.walkState);
        }

        if (!player._isGrounded)
        {
            player.stateMachine.ChangeState(player.jumpFallState);
        }
    }

    public override void PhysicsUpdate()
    {
        player.Move(InputManager.instance.moveInput);
    }
}
