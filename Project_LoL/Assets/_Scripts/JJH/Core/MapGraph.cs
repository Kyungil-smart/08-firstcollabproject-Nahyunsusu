using System.Collections.Generic;
using System.Linq;
using UnityEngine;
 
public class MapGraph
{
    public RoomNode startRoom;
    public RoomNode bossRoom;
    public List<RoomNode> allRooms = new List<RoomNode>();
 
    private int _nodeCounter;
    private int _spacing;
 
    private RoomNode CreateNode(RoomData data)
    {
        var node = new RoomNode($"room_{_nodeCounter++}", data);
        node.size = data.GetRandomSize();
        allRooms.Add(node);
        return node;
    }
 
    public void Generate(MapRoomPool pool, int spacing)
    {
        _spacing = spacing;
        _nodeCounter = 0;
        allRooms.Clear();
 
        startRoom = CreateNode(pool.startData);
        startRoom.gridOrigin = new Vector2Int(-startRoom.size.x / 2, -startRoom.size.y / 2);
 
        List<RoomData> shuffled = BuildShuffledPool(pool);
        int branchCount = Random.Range(2, 5);
        var branches = new List<List<RoomNode>>();
        for (int i = 0; i < branchCount; i++) branches.Add(new List<RoomNode>());
 
        for (int i = 0; i < shuffled.Count; i++)
            branches[i % branchCount].Add(CreateNode(shuffled[i]));
 
        PositionBranches(branches);
        EstablishConnections(branches, pool);
    }
 
    private void PositionBranches(List<List<RoomNode>> branches)
    {
        int totalWidth = branches.Sum(b => GetMaxWidth(b)) + _spacing * (branches.Count - 1);
        int currentX = startRoom.gridOrigin.x - totalWidth / 2;
 
        foreach (var branch in branches)
        {
            int maxW = GetMaxWidth(branch);
            int centerX = currentX + maxW / 2;
            int currentY = startRoom.gridOrigin.y + startRoom.size.y + _spacing;
 
            foreach (var node in branch)
            {
                node.gridOrigin = new Vector2Int(centerX - node.size.x / 2, currentY);
                currentY += node.size.y + _spacing;
            }
 
            currentX += maxW + _spacing;
        }
    }
 
    private void EstablishConnections(List<List<RoomNode>> branches, MapRoomPool pool)
    {
        foreach (var branch in branches)
            if (branch.Count > 0) startRoom.ConnectTo(branch[0]);

        foreach (var branch in branches)
            for (int i = 0; i < branch.Count - 1; i++)
                branch[i].ConnectTo(branch[i + 1]);

        MergeBranches(branches);

        RoomNode last = branches.OrderByDescending(b => b.Count).First().Last();

        bossRoom = CreateNode(pool.bossData);
        int bossCx = last.gridOrigin.x + last.size.x / 2 - bossRoom.size.x / 2;
        int bossY = last.gridOrigin.y + last.size.y + _spacing * 2;
        bossRoom.gridOrigin = new Vector2Int(bossCx, bossY);

        RoomNode repair = CreateNode(pool.repairData);
        int repairCx = bossCx + bossRoom.size.x / 2 - repair.size.x / 2;
        repair.gridOrigin = new Vector2Int(repairCx, last.gridOrigin.y + last.size.y + _spacing);

        last.ConnectTo(repair);
        repair.ConnectTo(bossRoom);
    }
 
    private void MergeBranches(List<List<RoomNode>> branches)
    {
        List<RoomNode> ends = branches.Where(b => b.Count > 0).Select(b => b.Last()).ToList();
        while (ends.Count > 1)
        {
            var next = new List<RoomNode>();
            for (int i = 0; i < ends.Count - 1; i += 2)
            {
                ends[i].ConnectTo(ends[i + 1]);
                next.Add(ends[i + 1]);
            }
            if (ends.Count % 2 != 0) next.Add(ends.Last());
            ends = next;
        }
    }
 
    private int GetMaxWidth(List<RoomNode> branch) =>
        branch.Count > 0 ? branch.Max(n => n.size.x) : 0;
 
    private List<RoomData> BuildShuffledPool(MapRoomPool pool)
    {
        var list = new List<RoomData>(pool.combatRooms);
        for (int i = list.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            (list[i], list[j]) = (list[j], list[i]);
        }
        return list;
    }
}