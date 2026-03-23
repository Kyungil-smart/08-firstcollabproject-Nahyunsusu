using System.Collections;
using UnityEngine;

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
		yield return null;
		_skill.Execute(FSM.BufferedSkillSlot);
		FSM.ChangeState(FSM.MoveInput == Vector2.zero ? FSM.Idle : FSM.Move);
	}
}