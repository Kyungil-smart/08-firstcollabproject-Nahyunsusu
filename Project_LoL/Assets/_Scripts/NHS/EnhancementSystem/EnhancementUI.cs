using _Scripts.LYC.Utils;
using DG.Tweening;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

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
    private List<Image>    _diceImage;
    private List<Animator> _animator;

    private static readonly int _stop  = Animator.StringToHash("Stop");
    private static readonly int _start = Animator.StringToHash("Start");

    private void Awake()
    {
        //this.gameObject.SetActive(false);
    }

    private void Start()
    {
        _enhanceButton.interactable = false;

        for (int i = 0; i < _equipmentButtons.Count; i++)
        {
            int index = i; 
            _equipmentButtons[i].onClick.AddListener(() => OnSelectEquipment(index));
        }

         _oddButton.onClick.AddListener(() => OnSelectOddEven(1));
        _evenButton.onClick.AddListener(() => OnSelectOddEven(0));

        _enhanceButton.onClick.AddListener(ExecuteEnhancement);

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

        RefreshEquipIcons();
        ResetSelection();   
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
        if (index >= _equipmentList.MyEquips.Count) return;

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
    }

    public void UpdateDatas(Sprite skillIcon, Sprite dice, int ammo)
    {
        _diceImage[0].sprite = dice;

        _diceImage[0].transform.DOComplete();
        _diceImage[0].transform.DOPunchScale(Vector3.one * 0.2f, 0.4f);
    }

    public void StartRolling()
    {
        _animator[0].SetTrigger(Start);
    }

    public void StopRolling()
    {
        _animator.SetTrigger(Stop);
    }
}