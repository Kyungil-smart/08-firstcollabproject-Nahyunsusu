using UnityEngine;

public class EquipTooltipData : TooltipComponent
{
    private EquipmentData _data;

    public void Setup(EquipmentData data)
    {
        _data = data;
    }

    public override string GetTooltipText()
    {
        Language currentLang = Language.Korean;
        if (LanguageManager.Instance != null)
        {
            currentLang = LanguageManager.Instance.Current;
        }

        if (_data == null)
        {
            return currentLang == Language.Korean ? "정보 없음" : "No Info";
        }

        if (currentLang == Language.Korean)
        {
            return $"<b>{_data.EquipName}</b> (+{_data.CurrentUpgradeLevel})\n" +
                   $"공격력: {_data.EquipAttackDamage}\n" +
                   $"체력: {_data.EquipHP}";
        }
        else
        {
            return $"<b>{_data.EquipNameEN}</b> (+{_data.CurrentUpgradeLevel})\n" +
                   $"ATK: {_data.EquipAttackDamage}\n" +
                   $"HP: {_data.EquipHP}";
        }
    }
}