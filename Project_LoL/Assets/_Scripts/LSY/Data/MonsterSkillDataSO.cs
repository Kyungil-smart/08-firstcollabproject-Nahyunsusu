using UnityEngine;

[CreateAssetMenu(fileName = "NewMonsterSkill", menuName = "Data/Monster Skill Data")]
public class MonsterSkillDataSO : ScriptableObject
{
    [Header("기획서 식별 정보")]
    public string skillId;
    public MonsterSkillType skillType;
    
    [Header("스킬 타겟팅 방식")]
    [Tooltip("체크: 플레이어 발밑에 생성")]
    public bool isFixedTarget = false;

    [Header("공격 설정 (다회 타격)")]
    public int attackCount = 1;
    public float attackInterval = 0.2f;
    public int baseDamage = 10;

    [Header("VFX 설정")]
    public GameObject castVfxPrefab;
    public GameObject mainVfxPrefab;
    public GameObject hitVfxPrefab;
    public GameObject trailVfxPrefabA;
    public GameObject trailVfxPrefabB;
    
    [Header("다회 타격 전용 설정")]
    public GameObject skillPrefab;
    public GameObject skillPrefab2;

    [Header("워닝 및 범위 설정")]
    public float warningDuration = 1.0f;
    public float skillDuration = 1.0f;
    public GameObject warningPrefab;
    public float range = 5f;
    public float damageRangeX = 1f;
    public float damageRangeY = 1f;
    public float projectileSpeed = 0f;

    [Header("타겟 설정")]
    public LayerMask targetLayer;
}