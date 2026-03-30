using UnityEngine;

namespace _Scripts.LYC.Skill
{
	[CreateAssetMenu(fileName = "Tornadice", menuName = "Player/Skill/Tornadice")]
	public class TornadiceSkill : SkillDataSO
	{
		[field: Header("Tornadice")]
		[field: SerializeField]
		public SkillProjectile ProjectilePrefab { get; private set; }

		[field: SerializeField] public DiceParticleSet ProjectileSet { get; private set; }

		public override void Use(SkillExecutor executor)
		{
			Vector2 position = executor.Controller.transform.position;
			var particle = ProjectileSet.Get(executor.LastDiceResult);

			int angle = 0;
			for (int i = 0; i < 8; i++)
			{
				Vector2 dir = Quaternion.Euler(0, 0, angle) * Vector2.right;
				var proj = Instantiate(ProjectilePrefab);
				proj.Init(dir, position + dir, executor, particle);

				angle += 45;
			}
		}
	}
}