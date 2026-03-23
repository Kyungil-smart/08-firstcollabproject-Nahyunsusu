using System.Collections;
using UnityEngine;

public abstract class BaseState
{
	protected PlayerFSM FSM { get; }
	protected PlayerInputHandler Input { get; }
	protected Coroutine Coroutine { get; set; }

	protected BaseState(PlayerFSM fsm, PlayerInputHandler input)
	{
		FSM = fsm;
		Input = input;
	}

	public virtual void Enter()
	{
	}

	public virtual void Update()
	{
	}

	public virtual void Exit()
	{
	}

	public virtual void OnDashed()
	{
	}

	public virtual void OnLeftSkill()
	{
	}

	public virtual void OnRightSkill()
	{
	}

	public virtual void OnDied()
	{
		FSM.ChangeState(FSM.Die);
	}

	public virtual void OnHit()
	{
		FSM.ChangeState(FSM.Hit);
	}

	protected void StartCoroutine(IEnumerator routine)
	{
		StopCoroutine();
		Coroutine = FSM.StartCoroutine(routine);
	}

	protected void StopCoroutine()
	{
		if (Coroutine == null) return;

		FSM.StopCoroutine(Coroutine);
		Coroutine = null;
	}
}