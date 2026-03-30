using UnityEngine;

[CreateAssetMenu(fileName = "NewMonsterSkill", menuName = "Data/Monster Skill Data")]
public class MonsterSkillDataSO : ScriptableObject
{
    public string skillId;
    public MonsterSkillType skillType = MonsterSkillType.Melee;

    [Header("데미지")]
    public int damage = 10;

    [Header("공격 횟수 설정")]
    public int hitCount = 1;         
    public float hitInterval = 0.2f; 

    [Header("워닝 설정")]
    public float warningDuration = 1f; 

    [Header("공통 범위 설정")]
    public float range = 5f;         
    public float damageRangeX = 2f;
    public float damageRangeY = 2f;

    [Header("투사체 설정 (Ranged)")]
    public MonsterProjectile projectilePrefab;
    public ParticleSystem projectileParticle;
    public float projectileSpeed = 5f;

    [Header("타켓 설정 (레이어)")]
    public LayerMask targetLayer; 
}