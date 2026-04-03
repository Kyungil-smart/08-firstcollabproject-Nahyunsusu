using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StoreItemSlot : TooltipComponent
{
    [SerializeField] private Button _imageButton;

    [SerializeField] private TextMeshProUGUI       _priceText;
    [SerializeField] private TextMeshProUGUI _descriptionText;

    [SerializeField] private Button _negoButton;
    [SerializeField] private NegotiationSystem _negoSystem = new();
    private int _currentPrice;

    [SerializeField] private EquipInspector equipInspector;

    [SerializeField] private PlayerSkillHandler _skillHandler;

    public System.Action OnPurchaseSuccess;

    [SerializeField] private Sprite _healIcon;

    public override string GetTooltipText()
    {
        return $"아이템: {_descriptionText.text}\n가격: {_priceText.text}G";
    }

    public void SetItem(EquipmentData_SO data)
    {
        if (data == null) return;

        _imageButton.interactable = true;
        _imageButton.image.sprite = data.EquipImages.Get(0);

        _descriptionText.text = $"<b>{data.EquipName_KO}</b>";
              _priceText.text = $"{   data.EquipPrice}";
                _currentPrice =       data.EquipPrice;

        Debug.Log("장비 세팅됨");

        _imageButton.onClick.RemoveAllListeners();
        _imageButton.onClick.AddListener(() =>
        {
            var equipList = GameDataManager.instance.equipItemList;

            if (equipList.MyEquips.Count < 4)
            {
                Debug.Log($"{data.EquipName_KO} 바로 구매 및 추가");

                equipList.AddEquip(-1, data.EquipID);

                MarkAsSold();
            }
            else
            {
                ReplaceSelectorUI.instance.Open();

                ReplaceSelectorUI.instance.onSlotSelected = (index) =>
                {
                    GameDataManager.instance.equipItemList.AddEquip(index, data.EquipID);

                    ReplaceSelectorUI.instance.onSlotSelected = null;
                };
                MarkAsSold();
            }

            equipInspector.RefreshUI();
        });
    }

    public void SetSkill(SkillDataSO skillDataSO, int diceValue = 1)
    {
        if (skillDataSO == null) return;

        SkillData data = skillDataSO.Get(diceValue);

        _imageButton.interactable = true;
        _imageButton.image.sprite = data.SkillImage;

        _descriptionText.text = $"<b>{data.SkillName}</b>";

        _priceText.text = $"{data.Price}";
          _currentPrice = data.Price;

        Debug.Log("스킬 세팅됨");

        _imageButton.onClick.RemoveAllListeners();
        _imageButton.onClick.AddListener(() =>
        {
            int emptyIndex = -1;
            for (int i = 0; i < _skillHandler.Skills.Length; i++)
            {
                if (_skillHandler.Skills[i] == null)
                {
                    emptyIndex = i;
                    break;
                }
            }

            if (emptyIndex != -1)
            {
                _skillHandler.SetSkill(skillDataSO, emptyIndex, diceValue);
                MarkAsSold();
            }
            else
            {
                ReplaceSelectorUI.instance.Open();
                ReplaceSelectorUI.instance.onSlotSelected = (index) =>
                {
                    _skillHandler.SetSkill(skillDataSO, index, diceValue);

                    MarkAsSold();
                    ReplaceSelectorUI.instance.onSlotSelected = null;
                };
            }
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

        _priceText.text = "<color=red>매진</color>";
        _priceText.fontSize = 20;

        OnPurchaseSuccess?.Invoke();

        Debug.Log("아이템 품절 처리됨");
    }

    public void SetHealItem()
    {
        int healAmount = DiceSystem.instance.RollDice();
        int price = 20;

        _imageButton.interactable = true;
        if (_healIcon != null) 
            _imageButton.image.sprite = _healIcon;

        _descriptionText.text = $"<b>회복약</b>";
              _priceText.text = $"{price}";
                _currentPrice = price;

        _imageButton.onClick.RemoveAllListeners();
        _imageButton.onClick.AddListener(() =>
        {
            var player = GameObject.FindWithTag("Player")?.GetComponent<PlayerController>();
            if (player != null)
            {
                Debug.Log("플레이어 찾음");
                player.Health += healAmount;

                if (player.Health > player.Data.HP)
                    player.Health = player.Data.HP;

                Debug.Log($"체력 {healAmount} 회복됨! 현재 체력: {player.Health}");
                MarkAsSold();
            }
            else
            {
                Debug.Log("플레이어 찾기 못함");
            }
        });
    }

    
}
