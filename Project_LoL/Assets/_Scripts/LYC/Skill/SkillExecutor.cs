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

	public PlayerController Controller { get; private set; }

	private float _lastExecutedTime = -999;

	public void Init(PlayerController controller, SkillDataSO skillInjection = null)
	{
		Controller = controller;

		if (skillInjection != null)
			SkillDataSO = skillInjection;

		if (SkillDataSO == null)
		{
			Debug.LogError($"[{typeof(SkillExecutor)}] SkillDataSO가 없습니다.");
			return;
		}

		RollSkillDice();
	}

	private void RollSkillDice()
	{
		_lastExecutedTime = -999;
		LastDiceResult = Roll();
		CurrentSkillData = SkillDataSO.Get(LastDiceResult);
		Debug.Log($"[{typeof(SkillExecutor)}] {SkillDataSO.name} Init(주사위: {LastDiceResult})");
	}

	public SkillExecuteResult TryExecute()
	{
		if (CurrentSkillData == null) return SkillExecuteResult.NotExist;
		if (_lastExecutedTime + CurrentSkillData.Delay > Time.time) return SkillExecuteResult.OnCooldown;
		// if(_isRolling) return ...

		SkillDataSO.Use(this);
		_lastExecutedTime = Time.time;

		return SkillExecuteResult.Success;
	}

	public static int Roll(int min = 1, int max = 6)
		=> UnityEngine.Random.Range(min, max + 1);
}