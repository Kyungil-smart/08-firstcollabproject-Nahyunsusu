using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "BombsTime", menuName = "Player/Skill/BombsTime")]
public class BombsTimeSkill : SkillDataSO
{
	[System.Serializable]
	public class BombsTimeExplosionData
	{
		public int dice;
		public ParticleSystem explosionParticle;
	}

	[field: Header("BombsTime")]
	[field: SerializeField]
	public SkillProjectile ProjectilePrefab { get; private set; }

	[field: SerializeField]
	public ParticleSystem ProjectileEffect { get; private set; }

	[field: SerializeField]
	public List<BombsTimeExplosionData> ExplosionEffectsList { get; private set; }

	public override void Use(SkillExecutor executor)
	{
		Vector2 facingDir = executor.Controller.FSM.FacingDir;
		Vector2 position = executor.Controller.transform.position;

		SkillProjectile proj = Instantiate(ProjectilePrefab);
		proj.Init(facingDir,
			position + facingDir,
			executor.CurrentSkillData,
			ProjectileEffect,
			GetExplosionParticle(executor.LastDiceResult));

		Debug.Log($"{skillName} 발동");
	}

	private ParticleSystem GetExplosionParticle(int dice)
		=> ExplosionEffectsList.First(e => e.dice == dice).explosionParticle;

	protected override void SetEffect(SkillData data, int dice)
	{
		base.SetEffect(data, dice);

		data.SkillDescription = $"전방으로 ({data.Range})거리 에 폭탄을 발사하여 적에게 ({data.Damage})만큼의 데미지를 줍니다\n" + data.SkillDescription;
	}
}