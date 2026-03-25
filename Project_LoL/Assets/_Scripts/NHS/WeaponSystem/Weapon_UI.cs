using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class Weapon_UI : MonoBehaviour
{
    [Header("UI Components")]
    [SerializeField] private Image _iconImage;
    [SerializeField] private TextMeshProUGUI _ammoText;
    //[SerializeField] private GameObject _activeHighlight;

    private Sprite _currentSlotIcon;
    private Tween _fadeTween; 

    public void UpdateUI(Weapon weapon, bool isSelectedSet)
    {
        if (weapon == null) return;

        if (_currentSlotIcon != weapon.WeaponIcon)
        {
            _currentSlotIcon = weapon.WeaponIcon;
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
}