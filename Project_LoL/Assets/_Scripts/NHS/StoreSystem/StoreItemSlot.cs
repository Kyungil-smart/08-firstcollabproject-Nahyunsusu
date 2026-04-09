using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StoreItemSlot : TooltipComponent
{
    [SerializeField] private Button _imageButton;

    [SerializeField] private TextMeshProUGUI       _priceText;
    [SerializeField] private TextMeshProUGUI _descriptionText;

    [SerializeField] private string _description;


    [SerializeField] private Button _negoButton;
    [SerializeField] private NegotiationSystem _negoSystem = new();
    private int _currentPrice;

    [SerializeField] private EquipInspector equipInspector;

    [SerializeField] private PlayerSkillHandler _skillHandler;
    [SerializeField] private PlayerController   _playerController;

    public System.Action OnPurchaseSuccess;

    [SerializeField] private Sprite _healIcon;

    private void OnEnable()
    {
        if (_playerController == null || _skillHandler == null)
        {
            GameObject playerObj = GameObject.FindWithTag("Player");

            if (playerObj != null)
            {
                if (_playerController == null)
                    _playerController = playerObj.GetComponent<PlayerController>();

                if (_skillHandler == null)
                    _skillHandler = playerObj.GetComponent<PlayerSkillHandler>();
            }
            else
            {
                Debug.LogError("StoreItemSlot: 'Player' 태그를 가진 오브젝트를 찾을 수 없습니다!");
            }
        }
    }

    public override string GetTooltipText()
    {
        return $"아이템: {_description}\n가격: {_priceText.text}G\n";
    }

    public void SetItem(EquipmentData_SO data)
    {
        if (data == null) return;

        _imageButton.interactable = true;
        _imageButton.image.sprite = data.EquipImages.Get(0);

        if (LanguageManager.Instance.Current == Language.Korean)
        {
            _descriptionText.text = $"<b>{data.EquipName_KO}</b>";
            _description = $"<b>{data.EquipName_KO}\n{data.EquipText}</b>";
        }
        else
        {
            _descriptionText.text = $"<b>{data.EquipName_EN}</b>";
        }

        _priceText.text = $"{data.EquipPrice}";
        _currentPrice   =    data.EquipPrice;

        _imageButton.onClick.RemoveAllListeners();
        _imageButton.onClick.AddListener(() =>
        {
            if (_playerController == null)
            {
                Debug.LogError("플레이어를 찾을 수 없습니다!");
                return;
            }

            if (_playerController.Gold < _currentPrice)
            {
                Debug.Log("골드가 부족합니다.");
                return;
            }

            _playerController.Gold -= _currentPrice;

            var equipList = GameDataManager.instance.equipItemList;

            if (equipList.MyEquips.Count < 4)
            {
                Debug.Log($"{data.EquipName_KO} 바로 구매 및 추가");

                equipList.AddEquip(-1, data.EquipID);

                MarkAsSold();
            }
            else
            {
                ReplaceSelectorUI.instance.Open(false);

                ReplaceSelectorUI.instance.onSlotSelected = (index) =>
                {
                    GameDataManager.instance.equipItemList.AddEquip(index, data.EquipID);

                    ReplaceSelectorUI.instance.onSlotSelected = null;
                };

                MarkAsSold();
            }

            equipInspector.RefreshUI();

            equipInspector.RefreshUI(false);
        });
    }

    public void SetSkill(SkillDataSO skillDataSO, int diceValue = 1)
    {
        if (_playerController == null)
        {
            Debug.LogError("플레이어를 찾을 수 없습니다!");
            return;
        }

        if (_playerController.Gold < _currentPrice)
        {
            Debug.Log("골드가 부족합니다.");
            return;
        }

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
            if (_playerController == null) return;

            if (_playerController.Gold < _currentPrice) return;

            _playerController.Gold -= _currentPrice;

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
                ReplaceSelectorUI.instance.Open(true);
                ReplaceSelectorUI.instance.onSlotSelected = (index) =>
                {
                    _skillHandler.SetSkill(skillDataSO, index, diceValue);

                    Mathf.RoundToInt(skillDataSO.Get(1).Price * 0.3f);
                    MarkAsSold();
                    ReplaceSelectorUI.instance.onSlotSelected = null;
                };

                equipInspector.RefreshUI(true);
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
            if (_playerController == null)
            {
                Debug.LogError("플레이어를 찾을 수 없습니다!");
                return;
            }

            if (_playerController.Gold < _currentPrice)
            {
                Debug.Log("골드가 부족합니다.");
                return;
            }

            _playerController.Gold -= _currentPrice;
            _playerController.Health += healAmount * 10;
            
            if (_playerController.Health > _playerController.Data.HP)
                _playerController.Health = _playerController.Data.HP;
            
            MarkAsSold();
        });
    }
}
