using UnityEngine;

[CreateAssetMenu(fileName = "EquipData", menuName = "ScriptableObjects/EquipData")]
public class EquipmentData_SO : ScriptableObject
{
    //SkillID	SkillEffect	SkillIcon	SkillText	SkillDesc	SkillDamage	SkillAttackSize	SkillObjectSpeed	SkillRange	SkillDelay	SkillUseCount	SkillCoolTime	SkillFunction	SkillLS	SkillPrice
    [field: SerializeField] public int    EquipID           { get; private set; }
    [field: SerializeField] public string EquipName         { get; private set; }
    [field: SerializeField] public string EquipText         { get; private set; }
    [field: SerializeField] public int    EquipUpgrade      { get; private set; }
    [field: SerializeField] public int    EquipPrice        { get; private set; }
    [field: SerializeField] public int    EquipHP           { get; private set; }
    [field: SerializeField] public int    EquipAttackDamage { get; private set; }
    [field: SerializeField] public int    EquipAttackSpeed  { get; private set; }
    [field: SerializeField] public int    EquipMoveSpeed    { get; private set; }
    [field: SerializeField] public int    EquipCritChance   { get; private set; }
    [field: SerializeField] public int    EquipCritDamage   { get; private set; }
    [field: SerializeField] public string EquipSpecial      { get; private set; }

    public void SetData(string[] datas)
    {
        if (datas.Length < 12) return;

        EquipID           = int.Parse(datas[0].Trim());
        EquipName         = datas[1].Trim();
        EquipText         = datas[2].Trim();
        EquipUpgrade      = int.Parse(datas[3].Trim());
        EquipPrice        = int.Parse(datas[4].Trim());
        EquipHP           = int.Parse(datas[5].Trim());
        EquipAttackDamage = int.Parse(datas[6].Trim());
        EquipAttackSpeed  = int.Parse(datas[7].Trim());
        EquipMoveSpeed    = int.Parse(datas[8].Trim());
        EquipCritChance   = int.Parse(datas[9].Trim());
        EquipCritDamage   = int.Parse(datas[10].Trim());
    
        EquipSpecial      = datas[11].Trim();
    }

}
