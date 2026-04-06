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
        if (_data == null) return "정보 없음";

        return $"<b>{_data.EquipName}</b> (+{_data.CurrentUpgradeLevel})\n" +
               $"공격력: {_data.EquipAttackDamage}\n" +
               $"체력: {_data.EquipHP}";
    }
}