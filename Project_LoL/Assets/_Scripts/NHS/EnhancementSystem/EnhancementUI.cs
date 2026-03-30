using NUnit.Framework;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class EnhancementUI : MonoBehaviour
{
    [SerializeField] private List<Button> _equipmentButtons = new List<Button>();

    [SerializeField] private Button     _oddButton;
    [SerializeField] private Button    _evenButton;
    [SerializeField] private Button _enhanceButton;

    [Header("Current Selection")]
    private Button _selectedEquipment = null;
    private int    _selectedOddEven = -1;

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
    }

    private void OnSelectEquipment(int index)
    {
        _selectedEquipment = _equipmentButtons[index];

        Debug.Log($"{index}번 무기 선택됨");

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
        if (_selectedEquipment != null && _selectedOddEven != -1)
        {
            _enhanceButton.interactable = true;
        }
    }

    private void ExecuteEnhancement()
    {
        Debug.Log("강화 시작");

        _selectedEquipment = null;
    }
}
