using UnityEngine;

namespace _Scripts.LYC.Skill
{
	[CreateAssetMenu(fileName = "GigaRoll", menuName = "Player/Skill/GigaRoll")]
	public class GigaRollSkill : SkillDataSO
	{
		[field: Header("GigaRoll")]
		[field: SerializeField]
		public GigaRollProjectile ProjectilePrefab { get; private set; }

		[field: SerializeField] public ParticleSystem ProjectileEffect { get; private set; }

		[field: SerializeField] public ParticleSystem ExplosionParticle { get; private set; }

		public override void Use(SkillExecutor executor)
		{
			Instantiate(ProjectilePrefab).Init(executor.MouseDir, executor.Position, executor, ProjectileEffect, ExplosionParticle);
		}
	}
}