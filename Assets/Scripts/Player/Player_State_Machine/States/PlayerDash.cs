using UnityEngine;

namespace Assets.Scripts.Player.Player_State_Machine.States
{
    public class PlayerDash : PlayerState
    {
        public float dashTime;
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
            player._isPastApexThreshold = true;
            player.dashDir = player.isFacingRight ? 1 : -1;
            player.rb.linearVelocityX = player.DASHSPEED * player.dashDir;
            dashTime = player.DASHTIME;
            player.dashCD = 1f;
        }

        public override void ExitState()
        {
            base.ExitState();
        }

        public override void FrameUpdate()
        {
            base.FrameUpdate();
            dashTime -= Time.deltaTime;
            if (dashTime <= 0)
            {
                player.stateMachine.ChangeState(player.jumpFallState);
            }
        }

        public override void PhysicsUpdate()
        {
            base.PhysicsUpdate();
        }
    }
}