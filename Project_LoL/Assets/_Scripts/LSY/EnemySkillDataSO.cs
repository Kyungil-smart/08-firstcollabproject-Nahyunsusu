using UnityEngine;

public abstract class EnemySkillDataSO : ScriptableObject
{
	public SkillData Get(int dice)
	{
		SkillData data = new()
		{
			SkillID = skillID,
			SkillName = skillName,
			SkillType = skillType,
			Damage = damage,
			DamageRangeX = damageRangeX,
			DamageRangeY = damageRangeY,
			ProjectileSpeed = projectileSpeed,
			Range = range,
			Delay = delay,
			MaxUseCount = maxUseCount,
			Cooldown = cooldown,
		};
		return data;
	}

	[Header("Common")] [SerializeField] protected string skillID;
	[SerializeField] protected string skillName;
	[SerializeField] protected SkillType skillType;

	[SerializeField] protected int damage;
	[SerializeField] protected int damageRangeX;
	[SerializeField] protected int damageRangeY;
	[SerializeField] protected float projectileSpeed;
	[SerializeField] protected int range;
	[SerializeField] protected float delay;
	[SerializeField] protected int maxUseCount;
	[SerializeField] protected int cooldown;

	public abstract void Use(EnemySkillExecutor executor);
}