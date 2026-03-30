using UnityEngine;

public class EnemySkillHandler : MonoBehaviour
{
	public EnemySkillExecutor Skill { get; private set; }

	private EnemyFSM _controller;

	private void Awake()
	{
		_controller = GetComponent<EnemyFSM>();
		Skill = new EnemySkillExecutor(_controller);
	}

	public void SetSkill(EnemySkillDataSO skillData)
	{
		Skill.Set(skillData);
	}

	public void Execute()
	{
		Skill.TryExecute();
	}
}