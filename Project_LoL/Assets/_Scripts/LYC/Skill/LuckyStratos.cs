using UnityEngine;
using UnityEngine.Serialization;

namespace _Scripts.LYC.Skill
{
	[CreateAssetMenu(fileName = "LuckyStratos", menuName = "Player/Skill/LuckyStratos")]
	public class LuckyStratosSkill : SkillDataSO
	{
		[field: Header("LuckyStratos")]
		[field: SerializeField]
		public SkillProjectile ProjectilePrefab { get; private set; }

		[field: FormerlySerializedAs("<ProjectileSet>k__BackingField")] [field: SerializeField] public DiceParticleSet EffectSet { get; private set; }

		public override void Use(SkillExecutor executor)
		{
			var particle = EffectSet.Get(executor.LastDiceResult);
			Instantiate(ProjectilePrefab).Init(executor.MouseDir, executor.Position, executor, particle, hitEffect);
		}
	}
}