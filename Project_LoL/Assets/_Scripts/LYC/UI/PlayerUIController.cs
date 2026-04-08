using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUIController : MonoBehaviour
{
	public PlayerController _controller;

	[Header("Slider")]
	public Slider hpSlider;

	public Slider expSlider;
	public Slider dashSlider;

	[Header("Image")]
	public Image equipment1;

	public Image equipment2;
	public Image equipment3;
	public Image equipment4;

	[Header("Text")]
	public TextMeshProUGUI healthSliderText;

	public TextMeshProUGUI expSliderText;
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

		hpSlider.maxValue   = 1;
		expSlider.maxValue  = 1;
		dashSlider.maxValue = 1;

		_equipmentSlots = new[] { equipment1, equipment2, equipment3, equipment4 };
		foreach (var slot in _equipmentSlots)
			slot.enabled = false;

		_controller.EquipmentChanged += OnEquipmentChanged;
		_controller.HealthChanged    += RefreshHealth;
		_controller.ExpChanged       += RefreshExp;
		_controller.LevelChanged     += RefreshLevel;
		_controller.StatsChanged     += RefreshStats;
		_controller.GoldChanged      += RefreshGold;
		_controller.dashed.AddListener(RefreshDash);
	}

	private void RefreshDash()
	{
		StartCoroutine(DashRoutine());
	}

	private IEnumerator DashRoutine()
	{
		dashSlider.value = 0;
		float time = 0;
		while (time < _controller.Data.DashCooldown)
		{
			time             += Time.deltaTime;
			dashSlider.value =  time / _controller.Data.DashCooldown;
			yield return null;
		}

		dashSlider.value = 1;
	}

	private void Start()
	{
		if (_controller.Data == null)
		{
			StartCoroutine(LateUpdateRoutine());
			return;
		}

		RefreshHealth();
		RefreshExp();
		RefreshLevel();
		RefreshStats();
		RefreshGold();
	}

	private IEnumerator LateUpdateRoutine()
	{
		while (_controller.Data == null)
		{
			yield return null;
		}

		RefreshHealth();
		RefreshExp();
		RefreshLevel();
		RefreshStats();
		RefreshGold();
	}

	private void OnDestroy()
	{
		if (_controller == null) return;
		_controller.EquipmentChanged -= OnEquipmentChanged;
		_controller.HealthChanged    -= RefreshHealth;
		_controller.ExpChanged       -= RefreshExp;
		_controller.LevelChanged     -= RefreshLevel;
		_controller.StatsChanged     -= RefreshStats;
		_controller.GoldChanged      -= RefreshGold;
		_controller.dashed.RemoveListener(RefreshDash);
	}

	private void OnEquipmentChanged(int slot, EquipmentData equipment)
	{
		_equipmentSlots[slot].sprite  = equipment?.EquipIconSet.Get(equipment.CurrentUpgradeLevel);
		_equipmentSlots[slot].enabled = equipment != null;
	}

	private void RefreshHealth()
	{
		hpSlider.value        = _controller.Health / (_controller.Data.HP + 0.001f);
		healthSliderText.text = $"{_controller.Health:F0}/{_controller.Data.HP:F0}";
	}

	private void RefreshExp()
	{
		expSlider.value    = _controller.Exp / (float)(_controller.ExpReq == 0 ? 1 : _controller.ExpReq);
		expSliderText.text = $"{_controller.Exp:F0}/{_controller.ExpReq:F0}";
	}

	private void RefreshLevel() =>
		levelText.text = $"Lv. {_controller.Level:D2}";

	private void RefreshStats()
	{
		atkText.text        = $"{_controller.Data.AtkDamage}";
		atkSpeedText.text   = $"{_controller.Data.AtkSpeed}";
		moveSpeedText.text  = $"{_controller.Data.MoveSpeed}";
		critRateText.text   = $"{_controller.Data.CritRate}";
		critDamageText.text = $"{_controller.Data.CritDamage}";

		RefreshHealth();
		RefreshExp();
		RefreshLevel();
	}

	private void RefreshGold() =>
		goldText.text = $"{_controller.Gold}";
}