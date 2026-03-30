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
			Vector2 position = executor.Controller.transform.position;

			var hitbox = Instantiate(Hitbox);
			hitbox.Init(Vector2.zero, position, executor, 0.2f, ParticleSet.Get(executor.LastDiceResult));
		}

		protected override void SetEffect(SkillData data, int dice)
		{
			base.SetEffect(data, dice);

			data.SkillDescription = $"전방으로 ({data.Range})거리 에 폭탄을 발사하여 적에게 ({data.Damage})만큼의 데미지를 줍니다\n" +
			                        data.SkillDescription;
		}
	}
}