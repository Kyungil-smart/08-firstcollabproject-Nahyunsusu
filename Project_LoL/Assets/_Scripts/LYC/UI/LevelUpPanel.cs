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

	public void OnButtonClicked()
	{
		DOTween.Sequence()
			.Append(transform.DOPunchScale(Vector3.one * -0.15f, 0.25f, 5, 0.5f))
			.Join(transform.DOPunchPosition(Vector3.up * 8f, 0.25f, 5, 0.5f))
			.SetUpdate(true)
			.OnComplete(() => Clicked?.Invoke());
	}
}