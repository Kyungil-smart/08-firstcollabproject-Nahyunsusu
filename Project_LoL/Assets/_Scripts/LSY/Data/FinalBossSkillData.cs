using UnityEngine;

[CreateAssetMenu(fileName = "FinalBossSkillData", menuName = "Monster/FinalBossSkillData")]
public class FinalBossSkillData : ScriptableObject
{
    [Header("MonsterSkill 데이터 테이블 필드")]
    public int   monsterSkillId;
    public int   monsterSkillDamage;
    public float monsterSkillProjectileSpeed;
    public float monsterSkillRange;
    public float monsterWarningSign;

    [Header("투사채 (Projectile / 워닝사인)")]
    public float monsterSkillProjectileScaleX;
    public float monsterSkillProjectileScaleY;
    public int   monsterSkillProjectileCount;
    public float monsterSkillProjectileTime;

    [Header("임팩트 (Impact / DamageBox)")]
    public float monsterSkillImpactScaleX;
    public float monsterSkillImpactScaleY;
    public float monsterSkillImpactTime;

    [Header("스프라이트 (Inspector 연결)")]
    public Sprite projectileSprite;
    public Sprite impactSprite;
    public Sprite indicatorSprite;
}
