using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 씬 전환 사이에 플레이어 스탯과 장비를 유지하는 싱글톤입니다.
/// 첫 씬에 배치하면 DontDestroyOnLoad로 게임 종료 전까지 유지됩니다.
/// 새 게임 시작 시 Clear()를 호출하면 저장 데이터가 초기화됩니다.
/// </summary>
public class PlayerPersistentData : MonoBehaviour
{
	public static PlayerPersistentData Instance { get; private set; }

	public bool HasData { get; private set; }

	/// <summary>레벨업 보너스 포함 순수 스탯 (장비 미적용)</summary>
	public PlayerData SavedBaseData { get; private set; }

	/// <summary>현재 HP/레벨/경험치/골드 및 장비 보정 후 스탯</summary>
	public PlayerData SavedRuntimeData { get; private set; }

	/// <summary>플레이어 스킬 데이터 저장</summary>
	public List<SkillDataSO> SavedSkills { get; private set; } = new();

	/// <summary>장착 중인 장비 목록</summary>
	public List<EquipmentData> SavedEquipments { get; private set; } = new();

	private void Awake()
	{
		if (Instance != null && Instance != this)
		{
			Destroy(gameObject);
			return;
		}

		Instance = this;
		DontDestroyOnLoad(gameObject);
	}

	/// <summary>씬 전환 전 PlayerController와 EquipmentList가 호출</summary>
	public void Save(PlayerData baseData, PlayerData runtimeData, List<EquipmentData> equips, List<SkillDataSO> skills)
	{
		Debug.Log("데이터 저장 실행");
		SavedBaseData    = baseData?.Clone();
		SavedRuntimeData = runtimeData?.Clone();
		SavedEquipments  = equips != null ? new List<EquipmentData>(equips) : new List<EquipmentData>();
		SavedSkills      = skills != null ? new List<SkillDataSO>(skills) : new List<SkillDataSO>();
		HasData          = true;
	}

	/// <summary>새 게임을 시작할 때 호출</summary>
	public void Clear()
	{
		HasData          = false;
		SavedBaseData    = null;
		SavedRuntimeData = null;
		SavedEquipments  = new List<EquipmentData>();
		SavedSkills      = new List<SkillDataSO>();
	}
}