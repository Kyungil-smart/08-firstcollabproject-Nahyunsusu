using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ReplaceSelectorUI : MonoBehaviour
{
    public static ReplaceSelectorUI instance;
    public Action<int> onSlotSelected;

    [SerializeField] private EquipInspector _inspector;

    [SerializeField] private List<Image> _popupSlotImages = new List<Image>(4);
    [SerializeField] private PlayerSkillHandler _skillHandler;

    private void Awake()
    {
        instance = this;
        this.gameObject.SetActive(false);
    }

    public void Open(bool isSkill = false)
    {
        gameObject.SetActive(true);

        if (_inspector != null)
        {
            _inspector.RefreshUI(isSkill);
        }

        UpdatePopupIcons(isSkill);
    }

    private void UpdatePopupIcons(bool isSkill)
    {
        foreach (var img in _popupSlotImages) img.enabled = false;

        if (isSkill)
        {
            for (int i = 0; i < _skillHandler.Skills.Length; i++)
            {
                if (i >= _popupSlotImages.Count) break;
                var executor = _skillHandler.Skills[i];
                if (executor == null || executor.SkillDataSO == null) continue;

                _popupSlotImages[i].sprite = executor.SkillDataSO.Get(1).SkillImage;
                _popupSlotImages[i].enabled = true;
            }
        }
        else
        {
            var equipList = GameDataManager.instance.equipItemList;
            for (int i = 0; i < equipList.MyEquips.Count; i++)
            {
                if (i >= _popupSlotImages.Count) break;
                var data = equipList.MyEquips[i];
                _popupSlotImages[i].sprite = data.EquipIconSet.Get(data.CurrentUpgradeLevel);
                _popupSlotImages[i].enabled = true;
            }
        }
    }

    public void ClickSlot(int index)
    {
        onSlotSelected?.Invoke(index);
        gameObject.SetActive(false);
    }
}