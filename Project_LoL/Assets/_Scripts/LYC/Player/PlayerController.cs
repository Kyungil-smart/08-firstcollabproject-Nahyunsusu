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

	private readonly EquipmentData[] _equippedItems = new EquipmentData[4];

	/// <summary>장비 변경 시 발행. (슬롯 인덱스, 장착된 장비 — null이면 해제)</summary>
	public event Action<int, EquipmentData> EquipmentChanged;

	/// <summary>장비 % 보정 이전의 순수 스탯. 레벨업 보너스까지 포함.</summary>
	private PlayerData _baseData;

	private void Awake()
	{
		FSM = GetComponent<PlayerFSM>();
	}

	private void Start()
	{
		LevelUpManager.Instance?.RegisterPlayer(this);

		if (AutoInit)
			InitPlayer();
	}

	private void OnDestroy()
	{
		LevelUpManager.Instance?.UnregisterPlayer(this);
	}

	public void InitPlayer(PlayerDataSO dataSO = null)
	{
		var so = dataSO ?? PlayerDataSOSample;
		_baseData = so?.Get();
		Data      = so?.Get();

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

	#region Equipment

	/// <summary>
	/// 지정 슬롯(0~3)에 장비를 장착합니다.
	/// 슬롯에 이미 장비가 있으면 교체됩니다.
	/// </summary>
	public void Equip(int slot, EquipmentData equipment)
	{
		if (slot < 0 || slot >= _equippedItems.Length) return;

		_equippedItems[slot] = equipment;
		RecalculateStats();
		EquipmentChanged?.Invoke(slot, equipment);
	}

	/// <summary>지정 슬롯의 장비를 해제합니다.</summary>
	public void Unequip(int slot) => Equip(slot, null);

	/// <summary>
	/// 레벨업 선택지 스탯을 베이스에 합산하고 장비 보정을 재적용합니다.
	/// LevelUpManager에서 호출합니다.
	/// </summary>
	public void AddBaseStat(LevelUpChoiceEntry entry)
	{
		_baseData.HP         += entry.hp;
		_baseData.AtkDamage  += entry.atkDamage;
		_baseData.AtkSpeed   += (int)entry.atkSpeed;
		_baseData.MoveSpeed  += entry.moveSpeed;
		_baseData.CritRate   += entry.critChance;
		_baseData.CritDamage += entry.critDamage;
		RecalculateStats();
	}

	/// <summary>
	/// 장착된 모든 장비의 % 합산을 베이스 스탯에 곱해 Data를 갱신합니다.
	/// </summary>
	private void RecalculateStats()
	{
		float hpPct = 0, atkPct = 0, atkSpdPct = 0, movSpdPct = 0, critRatePct = 0, critDmgPct = 0;

		foreach (var eq in _equippedItems)
		{
			if (eq == null) continue;
			hpPct       += eq.EquipHP;
			atkPct      += eq.EquipAttackDamage;
			atkSpdPct   += eq.EquipAttackSpeed;
			movSpdPct   += eq.EquipMoveSpeed;
			critRatePct += eq.EquipCritChance;
			critDmgPct  += eq.EquipCritDamage;
		}

		Data.HP         = _baseData.HP        * (1f + hpPct       / 100f);
		Data.AtkDamage  = Mathf.RoundToInt(_baseData.AtkDamage  * (1f + atkPct      / 100f));
		Data.AtkSpeed   = Mathf.RoundToInt(_baseData.AtkSpeed   * (1f + atkSpdPct   / 100f));
		Data.MoveSpeed  = _baseData.MoveSpeed  * (1f + movSpdPct   / 100f);
		Data.CritRate   = _baseData.CritRate   * (1f + critRatePct / 100f);
		Data.CritDamage = Mathf.RoundToInt(_baseData.CritDamage * (1f + critDmgPct  / 100f));
	}

	#endregion

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
			Level++;
			Exp = 50 * (Level - 1);
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

		if (GUILayout.Button("OnLevelUp", GUILayout.Height(70), GUILayout.Width(100)))
		{
			AddExperience(30);
		}
	}
#endif
}