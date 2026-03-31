using UnityEngine;

namespace _Scripts.LYC.Skill
{
	[CreateAssetMenu(fileName = "HookDice", menuName = "Player/Skill/HookDice")]
	public class HookDiceSkill : SkillDataSO
	{
		[field: Header("HookDice")]
		[field: SerializeField]
		public SkillHitbox Hitbox { get; private set; }

		[field: SerializeField]
		public DiceParticleSet ParticleSet { get; private set; }

		public override void Use(SkillExecutor executor)
		{
			Instantiate(Hitbox).Init(executor.MouseDir, executor.Position, executor, 0.2f, ParticleSet.Get(executor.LastDiceResult));
		}
	}
}