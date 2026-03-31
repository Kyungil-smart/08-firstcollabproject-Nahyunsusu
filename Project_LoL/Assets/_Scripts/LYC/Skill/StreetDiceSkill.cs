using UnityEngine;

namespace _Scripts.LYC.Skill
{
	[CreateAssetMenu(fileName = "StreetDice", menuName = "Player/Skill/StreetDice")]
	public class StreetDiceSkill : SkillDataSO
	{
		[field: Header("StreetDice")]
		[field: SerializeField]
		public StreetDiceHitbox Hitbox { get; private set; }

		[field: SerializeField]
		public DiceParticleSet ParticleSet { get; private set; }

		public override void Use(SkillExecutor executor)
		{
			Vector2 dir = executor.Controller.FSM.MouseDir;
			Vector2 position = executor.Controller.transform.position;

			var hitbox = Instantiate(Hitbox);
			hitbox.Init(dir, position, executor, 0.3f, ParticleSet.Get(executor.LastDiceResult));
		}
	}
}