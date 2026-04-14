using UnityEngine;

namespace _Scripts.LYC.Skill
{
	[CreateAssetMenu(fileName = "BurningDice", menuName = "Player/Skill/BurningDice")]
	public class BurningDiceSkill : SkillDataSO
	{
		[field: Header("BurningDice")]
		[field: SerializeField]
		public SkillHitbox Hitbox { get; private set; }

		[field: SerializeField]
		public ParticleSystem Effect { get; private set; }

		public override void Use(SkillExecutor executor)
		{
			Instantiate(Hitbox).Init(executor.MouseDir, executor.Position, executor, 0.2f, Effect, hitEffect);
		}
	}
}