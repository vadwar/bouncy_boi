using UnityEngine;

public class PlayerJumpingFalling : PlayerState
{
    private float buffer;

    public PlayerJumpingFalling(PlayerMovement player, PlayerStateMachine playerStateMachine) : base(player, playerStateMachine)
    {

    }

    public override void AnimationTriggerEvent(PlayerMovement.AnimationTriggerType triggerType)
    {
        base.AnimationTriggerEvent(triggerType);
    }

    public override void EnterState()
    {
        base.EnterState();
        buffer = 0.2f;
        player.Move(InputManager.instance.moveInput);
        player.JumpChecks();
        player.Jump();
    }

    public override void ExitState()
    {
        player.JumpChecks();
        base.ExitState();
    }

    public override void FrameUpdate()
    {
        base.FrameUpdate();

        buffer -= Time.deltaTime;

        player.yCameraDamping();

        player.JumpChecks();

        if (InputManager.instance.dashInput && player.dashCD <= 0)
        {
            player.stateMachine.ChangeState(player.dashState);
        }

        if (buffer < 0 && player._isGrounded)
        {
            player.stateMachine.ChangeState(player.idleState);
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();

        player.Move(InputManager.instance.moveInput);
        player.Jump();

    }
}
