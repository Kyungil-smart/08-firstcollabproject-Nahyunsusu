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

        _imageButton.interactable = true;

        _descriptionText.text = $"<b>{data.EquipName_KO}</b>";
              _priceText.text = $"{data.EquipPrice}";
                _currentPrice = data.EquipPrice;

        Debug.Log("장비 세팅됨");

        _imageButton.onClick.RemoveAllListeners();
        _imageButton.onClick.AddListener(() =>
        {
            var equipList = GameDataManager.instance.equipItemList;

            if (equipList.MyEquips.Count < 4)
            {
                Debug.Log($"{data.EquipName_KO} 바로 구매 및 추가");

                equipList.AddEquip(-1, data.EquipID);

                //MarkAsSold();
            }
            else
            {
                ReplaceSelectorUI.instance.Open();

                ReplaceSelectorUI.instance.onSlotSelected = (index) =>
                {
                    GameDataManager.instance.equipItemList.AddEquip(index, data.EquipID);

                    ReplaceSelectorUI.instance.onSlotSelected = null;
                };
                //MarkAsSold();
            }
        });
    }

    public void SetSkill(SkillDataSO skillDataSO, int diceValue = 1)
    {
        if (skillDataSO == null) return;

        SkillData data = skillDataSO.Get(diceValue);

        _descriptionText.text = $"<b>{data.SkillName}</b>";

        _priceText.text = $"{data.Price}";
          _currentPrice = data.Price;

        Debug.Log("스킬 세팅됨");

        //if (_image != null) _image.sprite = data.SkillImage;

        _imageButton.onClick.RemoveAllListeners();
        _imageButton.onClick.AddListener(() =>
        {
            Debug.Log($"{data.SkillName} 스킬 구매");

            MarkAsSold();
        });
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

    private void MarkAsSold()
    {
        _imageButton.interactable = false;

        if (_negoButton != null) _negoButton.interactable = false;

        _priceText.text = "<color=red>구매 완료</color>";
        Debug.Log("아이템 품절 처리됨");
    }
}
