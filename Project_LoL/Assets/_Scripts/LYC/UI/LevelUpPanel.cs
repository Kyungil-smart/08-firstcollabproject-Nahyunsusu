using System;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LevelUpPanel : MonoBehaviour
{
	public Action Clicked;
	public Image diceImage;
	public Image statImage;
	public TextMeshProUGUI titleText;
	public TextMeshProUGUI descriptionText;

	private Button _button;

	private void Awake()
	{
		_button = GetComponent<Button>();
	}

	private void OnEnable()
	{
		transform.localScale = Vector3.one;
		_button.onClick.AddListener(OnButtonClicked);
		_button.interactable = true;
	}

	private void OnDisable()
	{
		_button.onClick.RemoveListener(OnButtonClicked);
		_button.interactable = false;
	}

	void OnButtonClicked()
	{
		_button.interactable = false;
		transform.DOScale(Vector3.one * 0.7f, 0.1f).SetEase(Ease.InOutExpo).SetUpdate(true).OnComplete(() => Clicked?.Invoke());
	}
}