using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class SkillDataSO : ScriptableObject
{
	[System.Serializable]
	public class SkillImageData
	{
		public int dice;
		public UnityEngine.UI.Image skillImage;
	}

	public SkillData Get(int dice)
		=> new()
		{
			SkillID = skillID,
			SkillName = skillName,
			SkillType = skillType,
			SkillImage = skillIconList.FirstOrDefault(x => x.dice == dice)?.skillImage,
			Damage = damage,
			DamageRangeX = damageRangeX,
			DamageRangeY = damageRangeY,
			ProjectileSpeed = projectileSpeed,
			Range = range,
			Delay = delay,
			UsableCount = usableCount,
			Cooldown = cooldown,
		};

	[SerializeField] protected string skillID;
	[SerializeField] protected string skillName;
	[SerializeField] protected List<SkillImageData> skillIconList;
	[SerializeField] protected string skillType;

	[SerializeField] protected int damage;
	[SerializeField] protected int damageRangeX;
	[SerializeField] protected int damageRangeY;
	[SerializeField] protected float projectileSpeed;
	[SerializeField] protected int range;
	[SerializeField] protected float delay;
	[SerializeField] protected int usableCount;
	[SerializeField] protected int cooldown;
}