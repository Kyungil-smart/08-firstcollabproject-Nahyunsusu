using UnityEngine;

[CreateAssetMenu(fileName = "DiceRoll", menuName = "Player/Skill/DiceRoll")]
public class DiceRollSkill : SkillDataSO
{
	[field: Header("DiceRoll")]
	[field: SerializeField]
	public GameObject ProjectilePrefab { get; private set; }

	public override void Use(SkillBehaviour behaviour, SkillData skillData)
	{
		Debug.Log($"Used: Damage-{skillData.Damage}, MaxUseCount-{skillData.MaxUseCount}");
	}

	protected override void SetEffect(SkillData data, int dice)
	{
		base.SetEffect(data, dice);

		data.SkillDescription = $"전방으로 ({data.Range})거리만큼 공격을 날려 적에게 ({data.Damage})만큼 데미지를 줍니다\n" + data.SkillDescription;
	}
}