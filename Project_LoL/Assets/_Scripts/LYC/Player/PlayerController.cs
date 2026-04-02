using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

// 전투 시스템이나 이벤트 또는 플레이어 스탯 관련 기능 등 구현
public class PlayerController : MonoBehaviour, Damageable, IExperience
{
	#region Serializable Fields

	[Header("Debug")] public bool enableDebugMenu;

	[field: Header("Player")]
	[field: SerializeField]
	public PlayerDataSO PlayerDataSOSample { get; private set; }

	[field: SerializeField]
	public bool AutoInit { get; private set; } = true;

	[field: SerializeField]
	public bool IsInvincible { get; private set; }

	[Header("Events")] public UnityEvent hit;
	public UnityEvent dashed;
	public UnityEvent died;
	public UnityEvent<bool> invincibilityChanged;

	#endregion

	public float Health
	{
		get => Data.CurrentHp;
		set => Data.SetHp(value);
	}

	public int Level
	{
		get => Data.CurrentLevel;
		set => Data.SetLevel(value);
	}

	public int Gold
	{
		get => Data.CurrentGold;
		set => Data.SetGold(value);
	}

	public int Exp
	{
		get => Data.CurrentExp;
		set => Data.SetExp(value);
	}

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
		Data = dataSO == null ? PlayerDataSOSample?.Get() : dataSO.Get();

		if (Data != null)
		{
			Health = Data.HP;
			Level = 1;
			Gold = 0;
			Exp = 0;
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

	#region IExperiences

	public event Action LevelUp;

	public void AddExperience(int exp)
	{
		Exp += exp;
		if (Level >= 10) return;

		if (Exp >= 50 * Level)
		{
			Exp = 50 * Level;
			Level++;
			LevelUp?.Invoke();
		}
	}

	public void KnockBack()
	{
		var results = new List<Collider2D>();
		Physics2D.OverlapCircle(transform.position, 1.5f, ContactFilter2D.noFilter, results);
		foreach (Collider2D col in results)
		{
			if (col.gameObject == gameObject) continue;
			if (col.TryGetComponent<Damageable>(out _))
			{
				Vector2 dir = (col.transform.position - transform.position).normalized;
				Rigidbody2D rb = col.GetComponent<Rigidbody2D>();
				if (rb != null)
					rb.linearVelocity = dir * 1f;
				else
					col.transform.position += (Vector3)(dir * 1f);
			}
		}
	}

	#endregion

	#region Damageable

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

	#endregion

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