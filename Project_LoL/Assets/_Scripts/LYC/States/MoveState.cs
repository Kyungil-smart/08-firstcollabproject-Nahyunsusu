using UnityEngine;

public class MoveState : BaseState
{
	private readonly Rigidbody2D rb;
	private readonly float speed;

	public MoveState(PlayerFSM fsm, PlayerInputHandler input, Rigidbody2D rb, float speed)
		: base(fsm, input)
	{
		this.rb = rb;
		this.speed = speed;
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
		rb.linearVelocity = velocity * speed;
	}

	public override void Exit()
	{
		rb.linearVelocity = Vector2.zero;
	}

	public override void OnDashed() => FSM.ChangeState(FSM.Dash);
	public override void OnLeftSkill() => FSM.EnterAttack(SkillSlot.Left);
	public override void OnRightSkill() => FSM.EnterAttack(SkillSlot.Right);
}