using UnityEngine;
using System.Collections;

public class DashState : BaseState
{
	private readonly Rigidbody2D _rigidbody;
	private readonly float _dashSpeed;
	private readonly float _dashDuration;

	public DashState(PlayerFSM fsm, PlayerInputHandler input,
		Rigidbody2D rigidbody, float dashSpeed, float dashDuration)
		: base(fsm, input)
	{
		_rigidbody = rigidbody;
		_dashSpeed = dashSpeed;
		_dashDuration = dashDuration;
	}

	public override void Enter()
	{
		Vector2 dir = FSM.MoveInput != Vector2.zero
			? FSM.MoveInput.normalized
			: FSM.FacingDir;
		Input.SetInputEnabled(false);

		StartCoroutine(DashCoroutine(dir));
	}

	private IEnumerator DashCoroutine(Vector2 dir)
	{
		FSM.Controller.SetInvincible(true);
		FSM.Controller.dashed.Invoke();

		float elapsed = 0f;
		while (elapsed < _dashDuration)
		{
			_rigidbody.linearVelocity = dir * _dashSpeed;
			elapsed += Time.deltaTime;
			yield return null;
		}

		Input.SetInputEnabled(true);
		FSM.ChangeState(FSM.MoveInput != Vector2.zero ? FSM.Move : FSM.Idle);
	}

	public override void Exit()
	{
		FSM.Controller.SetInvincible(false);
		StopCoroutine();
		_rigidbody.linearVelocity = Vector2.zero;
	}

	public override void OnHit()
	{
	}
}