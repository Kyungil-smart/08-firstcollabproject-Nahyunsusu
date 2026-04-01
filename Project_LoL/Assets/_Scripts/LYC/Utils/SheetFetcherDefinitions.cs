using System;
using System.Linq;
using _Scripts.LYC.Skill;
using UnityEngine;

namespace _Scripts.LYC.Utils
{
	enum DataFetchingState
	{
		None,
		FetchingCSV,
		Applying,
		Parsing,
		End
	}

	class SkillRow
	{
		public string SkillText,
			SkillDesc,
			PlayerSkillType;

		public int SkillId,
			SkillDamage,
			SkillDamageRangeX,
			SkillDamageRangeY,
			SkillProjectileSpeed,
			SkillRange,
			SkillUseCount,
			SkillCoolTime,
			SkillPrice;

		public float SkillDelay;
	}

	class SkillDiceRow
	{
		public string SkillEffectType;

		public int SkillId,
			PlayerSkillDice;

		public float Amount;
	}

	public class SkillFetcher
	{
		public static Type GetSkillClassByID(string raw)
		{
			switch (raw)
			{
				case "40100001":
					return typeof(DiceRollSkill);
				case "40100002":
					return typeof(BombsTimeSkill);
				case "40100003":
					return typeof(DiceBuckSkill);
				case "40100004":
					return typeof(ShapeDiceSkill);
				case "40100005":
					return typeof(BombStaySkill);
				case "40100006":
					return typeof(LuckyStratosSkill);
				case "40100007":
					return typeof(TornadiceSkill);
				case "40100008":
					return typeof(GigaRollSkill);
				case "40100009":
					return typeof(BurningDiceSkill);
				case "40100010":
					return typeof(JackpotStratosSkill);
				case "40100011":
					return typeof(StreetDiceSkill);
				case "40100012":
					return typeof(HookDiceSkill);
				default:
					Debug.LogError($"[{nameof(SkillFetcher)}] {raw} is not in skill list");
					return null;
			}
		}

		public static DiceEffectSet GetDiceEffectSet(string effectType, float amount)
		{
			try
			{
				var type = effectType switch
				{
					"PlayerSkillDamage" => SkillBonusType.Damage,
					"PlayerSkillMaxUseCount" => SkillBonusType.MaxUseCount,
					"PlayerSkillProjectileSpeed" => SkillBonusType.ProjectileSpeed,
					"PlayerSkillRange" => SkillBonusType.Range,
					"PlayerSkillDelay" => SkillBonusType.Delay,
					"PlayerSkillDamageRangeX" => SkillBonusType.DamageRangeX,
					"PlayerSkillDamageRangeY" => SkillBonusType.DamageRangeY,
					"PlayerSkillCoolTime" => SkillBonusType.Cooldown,
					_ => throw new ArgumentOutOfRangeException(nameof(effectType), effectType, null)
				};
				var value = new DiceEffectSet { type = type, amount = amount };

				return value;
			}
			catch (Exception e)
			{
				Debug.LogError(e.Message);
			}

			return null;
		}

		public static SkillType ConvertStringToSkillType(string raw)
		{
			var value = SkillType.None;
			string[] skillTypes = raw.Split(';').Select(s => s.Trim()).ToArray();
			foreach (string skillType in skillTypes)
			{
				if (Enum.TryParse(typeof(SkillType), skillType, out object result))
				{
					value |= (SkillType)result;
				}
				else
				{
					Debug.LogWarning($"[{nameof(SkillFetcher)}] {skillType}을 {nameof(SkillType)}내에서 찾을 수 없음");
				}
			}

			return value;
		}
	}
}