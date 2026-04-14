using UnityEngine;

namespace _Scripts.LYC.Skill
{
	[CreateAssetMenu(fileName = "DiceRoll", menuName = "Player/Skill/DiceRoll")]
	public class DiceRollSkill : SkillDataSO
	{
		[field: Header("DiceRoll")]
		[field: SerializeField]
		public SkillProjectile ProjectilePrefab { get; private set; }

		[field: SerializeField]
		public DiceParticleSet ProjectileParticleSet { get; private set; }

		public override void Use(SkillExecutor executor)
		{
			var particle = ProjectileParticleSet.Get(executor.LastDiceResult);
			Instantiate(ProjectilePrefab).Init(executor.MouseDir, executor.Position, executor, particle, null, hitEffect);
		}
	}
}