using UnityEngine;

[CreateAssetMenu(fileName = "EquipData", menuName = "ScriptableObjects/EquipData")]
public class EquipmentData_SO : ScriptableObject
{
    [field: SerializeField] public UnityEngine.Sprite EquipImage;

    //SkillID	SkillEffect	SkillIcon	SkillText	SkillDesc	SkillDamage	SkillAttackSize	SkillObjectSpeed	SkillRange	SkillDelay	SkillUseCount	SkillCoolTime	SkillFunction	SkillLS	SkillPrice
    [field: SerializeField] public int    EquipID           { get; private set; }
    [field: SerializeField] public string EquipName         { get; private set; }
    [field: SerializeField] public string EquipText         { get; private set; }
    [field: SerializeField] public int    EquipUpgrade      { get; private set; }
    [field: SerializeField] public int    EquipChance       { get; private set; }
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
        if (datas.Length < 15)
        {
            Debug.LogWarning($"데이터 길이가 부족합니다. (현재: {datas.Length}개)");
            return;
        }

        // 0: EquipId (int)
        if (int.TryParse(datas[0].Trim(), out int id)) EquipID = id;

        // 1, 2: Name & Text (string)
        EquipName = datas[1].Trim();
        EquipText = datas[2].Trim();

        // 3: EquipUpgrade (int)
        if (int.TryParse(datas[3].Trim(), out int upgrade)) EquipUpgrade = upgrade;

        // 4: EquipChance (int)
        if (int.TryParse(datas[4].Trim(), out int chance)) EquipChance = chance;

        // 5: EquipPrice (int) ⭐ 상점 가격 핵심!
        if (int.TryParse(datas[5].Trim(), out int price)) EquipPrice = price;

        // 6: EquipHp (int)
        if (int.TryParse(datas[6].Trim(), out int hp)) EquipHP = hp;

        // 7: EquipAttackDamage (int)
        if (int.TryParse(datas[7].Trim(), out int atkDmg)) EquipAttackDamage = atkDmg;

        // 8: EquipAttackSpeed (float) ⭐ float.TryParse 필수!
        if (float.TryParse(datas[8].Trim(), out float atkSpd)) EquipAttackSpeed = atkSpd;

        // 9: EquipMoveSpeed (int)
        if (int.TryParse(datas[9].Trim(), out int movSpd)) EquipMoveSpeed = movSpd;

        // 10: EquipCritChance (int)
        if (int.TryParse(datas[10].Trim(), out int critCh)) EquipCritChance = critCh;

        // 11: EquipCritDamage (int)
        if (int.TryParse(datas[11].Trim(), out int critDmg)) EquipCritDamage = critDmg;

        // 12: EquipSpecial (string)
        EquipSpecial = datas[12].Trim();

        // 13, 14: KO & EN Names (string)
        EquipName_KO = datas[13].Trim();
        EquipName_EN = datas[14].Trim();

        //Debug.Log($"[데이터 로드 완료] {EquipID}: {EquipName_KO} / 가격: {EquipPrice}");
    }

}
