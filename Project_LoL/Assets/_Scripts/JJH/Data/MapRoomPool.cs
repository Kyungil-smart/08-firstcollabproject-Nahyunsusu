using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewMapRoomPool", menuName = "Map/MapRoomPool")]
public class MapRoomPool : ScriptableObject
{
    public RoomData startData;
    public RoomData bossData;
    public List<RoomData> combatRooms;
}