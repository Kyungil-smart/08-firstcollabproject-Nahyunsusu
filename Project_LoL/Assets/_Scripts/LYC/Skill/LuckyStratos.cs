using UnityEngine;

[CreateAssetMenu(fileName = "LuckyStratos", menuName = "Player/Skill/LuckyStratos")]
public class LuckyStratosSkill : SkillDataSO
{
    [field: Header("BombsTime")]
    [field: SerializeField]
    public SkillProjectile ProjectilePrefab { get; private set; }

    [field: SerializeField] public DiceParticleSet ProjectileSet { get; private set; }

    public override void Use(SkillExecutor executor)
    {
        Vector2 dir = executor.Controller.FSM.MouseDir;
        Vector2 position = executor.Controller.transform.position;
        var particle = ProjectileSet.Get(executor.LastDiceResult);

        SkillProjectile proj = Instantiate(ProjectilePrefab);
        proj.Init(dir, position + dir, executor, particle);

        Debug.Log($"{skillName} 발동");
    }

    protected override void SetEffect(SkillData data, int dice)
    {
        base.SetEffect(data, dice);

        data.SkillDescription = $"전방으로 ({data.Range})거리 에 폭탄을 발사하여 적에게 ({data.Damage})만큼의 데미지를 줍니다\n" +
                                data.SkillDescription;
    }
}