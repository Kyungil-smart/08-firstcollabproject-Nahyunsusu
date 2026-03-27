using UnityEngine;

[CreateAssetMenu(fileName = "DiceBuck", menuName = "Player/Skill/DiceBuck")]
public class DiceBuckSkill : SkillDataSO
{
	[field: Header("DiceRoll")]
	[field: SerializeField]
	public SkillProjectile ProjectilePrefab { get; private set; }

	[field: SerializeField]
	public DiceParticleSet ProjectileParticleSet { get; private set; }

	public override void Use(SkillExecutor executor)
	{
		Vector2 dir = executor.Controller.FSM.MouseDir;
		Vector2 position = executor.Controller.transform.position;
		var particle = ProjectileParticleSet.Get(executor.LastDiceResult);

		float angle = 15f;
		Vector2 upperDir = Quaternion.Euler(0, 0, angle) * dir;
		Vector2 lowerDir = Quaternion.Euler(0, 0, -angle) * dir;

		var upperProj = Instantiate(ProjectilePrefab);
		var centerProj = Instantiate(ProjectilePrefab);
		var lowerProj = Instantiate(ProjectilePrefab);
		upperProj.Init(upperDir, position + upperDir, executor, particle);
		centerProj.Init(dir, position + dir, executor, particle);
		lowerProj.Init(lowerDir, position + lowerDir, executor, particle);

		Debug.Log($"{skillName} 발동");
	}

	protected override void SetEffect(SkillData data, int dice)
	{
		base.SetEffect(data, dice);

		data.SkillDescription = $"전방으로 ({data.Range})거리만큼 공격을 날려 적에게 ({data.Damage})만큼 데미지를 줍니다\n" + data.SkillDescription;
	}
}