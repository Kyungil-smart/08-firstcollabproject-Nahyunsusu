using _Scripts.LYC.Skill;
using UnityEngine;

public class SkillUIController : MonoBehaviour
{
	public PlayerSkillHandler playerSkillHandler;

	public SkillUISlot[] slots;

	private void Start()
	{
		if (playerSkillHandler == null)
			playerSkillHandler = FindFirstObjectByType<PlayerSkillHandler>();
		if (playerSkillHandler == null)
			enabled = false;

		playerSkillHandler.SkillChanged.AddListener(OnSkillChanged);
		playerSkillHandler.SkillExecuted.AddListener(OnSkillExecuted);
		playerSkillHandler.SkillExecutionFailed.AddListener(OnSkillExecutionFailed);
		playerSkillHandler.SkillSetChanged.AddListener(OnSkillSetChanged);

		for (int i = 0; i < slots.Length; i++)
		{
			OnSkillChanged(i);
		}
	}

	public void OnSkillSetChanged(SkillSlot direction)
	{
	}

	public void OnSkillChanged(int slot)
	{
		slots[slot].UpdateSlot(playerSkillHandler.Skills[slot].CurrentSkillData);
	}

	public void OnSkillExecuted(int slot)
	{
	}

	public void OnSkillExecutionFailed(int slot, SkillExecuteResult result)
	{
	}
}