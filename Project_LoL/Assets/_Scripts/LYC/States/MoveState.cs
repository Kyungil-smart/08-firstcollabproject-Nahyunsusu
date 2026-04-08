using _Scripts.LYC.Skill;
using UnityEngine;

namespace _Scripts.LYC.States
{
	public class MoveState : BaseState
	{
		private readonly Rigidbody2D _rb;

		private float _speed;

		public MoveState(PlayerFSM fsm, PlayerInputHandler input, Rigidbody2D rb)
			: base(fsm, input)
		{
			_rb = rb;
		}

		public override void Enter()
		{
			_speed = FSM.Controller.Data.MoveSpeed;
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

		public override void OnDashed()     => FSM.ChangeState(FSM.Dash);
		public override void OnLeftSkill()  => FSM.EnterAttack(SkillSlot.Left);
		public override void OnRightSkill() => FSM.EnterAttack(SkillSlot.Right);
	}
}