using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class SkillUISlot : MonoBehaviour
{
	[Header("UI Components")] [SerializeField] private Image _iconImage;
	[SerializeField] private TextMeshProUGUI _ammoText;
	[SerializeField] private Image _coolTimeImage;

	// 쿨타임 시스템
	private float _maxCoolTime;
	private float _currentCoolTime;
	private bool _isCoolingDown;

	private Sprite _currentSlotIcon;
	private Tween _fadeTween;

	public void UpdateData(SkillData data)
	{
		if (data == null) return;
		_iconImage.sprite = data.SkillImage;
		_ammoText.text = data.MaxUseCount.ToString();
		_iconImage.transform.DOComplete();
		_iconImage.transform.DOPunchScale(Vector3.one * 0.2f, 0.4f, 10, 1f);

		// float targetAlpha = isSelectedSet ? 1.0f : 0.4f;
		// _iconImage.DOFade(targetAlpha, 0.25f).SetEase(Ease.OutCubic);
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