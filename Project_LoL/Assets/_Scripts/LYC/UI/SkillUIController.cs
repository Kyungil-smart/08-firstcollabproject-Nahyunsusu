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
		playerSkillHandler.SkillReloadStarted.AddListener(OnSkillReloadStarted);
		playerSkillHandler.SkillReloadFinished.AddListener(OnSkillReloadFinished);

		for (int i = 0; i < slots.Length; i++)
		{
			OnSkillChanged(i);
		}
	}

	private void OnSkillReloadStarted(int slot)
	{
		slots[slot].UpdateAmmo(0);
		slots[slot].StartCoolDown(playerSkillHandler.Skills[slot].CurrentSkillData.Cooldown);
	}

	private void OnSkillReloadFinished(int slot)
	{
		slots[slot].UpdateData(playerSkillHandler.Skills[slot].CurrentSkillData);
		slots[slot].UpdateAmmo(playerSkillHandler.Skills[slot].CurrentSkillData.CurrentAmmo);
	}

	public void OnSkillSetChanged(SkillSlot direction)
	{
	}

	public void OnSkillChanged(int slot)
	{
		slots[slot].UpdateData(playerSkillHandler.Skills[slot].CurrentSkillData);
	}

	public void OnSkillExecuted(int slot)
	{
		slots[slot].UpdateAmmo(playerSkillHandler.Skills[slot].CurrentSkillData.CurrentAmmo);
		slots[slot].StartCoolDown(playerSkillHandler.Skills[slot].CurrentSkillData.Delay);
	}

	public void OnSkillExecutionFailed(int slot, SkillExecuteResult result)
	{
	}
}