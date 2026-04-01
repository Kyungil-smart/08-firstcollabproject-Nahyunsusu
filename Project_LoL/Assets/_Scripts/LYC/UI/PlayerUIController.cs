using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUIController : MonoBehaviour
{
	public PlayerController _controller;

	public Slider hpSlider;
	public Slider expSlider;

	public TextMeshProUGUI atkText;
	public TextMeshProUGUI atkSpeedText;
	public TextMeshProUGUI moveSpeedText;
	public TextMeshProUGUI critRateText;
	public TextMeshProUGUI critDamageText;

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
	}

	private void LateUpdate()
	{
		hpSlider.value = _controller.Health / (_controller.Data.HP + 0.001f);
		atkText.text = _controller.Data.AtkDamage.ToString();
		atkSpeedText.text = _controller.Data.AtkSpeed.ToString();
		moveSpeedText.text = _controller.Data.MoveSpeed.ToString();
		critRateText.text = _controller.Data.CritRate.ToString();
		critDamageText.text = _controller.Data.CritDamage.ToString();
	}
}