using System;
using System.Collections.Generic;
using System.Linq;
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

	[Header("Interact")]
	[SerializeField] private Vector2 _interactSize = new Vector2(1.5f, 1.5f);

	[SerializeField] private LayerMask _interactLayer = Physics2D.AllLayers;

	[Header("Events")]
	public UnityEvent hit;

	public UnityEvent       dashed;
	public UnityEvent       died;
	public UnityEvent<bool> invincibilityChanged;

	#endregion

	public float Health
	{
		get => Data.CurrentHp;
		set
		{
			Data.SetHp(Mathf.Max(value, 0));
			HealthChanged?.Invoke();
		}
	}

	public int Level
	{
		get => Data.CurrentLevel;
		set
		{
			Data.SetLevel(value);
			LevelChanged?.Invoke();
		}
	}

	public int Gold
	{
		get => Data.CurrentGold;
		set
		{
			Data.SetGold(value);
			GoldChanged?.Invoke();
		}
	}

	public int Exp
	{
		get => Data.CurrentExp;
		set
		{
			Data.SetExp(value);
			ExpChanged?.Invoke();
		}
	}

	public int ExpReq { get; private set; }

	public PlayerData Data { get; private set; }
	public PlayerFSM  FSM  { get; private set; }

	private PlayerDataSO       _dataSO;
	private EquipmentList      _equipmentList;
	private PlayerInputHandler _inputHandler;
	private PlayerSkillHandler _skillHandler;

	/// <summary>장비 변경 시 발행. (슬롯 인덱스, 장착된 장비 — null이면 해제)</summary>
	public event Action<int, EquipmentData> EquipmentChanged;

	public event Action HealthChanged;
	public event Action ExpChanged;
	public event Action LevelChanged;
	public event Action StatsChanged;
	public event Action GoldChanged;

	/// <summary>PlayerDataSO 기반 초기 순수 스탯. 레벨업·장비 보너스 미적용. 씬 전환 후에도 불변.</summary>
	private PlayerData _initialBaseData;

	/// <summary>레벨업 선택지로 누적된 퍼센트 보너스.</summary>
	private StatBonusPercent _levelUpBonusPercent = new StatBonusPercent();

	/// <summary>레벨업 + 장비 보너스를 합산한 최종 퍼센트 보너스 (UI 표시용).</summary>
	public StatBonusPercent TotalBonusPercent { get; private set; } = new StatBonusPercent();

	private void Awake()
	{
		FSM            = GetComponent<PlayerFSM>();
		_equipmentList = GetComponent<EquipmentList>();
		_inputHandler  = GetComponent<PlayerInputHandler>();
		_skillHandler  = GetComponent<PlayerSkillHandler>();

		if (_equipmentList != null)
			_equipmentList.OnEquipChanged += OnEquipmentListChanged;
	}

	private void Start()
	{
		LevelUpManager.Instance?.RegisterPlayer(this);
		_inputHandler.Interacted += Interact;

		if (AutoInit)
			InitPlayer();
	}

	private void OnDestroy()
	{
		LevelUpManager.Instance?.UnregisterPlayer(this);
		if (_inputHandler != null) _inputHandler.Interacted -= Interact;

		if (_equipmentList != null)
			_equipmentList.OnEquipChanged -= OnEquipmentListChanged;

		// 씬 전환 시 현재 스탯과 장비를 저장
		if (Data != null)
			PlayerPersistentData.Instance?.Save(_initialBaseData, Data, _levelUpBonusPercent,
				_equipmentList?.MyEquips, _skillHandler.Skills.Select(s => s.SkillDataSO).ToList());
	}

	public void InitPlayer(PlayerDataSO dataSO = null)
	{
		var persistent = PlayerPersistentData.Instance;

		if (persistent != null && persistent.HasData)
		{
			// 이전 씬에서 저장된 데이터로 복원
			_initialBaseData     = persistent.SavedBaseData;
			_levelUpBonusPercent = persistent.SavedLevelUpBonusPercent?.Clone() ?? new StatBonusPercent();
			Data                 = persistent.SavedRuntimeData;
			var skillSaved = persistent.SavedSkills;

			if (skillSaved != null)
			{
				for (int i = 0; i < skillSaved.Count; i++)
				{
					if (i > 3) break;

					_skillHandler.SetSkill(skillSaved[i], i);
				}
			}

			AddExperience(0);

			// UI 이벤트 발행 및 장비 보정 재계산
			HealthChanged?.Invoke();
			LevelChanged?.Invoke();
			GoldChanged?.Invoke();
			ExpChanged?.Invoke();
			RecalculateStats();
		}
		else
		{
			// 최초 게임 시작: 기본값으로 초기화
			var so = dataSO ?? PlayerDataSOSample;
			_initialBaseData     = so?.Get();
			_levelUpBonusPercent = new StatBonusPercent();
			Data                 = so?.Get();

			if (Data != null)
			{
				Health = Data.HP;
				Level  = 1;
				Gold   = 10000000;
				Exp    = 0;
			}
			else
			{
				Debug.LogWarning($"{nameof(PlayerDataSO)} is null");
			}
		}

		FSM.Init();
	}

	private readonly List<RaycastHit2D> _interactHits = new List<RaycastHit2D>();

	public void Interact()
	{
		var filter = new ContactFilter2D { layerMask = _interactLayer, useLayerMask = true };
		int count  = Physics2D.BoxCast(transform.position, _interactSize, 0f, Vector2.zero, filter, _interactHits, 0f);
		for (int i = 0; i < count; i++)
		{
			if (_interactHits[i].collider.TryGetComponent<IInteract>(out var interactable))
			{
				interactable.OnInteracted();
			}
		}
	}

	#region Equipment

	/// <summary>
	/// EquipmentList.OnEquipChanged 구독 핸들러.
	/// 슬롯별 EquipmentChanged 이벤트를 발행하고 스탯을 재계산합니다.
	/// </summary>
	private void OnEquipmentListChanged(bool temp = false)
	{
		var equips = _equipmentList.MyEquips;
		for (int i = 0; i < 4; i++)
		{
			var eq = i < equips.Count ? equips[i] : null;
			EquipmentChanged?.Invoke(i, eq);
		}

		RecalculateStats();
	}

	/// <summary>
	/// 레벨업 선택지의 수치를 퍼센트 보너스로 누적하고 스탯을 재계산합니다.
	/// LevelUpManager에서 호출합니다.
	/// </summary>
	public void AddBaseStat(LevelUpChoiceEntry entry)
	{
		_levelUpBonusPercent.HP         += entry.hp;
		_levelUpBonusPercent.AtkDamage  += entry.atkDamage;
		_levelUpBonusPercent.AtkSpeed   += entry.atkSpeed;
		_levelUpBonusPercent.MoveSpeed  += entry.moveSpeed;
		_levelUpBonusPercent.CritRate   += entry.critChance;
		_levelUpBonusPercent.CritDamage += entry.critDamage;
		RecalculateStats();
	}

	/// <summary>
	/// 레벨업·장비 퍼센트 보너스를 초기 기본 스탯에 적용해 Data를 갱신합니다.
	/// 계산식: Data.X = InitialBase.X × (1 + totalPercent / 100)
	/// </summary>
	private void RecalculateStats()
	{
		float equipHpPct = 0, equipAtkPct = 0, equipAtkSpdPct = 0,
		      equipMovSpdPct = 0, equipCritRatePct = 0, equipCritDmgPct = 0;

		if (_equipmentList != null)
		{
			var total = _equipmentList.CalculateData();
			equipHpPct       = total.HP;
			equipAtkPct      = total.AttackDamage;
			equipAtkSpdPct   = total.AttackSpeed;
			equipMovSpdPct   = total.MoveSpeed;
			equipCritRatePct = total.CritChance;
			equipCritDmgPct  = total.CritDamage;
		}

		TotalBonusPercent = new StatBonusPercent
		{
			HP         = _levelUpBonusPercent.HP         + equipHpPct,
			AtkDamage  = _levelUpBonusPercent.AtkDamage  + equipAtkPct,
			AtkSpeed   = _levelUpBonusPercent.AtkSpeed   + equipAtkSpdPct,
			MoveSpeed  = _levelUpBonusPercent.MoveSpeed  + equipMovSpdPct,
			CritRate   = _levelUpBonusPercent.CritRate   + equipCritRatePct,
			CritDamage = _levelUpBonusPercent.CritDamage + equipCritDmgPct,
		};

		Data.HP         = _initialBaseData.HP         * (1f + TotalBonusPercent.HP         / 100f);
		Data.AtkDamage  = Mathf.RoundToInt(_initialBaseData.AtkDamage  * (1f + TotalBonusPercent.AtkDamage  / 100f));
		Data.AtkSpeed   = Mathf.RoundToInt(_initialBaseData.AtkSpeed   * (1f + TotalBonusPercent.AtkSpeed   / 100f));
		Data.MoveSpeed  = _initialBaseData.MoveSpeed  * (1f + TotalBonusPercent.MoveSpeed  / 100f);
		Data.CritRate   = _initialBaseData.CritRate   * (1f + TotalBonusPercent.CritRate   / 100f);
		Data.CritDamage = Mathf.RoundToInt(_initialBaseData.CritDamage * (1f + TotalBonusPercent.CritDamage / 100f));
		StatsChanged?.Invoke();
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
		const int   firstBaseExp   = 50;
		const int   secondBaseExp  = 50;
		const int   thirdBaseExp   = 50;
		const float firstExponent  = 1;
		const float secondExponent = 1.05f;
		const float thirdExponent  = 1.1f;
		const int   expDefault     = 0;
		const int   firstBasis     = 10;
		const int   secondBasis    = 20;

		ExpReq = (int)(Level switch
		{
			<= firstBasis  => firstBaseExp * Mathf.Pow(Level, firstExponent),
			<= secondBasis => secondBaseExp * Mathf.Pow(Level, secondExponent),
			_              => thirdBaseExp * Mathf.Pow(Level, thirdExponent)
		});

		Exp += exp;
		if (Exp >= ExpReq)
		{
			Level++;
			Exp -= ExpReq;

			ExpReq = (int)(Level switch
			{
				<= firstBasis  => firstBaseExp * Mathf.Pow(Level, firstExponent),
				<= secondBasis => secondBaseExp * Mathf.Pow(Level, secondExponent),
				_              => thirdBaseExp * Mathf.Pow(Level, thirdExponent)
			});

			LevelUp?.Invoke();
			RecalculateStats();
		}
	}

	public void KnockBack()
	{
		var results = new List<Collider2D>();
		Physics2D.OverlapCircle(transform.position, 2f, ContactFilter2D.noFilter, results);
		foreach (Collider2D col in results)
		{
			if (col.gameObject == gameObject) continue;
			if (col.TryGetComponent<Damageable>(out _))
			{
				Vector2     dir = (col.transform.position - transform.position).normalized;
				Rigidbody2D rb  = col.GetComponent<Rigidbody2D>();
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