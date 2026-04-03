using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class TutorialManager : MonoBehaviour
{
	public enum TutorialQuestType
	{
		MoveAndDodgeDash,
		UseSkillSlot
	}

	[Serializable]
	public class TutorialQuest
	{
		public string questName;
		public TutorialQuestType type;
		public GameObject[] GameObjects;

		[Tooltip("DodgeDash / UseSkillSlot / HitEnemy 에서 완료에 필요한 횟수")]
		public int requiredCount = 1;

		[Tooltip("UseSkillSlot 타입일 때: 감지할 슬롯 인덱스 (0-based)")]
		public int skillSlotIndex;

		[HideInInspector] public int currentCount;
	}

	[Header("References")] [SerializeField] private PlayerController player;
	[SerializeField] private TextMeshProUGUI questText;
	[SerializeField] private TextMeshProUGUI questProgressText;
	[SerializeField] public TextMeshProUGUI questStepText;
	[SerializeField] private PlayerInputHandler inputHandler;
	[SerializeField] private PlayerSkillHandler skillHandler;
	[SerializeField] private DummyEnemy dummyEnemy1;
	[SerializeField] private DummyEnemy dummyEnemy2;

	[Header("Quests (순서대로 진행)")]
	[SerializeField] private List<TutorialQuest> quests = new();

	[Header("이벤트")] public UnityEvent<bool> onWPressed;
	public UnityEvent<bool> onAPressed;
	public UnityEvent<bool> onSPressed;
	public UnityEvent<bool> onDPressed;
	public UnityEvent<bool> onLeftShiftPressed;
	public UnityEvent onTutorialCompleted;

	private int _currentIndex;
	private int _moveAndDodgePhase;
	private int _lastSkillUsedSlot;

	public void NotifyDashDodge()
	{
		if (IsCurrentType(TutorialQuestType.MoveAndDodgeDash) && _moveAndDodgePhase == 1)
			IncrementAndCheck();
	}

	private void Start()
	{
		inputHandler.Moved += OnMoved;
		player.dashed.AddListener(OnDashed);
		skillHandler.SkillExecuted.AddListener(OnSkillExecuted);

		if (dummyEnemy1 != null)
			dummyEnemy1.OnHit += OnDummyHit;
		if (dummyEnemy2 != null)
			dummyEnemy2.OnHit += OnDummyHit;

		ActivateQuest(0);
	}

	private void OnDestroy()
	{
		if (inputHandler != null) inputHandler.Moved -= OnMoved;
		if (player != null) player.dashed.RemoveListener(OnDashed);
		if (skillHandler != null) skillHandler.SkillExecuted.RemoveListener(OnSkillExecuted);
		if (dummyEnemy1 != null) dummyEnemy1.OnHit -= OnDummyHit;
		if (dummyEnemy2 != null) dummyEnemy2.OnHit -= OnDummyHit;
	}

	private void OnMoved(Vector2 dir)
	{
		onWPressed.Invoke(dir.y > 0);
		onAPressed.Invoke(dir.x < 0);
		onSPressed.Invoke(dir.y < 0);
		onDPressed.Invoke(dir.x > 0);
		if (dir == Vector2.zero) return;
		if (!IsCurrentType(TutorialQuestType.MoveAndDodgeDash)) return;

		if (_moveAndDodgePhase == 0)
			_moveAndDodgePhase = 1;
	}

	private void OnDashed()
	{
		if (!IsCurrentType(TutorialQuestType.MoveAndDodgeDash)) return;

		onLeftShiftPressed.Invoke(true);
		Invoke(nameof(CancelLeftShift), 0.1f);
	}

	void CancelLeftShift()
	{
		onLeftShiftPressed.Invoke(false);
	}

	private void OnSkillExecuted(int slotIndex)
	{
		if (!IsCurrentType(TutorialQuestType.UseSkillSlot)) return;
		_lastSkillUsedSlot = slotIndex;
	}

	private void OnDummyHit(DummyEnemy e)
	{
		if (!IsCurrentType(TutorialQuestType.UseSkillSlot)) return;
		int targetSlot = quests[_currentIndex].skillSlotIndex;
		if (_lastSkillUsedSlot != targetSlot) return;

		if (targetSlot is 0 or 2 && e == dummyEnemy1)
		{
			if (quests[_currentIndex].currentCount == 4) e.TakeDown();
			IncrementAndCheck();
		}
		else if (targetSlot is 1 or 3 && e == dummyEnemy2)
		{
			if (quests[_currentIndex].currentCount == 4) e.TakeDown();
			IncrementAndCheck();
		}
	}

	private void IncrementAndCheck()
	{
		if (_currentIndex >= quests.Count) return;
		TutorialQuest quest = quests[_currentIndex];
		quest.currentCount++;
		questProgressText.text = $"({quests[_currentIndex].currentCount}/{quests[_currentIndex].requiredCount})";

		if (quest.currentCount >= quest.requiredCount)
		{
			CompleteCurrentQuest();
		}
	}

	private void CompleteCurrentQuest()
	{
		foreach (GameObject g in quests[_currentIndex].GameObjects)
		{
			g.SetActive(true);
		}

		_currentIndex++;
		if (_currentIndex >= quests.Count)
		{
			onTutorialCompleted?.Invoke();
			return;
		}

		ActivateQuest(_currentIndex);
	}

	private void ActivateQuest(int index)
	{
		foreach (GameObject g in quests.SelectMany(q => q.GameObjects))
		{
			g.SetActive(false);
		}

		if (index >= quests.Count)
		{
			onTutorialCompleted?.Invoke();
			return;
		}

		_moveAndDodgePhase = 0;
		foreach (var g in quests[index].GameObjects)
		{
			g.SetActive(true);
		}

		questText.text = quests[index].questName;
		questProgressText.text = $"({0}/{quests[index].requiredCount})";
		questStepText.text = $"STEP {index + 1}/{quests.Count}";

		if (IsCurrentType(TutorialQuestType.UseSkillSlot))
		{
			(quests[index].skillSlotIndex is 0 or 2 ? dummyEnemy1 : dummyEnemy2).targetIndex = quests[index].skillSlotIndex;
			(quests[index].skillSlotIndex is 0 or 2 ? dummyEnemy1 : dummyEnemy2).ChangeSkillIcon(quests[index].skillSlotIndex);
		}
	}

	private bool IsCurrentType(TutorialQuestType type)
	{
		return _currentIndex < quests.Count && quests[_currentIndex].type == type;
	}
}