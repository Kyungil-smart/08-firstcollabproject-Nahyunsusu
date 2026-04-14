using System.Collections;
using UnityEngine;

namespace _Scripts.LYC.States
{
	public class HitState : BaseState
	{
		private YieldInstruction _yield;

		public HitState(PlayerFSM fsm, PlayerInputHandler input) : base(fsm, input)
		{
			_yield = new WaitForSeconds(0.2f);
		}

		public override void Enter()
		{
			Input.SetInputEnabled(false);
			StartCoroutine(HitStunRoutine());
		}

		private IEnumerator HitStunRoutine()
		{
			FSM.Controller.hit.Invoke();
			FSM.Controller.SetInvincible(true);
			yield return _yield;

			FSM.ChangeState(FSM.Idle);
		}

		public override void Exit()
		{
			Input.SetInputEnabled(true);
			FSM.Controller.SetInvincible(false);
		}

		public override void OnHit()
		{
		}
	}
}