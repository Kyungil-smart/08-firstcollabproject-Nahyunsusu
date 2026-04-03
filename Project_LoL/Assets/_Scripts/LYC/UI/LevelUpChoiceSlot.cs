using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LevelUpChoiceSlot : MonoBehaviour
{
	[SerializeField] private TMP_Text nameText;
	[SerializeField] private Image iconImage;

	/// <summary>
	/// 슬롯 클릭 시 선택된 항목을 전달하는 이벤트.
	/// Button의 OnClick에서 <see cref="OnClick"/>을 호출하도록 연결합니다.
	/// </summary>
	public event Action<LevelUpChoiceEntry> Clicked;

	private LevelUpChoiceEntry _entry;

	public void Setup(LevelUpChoiceEntry entry)
	{
		_entry = entry;
		nameText.text = entry.displayName;
		iconImage.sprite = entry.icon;
		GetComponent<Button>().interactable = true;
	}

	/// <summary>
	/// Button의 OnClick 이벤트에 연결합니다.
	/// </summary>
	public void OnClick()
	{
		GetComponent<Button>().interactable = false;
		Clicked?.Invoke(_entry);
	}
}
