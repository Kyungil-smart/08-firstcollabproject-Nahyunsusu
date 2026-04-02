using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUIController : MonoBehaviour
{
	public PlayerController _controller;

	public Slider hpSlider;
	public Slider expSlider;

	public Image equipment1;
	public Image equipment2;
	public Image equipment3;
	public Image equipment4;

	public TextMeshProUGUI levelText;
	public TextMeshProUGUI atkText;
	public TextMeshProUGUI atkSpeedText;
	public TextMeshProUGUI moveSpeedText;
	public TextMeshProUGUI critRateText;
	public TextMeshProUGUI critDamageText;
	public TextMeshProUGUI goldText;

	private Image[] _equipmentSlots;

	private void Awake()
	{
		if (_controller == null)
		{
			_controller = FindFirstObjectByType<PlayerController>();
			if (_controller == null)
			{
				enabled = false;
				return;
			}
		}

		hpSlider.maxValue = 1;
		expSlider.maxValue = 1;

		_equipmentSlots = new[] { equipment1, equipment2, equipment3, equipment4 };
		foreach (var slot in _equipmentSlots)
			slot.enabled = false;

		_controller.EquipmentChanged += OnEquipmentChanged;
	}

	private void OnDestroy()
	{
		if (_controller != null)
			_controller.EquipmentChanged -= OnEquipmentChanged;
	}

	private void OnEquipmentChanged(int slot, EquipmentData equipment)
	{
		_equipmentSlots[slot].sprite  = equipment?.EquipIconSet.Get(equipment.CurrentUpgradeLevel);
		_equipmentSlots[slot].enabled = equipment != null;
	}

	// 이벤트 기반해서 변경하도록 리팩토링하고 싶은데 시간이 없음
	private void LateUpdate()
	{
		hpSlider.value = _controller.Health / (_controller.Data.HP + 0.001f);
		expSlider.value = (_controller.Exp % 50) / 50.0f;
		levelText.text = $"Lv. {_controller.Level:D2}";
		atkText.text = $"{_controller.Data.AtkDamage}";
		atkSpeedText.text = $"{_controller.Data.AtkSpeed}";
		moveSpeedText.text = $"{_controller.Data.MoveSpeed}";
		critRateText.text = $"{_controller.Data.CritRate}";
		critDamageText.text = $"{_controller.Data.CritDamage}";
		goldText.text = $"{_controller.Gold}";
	}
}