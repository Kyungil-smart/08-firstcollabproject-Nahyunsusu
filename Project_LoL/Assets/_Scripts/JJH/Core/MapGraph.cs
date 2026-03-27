using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class MapGraph
{
    public RoomNode startRoom;
    public RoomNode bossRoom;
    public List<RoomNode> allRooms;

    private int _nodeCounter = 0;
    private int _roomSpacing = 15;

    public MapGraph()
    {
        allRooms = new List<RoomNode>();
    }

    private RoomNode CreateNode(RoomData data)
    {
        RoomNode node = new RoomNode($"room_{_nodeCounter++}", data);
        node.size = data.GetRandomSize();
        allRooms.Add(node);
        return node;
    }

    public void Generate(MapRoomPool pool, int roomSpacing)
    {
        _roomSpacing = roomSpacing;
        _nodeCounter = 0;
        allRooms.Clear();

        startRoom = CreateNode(pool.startData);
        startRoom.worldPosition = Vector2.zero;

        int branchCount = Random.Range(2, 5);
        List<RoomData> roomPool = BuildShuffledPool(pool);
        List<List<RoomNode>> branches = new List<List<RoomNode>>();
        for (int i = 0; i < branchCount; i++) branches.Add(new List<RoomNode>());

        int branchIndex = 0;
        foreach (RoomData data in roomPool)
        {
            branches[branchIndex % branchCount].Add(CreateNode(data));
            branchIndex++;
        }

        PositionBranches(branches);
        EstablishConnections(branches, pool);
    }

    private void PositionBranches(List<List<RoomNode>> branches)
    {
        List<int> branchWidths = branches.Select(b => GetMaxBranchWidth(b)).ToList();
        int totalWidth = branchWidths.Sum() + (_roomSpacing * (branches.Count - 1));

        int currentX = -totalWidth / 2;

        for (int i = 0; i < branches.Count; i++)
        {
            int branchCenterX = currentX + (branchWidths[i] / 2);
            int currentY = Mathf.RoundToInt(startRoom.size.y + _roomSpacing);

            foreach (var node in branches[i])
            {
                node.worldPosition = new Vector2(branchCenterX - (node.size.x / 2), currentY);
                currentY += node.size.y + _roomSpacing;
            }

            currentX += branchWidths[i] + _roomSpacing;
        }
    }

    private void EstablishConnections(List<List<RoomNode>> branches, MapRoomPool pool)
    {
        foreach (var branch in branches)
        {
            if (branch.Count > 0)
                startRoom.ConnectTo(branch[0]);
        }

        foreach (var branch in branches)
        {
            for (int i = 0; i < branch.Count - 1; i++)
            {
                branch[i].ConnectTo(branch[i + 1]);
            }
        }

        MergeBranches(branches);

        RoomNode lastNode = GetLongestBranchEnd(branches);

        RoomNode repairNode = CreateNode(pool.repairData);
        repairNode.worldPosition = new Vector2(
            lastNode.worldPosition.x,
            lastNode.worldPosition.y + lastNode.size.y + _roomSpacing
        );
        lastNode.ConnectTo(repairNode);

        bossRoom = CreateNode(pool.bossData);
        bossRoom.worldPosition = new Vector2(
            repairNode.worldPosition.x,
            repairNode.worldPosition.y + repairNode.size.y + _roomSpacing
        );
        repairNode.ConnectTo(bossRoom);
    }

    private int GetMaxBranchWidth(List<RoomNode> branch)
    {
        return branch.Count > 0 ? branch.Max(n => n.size.x) : 0;
    }

    private List<RoomData> BuildShuffledPool(MapRoomPool pool)
    {
        List<RoomData> list = new List<RoomData>(pool.combatRooms);
        for (int i = list.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            (list[i], list[j]) = (list[j], list[i]);
        }
        return list;
    }

    private void MergeBranches(List<List<RoomNode>> branches)
    {
        List<RoomNode> ends = branches.Where(b => b.Count > 0).Select(b => b.Last()).ToList();
        while (ends.Count > 1)
        {
            List<RoomNode> nextEnds = new List<RoomNode>();
            for (int i = 0; i < ends.Count - 1; i += 2)
            {
                ends[i].ConnectTo(ends[i + 1]);
                nextEnds.Add(ends[i + 1]);
            }

            if (ends.Count % 2 != 0) nextEnds.Add(ends.Last());
            ends = nextEnds;
        }
    }

    private RoomNode GetLongestBranchEnd(List<List<RoomNode>> branches)
    {
        return branches.OrderByDescending(b => b.Count).First().Last();
    }
}