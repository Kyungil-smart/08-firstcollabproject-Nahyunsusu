using UnityEngine;

public enum SkillExecuteResult
{
	Success,
	NotExist,
	OnCooldown,
	Rolling
}

[System.Serializable]
public class SkillExecutor
{
	[field: SerializeField] public SkillData CurrentSkillData { get; private set; }
	[field: SerializeField] public SkillDataSO SkillDataSO { get; private set; }
	[field: SerializeField] public int LastDiceResult { get; private set; }
	[field: SerializeField] public float LastExecutedTime { get; private set; }

	public PlayerController Controller { get; private set; }

	public SkillExecutor(PlayerController controller)
	{
		Controller = controller;
		CurrentSkillData = null;
		SkillDataSO = null;

		LastExecutedTime = -1;
		LastDiceResult = -1;
	}

	public void Set(SkillDataSO newSkill = null)
	{
		SkillDataSO = newSkill;
		CurrentSkillData = null;
		LastDiceResult = 0;

		if (SkillDataSO != null)
		{
			RefreshData();
		}
	}

	private void RefreshData()
	{
		LastDiceResult = Roll(); // TODO: Wait for rolling dice
		CurrentSkillData = SkillDataSO.Get(LastDiceResult);
	}

	public SkillExecuteResult TryExecute()
	{
		if (CurrentSkillData == null) return SkillExecuteResult.NotExist;
		if (LastExecutedTime + CurrentSkillData.Delay > Time.time) return SkillExecuteResult.OnCooldown;
		// if(_isRolling) return ...

		SkillDataSO.Use(this);
		LastExecutedTime = Time.time;

		return SkillExecuteResult.Success;
	}

	public static int Roll(int min = 1, int max = 6)
		=> UnityEngine.Random.Range(min, max + 1);
}