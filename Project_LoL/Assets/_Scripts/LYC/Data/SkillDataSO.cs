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
	{
		SkillData data = new()
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
			MaxUseCount = maxUseCount,
			Cooldown = cooldown,
		};

		SetEffect(data, dice);
		return data;
	}

	[Header("Common")] [SerializeField] protected string skillID;
	[SerializeField] protected string skillName;
	[SerializeField] protected List<SkillImageData> skillIconList;
	[SerializeField] protected AttackType skillType;

	[SerializeField] protected int damage;
	[SerializeField] protected int damageRangeX;
	[SerializeField] protected int damageRangeY;
	[SerializeField] protected float projectileSpeed;
	[SerializeField] protected int range;
	[SerializeField] protected float delay;
	[SerializeField] protected int maxUseCount;
	[SerializeField] protected int cooldown;

	[Header("Bonus")]
	[SerializeField] protected SkillDiceEffectGroup diceEffects;

	public abstract void Use(SkillExecutor executor);

	protected virtual void SetEffect(SkillData data, int dice)
	{
		var list = diceEffects.Get(dice);
		if (list == null || list.Count == 0) return;

		data.SkillDescription = "스킬 강화 ";

		foreach (DiceEffectSet effect in list)
		{
			switch (effect.type)
			{
				case SkillBonusType.Damage:
					data.Damage += (int)effect.amount;
					break;
				case SkillBonusType.DamageRangeX:
					data.DamageRangeX += (int)effect.amount;
					break;
				case SkillBonusType.DamageRangeY:
					data.DamageRangeY += (int)effect.amount;
					break;
				case SkillBonusType.ProjectileSpeed:
					data.ProjectileSpeed += (int)effect.amount;
					break;
				case SkillBonusType.Range:
					data.Range += (int)effect.amount;
					break;
				case SkillBonusType.Delay:
					data.Delay += effect.amount;
					break;
				case SkillBonusType.MaxUseCount:
					data.MaxUseCount += (int)effect.amount;
					break;
				case SkillBonusType.Cooldown:
					data.Delay += effect.amount;
					break;
				default:
					throw new System.ArgumentOutOfRangeException();
			}

			var str = $"\"{effect.type.ToKorean()} {(effect.amount >= 0 ? "+" : "-")}{effect.amount}\" ";
			data.SkillDescription += str;
		}
	}
}