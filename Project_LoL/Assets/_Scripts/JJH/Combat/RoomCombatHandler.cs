using System.Collections.Generic;
using UnityEngine;

public class RoomCombatHandler : MonoBehaviour
{
    [Header("스폰 설정")]
    
    [Header("위치 찾기 최대 시도 횟수")]
    [SerializeField] private int _maxSpawnAttempts = 30;   // 위치 찾기 최대 시도 횟수
    [Header("벽에서 떨어질 최소 타일 거리")]
    [SerializeField] private int _wallMargin = 1;          // 벽에서 떨어질 최소 타일 거리
    [Header("문 주변 제외 반경")]
    [SerializeField] private float _doorExcludeRadius = 4f; // 문 주변 제외 반경
    [Header("몬스터 간 최소 거리")]
    [SerializeField] private float _monsterSpacing = 3f;    // 몬스터 간 최소 거리

    private TileMapGenerator_Grid _tileMapGenerator;

    private void Awake()
    {
        _tileMapGenerator = GetComponent<TileMapGenerator_Grid>();
    }

    // 전투 방 기준 스폰 위치 반환
    public List<Vector2> GetSpawnPositions(RoomNode room, int count)
    {
        List<Vector2> spawnPositions = new List<Vector2>();

        if (room == null)
            return spawnPositions;

        if (_tileMapGenerator == null)
        {
            Debug.LogWarning("[RoomCombatHandler] TileMapGenerator_Grid 가 없습니다.");
            return spawnPositions;
        }

        if (count <= 0)
            return spawnPositions;

        // 전투 방이 아니면 스폰하지 않음
        if (room.roomData == null || room.roomData.roomType != RoomType.Combat)
            return spawnPositions;

        List<Vector2Int> candidates = _tileMapGenerator.GetFloorPositionsInRoom(room);

        candidates = FilterByWallMargin(candidates, room);
        candidates = FilterByDoorRadius(candidates, room);

        if (candidates.Count == 0)
        {
            Debug.LogWarning($"[RoomCombatHandler] {room.nodeId} 스폰 가능한 위치가 없습니다.");
            return spawnPositions;
        }

        int attempts = 0;

        // 재시도 방식으로 위치 선정
        while (spawnPositions.Count < count &&
               attempts < _maxSpawnAttempts &&
               candidates.Count > 0)
        {
            attempts++;

            int randomIndex = Random.Range(0, candidates.Count);
            Vector2Int candidate = candidates[randomIndex];
            Vector2 worldPos = new Vector2(candidate.x + 0.5f, candidate.y + 0.5f);

            if (IsTooCloseToOther(worldPos, spawnPositions))
                continue;

            spawnPositions.Add(worldPos);

            // 이미 선택된 위치는 재사용하지 않음
            candidates.RemoveAt(randomIndex);
        }

        if (spawnPositions.Count < count)
        {
            Debug.LogWarning(
                $"[RoomCombatHandler] {room.nodeId} 요청 {count}마리 중 {spawnPositions.Count}개 위치만 확보됨.");
        }

        return spawnPositions;
    }

    // 방 경계 기준으로 벽에서 일정 거리 이내 좌표 제외
    private List<Vector2Int> FilterByWallMargin(List<Vector2Int> candidates, RoomNode room)
    {
        int startX = Mathf.RoundToInt(room.worldPosition.x - room.size.x * 0.5f);
        int startY = Mathf.RoundToInt(room.worldPosition.y - room.size.y * 0.5f);
        int endX = startX + room.size.x;
        int endY = startY + room.size.y;

        List<Vector2Int> filtered = new List<Vector2Int>();

        foreach (Vector2Int pos in candidates)
        {
            if (pos.x >= startX + _wallMargin && pos.x < endX - _wallMargin &&
                pos.y >= startY + _wallMargin && pos.y < endY - _wallMargin)
            {
                filtered.Add(pos);
            }
        }

        return filtered;
    }

    // 문 주변 일정 반경 이내 좌표 제외
    private List<Vector2Int> FilterByDoorRadius(List<Vector2Int> candidates, RoomNode room)
    {
        List<Vector2Int> filtered = new List<Vector2Int>();
        float radiusSq = _doorExcludeRadius * _doorExcludeRadius;

        foreach (Vector2Int pos in candidates)
        {
            bool tooClose = false;

            foreach (DoorData door in room.doors)
            {
                Vector2 doorWorldPos = room.worldPosition + door.localPosition;
                float distSq = ((Vector2)pos - doorWorldPos).sqrMagnitude;

                if (distSq < radiusSq)
                {
                    tooClose = true;
                    break;
                }
            }

            if (!tooClose)
                filtered.Add(pos);
        }

        return filtered;
    }

    // 이미 선택된 위치와 일정 거리 이내면 제외
    private bool IsTooCloseToOther(Vector2 pos, List<Vector2> others)
    {
        float spacingSq = _monsterSpacing * _monsterSpacing;

        foreach (Vector2 other in others)
        {
            if ((pos - other).sqrMagnitude < spacingSq)
                return true;
        }

        return false;
    }
}