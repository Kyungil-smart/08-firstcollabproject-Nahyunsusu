using UnityEngine;
using UnityEngine.Events;

public class PlayerSkillHandler : MonoBehaviour
{
	public UnityEvent<SkillSlot> SkillSetChanged;

	private PlayerInputHandler _inputHandler;

	private SkillSlot _currentSkillSet = SkillSlot.Left;

	private void Awake()
	{
		_inputHandler = GetComponent<PlayerInputHandler>();
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
		Debug.Log($"{_currentSkillSet}의 {slot} 스킬 실행");
	}

	private void ChangeSkillSet()
	{
		_currentSkillSet = _currentSkillSet == SkillSlot.Left ? SkillSlot.Right : SkillSlot.Left;
		Debug.Log($"스킬셋 변경 -> {_currentSkillSet}");
		SkillSetChanged?.Invoke(_currentSkillSet);
	}
}