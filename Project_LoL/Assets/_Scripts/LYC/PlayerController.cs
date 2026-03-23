using UnityEngine;
using UnityEngine.Events;

// 전투 시스템이나 이벤트 또는 플레이어 스탯 관련 기능 등 구현
public class PlayerController : MonoBehaviour
{
	[Header("Stats")] [field: SerializeField] public int Health { get; private set; } = 10;
	[field: SerializeField] public int Experience { get; private set; } = 10;

	[Header("Events")] [Tooltip("대응하는 State에서 Invoke 호출")] public UnityEvent hit;
	public UnityEvent dashed;
	public UnityEvent died;
	public UnityEvent<bool> invincibilityChanged;

	public bool IsInvincible
	{
		get => _isInvincible;
		set
		{
			_isInvincible = value;
			invincibilityChanged.Invoke(_isInvincible);
		}
	}

	private bool _isInvincible;

	private PlayerFSM _fsm;

	private void Awake()
	{
		_fsm = GetComponent<PlayerFSM>();
	}

	public void OnHit(int damageAmount)
	{
		if (IsInvincible) return;

		Health -= damageAmount;
		if (Health <= 0)
		{
			_fsm.ChangeState(_fsm.Die);
			return;
		}

		_fsm.ChangeState(_fsm.Hit);
	}

#if UNITY_EDITOR
	[ContextMenu("Debug: OnHit")]
	private void OnHit()
	{
		OnHit(1);
	}
#endif
}