using UnityEngine;
using UnityEngine.Events;

// 전투 시스템이나 이벤트 또는 플레이어 스탯 관련 기능 등 구현
public class PlayerController : MonoBehaviour
{
	public UnityEvent Hit;
	public UnityEvent Dashed;
	public UnityEvent Died;
}