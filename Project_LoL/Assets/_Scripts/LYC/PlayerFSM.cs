using UnityEngine;

public class PlayerFSM : MonoBehaviour
{
	[Header("Stats")] [SerializeField] private float moveSpeed = 8f;
	[SerializeField] private float dashSpeed = 20f;
	[SerializeField] private float dashDuration = 0.2f;
	[SerializeField] private float stunDuration = 0.2f;

	#region States

	public IdleState Idle { get; private set; }
	public MoveState Move { get; private set; }
	public DashState Dash { get; private set; }
	public AttackState Attack { get; private set; }
	public DieState Die { get; private set; }
	public HitState Hit { get; private set; }

	#endregion

	public PlayerController Controller { get; private set; }
	public Vector2 MoveInput { get; private set; }
	public Vector2 FacingDir { get; private set; } = Vector2.right;
	public SkillSlot BufferedSkillSlot { get; private set; }

	private PlayerInputHandler _inputHandler;
	private PlayerSkillHandler _skillHandler;
	private Rigidbody2D _rigidbody;
	private BaseState _currentState;

	private void Awake()
	{
		Controller = GetComponent<PlayerController>();
		_inputHandler = GetComponent<PlayerInputHandler>();
		_skillHandler = GetComponent<PlayerSkillHandler>();
		_rigidbody = GetComponent<Rigidbody2D>();

		Idle = new IdleState(this, _inputHandler);
		Move = new MoveState(this, _inputHandler, _rigidbody, moveSpeed);
		Dash = new DashState(this, _inputHandler, _rigidbody, dashSpeed, dashDuration);
		Attack = new AttackState(this, _inputHandler, _skillHandler);
		Die = new DieState(this, _inputHandler);
		Hit = new HitState(this, _inputHandler, stunDuration);
	}

	private void OnEnable()
	{
		BindInputCallbacks();
		ChangeState(Idle);
	}

	private void Update()
	{
		_currentState?.Update();
	}

	private void OnDisable()
	{
		UnbindInputCallbacks();
	}

	private void BindInputCallbacks()
	{
		_inputHandler.Moved += OnMoved;
		_inputHandler.Dashed += OnDash;
		_inputHandler.LeftSkillPerformed += OnLeftSkill;
		_inputHandler.RightSkillPerformed += OnRightSkill;
	}

	private void UnbindInputCallbacks()
	{
		_inputHandler.Moved -= OnMoved;
		_inputHandler.Dashed -= OnDash;
		_inputHandler.LeftSkillPerformed -= OnLeftSkill;
		_inputHandler.RightSkillPerformed -= OnRightSkill;
	}

	#region Input Callbacks

	private void OnMoved(Vector2 dir)
	{
		MoveInput = dir;
		if (dir != Vector2.zero) FacingDir = dir.normalized;
	}

	private void OnDash() => _currentState?.OnDashed();
	private void OnLeftSkill() => _currentState?.OnLeftSkill();
	private void OnRightSkill() => _currentState?.OnRightSkill();

	#endregion

	public void ChangeState(BaseState next)
	{
		if (_currentState == next) return;
		_currentState?.Exit();
		_currentState = next;
		_currentState.Enter();
	}

	public void EnterAttack(SkillSlot skillSlot)
	{
		BufferedSkillSlot = skillSlot;
		ChangeState(Attack);
	}
}