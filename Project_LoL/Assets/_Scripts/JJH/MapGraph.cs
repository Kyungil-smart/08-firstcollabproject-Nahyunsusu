using System.Collections.Generic;
using UnityEngine;

public class MapGraph
{
    public RoomNode startRoom;
    public RoomNode bossRoom;
    public List<RoomNode> allRooms;

    private int _nodeCounter = 0;
    private const int RoomSpacing = 5;

    public MapGraph()
    {
        allRooms = new List<RoomNode>();
    }

    // 노드 생성 공통 처리
    // ID 부여와 전체 목록 등록을 같이 묶음
    private RoomNode CreateNode(RoomData data)
    {
        RoomNode node = new RoomNode($"room_{_nodeCounter++}", data);
        node.size = data.GetRoomSize();
        allRooms.Add(node);
        return node;
    }

    public void Generate(MapRoomPool pool)
    {
        // 시작 방 먼저 생성
        startRoom = CreateNode(pool.startData);
        startRoom.worldPosition = Vector2.zero;

        // 시작 방 이후 갈래 수
        int branchCount = Random.Range(2, 5);

        // 시작 / 강화 / 보스를 제외한 일반 배치용 방 목록
        List<RoomData> roomPool = BuildShuffledPool(pool);

        // 갈래별 방 목록
        List<List<RoomNode>> branches = new List<List<RoomNode>>();
        for (int i = 0; i < branchCount; i++)
            branches.Add(new List<RoomNode>());

        // 셔플된 방을 갈래에 순서대로 분배
        int branchIndex = 0;
        foreach (RoomData data in roomPool)
        {
            branches[branchIndex % branchCount].Add(CreateNode(data));
            branchIndex++;
        }
        
        // 갈래별 위치 계산
        // 각 갈래의 실제 최대 너비 기반으로 겹침 없이 배치
        List<float> branchWidths = new List<float>();
        foreach (List<RoomNode> branch in branches)
            branchWidths.Add(GetBranchWidth(branch));

        float totalWidth = 0f;
        foreach (float w in branchWidths)
            totalWidth += w;
        totalWidth += RoomSpacing * (branches.Count - 1);

        float currentX = -totalWidth * 0.5f;

        for (int i = 0; i < branches.Count; i++)
        {
            List<RoomNode> branch = branches[i];
            float branchWidth = branchWidths[i];
            float branchCenterX = currentX + branchWidth * 0.5f;
            float currentY = -(startRoom.size.y * 0.5f + RoomSpacing + branch[0].size.y * 0.5f);

            foreach (RoomNode node in branch)
            {
                node.worldPosition = new Vector2(branchCenterX, currentY);
                currentY -= node.size.y + RoomSpacing;
            }

            currentX += branchWidth + RoomSpacing;
        }
        
        // 시작 방에서 각 갈래 첫 방으로 연결
        foreach (List<RoomNode> branch in branches)
        {
            if (branch.Count > 0)
            {
                startRoom.ConnectTo(branch[0]);
            }
        }

        // 같은 갈래 안에서는 앞에서 뒤로 연결
        foreach (List<RoomNode> branch in branches)
        {
            for (int i = 0; i < branch.Count - 1; i++)
            {
                branch[i].ConnectTo(branch[i + 1]);
            }
        }

        // 여러 갈래를 뒤쪽에서 하나로 모으는 처리
        MergeBranches(branches);

        // 합류 후에는 가장 긴 갈래 끝에 강화 방과 보스 방 배치
        RoomNode endNode = GetLongestBranchEnd(branches);
        RoomNode repairNode = CreateNode(pool.repairData);
        bossRoom = CreateNode(pool.bossData);

        float repairOffsetY = endNode.size.y * 0.5f;
        repairOffsetY += RoomSpacing;
        repairOffsetY += repairNode.size.y * 0.5f;

        repairNode.worldPosition = new Vector2(
            endNode.worldPosition.x,
            endNode.worldPosition.y - repairOffsetY
        );
        
        float bossOffsetY = repairNode.size.y * 0.5f;
        bossOffsetY += RoomSpacing;
        bossOffsetY += bossRoom.size.y * 0.5f;

        bossRoom.worldPosition = new Vector2(
            repairNode.worldPosition.x,
            repairNode.worldPosition.y - bossOffsetY
        );

        endNode.ConnectTo(repairNode);
        repairNode.ConnectTo(bossRoom);
    }
    
    // 갈래에서 가장 넓은 방 기준으로 가로 폭 계산
    private float GetBranchWidth(List<RoomNode> branch)
    {
        float max = 0f;
        foreach (RoomNode node in branch)
        {
            if (node.size.x > max)
                max = node.size.x;
        }
        return max;
    }

    // 일반 배치용 방만 모아서 섞기
    private List<RoomData> BuildShuffledPool(MapRoomPool pool)
    {
        List<RoomData> list = new List<RoomData>();

        foreach (RoomData combat in pool.combatRooms)
        {
            list.Add(combat);
        }

        // Fisher-Yates 셔플
        for (int i = list.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);

            RoomData temp = list[i];
            list[i] = list[j];
            list[j] = temp;
        }

        return list;
    }

    // 갈래 끝끼리 순차적으로 연결해서 뒤쪽 구조를 좁혀감
    // 새 합류 노드를 만들지는 않고, 기존 끝 노드를 이어 붙이는 방식
    private void MergeBranches(List<List<RoomNode>> branches)
    {
        List<RoomNode> currentEnds = new List<RoomNode>();

        foreach (List<RoomNode> branch in branches)
        {
            if (branch.Count > 0)
            {
                currentEnds.Add(branch[branch.Count - 1]);
            }
        }

        while (currentEnds.Count > 1)
        {
            List<RoomNode> nextEnds = new List<RoomNode>();

            for (int i = 0; i < currentEnds.Count - 1; i += 2)
            {
                currentEnds[i].ConnectTo(currentEnds[i + 1]);
                nextEnds.Add(currentEnds[i + 1]);
            }

            // 홀수 개 남으면 마지막 끝 노드를 직전 연결 그룹에 이어 붙임
            if (currentEnds.Count % 2 != 0 && nextEnds.Count > 0)
            {
                RoomNode last = currentEnds[currentEnds.Count - 1];
                nextEnds[nextEnds.Count - 1].ConnectTo(last);
                nextEnds.Add(last);
            }

            currentEnds = nextEnds;
        }
    }

    // 비어 있지 않은 갈래 중 가장 긴 갈래의 마지막 노드 반환
    private RoomNode GetLongestBranchEnd(List<List<RoomNode>> branches)
    {
        List<RoomNode> longest = null;

        foreach (List<RoomNode> branch in branches)
        {
            if (branch.Count == 0)
            {
                continue;
            }

            if (longest == null || branch.Count > longest.Count)
            {
                longest = branch;
            }
        }

        return longest[longest.Count - 1];
    }
}