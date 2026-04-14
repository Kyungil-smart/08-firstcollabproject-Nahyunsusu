using UnityEngine;

namespace _Scripts.LYC.Skill
{
    public static class SkillDamageCalculator
    {
        public static int Calculate(SkillData skillData, PlayerData playerData)
        {
            int damage = skillData.Damage + playerData.AtkDamage;
            if (Random.Range(0, 100) < playerData.CritRate)
                damage *= (int)(playerData.CritDamage / 100.0f);
            return damage;
        }
    }
}