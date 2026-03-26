using UnityEngine;

[CreateAssetMenu(fileName = "DiceRoll", menuName = "Player/Skill/DiceRoll")]
public class DiceRollSkill : SkillDataSO
{
	[field: Header("DiceRoll")]
	[field: SerializeField]
	public DiceRollProjectile ProjectilePrefab { get; private set; }

	public override void Use(SkillExecutor executor)
	{
		var proj = Instantiate(ProjectilePrefab);
		proj.Launch(executor);

		Debug.Log($"{skillName} 발동");
	}

	protected override void SetEffect(SkillData data, int dice)
	{
		base.SetEffect(data, dice);

		data.SkillDescription = $"전방으로 ({data.Range})거리만큼 공격을 날려 적에게 ({data.Damage})만큼 데미지를 줍니다\n" + data.SkillDescription;
	}
}