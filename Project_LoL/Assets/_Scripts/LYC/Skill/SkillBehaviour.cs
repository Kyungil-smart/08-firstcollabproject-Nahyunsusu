using System;
using UnityEngine;

public class SkillBehaviour : MonoBehaviour
{
	[Header("Skill")] [SerializeField]
	private SkillDataSO skillDataSO;

	[field: SerializeField]
	public SkillData CurrentSkillData { get; private set; }

	[field: SerializeField]
	public int LastDiceResult { get; private set; }

	private void Start()
	{
		Init(); // Debug
	}

	public void Init(SkillDataSO skillInjection = null)
	{
		if (skillInjection != null)
			skillDataSO = skillInjection;

		if (skillDataSO == null)
		{
			Debug.LogError($"[SkillBehaviour] {name}: SkillDataSO가 없습니다.");
			return;
		}

		InitSKill();
	}

	public bool TryExecute()
	{
		// if (!cooldown)
		skillDataSO.Use(this, CurrentSkillData);

		return true;
	}

	private void InitSKill()
	{
		LastDiceResult = Roll();
		CurrentSkillData = skillDataSO.Get(LastDiceResult);

		Debug.Log($"[SkillBehaviour] {skillDataSO.name} — 주사위: {LastDiceResult}");
	}

	public static int Roll(int min = 1, int max = 6)
		=> UnityEngine.Random.Range(min, max + 1);
}