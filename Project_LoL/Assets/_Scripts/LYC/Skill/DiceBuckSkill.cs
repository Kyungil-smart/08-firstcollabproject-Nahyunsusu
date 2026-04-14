using UnityEngine;

namespace _Scripts.LYC.Skill
{
	[CreateAssetMenu(fileName = "DiceBuck", menuName = "Player/Skill/DiceBuck")]
	public class DiceBuckSkill : SkillDataSO
	{
		[field: Header("DiceBuck")]
		[field: SerializeField]
		public SkillProjectile ProjectilePrefab { get; private set; }

		[field: SerializeField]
		public DiceParticleSet ProjectileParticleSet { get; private set; }

		public override void Use(SkillExecutor executor)
		{
			Vector2 dir = executor.MouseDir;
			Vector2 position = executor.Position;
			var particle = ProjectileParticleSet.Get(executor.LastDiceResult);

			float angle = 15f;
			Vector2 upperDir = Quaternion.Euler(0, 0, angle) * dir;
			Vector2 lowerDir = Quaternion.Euler(0, 0, -angle) * dir;

			var upperProj = Instantiate(ProjectilePrefab);
			var centerProj = Instantiate(ProjectilePrefab);
			var lowerProj = Instantiate(ProjectilePrefab);
			upperProj.Init(upperDir, position + upperDir, executor, particle, null, hitEffect);
			centerProj.Init(dir, position + dir, executor, particle, null, hitEffect);
			lowerProj.Init(lowerDir, position + lowerDir, executor, particle, null, hitEffect);
		}
	}
}