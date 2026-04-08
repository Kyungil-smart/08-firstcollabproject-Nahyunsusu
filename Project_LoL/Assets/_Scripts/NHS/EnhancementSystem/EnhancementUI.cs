using _Scripts.LYC.Utils;
using DG.Tweening;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

enum EnhancePrice
{
    Level1 = 5,
    Level2 = 10,
    Level3 = 15,
    Level4 = 20,
    Level5 = 30,
    Level6 = 40
}

public class EnhancementUI : MonoBehaviour
{
    [SerializeField] private List<Button>  _equipmentButtons = new List<Button>();
    [SerializeField] private List<Image>   _equipImages;
    [SerializeField] private List<Image>  _selectedImages = new List<Image>(4);

    [SerializeField] private Button     _oddButton;
    [SerializeField] private Button    _evenButton;
    [SerializeField] private Button _enhanceButton;

    [SerializeField] private EquipmentList _equipmentList;

    [SerializeField] private Image _selectedImage;

    [Header("Current Selection")]
    private int   _selectedIndex = -1;
    private int _selectedOddEven = -1;

    [Header("Dice Animation Settings")]
    [SerializeField] private List<Image> _diceImage;    // 3개의 주사위 결과 이미지
    [SerializeField] private List<Animator> _animator;  // 3개의 주사위 애니메이터 (GIF 연출용)
    [SerializeField] private Sprite[] _diceSprites;     // 주사위 1~6번 눈 스프라이트
    [SerializeField] private TextMeshProUGUI sum;     // 주사위 1~6번 눈 스프라이트

    private static readonly int _stop  = Animator.StringToHash("Stop");
    private static readonly int _start = Animator.StringToHash("Start");

    private float _rollDuration = 1.5f; // 주사위가 실제로 굴러갈 시간
    private float _timer = 0f;
    private bool _isRolling = false;
    
    [Header("EnhancePriceSetting")]
    private int[] _enhancePrices = { 5, 10, 15, 20, 30, 40 };
    private int[] _enhanceSum = { 4, 5, 6, 8, 10, 12 };
    [SerializeField] private PlayerController _playerController;


    private void Awake()
    {
        this.gameObject.SetActive(false);
    }

    private void Start()
    {
        _enhanceButton.interactable = false;
        _enhanceButton.onClick.AddListener(ExecuteEnhancement);

        for (int i = 0; i < _equipmentButtons.Count; i++)
        {
            int index = i; 
            _equipmentButtons[i].onClick.AddListener(() => OnSelectEquipment(index));
        }

         _oddButton.onClick.AddListener(() => OnSelectOddEven(1));
        _evenButton.onClick.AddListener(() => OnSelectOddEven(0));

        for (int i = 0; i < _selectedImages.Count; i++)
        {
            _selectedImages[i].gameObject.SetActive(false);
        }
    }

    private void OnEnable()
    {
        if (_equipmentList != null)
        {
            _equipmentList.OnEquipChanged += RefreshEquipIcons;
        }

        if (_playerController == null)
        {
            GameObject playerObj = GameObject.FindWithTag("Player");

            if (playerObj != null)
            {
                if (_playerController == null)
                    _playerController = playerObj.GetComponent<PlayerController>();
            }
            else
            {
                Debug.LogError("StoreUI: 'Player' 태그를 가진 오브젝트를 찾을 수 없습니다!");
            }
        }

        RefreshEquipIcons();
        ResetSelection();   
    }

    private void Update()
    {
        if (!_isRolling) return;

        _timer -= Time.deltaTime;

        for (int i = 0; i < _diceImage.Count; i++)
        {
            if (_diceImage[i] != null && _diceSprites.Length > 0)
            {
                int tempIndex = UnityEngine.Random.Range(0, _diceSprites.Length);
                _diceImage[i].sprite = _diceSprites[tempIndex];
            }
        }

        if (_timer <= 0)
        {
            StopRollingAndCheckResult();
        }
    }

    private void OnDisable()
    {
        if (_equipmentList != null)
        {
            _equipmentList.OnEquipChanged -= RefreshEquipIcons;
        }
    }

    public void ExecuteEnhancement()
    {
        int currentLevel = _equipmentList.MyEquips[_selectedIndex].CurrentUpgradeLevel;
        int cost = _enhancePrices[Mathf.Clamp(currentLevel, 0, _enhancePrices.Length - 1)];

        if (_playerController.Gold < cost)
        {
            Debug.Log("골드가 부족합니다!");
            return;
        }

        _playerController.Gold -= cost;

        Debug.Log("주사위 굴리기 시작!");
        _enhanceButton.interactable = false;

        _timer = _rollDuration;
        _isRolling = true;

        foreach (var anim in _animator)
        {
            if (anim != null)
            {
                anim.enabled = true;
                anim.SetTrigger(_start);
            }
        }
    }

    private void StopRollingAndCheckResult()
    {
        _isRolling = false;
        int totalSum = 0;

        int currentLevel = _equipmentList.MyEquips[_selectedIndex].CurrentUpgradeLevel;

        int levelIndex = Mathf.Clamp(currentLevel, 0, _enhanceSum.Length - 1);

        for (int i = 0; i < _animator.Count; i++)
        {
            if (_animator[i] != null) _animator[i].SetTrigger(_stop);

            int diceValue = UnityEngine.Random.Range(1, 7);
            totalSum += diceValue;

            if (i < _diceImage.Count && _diceSprites.Length >= 6)
            {
                if (_animator[i] != null) _animator[i].enabled = false;

                _diceImage[i].sprite = _diceSprites[diceValue - 1];
                _diceImage[i].transform.DOComplete();
                _diceImage[i].transform.DOPunchScale(Vector3.one * 0.2f, 0.4f);
            }
        }

        bool isSumEnough = (totalSum >= _enhanceSum[levelIndex]);

        if (isSumEnough)
        {
            _equipmentList.UpgradeEquipment(_selectedIndex);
        }

        ResetSelection();
        RefreshEquipIcons();
    }

    public void RefreshEquipIcons(bool isSkillMode = false)
    {
        for (int i = 0; i < _equipImages.Count; i++)
        {
            if (i < _equipmentList.MyEquips.Count)
            {
                var data = _equipmentList.MyEquips[i];
                _equipImages[i].sprite = data.EquipIconSet.Get(data.CurrentUpgradeLevel);
                _equipImages[i].enabled = true;

                _equipmentButtons[i].interactable = true;
            }
            else
            {
                     _equipImages[i].enabled = false;
                _equipmentButtons[i].interactable = false;

                _selectedImages[i].gameObject.SetActive(false);
            }
        }
    }

    private void OnSelectEquipment(int index)
    {
        if (index > _equipmentList.MyEquips.Count) 
            return;

        _selectedIndex = index;
        var selectedData = _equipmentList.MyEquips[index];

        Debug.Log($"{index}번 무기 선택됨");

        for(int i=0;i<_selectedImages.Count;i++)
        {
            if (_selectedImages[i] == null) continue;

            if (i==index && _equipmentList.MyEquips[i].CurrentUpgradeLevel < 6)
            {
                _selectedImages[i].gameObject.SetActive(true);
            }
            else
            {
                _selectedImages[i].gameObject.SetActive(false);
            }
        }

        if (_selectedImage != null)
        {
            _selectedImage.sprite = selectedData.EquipIconSet.Get(selectedData.CurrentUpgradeLevel);
            _selectedImage.enabled = true;
        }

        UpdateTargetSumText(index);
        CheckRequirement();
    }

    private void OnSelectOddEven(int value)
    {
        _selectedOddEven = value;
        Debug.Log(_selectedOddEven == 1 ? "홀 선택" : "짝 선택");

        CheckRequirement();
    }

    private void CheckRequirement()
    {
        if (_selectedIndex != -1 && _selectedOddEven != -1)
        {
            _enhanceButton.interactable = true;
        }
    }

    private void ResetSelection()
    {
          _selectedIndex = -1;
        _selectedOddEven = -1;

        _enhanceButton.interactable = false;

        for (int i = 0; i < _selectedImages.Count; i++)
        {
            if (_selectedImages[i] != null)
                _selectedImages[i].gameObject.SetActive(false);
        }

        if (_selectedImage != null)
        {
            _selectedImage.enabled = false;
            _selectedImage.sprite  = null;
        }

        UpdateTargetSumText(-1);
    }
    private void UpdateTargetSumText(int index)
    {
        if (sum == null) return;

        if (index == -1 || index >= _equipmentList.MyEquips.Count)
        {
            sum.text = LanguageManager.Instance.Current == Language.Korean ? "목표: -" : "Target: -";
            return;
        }

        int currentLevel = _equipmentList.MyEquips[index].CurrentUpgradeLevel;
        int levelIndex = Mathf.Clamp(currentLevel, 0, _enhanceSum.Length - 1);
        int targetValue = _enhanceSum[levelIndex];

        if (LanguageManager.Instance.Current == Language.Korean)
        {
            sum.text = $"목표 합계: {targetValue} 이상";
        }
        else
        {
            sum.text = $"Target Sum: {targetValue}+";
        }
    }
}