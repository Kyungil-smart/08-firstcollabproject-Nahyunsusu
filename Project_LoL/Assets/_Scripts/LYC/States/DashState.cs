using System.Collections;
using UnityEngine;

namespace _Scripts.LYC.States
{
	public class DashState : BaseState
	{
		private readonly Rigidbody2D _rigidbody;

		private float   _dashDistance;
		private float   _dashTime;
		private float   _dashCooldown;
		private float   _lastDashTime;
		private Vector3 _dashStartPosition;

		public DashState(PlayerFSM fsm, PlayerInputHandler input, Rigidbody2D rigidbody)
			: base(fsm, input)
		{
			_rigidbody    = rigidbody;
			_dashDistance = 2;
			_lastDashTime = -9999;
		}

		public override void Enter()
		{
			_dashTime     = FSM.Controller.Data.DashTime;
			_dashCooldown = FSM.Controller.Data.DashCooldown;

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
			_lastDashTime             = Time.time;
			_dashStartPosition        = FSM.transform.position;
			_rigidbody.linearVelocity = dir * (_dashDistance / _dashTime);

			FSM.Controller.SetInvincible(true);
			FSM.Controller.dashed.Invoke();

			while (Vector3.Distance(_dashStartPosition, FSM.transform.position) < _dashDistance)
			{
				if (_lastDashTime + _dashTime < Time.time) break;
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