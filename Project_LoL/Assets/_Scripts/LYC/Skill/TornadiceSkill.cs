using UnityEngine;

[CreateAssetMenu(fileName = "Tornadice", menuName = "Player/Skill/Tornadice")]
public class TornadiceSkill : SkillDataSO
{
    [field: Header("BombsTime")]
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
            SkillProjectile proj = Instantiate(ProjectilePrefab);
            proj.Init(dir, position + dir, executor, particle);

            angle += 45;
        }

        Debug.Log($"{skillName} 발동");
    }

    protected override void SetEffect(SkillData data, int dice)
    {
        base.SetEffect(data, dice);

        data.SkillDescription = $"전방으로 ({data.Range})거리 에 폭탄을 발사하여 적에게 ({data.Damage})만큼의 데미지를 줍니다\n" +
                                data.SkillDescription;
    }
}