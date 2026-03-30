using UnityEngine;

namespace _Scripts.LYC.Skill
{
    [CreateAssetMenu(fileName = "StreetDice", menuName = "Player/Skill/StreetDice")]
    public class StreetDiceSkill : SkillDataSO
    {
        [field: Header("StreetDice")]
        [field: SerializeField]
        public ParticleSystem Effect { get; private set; }

        public override void Use(SkillExecutor executor)
        {
            Vector2 dir = executor.Controller.FSM.MouseDir;
            Vector2 position = executor.Controller.transform.position;

            // TODO
        }

        protected override void SetEffect(SkillData data, int dice)
        {
            base.SetEffect(data, dice);

            data.SkillDescription = $"전방으로 ({data.Range})거리 에 폭탄을 발사하여 적에게 ({data.Damage})만큼의 데미지를 줍니다\n" +
                                    data.SkillDescription;
        }
    }
}