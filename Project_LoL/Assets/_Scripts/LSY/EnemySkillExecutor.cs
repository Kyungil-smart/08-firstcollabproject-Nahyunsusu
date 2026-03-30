using UnityEngine;

[System.Serializable]
public class EnemySkillExecutor
{
	[field: SerializeField] public SkillData CurrentSkillData { get; private set; }
	[field: SerializeField] public EnemySkillDataSO SkillDataSO { get; private set; }
	[field: SerializeField] public float LastExecutedTime { get; private set; }

	public EnemyFSM Controller { get; private set; }

	public EnemySkillExecutor(EnemyFSM controller)
	{
		Controller = controller;
		CurrentSkillData = null;
		SkillDataSO = null;

		LastExecutedTime = -1;
	}

	public void Set(EnemySkillDataSO newSkill = null)
	{
		SkillDataSO = newSkill;
		CurrentSkillData = null;

		if (SkillDataSO != null)
		{
			CurrentSkillData = SkillDataSO.Get(1);
		}
	}

	public bool TryExecute()
	{
		if (CurrentSkillData == null) return false;
		if (LastExecutedTime + CurrentSkillData.Delay > Time.time) return false;

		SkillDataSO.Use(this);
		LastExecutedTime = Time.time;

		return true;
	}
}