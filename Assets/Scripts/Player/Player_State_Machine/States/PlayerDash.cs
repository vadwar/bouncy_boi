using UnityEngine;

namespace Assets.Scripts.Player.Player_State_Machine.States
{
    public class PlayerDash : PlayerState
    {
        float dashTime;
        public PlayerDash(PlayerMovement player, PlayerStateMachine playerStateMachine) : base(player, playerStateMachine)
        {

        }

        public override void AnimationTriggerEvent(PlayerMovement.AnimationTriggerType triggerType)
        {
            base.AnimationTriggerEvent(triggerType);
        }

        public override void EnterState()
        {
            base.EnterState();
            player._isPastApexThreshold = false;
            player.dashDir = player.isFacingRight ? 1 : -1;
            player.rb.linearVelocityX = 0;
            player.dashCD = 0.25f;
            player.rb.linearVelocityY = 0;
            player.VerticalVelocity = 0;
            dashTime = 0.33f;
            player.animator.SetTrigger("Dash");
        }

        public override void ExitState()
        {
            player.rb.linearVelocityX += player.dashDir * player.DASHSPEED;
            player.rb.linearVelocityY = 20;
            
            player.animator.SetTrigger("Still");
            base.ExitState();
        }

        public override void FrameUpdate()
        {
            base.FrameUpdate();
        }

        public override void PhysicsUpdate()
        {
            base.PhysicsUpdate();
            dashTime -= Time.fixedDeltaTime;
            player.rb.linearVelocityX = 0;
            player.dashCD = 1f;
            player.rb.linearVelocityY = 0;
            player.VerticalVelocity = 0;
            if (dashTime <= 0)
            {
                player.stateMachine.ChangeState(player.jumpFallState);
            }
        }
    }
}