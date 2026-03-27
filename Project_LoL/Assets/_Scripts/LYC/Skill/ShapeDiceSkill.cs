using UnityEngine;

namespace _Scripts.LYC.Skill
{
    [CreateAssetMenu(fileName = "ShapeDice", menuName = "Player/Skill/ShapeDice")]
    public class ShapeDiceSkill : SkillDataSO
    {
        [field: Header("ShapeDice")]
        [field: SerializeField]
        public SkillProjectile ProjectilePrefab { get; private set; }

        [field: SerializeField] public DiceParticleSet ProjectileParticleSet { get; private set; }

        public override void Use(SkillExecutor executor)
        {
            Vector2 dir = executor.Controller.FSM.MouseDir;
            Vector2 position = executor.Controller.transform.position;
            var particle = ProjectileParticleSet.Get(executor.LastDiceResult);

            var projectile = Instantiate(ProjectilePrefab);
            projectile.Init(dir, position + dir, executor, particle);
        }

        protected override void SetEffect(SkillData data, int dice)
        {
            base.SetEffect(data, dice);

            data.SkillDescription = $"전방으로 ({data.Range})거리만큼 공격을 날려 적에게 ({data.Damage})만큼 데미지를 줍니다\n" +
                                    data.SkillDescription;
        }
    }
}