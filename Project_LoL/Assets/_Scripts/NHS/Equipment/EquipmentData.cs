using UnityEngine;

[System.Serializable]
public class EquipmentData
{
    public Sprite EquipImage;
    public int    EquipID;
    public string EquipName;
    public int    EquipHP;
    public int    EquipAttackDamage;
    public float  EquipAttackSpeed;
    public int    EquipMoveSpeed;
    public int    EquipCritChance;
    public int    EquipCritDamage;
    public string EquipSpecial;

    public int CurrentUpgradeLevel = 0;

    public EquipmentData(EquipmentData_SO so)
    {
        this.EquipImage        = so.EquipImage;
        this.EquipID           = so.EquipID;
        this.EquipName         = so.EquipName_KO;
        this.EquipHP           = so.EquipHP;
        this.EquipAttackDamage = so.EquipAttackDamage;
        this.EquipAttackSpeed  = so.EquipAttackSpeed;
        this.EquipMoveSpeed    = so.EquipMoveSpeed;
        this.EquipCritChance   = so.EquipCritChance;
        this.EquipCritDamage   = so.EquipCritDamage;
        this.EquipSpecial      = so.EquipSpecial;
    }
}