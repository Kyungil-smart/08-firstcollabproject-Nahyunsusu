using UnityEngine;

public class DieState : BaseState
{
	public DieState(PlayerFSM fsm, PlayerInputHandler input) : base(fsm, input)
	{
	}

	public override void Enter()
	{
		FSM.Controller.died.Invoke();
		Debug.Log("플레이어 사망");
	}

	public override void OnHit()
	{
	}

	public override void OnDied()
	{
	}
}