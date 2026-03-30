using _Scripts.LYC.Skill;
using UnityEngine;
using UnityEngine.Events;

public class PlayerSkillHandler : MonoBehaviour
{
	[Header("Current skill information")]
	[field: SerializeField]
	public SkillSlot CurrentSkillSlot { get; private set; }

	[field: SerializeField]
	public SkillExecutor[] Skills { get; private set; }

	[Header("Skill Events")]
	[field: SerializeField]
	public UnityEvent<SkillSlot> SkillSetChanged { get; private set; }

	[field: SerializeField]
	public UnityEvent<int> SkillChanged { get; private set; }

	[field: SerializeField]
	public UnityEvent<int> SkillExecuted { get; private set; }

	[field: SerializeField]
	public UnityEvent<int, SkillExecuteResult> SkillExecutionFailed { get; private set; }

	private PlayerController _controller;
	private PlayerInputHandler _inputHandler;

	private void Awake()
	{
		_inputHandler = GetComponent<PlayerInputHandler>();
		_controller = GetComponent<PlayerController>();

		Skills = new SkillExecutor[4];
		for (var i = 0; i < Skills.Length; i++)
			Skills[i] = new SkillExecutor(_controller);
	}

	private void OnEnable()
	{
		_inputHandler.SkillSetChanged += ChangeSkillSet;
	}

	private void OnDisable()
	{
		_inputHandler.SkillSetChanged -= ChangeSkillSet;
	}

	public void SetSkill(SkillDataSO skillData, int slotIndex, int dice = 0)
	{
		if (slotIndex >= Skills.Length)
		{
			Debug.LogError($"{nameof(slotIndex)} {slotIndex} is invalid");
			return;
		}

		if (dice != 0)
		{
			Skills[slotIndex].Set(skillData, dice);
		}
		else
		{
			Skills[slotIndex].Set(skillData);
		}

		SkillChanged.Invoke(slotIndex);
	}

	public void Execute(SkillSlot slot)
	{
		int index = ConvertSlotToIndex(slot);
		SkillExecuteResult result = Skills[index].TryExecute();

		if (result == SkillExecuteResult.Success)
			SkillExecuted.Invoke(index);
		else
			SkillExecutionFailed.Invoke(index, result);
	}

	private void ChangeSkillSet()
	{
		CurrentSkillSlot = CurrentSkillSlot == SkillSlot.Left ? SkillSlot.Right : SkillSlot.Left;
		SkillSetChanged?.Invoke(CurrentSkillSlot);
	}

	private int ConvertSlotToIndex(SkillSlot slot)
		=> (CurrentSkillSlot == SkillSlot.Left ? 0 : 2) + (int)slot;
}