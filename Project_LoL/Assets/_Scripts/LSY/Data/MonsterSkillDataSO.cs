using UnityEngine;

[CreateAssetMenu(fileName = "NewMonsterSkill", menuName = "Monster/Monster Skill Data")]
public class MonsterSkillDataSO : ScriptableObject
{
    [Header("식별 정보")]
    public int skillId; 

    [Header("스킬 스탯")]
    public int baseDamage = 10;
    
    [Tooltip("투사체 속도")]
    public float projectileSpeed = 0f;
    
    [Tooltip("사거리")]
    public float range = 5f;
    
    [Tooltip("워닝 시간")]
    public float warningDuration = 1.0f;

    [Header("투사체 / 타격 범위")]
    [Tooltip("가로 크기")]
    public float damageRangeX = 1f;
    
    [Tooltip("세로 크기")]
    public float damageRangeY = 1f;
    
    [Tooltip("발사 갯수")]
    public int attackCount = 1;
    
    [Tooltip("투사체 유지 시간")]
    public float skillDuration = 1.0f; 

    [Header("피격 이펙트")]
    [Tooltip("데미지박스 이펙트 크기")]
    public float impactScale = 1f;
    
    [Tooltip("데미지박스 이펙트 존재 시간")]
    public float impactTime = 1f;


    [Header("내부 시스템 로직")]
    public MonsterSkillType skillType;
    [Tooltip("체크: 플레이어 발밑에 생성 (장판) / 체크 해제: 바라보는 방향 (투사체)")]
    public bool isFixedTarget = false;
    [Tooltip("타격 사이의 간격")]
    public float attackInterval = 0.2f;
    public LayerMask targetLayer;

    [Header("VFX 및 프리팹")]
    public GameObject skillPrefab;
    public GameObject skillPrefab2;
    public GameObject warningPrefab;
    public GameObject mainVfxPrefab;
    public GameObject hitVfxPrefab;
    public GameObject trailVfxPrefabA;
    public GameObject trailVfxPrefabB;
}