using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class SkillUISlot : MonoBehaviour
{
	private static readonly int Stop = Animator.StringToHash("Stop");
	private static readonly int Start = Animator.StringToHash("Start");
	private Image _iconImage;
	private Image _coolTimeImage;
	private TextMeshProUGUI _ammoText;
	private Image _diceImage;

	// 쿨타임 시스템
	private float _maxCoolTime;
	private float _currentCoolTime;
	private bool _isCoolingDown;

	private Animator _animator;
	private Sprite _currentSlotIcon;
	private Tween _fadeTween;

	private void Awake()
	{
		_iconImage = transform.GetChild(0).GetComponent<Image>();
		_coolTimeImage = transform.GetChild(1).GetComponent<Image>();
		_ammoText = transform.GetChild(2).GetComponent<TextMeshProUGUI>();
		_diceImage = transform.GetChild(3).GetComponent<Image>();
		_animator = GetComponent<Animator>();
	}

	public void UpdateData(Sprite skillIcon, Sprite dice, int ammo)
	{
		_iconImage.sprite = skillIcon;
		_diceImage.sprite = dice;
		UpdateAmmo(ammo);

		_iconImage.transform.DOComplete();
		_iconImage.transform.DOPunchScale(Vector3.one * 0.2f, 0.4f);
		_diceImage.transform.DOComplete();
		_diceImage.transform.DOPunchScale(Vector3.one * 0.2f, 0.4f);
	}

	public void UpdateAmmo(int currentAmmo)
	{
		_ammoText.text = currentAmmo.ToString();
		_ammoText.transform.DOComplete();
		_ammoText.transform.DOScale(1.2f, 0.1f).OnComplete(() => _ammoText.transform.DOScale(1f, 0.1f));
	}

	public void StartCoolDown(float duration)
	{
		_maxCoolTime = duration;
		_currentCoolTime = duration;
		_isCoolingDown = true;

		if (_coolTimeImage != null) _coolTimeImage.gameObject.SetActive(true);
	}

	public void StartRolling()
	{
		_animator.SetTrigger(Start);
	}

	public void StopRolling()
	{
		_animator.SetTrigger(Stop);
	}

	private void Update()
	{
		if (!_isCoolingDown) return;

		_currentCoolTime -= Time.deltaTime;

		if (_currentCoolTime <= 0)
		{
			_isCoolingDown = false;
			_currentCoolTime = 0;
			_coolTimeImage.fillAmount = 0;
			_coolTimeImage.gameObject.SetActive(false);
		}
		else
		{
			_coolTimeImage.fillAmount = _currentCoolTime / _maxCoolTime;
		}
	}
}