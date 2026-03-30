using UnityEngine;

namespace _Scripts.LYC.Skill
{
	[CreateAssetMenu(fileName = "BombsTime", menuName = "Player/Skill/BombsTime")]
	public class BombsTimeSkill : SkillDataSO
	{
		[field: Header("BombsTime")]
		[field: SerializeField]
		public SkillProjectile ProjectilePrefab { get; private set; }

		[field: SerializeField]
		public ParticleSystem ProjectileEffect { get; private set; }

		[field: SerializeField]
		public DiceParticleSet ExplosionParticleSet { get; private set; }

		public override void Use(SkillExecutor executor)
		{
			var explosionParticle = ExplosionParticleSet.Get(executor.LastDiceResult);

			Instantiate(ProjectilePrefab).Init(executor.MouseDir, executor.Position, executor, ProjectileEffect, explosionParticle);
		}
	}
}