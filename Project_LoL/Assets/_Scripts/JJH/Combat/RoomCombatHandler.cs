using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class RoomCombatHandler : MonoBehaviour
{
    [Header("스폰 설정")]
    [SerializeField] private int _wallMargin = 1;
    [SerializeField] private float _doorExcludeRadius = 3f;
    [SerializeField] private float _monsterSpacing = 2f;

    [Header("참조")]
    [SerializeField] private TileMapGenerator_Grid _tileGenerator;
    [SerializeField] private DoorController _doorController;

    public List<Vector2> GetSpawnPositions(RoomNode room, int count)
    {
        List<Vector2> finalPositions = new List<Vector2>();

        List<Vector2Int> candidates = _tileGenerator.GetFloorPositionsInRoom(room);
        candidates = FilterByMargin(candidates, room);

        List<Vector3> doorPositions = _doorController.GetDoorWorldPositions(room);
        candidates = FilterByDoorDistance(candidates, doorPositions);

        if (candidates.Count == 0) return finalPositions;

        var shuffled = candidates.OrderBy(x => Random.value).ToList();
        foreach (var pos in shuffled)
        {
            Vector2 worldPos = new Vector2(pos.x + 0.5f, pos.y + 0.5f);

            if (!IsTooCloseToOthers(worldPos, finalPositions))
            {
                finalPositions.Add(worldPos);
                if (finalPositions.Count >= count) break;
            }
        }

        return finalPositions;
    }

    private List<Vector2Int> FilterByMargin(List<Vector2Int> origin, RoomNode room)
    {
        RectInt b = new RectInt(
            Mathf.RoundToInt(room.worldPosition.x),
            Mathf.RoundToInt(room.worldPosition.y),
            room.size.x,
            room.size.y
        );

        return origin.Where(p =>
            p.x >= b.xMin + _wallMargin && p.x < b.xMax - _wallMargin &&
            p.y >= b.yMin + _wallMargin && p.y < b.yMax - _wallMargin
        ).ToList();
    }

    private List<Vector2Int> FilterByDoorDistance(List<Vector2Int> origin, List<Vector3> doors)
    {
        float sqrRad = _doorExcludeRadius * _doorExcludeRadius;

        return origin.Where(p =>
        {
            Vector2 pWorld = new Vector2(p.x + 0.5f, p.y + 0.5f);
            return doors.All(dPos => (pWorld - (Vector2)dPos).sqrMagnitude > sqrRad);
        }).ToList();
    }

    private bool IsTooCloseToOthers(Vector2 pos, List<Vector2> others)
    {
        float sqrSpace = _monsterSpacing * _monsterSpacing;
        return others.Any(o => (pos - o).sqrMagnitude < sqrSpace);
    }
}