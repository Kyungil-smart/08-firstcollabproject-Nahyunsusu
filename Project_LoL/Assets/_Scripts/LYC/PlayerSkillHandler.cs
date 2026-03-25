using UnityEngine;
using UnityEngine.Events;

public class PlayerSkillHandler : MonoBehaviour
{
	[SerializeField] private SkillDataSO sampleSkillDataSO;
	[SerializeField] private SkillExecutor skill1;
	[SerializeField] private SkillExecutor skill2;
	[SerializeField] private SkillExecutor skill3;
	[SerializeField] private SkillExecutor skill4;

	public UnityEvent<SkillSlot> SkillSetChanged;

	private PlayerController _controller;
	private PlayerInputHandler _inputHandler;
	private SkillSlot _currentSkillSet = SkillSlot.Left;

	private void Awake()
	{
		_inputHandler = GetComponent<PlayerInputHandler>();
		_controller = GetComponent<PlayerController>();

		skill1.Init(_controller, sampleSkillDataSO);
		skill2.Init(_controller, sampleSkillDataSO);
		skill3.Init(_controller, sampleSkillDataSO);
		skill4.Init(_controller, sampleSkillDataSO);
	}

	private void OnEnable()
	{
		_inputHandler.SkillSetChanged += ChangeSkillSet;
	}

	private void OnDisable()
	{
		_inputHandler.SkillSetChanged -= ChangeSkillSet;
	}

	public void Execute(SkillSlot slot)
	{
		GetSkill(slot).TryExecute();
	}

	private void ChangeSkillSet()
	{
		_currentSkillSet = _currentSkillSet == SkillSlot.Left ? SkillSlot.Right : SkillSlot.Left;
		Debug.Log($"스킬셋 변경 -> {_currentSkillSet}");
		SkillSetChanged?.Invoke(_currentSkillSet);
	}

	private SkillExecutor GetSkill(SkillSlot slot)
	{
		switch (slot)
		{
			case SkillSlot.Left when _currentSkillSet == SkillSlot.Left:
				return skill1;
			case SkillSlot.Left when _currentSkillSet == SkillSlot.Left:
				return skill2;
			case SkillSlot.Left:
				return skill3;
			case SkillSlot.Right:
			default:
				return skill4;
		}
	}
}