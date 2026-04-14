using _Scripts.LYC.Skill;
using DG.Tweening;
using UnityEngine;

public class SkillUIController : MonoBehaviour
{
	public PlayerSkillHandler playerSkillHandler;

	public RectTransform mouseRight;
	public RectTransform mouseLeft;

	public RectTransform mousePivotLL;
	public RectTransform mousePivotLR;
	public RectTransform mousePivotRL;
	public RectTransform mousePivotRR;

	public UnityEngine.UI.Image dimLeft;
	public UnityEngine.UI.Image dimRight;
	public SkillUISlot[] slots;
	public Sprite[] dices;

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

		OnSkillSetChanged(playerSkillHandler.CurrentSkillSlot);
	}

	private void OnSkillReloadStarted(int slot)
	{
		slots[slot].StartRolling();
		slots[slot].UpdateAmmo(0);
		slots[slot].StartCoolDown(playerSkillHandler.Skills[slot].CurrentSkillData.Cooldown);
	}

	private void OnSkillReloadFinished(int slot)
	{
		var data = playerSkillHandler.Skills[slot].CurrentSkillData;
		Sprite icon = data?.SkillImage;
		Sprite dice = dices[data?.CurrentDice ?? 0];
		int ammo = data?.CurrentAmmo ?? 0;

		slots[slot].StopRolling();
		slots[slot].UpdateData(icon, dice, ammo);
	}

	public void OnSkillSetChanged(SkillSlot direction)
	{
		(direction == SkillSlot.Left ? dimLeft : dimRight).DOColor(new Color(0, 0, 0, 0), 0.4f);
		(direction == SkillSlot.Left ? dimRight : dimLeft).DOColor(new Color(0, 0, 0, 0.7f), 0.4f);
		mouseLeft.DOMove((direction == SkillSlot.Left ? mousePivotLL : mousePivotRL).position, 0.4f);
		mouseRight.DOMove((direction == SkillSlot.Left ? mousePivotLR : mousePivotRR).position, 0.4f);
	}

	public void OnSkillChanged(int slot)
	{
		var data = playerSkillHandler.Skills[slot].CurrentSkillData;
		Sprite icon = data?.SkillImage;
		Sprite dice = dices[data?.CurrentDice ?? 0];
		int ammo = data?.CurrentAmmo ?? 0;

		slots[slot].UpdateData(icon, dice, ammo);
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