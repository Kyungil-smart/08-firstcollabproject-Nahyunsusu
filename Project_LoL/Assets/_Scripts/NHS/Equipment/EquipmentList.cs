using System;
using System.Collections.Generic;
using UnityEngine;

public class EquipmentList : MonoBehaviour
{
    [SerializeField] private DataManager dataManager;

    public List<EquipmentData> MyEquips => _playersEquip; // UI에 보여주기용
    [SerializeField] List<EquipmentData> _playersEquip = new List<EquipmentData>();

    public Action<bool> OnEquipChanged;

    private void Awake()
    {
        // 이전 씬에서 저장된 장비가 있으면 조용히 복원 (이벤트 미발행)
        // PlayerController.Start() → InitPlayer() → RecalculateStats() 에서 반영됨
        var persistent = PlayerPersistentData.Instance;
        if (persistent != null && persistent.HasData && persistent.SavedEquipments?.Count > 0)
        {
            _playersEquip = new List<EquipmentData>(persistent.SavedEquipments);
        }
    }

    public int CurrentCount => _playersEquip.Count; // 현재 장비 개수 확인용

    public void AddEquip(int index, int newEqiupId) // 보상이나 상점에서 호출할 함수
    {
        EquipmentData_SO so = dataManager.equipDataList.Find(x => x.EquipID == newEqiupId);
        if (so == null) return;
        Debug.Log("AddEquip실행");

        if(_playersEquip.Count < 4)
        {
            _playersEquip.Add(so.CreateRuntimeData());
            Debug.Log($"{so.EquipName_KO} 장비 추가됨 (현재 개수: {_playersEquip.Count})");
        }
        else
        {
            if (index < 0 || index >= _playersEquip.Count)
            {
                Debug.LogWarning("교체할 슬롯 번호가 잘못되었습니다.");
                return;
            }
            _playersEquip[index] = so.CreateRuntimeData();
            Debug.Log($"{index}번 슬롯 장비를 {so.EquipName_KO}(으)로 교체 완료");
        }

        OnEquipChanged?.Invoke(false);
    }

    public TotalStat CalculateData() // 플레이어가 가져갈 함수
    {
        TotalStat total = new TotalStat();

        for (int i = 0; i < _playersEquip.Count; i++)
        {
            total.HP           += _playersEquip[i].EquipHP;
            total.AttackDamage += _playersEquip[i].EquipAttackDamage;
            total.AttackSpeed  += _playersEquip[i].EquipAttackSpeed;
            total.MoveSpeed    += _playersEquip[i].EquipMoveSpeed;
            total.CritChance   += _playersEquip[i].EquipCritChance;
            total.CritDamage   += _playersEquip[i].EquipCritDamage;
        }

        return total;
    }

    public void UpgradeEquipment(int index) // EnhancementUI에서 호출할 함수
    {
        if (index < _playersEquip.Count)
        {
            switch(_playersEquip[index].EquipUpgrade)
            {
                case 1:
                    break;
                case 2:
                    break;
                case 3:
                    break;
                case 4:
                    break;
                case 5:
                    break;
                case 6:
                    break;
                default:
                    Debug.Log("옳은 인덱스가 아닙니다");
                    break;

            }
                
            _playersEquip[index].EquipAttackDamage += 5;
            _playersEquip[index].CurrentUpgradeLevel += 1;
            Debug.Log($"{_playersEquip[index].EquipName} 강화! 현재 데미지: {_playersEquip[index].EquipAttackDamage}");
        }
    }
}