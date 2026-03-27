using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "DiceRoll", menuName = "Player/Skill/DiceRoll")]
public class DiceRollSkill : SkillDataSO
{
	[field: Header("DiceRoll")]
	[field: SerializeField]
	public SkillProjectile ProjectilePrefab { get; private set; }

	[field: SerializeField]
	public List<DiceParticleSet> ProjectileVFX { get; private set; }

	public override void Use(SkillExecutor executor)
	{
		Vector2 facingDir = executor.Controller.FSM.FacingDir;
		Vector2 position = executor.Controller.transform.position;

		var proj = Instantiate(ProjectilePrefab);
		proj.Init(facingDir, position + facingDir, executor.CurrentSkillData, GetProjectile(executor.LastDiceResult));

		Debug.Log($"{skillName} 발동");
	}

	private ParticleSystem GetProjectile(int dice)
		=> ProjectileVFX.First(d => d.dice == dice).particle;

	protected override void SetEffect(SkillData data, int dice)
	{
		base.SetEffect(data, dice);

		data.SkillDescription = $"전방으로 ({data.Range})거리만큼 공격을 날려 적에게 ({data.Damage})만큼 데미지를 줍니다\n" + data.SkillDescription;
	}
}