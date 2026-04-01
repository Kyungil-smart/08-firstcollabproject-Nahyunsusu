using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUIController : MonoBehaviour
{
	public PlayerController _controller;

	public Slider hpSlider;
	public Slider expSlider;

	public TextMeshProUGUI levelText;
	public TextMeshProUGUI atkText;
	public TextMeshProUGUI atkSpeedText;
	public TextMeshProUGUI moveSpeedText;
	public TextMeshProUGUI critRateText;
	public TextMeshProUGUI critDamageText;
	public TextMeshProUGUI goldText;

	private void Awake()
	{
		if (_controller == null)
		{
			_controller = FindFirstObjectByType<PlayerController>();
			if (_controller == null)
			{
				enabled = false;
			}
		}

		hpSlider.maxValue = 1;
		expSlider.maxValue = 1;
	}

	// 이벤트 기반해서 변경하도록 리팩토링하고 싶은데 시간이 없음
	private void LateUpdate()
	{
		hpSlider.value = _controller.Health / (_controller.Data.HP + 0.001f);
		expSlider.value = _controller.Exp / 50.0f;
		levelText.text = $"Lv. {_controller.Level:D2}";
		atkText.text = $"{_controller.Data.AtkDamage}";
		atkSpeedText.text = $"{_controller.Data.AtkSpeed}";
		moveSpeedText.text = $"{_controller.Data.MoveSpeed}";
		critRateText.text = $"{_controller.Data.CritRate}";
		critDamageText.text = $"{_controller.Data.CritDamage}";
		goldText.text = $"{_controller.Gold}";
	}
}