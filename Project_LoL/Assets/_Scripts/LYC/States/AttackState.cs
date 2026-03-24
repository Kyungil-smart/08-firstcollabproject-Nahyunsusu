using System.Collections;
using UnityEngine;

namespace _Scripts.LYC.States
{
	public class AttackState : BaseState
	{
		private PlayerSkillHandler _skill;

		public AttackState(PlayerFSM fsm, PlayerInputHandler input, PlayerSkillHandler skill)
			: base(fsm, input)
		{
			_skill = skill;
		}

		public override void Enter()
		{
			StartCoroutine(ExecuteSkill());
		}

		private IEnumerator ExecuteSkill()
		{
			_skill.Execute(FSM.BufferedSkillSlot);
			yield return null;

			FSM.ChangeState(FSM.MoveInput == Vector2.zero ? FSM.Idle : FSM.Move);
		}
	}
}