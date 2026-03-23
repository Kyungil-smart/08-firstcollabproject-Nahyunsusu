using System.Collections.Generic;
using UnityEngine;

/// 맵 생성 시 사용하는 방 데이터 모음
// 난이도 곡선 및 진행 흐름을 제어하기 위한 데이터 세트
// MapGraph에서 노드를 생성할 때 참조
[CreateAssetMenu(fileName = "NewMapRoomPool", menuName = "Map/MapRoomPool")]
public class MapRoomPool : ScriptableObject
{
    public RoomData startData;     // 시작 노드로 고정 배치
    
    public RoomData upgradeData;   // 보스 직전 강화 구간에 사용
    
    public RoomData bossData;      // 마지막 노드 (목표 지점)
    
    public RoomData shopData;      // 중간 경로에 1회 포함
    
    public List<RoomData> combatRooms; // 일반 전투 방 후보 (랜덤 배치)
}