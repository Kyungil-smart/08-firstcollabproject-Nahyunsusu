using UnityEngine;
using System.Collections.Generic;
using System;
using TMPro;
using Unity.VisualScripting;
using UnityEngine.UI;

public class StoreUI : MonoBehaviour
{
    [SerializeField] private List<StoreItemSlot> _storeItems = new List<StoreItemSlot>(4);

    [SerializeField] private List<SkillDataSO>   _haveSkillList = new List<SkillDataSO>();
    [SerializeField] private List<Button>      _skillButtonList = new List<Button>();

    private bool _isInitialized = false;

    [SerializeField] private EquipInspector _itemInspector;

    [Header("Player")]
    [SerializeField] private PlayerSkillHandler     _skillHandler;
    [SerializeField] private PlayerController   _playerController;

    [SerializeField] private TextMeshProUGUI _curGold;

    private void Awake()
    {
        this.gameObject.SetActive(false);
    }

    private void Start()
    {
        if (GameDataManager.instance != null)
        {
            GameDataManager.instance.equipDataManager.OnDataLoaded += RefreshItem;
        }
    }

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
                Debug.LogError("StoreUI: 'Player' 태그를 가진 오브젝트를 찾을 수 없습니다!");
            }
        }

        if (GameDataManager.instance != null && GameDataManager.instance.equipDataManager.IsLoaded)
        {
            if (!_isInitialized)
            {
                RefreshItem();
                _isInitialized = true;
                Debug.Log("상점 물건 신규 입고 완료!");
            }
            else
            {
                RefreshSkillUI();
                Debug.Log("이전 상점 물건 목록을 유지합니다.");
            }
        }

        if (_playerController != null)
        {
            UpdateGoldText();

            _playerController.GoldChanged -= UpdateGoldText;
            _playerController.GoldChanged += UpdateGoldText;
        }
    }

    public void OnDisable()
    {
        _playerController.GoldChanged -= UpdateGoldText;
    }

    private void OnDestroy()
    {
        if (GameDataManager.instance != null)
        {
            GameDataManager.instance.equipDataManager.OnDataLoaded -= RefreshItem;
        }
    }

    public void RefreshItem()
    {
        if (this == null || gameObject == null) return;
        if (GameDataManager.instance == null) return;

        Debug.Log("상점 아이템 & 스킬 세팅 시작");

        // 1. 장비 아이템 세팅
        var selectedEquips = GameDataManager.instance.equipDataManager.GetRandomEquips(2);
        if (selectedEquips != null && selectedEquips.Count >= 2)
        {
            _storeItems[0].SetItem(selectedEquips[0]);
            _storeItems[1].SetItem(selectedEquips[1]);
        }

        // 2. 스킬 아이템 세팅
        var skillManager = GameDataManager.instance.skillDataManager;
        var selectedSkills = skillManager.GetRandomSkills(2);
        if (selectedSkills != null && selectedSkills.Count >= 1)
        {
            _storeItems[2].SetSkill(selectedSkills[0], 1);
        }
        else
        {
            Debug.LogWarning("스킬 데이터를 뽑아오지 못했습니다.");
        }

        // 3. 회복 아이템 세팅
        if (_storeItems.Count > 3 && _storeItems[3] != null)
        {
            _storeItems[3].gameObject.SetActive(true);

            _storeItems[3].SetHealItem();
        }

        foreach (var slot in _storeItems)
        {
            if (slot == null) continue;

            slot.OnPurchaseSuccess  = null;
            slot.OnPurchaseSuccess += RefreshSkillUI;
        }

        RefreshSkillUI();
    }

    public void RefreshSkillUI()
    {
        if (_haveSkillList == null) 
        { 
            _haveSkillList = new List<SkillDataSO>(); 
        }

        else 
        { 
            _haveSkillList.Clear(); 
        }

        for (int i = 0; i < _skillHandler.Skills.Length; i++)
        {
            var executor = _skillHandler.Skills[i];

            if (executor != null && executor.SkillDataSO != null)
            {
                SkillDataSO currentSO = executor.SkillDataSO;
                _haveSkillList.Add(currentSO);

                if (i < _skillButtonList.Count && _skillButtonList[i] != null)
                {
                    _skillButtonList[i].gameObject.SetActive(true);
                    _skillButtonList[i].image.sprite = currentSO.Get(1).SkillImage;

                    var tooltipData = _skillButtonList[i].GetComponent<SkillTooltipData>();

                    if (tooltipData == null) 
                        tooltipData = _skillButtonList[i].gameObject.AddComponent<SkillTooltipData>();

                    tooltipData.Setup(currentSO);

                    var trigger = _skillButtonList[i].GetComponent<TooltipTrigger>();
                    if (trigger == null) 
                        trigger = _skillButtonList[i].gameObject.AddComponent<TooltipTrigger>();
                }
                //Debug.Log($"{i}번 슬롯 스킬 세팅됨: {currentSO.name}");
            }
            else
            {
                if (i < _skillButtonList.Count && _skillButtonList[i] != null)
                    _skillButtonList[i].gameObject.SetActive(false);

                //Debug.Log($"{i}번 슬롯 비어있음");
            }
        }
    }

    private void UpdateGoldText()
    {
        if (_curGold != null && _playerController != null)
        {
            _curGold.text = $"{_playerController.Gold}";
        }
    }
}