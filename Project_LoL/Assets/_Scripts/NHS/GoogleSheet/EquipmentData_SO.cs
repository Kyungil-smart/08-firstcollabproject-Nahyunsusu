using UnityEngine;

[CreateAssetMenu(fileName = "EquipData", menuName = "ScriptableObjects/EquipData")]
public class EquipmentData_SO : ScriptableObject
{
    [System.Serializable]
    public class DiceEquipIconSet
    {
        [Header("Dice 0")]
        public Sprite icon0;
        [Header("Dice 1")]
        public Sprite icon1;

        [Header("Dice 2")]
        public Sprite icon2;

        [Header("Dice 3")]
        public Sprite icon3;

        [Header("Dice 4")]
        public Sprite icon4;

        [Header("Dice 5")]
        public Sprite icon5;

        [Header("Dice 6")]
        public Sprite icon6;

        public Sprite Get(int dice = 0) => dice switch
        {
            0 => icon0,
            1 => icon1,
            2 => icon2,
            3 => icon3,
            4 => icon4,
            5 => icon5,
            6 => icon6,
            _ => icon1
        };
    }

    [field: SerializeField] public DiceEquipIconSet EquipImages { get; private set; }
    [field: SerializeField] public int    EquipID           { get; private set; }
    [field: SerializeField] public string EquipText         { get; private set; }
    [field: SerializeField] public int    EquipUpgrade      { get; private set; }
    [field: SerializeField] public int    EquipChance1      { get; private set; }
    [field: SerializeField] public int    EquipChance2      { get; private set; }
    [field: SerializeField] public int    EquipChance3      { get; private set; }
    [field: SerializeField] public int    EquipPrice        { get; private set; }
    [field: SerializeField] public int    EquipHP           { get; private set; }
    [field: SerializeField] public int    EquipAttackDamage { get; private set; }
    [field: SerializeField] public float  EquipAttackSpeed  { get; private set; }
    [field: SerializeField] public int    EquipMoveSpeed    { get; private set; }
    [field: SerializeField] public int    EquipCritChance   { get; private set; }
    [field: SerializeField] public int    EquipCritDamage   { get; private set; }
    [field: SerializeField] public string EquipSpecial      { get; private set; }
    [field: SerializeField] public string EquipName_KO      { get; private set; }
    [field: SerializeField] public string EquipName_EN      { get; private set; }

    public void SetData(string[] datas)
    {
        if (datas.Length < 16)
        {
            Debug.LogWarning($"데이터 길이가 부족합니다. (현재: {datas.Length}개)");
            return;
        }

        if (int.TryParse(datas[0].Trim(), out int id)) EquipID = id;
        EquipText = datas[1].Trim();
        if (int.  TryParse(datas[ 2].Trim(), out int   upgrade))      EquipUpgrade = upgrade;
        if (int.  TryParse(datas[ 3].Trim(), out int    chance1))     EquipChance1 = chance1;
        if (int.  TryParse(datas[ 4].Trim(), out int    chance2))     EquipChance2 = chance2;
        if (int.  TryParse(datas[ 5].Trim(), out int    chance3))     EquipChance3 = chance3;
        if (int.  TryParse(datas[ 6].Trim(), out int     price))        EquipPrice = price;
        if (int.  TryParse(datas[ 7].Trim(), out int        hp))           EquipHP = hp;
        if (int.  TryParse(datas[ 8].Trim(), out int    atkDmg)) EquipAttackDamage = atkDmg;
        if (float.TryParse(datas[ 9].Trim(), out float  atkSpd))  EquipAttackSpeed = atkSpd;
        if (int.  TryParse(datas[10].Trim(), out int    movSpd))    EquipMoveSpeed = movSpd;
        if (int.  TryParse(datas[11].Trim(), out int    critCh))   EquipCritChance = critCh;
        if (int.  TryParse(datas[12].Trim(), out int   critDmg))   EquipCritDamage = critDmg;
        EquipSpecial = datas[13].Trim();
        EquipName_KO = datas[14].Trim();
        EquipName_EN = datas[15].Trim();

        //Debug.Log($"데이터 저장 완료");
    }

    public EquipmentData CreateRuntimeData()
    {
        return new EquipmentData(this);
    }
}
