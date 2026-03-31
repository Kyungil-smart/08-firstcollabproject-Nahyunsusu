using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

// 전투 시스템이나 이벤트 또는 플레이어 스탯 관련 기능 등 구현
public class PlayerController : MonoBehaviour
{
	#region Serializable Fields

	[Header("Debug")] public bool enableDebugMenu;

	[field: Header("Player")]
	[field: SerializeField]
	public PlayerDataSO PlayerDataSOSample { get; private set; }

	[field: SerializeField]
	public bool AutoInit { get; private set; } = true;

	[field: SerializeField]
	public int Health { get; private set; }

	[field: SerializeField]
	public int Level { get; private set; }

	[field: SerializeField]
	public int Gold { get; private set; }

	[field: SerializeField]
	public int Exp { get; private set; }

	[field: SerializeField]
	public bool IsInvincible { get; private set; }

	[Header("Events")] public UnityEvent hit;
	public UnityEvent dashed;
	public UnityEvent died;
	public UnityEvent<bool> invincibilityChanged;

	#endregion

	public PlayerData Data { get; private set; }
	public PlayerFSM FSM { get; private set; }

	private PlayerDataSO _dataSO;

	private void Awake()
	{
		FSM = GetComponent<PlayerFSM>();
	}

	private void Start()
	{
		if (AutoInit)
			InitPlayer();
	}

	public void InitPlayer(PlayerDataSO dataSO = null)
	{
		// === Player Data ===
		Data = dataSO == null ? PlayerDataSOSample?.Get() : dataSO.Get();

		if (Data != null)
		{
			Health = Data.HP;
		}
		else
		{
			Debug.LogWarning($"{nameof(PlayerDataSO)} is null");
		}

		FSM.Init();
	}

	public void SetInvincible(bool enable)
	{
		IsInvincible = enable;
		invincibilityChanged.Invoke(enable);
	}

	public void TakeDamage(int damageAmount)
	{
		if (IsInvincible) return;

		Health -= damageAmount;
		if (Health <= 0)
		{
			FSM.ChangeState(FSM.Die);
			return;
		}

		FSM.ChangeState(FSM.Hit);
	}

#if UNITY_EDITOR
	private void OnGUI()
	{
		if (!enableDebugMenu) return;

		if (GUILayout.Button("Reset Stat as sample", GUILayout.Height(70), GUILayout.Width(100)))
		{
			InitPlayer();
		}

		if (GUILayout.Button("OnHit", GUILayout.Height(70), GUILayout.Width(100)))
		{
			TakeDamage(Random.Range(5, 15));
		}
	}

	[ContextMenu("Debug: OnHit")]
	private void TakeDamage()
	{
		TakeDamage(1);
	}
#endif
}