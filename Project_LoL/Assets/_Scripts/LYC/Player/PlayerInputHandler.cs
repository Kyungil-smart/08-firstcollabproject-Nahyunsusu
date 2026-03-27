using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputHandler : MonoBehaviour
{
	public event Action<Vector2> Moved;
	public event Action LeftSkillPerformed;
	public event Action RightSkillPerformed;
	public event Action Dashed;
	public event Action SkillSetChanged;

	[SerializeField] private float skillRepeatInterval = 0.1f;

	private PlayerInputSystem _inputSystem;
	private InputAction _moveAction;

	private Coroutine _leftSkillCoroutine;
	private Coroutine _rightSkillCoroutine;
	private Coroutine _moveCoroutine;
	private YieldInstruction _skillRepeatYield;

	private bool _isMoving;

	private void Awake()
	{
		InitInput();
		BindActions();
	}

	private void Update()
	{
		if (_isMoving)
		{
			Moved?.Invoke(_moveAction.ReadValue<Vector2>());
		}
	}

	private void OnDestroy()
	{
		DisposeAll();
	}

	public void SetInputEnabled(bool enable)
	{
		if (enable) _inputSystem?.Enable();
		else _inputSystem?.Disable();
	}

	private void InitInput()
	{
		_skillRepeatYield = new WaitForSeconds(skillRepeatInterval);
		_inputSystem = new PlayerInputSystem();
		_inputSystem.Enable();
	}

	private void BindActions()
	{
		if (_inputSystem == null) return;

		// Skills
		_inputSystem.Player.LeftSkill.started += _ => StartSkillRepeat(ref _leftSkillCoroutine, OnLeftSkillPerformed);
		_inputSystem.Player.LeftSkill.canceled += _ => StopRepeat(ref _leftSkillCoroutine);
		_inputSystem.Player.RightSkill.started += _ => StartSkillRepeat(ref _rightSkillCoroutine, OnRightSkillPerformed);
		_inputSystem.Player.RightSkill.canceled += _ => StopRepeat(ref _rightSkillCoroutine);

		// Movement
		_moveAction = _inputSystem.Player.Move;
		_moveAction.performed += _ => _isMoving = true;
		_moveAction.canceled += _ =>
		{
			Moved?.Invoke(Vector2.zero);
			_isMoving = false;
		};

		// Dash
		_inputSystem.Player.Dash.performed += _ => Dashed?.Invoke();

		// Skill Change
		_inputSystem.Player.ChangeSkillGroup.performed += _ => SkillSetChanged?.Invoke();
	}

	private void DisposeAll()
	{
		if (_inputSystem == null) return;
		_inputSystem.Disable();
		_inputSystem.Dispose();
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

	private void OnLeftSkillPerformed() => LeftSkillPerformed?.Invoke();
	private void OnRightSkillPerformed() => RightSkillPerformed?.Invoke();
}