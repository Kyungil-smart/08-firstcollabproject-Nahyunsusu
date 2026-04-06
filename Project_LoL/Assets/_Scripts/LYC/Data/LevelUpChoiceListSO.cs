using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LevelUpChoiceList", menuName = "LYC/LevelUp Choice List")]
public class LevelUpChoiceListSO : ScriptableObject
{
	[SerializeField] private List<LevelUpChoiceEntry> entries = new();

	public IReadOnlyList<LevelUpChoiceEntry> Entries => entries;

#if UNITY_EDITOR
	public List<LevelUpChoiceEntry> EntriesForEditor => entries;
#endif
}

[Serializable]
public class LevelUpChoiceEntry
{
	[SerializeField] public int id;
	[SerializeField] public string displayName;

	[SerializeField] public int hp;
	[SerializeField] public int atkDamage;
	[SerializeField] public float atkSpeed;
	[SerializeField] public int moveSpeed;
	[SerializeField] public int critChance;
	[SerializeField] public int critDamage;

	/// <summary>등장 확률 가중치 (CSV Probability 열)</summary>
	[SerializeField] public int spawnWeight;

	[SerializeField] public Sprite icon;

	/// <summary>비-0 스탯만 모아 표시용 문자열을 반환합니다.</summary>
	public string BuildDescription()
	{
		var parts = new List<string>();
		if (hp != 0) parts.Add($"체력 {hp:+#;-#;0}");
		if (atkDamage != 0) parts.Add($"공격력 {atkDamage:+#;-#;0}");
		if (atkSpeed != 0) parts.Add($"공격 속도 {atkSpeed:+#;-#;0}");
		if (moveSpeed != 0) parts.Add($"이동 속도 {moveSpeed:+#;-#;0}");
		if (critChance != 0) parts.Add($"치명타 확률 {critChance:+#;-#;0}");
		if (critDamage != 0) parts.Add($"치명타 피해 {critDamage:+#;-#;0}");
		return string.Join("\n", parts);
	}
}