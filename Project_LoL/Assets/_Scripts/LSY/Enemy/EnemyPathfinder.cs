using System.Collections.Generic;
using UnityEngine;

// A* 경로 탐색
// 방 범위 안에서 그리드 생성 후 장애물 감지 → 경로 계산
public class EnemyPathfinder : MonoBehaviour
{
    [Header("그리드 설정")]
    public float nodeSize    = 1f;
    public LayerMask obstacleLayer;

    private Node[,] _grid;
    private int _gridWidth;
    private int _gridHeight;
    private Vector2 _gridOrigin;

    private class Node
    {
        public bool walkable;
        public Vector2 worldPos;
        public int gridX;
        public int gridY;
        public int gCost;
        public int hCost;
        public Node parent;

        public int fCost => gCost + hCost;
    }

    public void InitGrid(RoomNode room)
    {
        Rect rect = room.GetRect();

        _gridOrigin = new Vector2(rect.xMin, rect.yMin);
        _gridWidth  = Mathf.RoundToInt(rect.width  / nodeSize);
        _gridHeight = Mathf.RoundToInt(rect.height / nodeSize);
        _grid       = new Node[_gridWidth, _gridHeight];

        for (int x = 0; x < _gridWidth; x++)
        {
            for (int y = 0; y < _gridHeight; y++)
            {
                Vector2 worldPos = _gridOrigin + new Vector2(
                    x * nodeSize + nodeSize * 0.5f,
                    y * nodeSize + nodeSize * 0.5f
                );

                bool walkable = !Physics2D.OverlapBox(
                    worldPos,
                    Vector2.one * (nodeSize * 0.8f),
                    0f,
                    obstacleLayer
                );

                _grid[x, y] = new Node
                {
                    walkable = walkable,
                    worldPos = worldPos,
                    gridX    = x,
                    gridY    = y
                };
            }
        }
    }

    public List<Vector2> FindPath(Vector2 startPos, Vector2 targetPos)
    {
        if (_grid == null) return new List<Vector2>();

        Node startNode  = GetNodeFromWorldPos(startPos);
        Node targetNode = GetNodeFromWorldPos(targetPos);

        if (startNode == null || targetNode == null) return new List<Vector2>();
        if (!targetNode.walkable) targetNode = GetNearestWalkable(targetNode);
        if (targetNode == null) return new List<Vector2>();

        for (int x = 0; x < _gridWidth; x++)
            for (int y = 0; y < _gridHeight; y++)
            {
                _grid[x, y].gCost  = int.MaxValue;
                _grid[x, y].parent = null;
            }

        List<Node> openList     = new List<Node>();
        HashSet<Node> closedSet = new HashSet<Node>();

        startNode.gCost = 0;
        startNode.hCost = GetDistance(startNode, targetNode);
        openList.Add(startNode);

        while (openList.Count > 0)
        {
            Node current = GetLowestFCost(openList);
            openList.Remove(current);

            // 꺼낸 직후 closedSet 체크 → 중복 처리 방지
            if (closedSet.Contains(current)) continue;
            closedSet.Add(current);

            if (current == targetNode)
                return RetracePath(startNode, targetNode);

            foreach (Node neighbor in GetNeighbors(current))
            {
                if (!neighbor.walkable || closedSet.Contains(neighbor)) continue;

                int newGCost = current.gCost + GetDistance(current, neighbor);
                if (newGCost < neighbor.gCost)
                {
                    neighbor.gCost  = newGCost;
                    neighbor.hCost  = GetDistance(neighbor, targetNode);
                    neighbor.parent = current;

                    // openHashSet 제거
                    // 비용 낮아진 노드는 무조건 재추가
                    // closedSet이 중복 처리를 막아주므로 안전
                    openList.Add(neighbor);
                }
            }
        }

        return new List<Vector2>();
    }

    private Node GetLowestFCost(List<Node> list)
    {
        Node lowest = list[0];
        for (int i = 1; i < list.Count; i++)
        {
            if (list[i].fCost < lowest.fCost ||
               (list[i].fCost == lowest.fCost && list[i].hCost < lowest.hCost))
                lowest = list[i];
        }
        return lowest;
    }

    private List<Vector2> RetracePath(Node start, Node end)
    {
        List<Vector2> path = new List<Vector2>();
        Node current = end;

        while (current != start)
        {
            path.Add(current.worldPos);
            current = current.parent;
        }

        path.Reverse();
        return path;
    }

    private List<Node> GetNeighbors(Node node)
    {
        List<Node> neighbors = new List<Node>();

        for (int dx = -1; dx <= 1; dx++)
        {
            for (int dy = -1; dy <= 1; dy++)
            {
                if (dx == 0 && dy == 0) continue;

                int nx = node.gridX + dx;
                int ny = node.gridY + dy;

                if (nx >= 0 && nx < _gridWidth && ny >= 0 && ny < _gridHeight)
                    neighbors.Add(_grid[nx, ny]);
            }
        }

        return neighbors;
    }

    private int GetDistance(Node a, Node b)
    {
        int dx = Mathf.Abs(a.gridX - b.gridX);
        int dy = Mathf.Abs(a.gridY - b.gridY);
        return dx > dy ? 14 * dy + 10 * (dx - dy) : 14 * dx + 10 * (dy - dx);
    }

    private Node GetNodeFromWorldPos(Vector2 worldPos)
    {
        int x = Mathf.FloorToInt((worldPos.x - _gridOrigin.x) / nodeSize);
        int y = Mathf.FloorToInt((worldPos.y - _gridOrigin.y) / nodeSize);

        x = Mathf.Clamp(x, 0, _gridWidth  - 1);
        y = Mathf.Clamp(y, 0, _gridHeight - 1);

        return _grid[x, y];
    }

    private Node GetNearestWalkable(Node node)
    {
        for (int radius = 1; radius < 5; radius++)
        {
            for (int dx = -radius; dx <= radius; dx++)
            {
                for (int dy = -radius; dy <= radius; dy++)
                {
                    int nx = node.gridX + dx;
                    int ny = node.gridY + dy;

                    if (nx >= 0 && nx < _gridWidth && ny >= 0 && ny < _gridHeight)
                        if (_grid[nx, ny].walkable)
                            return _grid[nx, ny];
                }
            }
        }
        return null;
    }
}