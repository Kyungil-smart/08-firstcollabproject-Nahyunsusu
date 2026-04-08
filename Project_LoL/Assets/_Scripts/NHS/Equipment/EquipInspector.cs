using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System;

public class EquipInspector : MonoBehaviour
{
    [SerializeField] private EquipmentList _equipmentList;
    [SerializeField] private PlayerSkillHandler _skillList;

    [SerializeField] private List<Image> images = new List<Image>(4);

    private bool _currentIsSkillMode = false;

    private void Awake()
    {
        _equipmentList = FindAnyObjectByType<EquipmentList>();
    }

    private void OnEnable()
    {
        if (_skillList == null)
        {
            GameObject playerObj = GameObject.FindWithTag("Player");

            if (playerObj != null)
            {
                if (_skillList == null)
                    _skillList = playerObj.GetComponent<PlayerSkillHandler>();
            }
            else
            {
                Debug.LogError("StoreUI: 'Player' 태그를 가진 오브젝트를 찾을 수 없습니다!");
            }
        }

        if (GameDataManager.instance == null) return;

        if (_equipmentList != null)
        {
            _equipmentList.OnEquipChanged += HandleEquipChanged;
        }
        RefreshUI();
    }

    public void OnDisable()
    {
        if (_equipmentList != null)
        {
            _equipmentList.OnEquipChanged -= HandleEquipChanged;
        }
    }

    private void HandleEquipChanged(bool isSkill)
    {
        RefreshUI(isSkill);
    }

    public void RefreshUI(bool isSkillMode = false)
    {
        _currentIsSkillMode = isSkillMode;

        for (int i = 0; i < images.Count; i++)
        {
            images[i].enabled = false;
            if (images[i].TryGetComponent<EquipTooltipData>(out var tooltip))
            {
                tooltip.Setup(null);
            }
        }

        if (isSkillMode == false)
        {
            if (_equipmentList == null) return;

            for (int i = 0; i < _equipmentList.CurrentCount; i++)
            {
                if (i >= images.Count) break;
                var data = _equipmentList.MyEquips[i];
                if (data == null) continue;

                images[i].sprite = data.EquipIconSet.Get(data.CurrentUpgradeLevel);
                images[i].enabled = true;

                if (images[i].TryGetComponent<EquipTooltipData>(out var tooltip))
                {
                    tooltip.Setup(data);
                }
            }
        }
        else
        {
            Debug.Log("스킬을 그립니다");
            if (_skillList == null) return;

            // 2. 스킬 그리기
            for (int i = 0; i < _skillList.Skills.Length; i++)
            {
                if (i >= images.Count) break;

                var skillExecutor = _skillList.Skills[i];
                if (skillExecutor == null || skillExecutor.SkillDataSO == null)
                {
                    continue; 
                }

                SkillDataSO currentSO = skillExecutor.SkillDataSO;

                images[i].sprite = currentSO.Get(1).SkillImage;
                images[i].enabled = true;
            }
        }
    }
}
