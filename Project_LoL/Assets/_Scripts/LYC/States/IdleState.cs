using UnityEngine;

namespace _Scripts.LYC.States
{
	public class IdleState : BaseState
	{
		public IdleState(PlayerFSM fsm, PlayerInputHandler input) : base(fsm, input)
		{
		}

		public override void Update()
		{
			if (FSM.MoveInput != Vector2.zero)
			{
				FSM.ChangeState(FSM.Move);
			}
		}

		public override void OnDashed() => FSM.ChangeState(FSM.Dash);

		public override void OnLeftSkill() => FSM.EnterAttack(SkillSlot.Left);

		public override void OnRightSkill() => FSM.EnterAttack(SkillSlot.Right);
	}
}