using UnityEngine;
using static EquipmentData_SO;

[System.Serializable]
public class EquipmentData
{
    public DiceEquipIconSet EquipIconSet;
    public int              EquipID;
    public string           EquipName;
    public int              EquipHP;
    public int              EquipAttackDamage;
    public float            EquipAttackSpeed;
    public int              EquipMoveSpeed;
    public int              EquipCritChance;
    public int              EquipCritDamage;
    public string           EquipSpecial;
    public string           EquipNameEN;
    public int              EquipUpgrade;

    public int CurrentUpgradeLevel = 0;

    public EquipmentData(EquipmentData_SO so)
    {
        this.EquipIconSet        = so.EquipImages;
        this.EquipID             = so.EquipID;
        this.EquipName           = so.EquipName_KO;
        this.EquipNameEN         = so.EquipName_EN;
        this.EquipHP             = so.EquipHP;
        this.EquipAttackDamage   = so.EquipAttackDamage;
        this.EquipAttackSpeed    = so.EquipAttackSpeed;
        this.EquipMoveSpeed      = so.EquipMoveSpeed;
        this.EquipCritChance     = so.EquipCritChance;
        this.EquipCritDamage     = so.EquipCritDamage;
        this.EquipSpecial        = so.EquipSpecial;
        this.CurrentUpgradeLevel = 0;
        this.EquipUpgrade        = so.EquipUpgrade;
    }
}