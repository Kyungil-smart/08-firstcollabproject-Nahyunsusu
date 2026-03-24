using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

// 전투 시스템이나 이벤트 또는 플레이어 스탯 관련 기능 등 구현
public class PlayerController : MonoBehaviour
{
	public bool showPlayerDebugMenu;

	[field: Header("Player")]
	[field: SerializeField]
	public PlayerDataSO PlayerData { get; private set; }

	[field: SerializeField]
	public int Health { get; private set; }

	[field: SerializeField]
	public int Experience { get; private set; }

	[field: SerializeField]
	public bool IsInvincible { get; private set; }

	[Header("Events")] public UnityEvent hit;
	public UnityEvent dashed;
	public UnityEvent died;
	public UnityEvent<bool> invincibilityChanged;

	private PlayerFSM _fsm;

	private void Awake()
	{
		_fsm = GetComponent<PlayerFSM>();

		Init();
	}

	[ContextMenu("Init")]
	public void Init(PlayerDataSO data = null)
	{
		// === Player Data ===
		if (data != null) PlayerData = data;
		if (PlayerData != null)
		{
			Health = PlayerData.MaxHp;
			// ...
		}
		else
		{
			Debug.LogWarning($"{nameof(PlayerDataSO)} is null");
		}
	}

	public void SetInvincible(bool enable)
	{
		IsInvincible = enable;
		invincibilityChanged.Invoke(enable);
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

	private void OnGUI()
	{
		if (!showPlayerDebugMenu) return;

		if (GUILayout.Button("Reset Health", GUILayout.Height(70), GUILayout.Width(100)))
		{
			Health = 100;
		}

		if (GUILayout.Button("OnHit", GUILayout.Height(70), GUILayout.Width(100)))
		{
			OnHit(Random.Range(5, 15));
		}
	}

#if UNITY_EDITOR
	[ContextMenu("Debug: OnHit")]
	private void OnHit()
	{
		OnHit(1);
	}
#endif
}