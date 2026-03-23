using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputHandler : MonoBehaviour
{
	public event Action<Vector2> moved;
	public event Action leftSkillPerformed;
	public event Action rightSkillPerformed;
	public event Action dashed;

	[SerializeField] private float _skillRepeatInterval = 0.1f;

	private PlayerInputSystem _inputSystem;
	private InputAction _moveAction;

	private Coroutine _leftSkillCoroutine;
	private Coroutine _rightSkillCoroutine;
	private Coroutine _moveCoroutine;
	private YieldInstruction _skillRepeatYield;

	private bool _isMoving;

	private void Awake()
	{
		_inputSystem = new PlayerInputSystem();
		_inputSystem.Enable();
		_skillRepeatYield = new WaitForSeconds(_skillRepeatInterval);

		Subscribe();
	}

	private void Update()
	{
		if (_isMoving)
		{
			moved?.Invoke(_moveAction.ReadValue<Vector2>());
		}
	}

	private void OnDestroy()
	{
		UnSubscribe();
		_inputSystem.Dispose();
	}

	void Subscribe()
	{
		if (_inputSystem == null) return;

		_inputSystem.Player.LeftSkill.started += _ => StartSkillRepeat(ref _leftSkillCoroutine, OnLeftSkillPerformed);
		_inputSystem.Player.LeftSkill.canceled += _ => StopRepeat(ref _leftSkillCoroutine);

		_inputSystem.Player.RightSkill.started += _ => StartSkillRepeat(ref _leftSkillCoroutine, OnRightSkillPerformed);
		_inputSystem.Player.RightSkill.canceled += _ => StopRepeat(ref _leftSkillCoroutine);

		_moveAction = _inputSystem.Player.Move;
		_moveAction.performed += _ => _isMoving = true;
		_moveAction.canceled += _ => _isMoving = false;
	}

	void UnSubscribe()
	{
		if (_inputSystem == null) return;

		_inputSystem.Player.LeftSkill.Dispose();
		_inputSystem.Player.RightSkill.Dispose();
		_moveAction.Dispose();
	}

	private void StartSkillRepeat(ref Coroutine c, Action skillAction)
	{
		if (c != null) StopCoroutine(c);
		c = StartCoroutine(SkillRepeatCoroutine(skillAction));
	}

	private void StopRepeat(ref Coroutine c)
	{
		if (c != null) StopCoroutine(c);
		c = null;
	}

	private IEnumerator SkillRepeatCoroutine(Action skillAction)
	{
		skillAction();
		while (true)
		{
			yield return _skillRepeatYield;
			skillAction();
		}
	}

	private void OnLeftSkillPerformed() => leftSkillPerformed?.Invoke();
	private void OnRightSkillPerformed() => rightSkillPerformed?.Invoke();
}