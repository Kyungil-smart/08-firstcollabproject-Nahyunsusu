using UnityEngine;

namespace _Scripts.LYC.Skill
{
	[CreateAssetMenu(fileName = "JackpotStratos", menuName = "Player/Skill/JackpotStratos")]
	public class JackpotStratosSkill : SkillDataSO
	{
		[field: Header("JackpotStratos")]
		[field: SerializeField]
		public SkillHitbox Hitbox { get; private set; }

		[field: SerializeField]
		public DiceParticleSet ParticleSet { get; private set; }

		public override void Use(SkillExecutor executor)
		{
			Instantiate(Hitbox).Init(Vector2.zero, executor.Position, executor, 0.2f, ParticleSet.Get(executor.LastDiceResult));
		}
	}
}