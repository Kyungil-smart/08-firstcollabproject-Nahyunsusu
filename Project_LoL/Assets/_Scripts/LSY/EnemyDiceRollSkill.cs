using UnityEngine;

[CreateAssetMenu(fileName = "DiceRoll", menuName = "Enemy/Skill/DiceRoll")]
public class EnemyDiceRollSkill : EnemySkillDataSO
{
	[field: Header("DiceRoll")]
	[field: SerializeField]
	public EnemySkillProjectile ProjectilePrefab { get; private set; }

	[field: SerializeField]
	public ParticleSystem ProjectileParticle { get; private set; }

	public override void Use(EnemySkillExecutor executor)
	{
		Instantiate(ProjectilePrefab).EnemyInit(
			Vector2.right,
			executor.Controller.transform.position,
			executor, ProjectileParticle);
	}
}