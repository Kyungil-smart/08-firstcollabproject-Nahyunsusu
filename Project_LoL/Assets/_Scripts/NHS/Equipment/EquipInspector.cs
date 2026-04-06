using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System;

public class EquipInspector : MonoBehaviour
{
    [SerializeField] private EquipmentList _equipmentList;

    [SerializeField] private List<Image> images = new List<Image>(4);

    public void OnEnable()
    {
        if (GameDataManager.instance == null) return;

        if (_equipmentList != null)
        {
            _equipmentList.OnEquipChanged += RefreshUI;
        }
        RefreshUI();
    }

    public void OnDisable()
    {
        if (_equipmentList != null)
        {
            _equipmentList.OnEquipChanged -= RefreshUI;
        }
    }

    public void RefreshUI()
    {
        // 1. 초기화
        for (int i = 0; i < images.Count; i++)
        {
            images[i].enabled = false;

            var trigger = images[i].GetComponent<TooltipTrigger>();
            if (trigger != null) trigger.enabled = false;
        }

        // 2. 장비 그리기
        for (int i = 0; i < _equipmentList.CurrentCount; i++)
        {
            if (i >= images.Count) break;

            var data = _equipmentList.MyEquips[i];

            images[i].sprite = data.EquipIconSet.Get(data.CurrentUpgradeLevel);
            images[i].enabled = true;

            var tooltipData = images[i].GetComponent<EquipTooltipData>();
            if (tooltipData == null) 
                tooltipData = images[i].gameObject.AddComponent<EquipTooltipData>();

            tooltipData.Setup(data);

            var trigger = images[i].GetComponent<TooltipTrigger>();
            if (trigger == null) 
                trigger = images[i].gameObject.AddComponent<TooltipTrigger>();

            trigger.enabled = true;
        }
    }
}
