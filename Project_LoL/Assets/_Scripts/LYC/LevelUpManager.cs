using System.Collections.Generic;
using _Scripts.LYC.Data;
using _Scripts.LYC.Skill;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

/// <summary>
/// 레벨업 시 주사위를 굴려 선택지를 제공하는 싱글톤 매니저.
/// 주사위 1~2: 1회 선택 / 3~4: 2회 선택 / 5~6: 3회 선택
/// </summary>
public class LevelUpManager : MonoBehaviour
{
	public static LevelUpManager Instance { get; private set; }

	[SerializeField] private LevelUpChoiceListSO choiceList;
	[SerializeField] private LevelUpPanel[] panels = new LevelUpPanel[3];
	[SerializeField] private Sprite[] diceImages = new Sprite[6];
	[SerializeField] private Image selectionDice;

	private PlayerController _player;
	private int _remainingSelections;

	private void Awake()
	{
		if (Instance != null && Instance != this)
		{
			Destroy(gameObject);
			return;
		}

		Instance = this;
		DontDestroyOnLoad(gameObject);
	}

	public void RegisterPlayer(PlayerController player)
	{
		_player = player;
		player.LevelUp += HandleLevelUp;
	}

	public void UnregisterPlayer(PlayerController player)
	{
		player.LevelUp -= HandleLevelUp;
		_player = null;
	}

	private void HandleLevelUp()
	{
		int dice = SkillExecutor.Roll();
		int selectCount = dice switch
		{
			1 or 2 => 1,
			3 or 4 => 2,
			_ => 3
		};

		Time.timeScale = 0f;
		SetChildrenActive(true);

		var choices = PickChoices(3);
		OpenPanels(choices, selectCount, dice);
	}

	private void OpenPanels(List<LevelUpChoiceEntry> choices, int selectCount, int dice)
	{
		_remainingSelections = selectCount;

		Sprite diceSprite = diceImages[dice - 1];
		selectionDice.sprite = diceSprite;

		for (int i = 0; i < panels.Length; i++)
		{
			LevelUpChoiceEntry entry = choices[i];
			LevelUpPanel panel = panels[i];

			panel.diceImage.sprite = diceImages[Mathf.Clamp(entry.spawnWeight, DiceConst.MIN, DiceConst.MAX) - 1];
			panel.titleText.text = entry.displayName;
			panel.statImage.sprite = entry.icon;
			panel.descriptionText.text = entry.BuildDescription();

			// 이전 구독 초기화 후 재등록 (클로저로 entry 캡처)
			panel.Clicked = null;
			panel.Clicked += () => OnPanelClicked(entry);
		}
	}

	private void OnPanelClicked(LevelUpChoiceEntry entry)
	{
		ApplyStat(entry);
		_remainingSelections--;

		if (_remainingSelections > 0) return;

		foreach (var panel in panels)
			panel.Clicked = null;

		SetChildrenActive(false);
		Time.timeScale = 1f;
		_player.KnockBack();
	}

	private void ApplyStat(LevelUpChoiceEntry entry)
	{
		_player.AddBaseStat(entry);
	}

	private void SetChildrenActive(bool active)
	{
		foreach (Transform child in transform)
			child.gameObject.SetActive(active);
	}

	/// <summary>
	/// 등장 가중치(spawnWeight) 기반으로 중복 없이 count개 선택지를 뽑습니다.
	/// </summary>
	private List<LevelUpChoiceEntry> PickChoices(int count)
	{
		if (choiceList == null || choiceList.Entries.Count == 0)
		{
			Debug.LogWarning($"[{nameof(LevelUpManager)}] choiceList가 비어있거나 할당되지 않음");
			return new List<LevelUpChoiceEntry>();
		}

		var pool = new List<LevelUpChoiceEntry>(choiceList.Entries);
		var result = new List<LevelUpChoiceEntry>();
		int pickCount = Mathf.Min(count, pool.Count);

		for (int i = 0; i < pickCount; i++)
		{
			int totalWeight = 0;
			foreach (var e in pool)
				totalWeight += e.spawnWeight;

			int roll = Random.Range(0, totalWeight);
			int cumulative = 0;

			foreach (var e in pool)
			{
				cumulative += e.spawnWeight;
				if (roll < cumulative)
				{
					result.Add(e);
					pool.Remove(e);
					break;
				}
			}
		}

		return result;
	}
}