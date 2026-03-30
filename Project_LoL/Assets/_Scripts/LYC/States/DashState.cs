using System.Collections;
using UnityEngine;

namespace _Scripts.LYC.States
{
	public class DashState : BaseState
	{
		private readonly Rigidbody2D _rigidbody;
		private readonly float _dashDistance;
		private readonly float _dashTime;
		private readonly float _dashCooldown;

		private float _lastDashTime;
		private Vector3 _dashStartPosition;

		public DashState(PlayerFSM fsm, PlayerInputHandler input, Rigidbody2D rigidbody,
			float dashTime, float dashDistance, float dashCooldown)
			: base(fsm, input)
		{
			_rigidbody = rigidbody;
			_dashDistance = dashDistance;
			_dashTime = dashTime;
			_dashCooldown = dashCooldown;
			_lastDashTime = -9999;
		}

		public override void Enter()
		{
			if (Time.time < _lastDashTime + _dashCooldown)
			{
				Debug.Log($"Dash cooldown {_lastDashTime + _dashCooldown - Time.time:F2}");
				FSM.ChangeState(FSM.MoveInput != Vector2.zero ? FSM.Move : FSM.Idle);
				return;
			}

			Vector2 dir = FSM.MoveInput != Vector2.zero
				? FSM.MoveInput.normalized
				: FSM.FacingDir;
			Input.SetInputEnabled(false);

			StartCoroutine(DashCoroutine(dir));
		}

		private IEnumerator DashCoroutine(Vector2 dir)
		{
			_lastDashTime = Time.time;
			_dashStartPosition = FSM.transform.position;
			_rigidbody.linearVelocity = dir * (_dashDistance / _dashTime);

			FSM.Controller.SetInvincible(true);
			FSM.Controller.dashed.Invoke();

			while (Vector3.Distance(_dashStartPosition, FSM.transform.position) < _dashDistance)
			{
				yield return null;
			}

			FSM.ChangeState(FSM.MoveInput != Vector2.zero ? FSM.Move : FSM.Idle);
		}

		public override void Exit()
		{
			StopCoroutine();
			FSM.Controller.SetInvincible(false);
			Input.SetInputEnabled(true);

			_rigidbody.linearVelocity = Vector2.zero;
		}

		public override void OnHit()
		{
		}
	}
}