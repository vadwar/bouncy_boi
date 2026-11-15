using UnityEngine;
using UnityEngine.InputSystem;

namespace Assets.Scripts.Player.Player_State_Machine.States
{
    public class PlayerWalk : PlayerState
    {
        public PlayerWalk(PlayerMovement player, PlayerStateMachine playerStateMachine) : base(player, playerStateMachine)
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
            player.Move(InputManager.instance.moveInput);
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

            if (InputManager.instance.jumpJustPressed)
            {
                player.stateMachine.ChangeState(player.jumpFallState);
            }
            if (!player._isGrounded)
            {
                player.stateMachine.ChangeState(player.jumpFallState);
            }

            if (InputManager.instance.moveInput.Equals(Vector2.zero))
            {
                player.stateMachine.ChangeState(player.idleState);
            }
        }

        public override void PhysicsUpdate()
        {
            player.Move(InputManager.instance.moveInput);
        }
    }
}