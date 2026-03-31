using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StoreItemSlot : MonoBehaviour
{
    [SerializeField] private Button _imageButton;

    [SerializeField] private TextMeshProUGUI       _priceText;
    [SerializeField] private TextMeshProUGUI _descriptionText;

    [SerializeField] private Button _negoButton;
    [SerializeField] private NegotiationSystem _negoSystem = new();
    private int _currentPrice;

    private void OnMouseEnter()
    {
        Debug.Log("마우스가 버튼 영역에 들어옴");
    }

    public void SetItem(EquipmentData_SO data)
    {
        if (data == null) return;

        _descriptionText.text = $"<b>{data.EquipName}</b>";

        _priceText.text = $"{data.EquipPrice}";
        _currentPrice = data.EquipPrice;

        Debug.Log("장비 세팅됨");

        _imageButton.onClick.RemoveAllListeners();
        _imageButton.onClick.AddListener(() => Debug.Log($"{data.EquipName} 구매"));
    }

    public void SetSkill(SkillDataSO skillDataSO, int diceValue = 1)
    {
        if (skillDataSO == null) return;

        SkillData data = skillDataSO.Get(diceValue);

        _descriptionText.text = $"<b>{data.SkillName}</b>\n{data.SkillDescription}";

        _priceText.text = $"{data.Price}";
        _currentPrice = data.Price;

        //if (_image != null) _image.sprite = data.SkillImage;

        _imageButton.onClick.RemoveAllListeners();
        _imageButton.onClick.AddListener(() => Debug.Log($"{data.SkillName} 스킬 구매"));
    }

    public void OnClickNegotiate()
    {
        Debug.Log("네고 버튼 클릭됨!");
        NegotiatePrice();
    }

    private void NegotiatePrice()
    {
        if (_negoSystem.SetTable())
        {
            _currentPrice = _negoSystem.DecreasePrice(_currentPrice);
            Debug.Log("협상 성공!");
        }
        else
        {
            _currentPrice = _negoSystem.IncreasePrice(_currentPrice);
            Debug.Log("협상 실패!");
        }

        _priceText.text = _currentPrice.ToString();

        if (_negoButton != null) 
            _negoButton.interactable = false;
    }
}
