using System;
using Unity.Android.Gradle.Manifest;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

// 전투 시스템이나 이벤트 또는 플레이어 스탯 관련 기능 등 구현
public class PlayerController : MonoBehaviour
{
	[Header("Stats")] [field: SerializeField] public int Health { get; private set; } = 10;
	[field: SerializeField] public int Experience { get; private set; } = 10;

	[Header("Events")] [Tooltip("대응하는 State에서 Invoke 호출")] public UnityEvent hit;
	public UnityEvent dashed;
	public UnityEvent died;
	public UnityEvent<bool> invincibilityChanged;

	[Header("Debug")]
	public bool _showPlayerDebugMenu;

	public bool IsInvincible { get; private set; }

	private PlayerFSM _fsm;

	private void Awake()
	{
		_fsm = GetComponent<PlayerFSM>();
	}

	public void SetInvincible(bool enable)
	{
		IsInvincible = enable;
		invincibilityChanged.Invoke(enable);
	}

	public void OnHit(int damageAmount)
	{
		if (IsInvincible) return;

		Health -= damageAmount;
		if (Health <= 0)
		{
			_fsm.ChangeState(_fsm.Die);
			return;
		}

		_fsm.ChangeState(_fsm.Hit);
	}

	private void OnGUI()
	{
		if (!_showPlayerDebugMenu) return;
		
		if (GUILayout.Button("Reset Health", GUILayout.Height(70), GUILayout.Width(100)))
		{
			Health = 100;
		}

		if (GUILayout.Button("OnHit", GUILayout.Height(70), GUILayout.Width(100)))
		{
			OnHit(Random.Range(5, 15));
		}
	}

#if UNITY_EDITOR
	[ContextMenu("Debug: OnHit")]
	private void OnHit()
	{
		OnHit(1);
	}
#endif
}