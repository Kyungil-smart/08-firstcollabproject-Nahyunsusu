using UnityEngine;
using UnityEngine.Events;

public class PlayerSkillHandler : MonoBehaviour
{
	public UnityEvent<SkillSlot> GroupChanged;

	private PlayerInputHandler _inputHandler;

	private SkillSlot _currentGroup = SkillSlot.Left;

	private void Awake()
	{
		_inputHandler = GetComponent<PlayerInputHandler>();
	}

	private void OnEnable()
	{
		_inputHandler.GroupChanged += Group;
	}

	private void OnDisable()
	{
		_inputHandler.GroupChanged -= Group;
	}

	public void Execute(SkillSlot slot)
	{
		Debug.Log($"{_currentGroup}.{slot} 스킬 실행");
	}

	private void Group()
	{
		_currentGroup = _currentGroup == SkillSlot.Left ? SkillSlot.Right : SkillSlot.Left;
		Debug.Log($"스킬 그룹 변경 -> {_currentGroup}");
		GroupChanged?.Invoke(_currentGroup);
	}
}