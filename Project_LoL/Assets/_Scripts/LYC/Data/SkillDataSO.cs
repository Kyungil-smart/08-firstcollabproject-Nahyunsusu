using System.Diagnostics.CodeAnalysis;
using _Scripts.LYC.Skill;
using UnityEngine;

public abstract class SkillDataSO : ScriptableObject
{
	[System.Serializable]
	public class DiceSkillIconSet
	{
		[Header("Dice 1")]
		public Sprite icon1;

		[Header("Dice 2")]
		public Sprite icon2;

		[Header("Dice 3")]
		public Sprite icon3;

		[Header("Dice 4")]
		public Sprite icon4;

		[Header("Dice 5")]
		public Sprite icon5;

		[Header("Dice 6")]
		public Sprite icon6;

		public Sprite Get(int dice) => dice switch
		{
			1 => icon1,
			2 => icon2,
			3 => icon3,
			4 => icon4,
			5 => icon5,
			_ => icon6
		};
	}

	public SkillData Get(int dice)
	{
		SkillData data = new()
		{
			SkillID = skillID,
			SkillName = skillName,
			SkillType = skillType,
			SkillImage = skillIconSet.Get(dice),
			Damage = damage,
			DamageRangeX = damageRangeX,
			DamageRangeY = damageRangeY,
			ProjectileSpeed = projectileSpeed,
			Range = range,
			Delay = delay,
			MaxUseCount = maxUseCount,
			Cooldown = cooldown,
			Price = price
		};

		SetEffect(data, dice);
		return data;
	}

	[Header("Common")] [SerializeField] protected string skillID;
	[SerializeField] protected string skillName;
	[SerializeField] protected string description;
	[SerializeField] protected SkillType skillType;
	[SerializeField] protected DiceSkillIconSet skillIconSet;

	[SerializeField] protected int damage;
	[SerializeField] protected int damageRangeX;
	[SerializeField] protected int damageRangeY;
	[SerializeField] protected float projectileSpeed;
	[SerializeField] protected int range;
	[SerializeField] protected float delay;
	[SerializeField] protected int maxUseCount;
	[SerializeField] protected int cooldown;
	[SerializeField] protected int price;

	[Header("Bonus")]
	[SerializeField]
	protected SkillDiceEffectGroup diceEffects;

	public abstract void Use(SkillExecutor executor);

	protected void SetEffect(SkillData data, int dice)
	{
		var list = diceEffects.Get(dice);

		if (list != null && list.Count > 0)
		{
			string effectDescription = "스킬 강화 ";

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
				effectDescription += str;
			}

			data.SkillDescription = ResolveDescription(data) + "\n" + effectDescription;
		}
		else
		{
			data.SkillDescription = ResolveDescription(data);
		}
	}

	[SuppressMessage("ReSharper", "SpecifyACultureInStringConversionExplicitly")]
	private string ResolveDescription(SkillData data)
	{
		return description
			.Replace("{damage}", data.Damage.ToString())
			.Replace("{damageRangeX}", data.DamageRangeX.ToString())
			.Replace("{damageRangeY}", data.DamageRangeY.ToString())
			.Replace("{projectileSpeed}", data.ProjectileSpeed.ToString())
			.Replace("{range}", data.Range.ToString())
			.Replace("{delay}", data.Delay.ToString())
			.Replace("{maxUseCount}", data.MaxUseCount.ToString())
			.Replace("{cooldown}", data.Cooldown.ToString());
	}
}