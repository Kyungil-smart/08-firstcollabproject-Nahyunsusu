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
        if (_data == null)
        {
            return LanguageManager.Instance.Current == Language.Korean ? "정보 없음" : "No Info";
        }

        if (LanguageManager.Instance.Current == Language.Korean)
        {
            return $"<b>{_data.EquipName}</b> (+{_data.CurrentUpgradeLevel})\n" +
                   $"공격력: {_data.EquipAttackDamage}\n" +
                   $"체력: {_data.EquipHP}";
        }
        else
        {
            // 영어 툴팁 텍스트
            return $"<b>{_data.EquipNameEN}</b> (+{_data.CurrentUpgradeLevel})\n" +
                   $"ATK: {_data.EquipAttackDamage}\n" +
                   $"HP: {_data.EquipHP}";
        }

    }
}