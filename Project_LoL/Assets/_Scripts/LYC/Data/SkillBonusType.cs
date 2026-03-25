public enum SkillBonusType
{
	Damage,
	DamageRangeX,
	DamageRangeY,
	ProjectileSpeed,
	Range,
	Delay,
	MaxUseCount,
	Cooldown
}

public static class SkillBonusTypeExtensions
{
	public static string ToKorean(this SkillBonusType type) => type switch
	{
		SkillBonusType.Damage => "데미지",
		SkillBonusType.DamageRangeX => "데미지 X 범위",
		SkillBonusType.DamageRangeY => "데미지 Y 범위",
		SkillBonusType.ProjectileSpeed => "투사체 속도",
		SkillBonusType.Range => "사거리",
		SkillBonusType.Delay => "공격 딜레이",
		SkillBonusType.MaxUseCount => "스킬 사용 횟수",
		SkillBonusType.Cooldown => "재사용 대기시간",
		_ => "알 수 없음",
	};

	public static System.Type ToType(this SkillBonusType type) => type switch
	{
		SkillBonusType.Damage => typeof(int),
		SkillBonusType.DamageRangeX => typeof(int),
		SkillBonusType.DamageRangeY => typeof(int),
		SkillBonusType.ProjectileSpeed => typeof(int),
		SkillBonusType.Range => typeof(int),
		SkillBonusType.MaxUseCount => typeof(int),
		SkillBonusType.Delay => typeof(float),
		SkillBonusType.Cooldown => typeof(float),
		_ => typeof(float),
	};
}