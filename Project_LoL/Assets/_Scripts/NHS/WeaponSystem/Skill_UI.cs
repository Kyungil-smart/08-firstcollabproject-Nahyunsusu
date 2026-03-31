using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class Skill_UI : MonoBehaviour
{
    [Header("UI Components")]
    [SerializeField] private Image           _iconImage;
    [SerializeField] private TextMeshProUGUI  _ammoText;
    //[SerializeField] private GameObject _activeHighlight;

    // Cooltime UI
    [SerializeField] private Image _coolTimeImage;

    // 쿨타임 시스템

    private Skill  _assignedWeapon;
    private float     _maxCoolTime;
    private float _currentCoolTime;
    private bool    _isCoolingDown = false;
    public bool isCoolingDown => _isCoolingDown;

    private Sprite _currentSlotIcon;
    private Tween        _fadeTween; 

    public void UpdateUI(Skill weapon, bool isSelectedSet)
    {
        if (weapon == null) return;

        if (_currentSlotIcon != weapon.SkillIcon)
        {
            _currentSlotIcon = weapon.SkillIcon;
            _iconImage.sprite = _currentSlotIcon;

            _iconImage.transform.DOComplete();
            _iconImage.transform.DOPunchScale(Vector3.one * 0.2f, 0.4f, 10, 1f);
        }

        _ammoText.text = weapon.CurrentAmmo.ToString();

        //_activeHighlight?.SetActive(isSelectedSet);

        float targetAlpha = isSelectedSet ? 1.0f : 0.4f;
        _iconImage.DOFade(targetAlpha, 0.25f).SetEase(Ease.OutCubic);
    }

    public void UpdateAmmoOnly(int count)
    {
        _ammoText.text = count.ToString();
        _ammoText.transform.DOComplete();
        _ammoText.transform.DOScale(1.2f, 0.1f).OnComplete(() => _ammoText.transform.DOScale(1f, 0.1f));
    }

    public void StartCoolDown(float duration, Skill weapon)
    {
         _assignedWeapon = weapon;
            _maxCoolTime = duration;
        _currentCoolTime = duration;
          _isCoolingDown = true;

        if (_coolTimeImage != null) _coolTimeImage.gameObject.SetActive(true);
    }

    private void Update()
    {
        if (!_isCoolingDown) return;

        _currentCoolTime -= Time.deltaTime;

        if(_currentCoolTime <=0)
        {
                       _isCoolingDown = false;
                     _currentCoolTime = 0;
            _coolTimeImage.fillAmount = 0;

            if (_assignedWeapon != null)
            {
                _assignedWeapon.Reload();
                UpdateAmmoOnly(_assignedWeapon.CurrentAmmo);
            }
            _coolTimeImage.gameObject.SetActive(false);
        }
        else
        {
            _coolTimeImage.fillAmount = _currentCoolTime / _maxCoolTime;
        }
    }
}