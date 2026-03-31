using UnityEngine;

namespace _Scripts.LYC.Skill
{
	[CreateAssetMenu(fileName = "BombStay", menuName = "Player/Skill/BombStay")]
	public class BombStaySkill : SkillDataSO
	{
		[field: Header("BombStay")]
		[field: SerializeField]
		public SkillProjectile ProjectilePrefab { get; private set; }

		[field: SerializeField]
		public ParticleSystem ProjectileEffect { get; private set; }

		[field: SerializeField]
		public DiceParticleSet ExplosionParticleSet { get; private set; }

		public override void Use(SkillExecutor executor)
		{
			var explosionParticle = ExplosionParticleSet.Get(executor.LastDiceResult);

			var proj = Instantiate(ProjectilePrefab);
			proj.Init(executor.MouseDir, executor.Position, executor, ProjectileEffect, explosionParticle);
		}

	}
}