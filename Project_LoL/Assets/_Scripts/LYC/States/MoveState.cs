using UnityEngine;

namespace _Scripts.LYC.States
{
	public class MoveState : BaseState
	{
		private readonly Rigidbody2D _rb;
		private readonly float _speed;

		public MoveState(PlayerFSM fsm, PlayerInputHandler input, Rigidbody2D rb, float speed)
			: base(fsm, input)
		{
			_rb = rb;
			_speed = speed;
		}

		public override void Enter()
		{
		}

		public override void Update()
		{
			if (FSM.MoveInput == Vector2.zero)
			{
				FSM.ChangeState(FSM.Idle);
				return;
			}

			var velocity = FSM.MoveInput.normalized;
			_rb.linearVelocity = velocity * _speed;
		}

		public override void Exit()
		{
			_rb.linearVelocity = Vector2.zero;
		}

		public override void OnDashed() => FSM.ChangeState(FSM.Dash);
		public override void OnLeftSkill() => FSM.EnterAttack(SkillSlot.Left);
		public override void OnRightSkill() => FSM.EnterAttack(SkillSlot.Right);
	}
}