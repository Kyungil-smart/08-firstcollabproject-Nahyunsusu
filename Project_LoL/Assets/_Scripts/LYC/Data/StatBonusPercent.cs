/// <summary>
/// 스탯별 누적 퍼센트 보너스를 보관하는 클래스.
/// 레벨업 선택지 및 장비의 수치 합산값이 저장되며, UI 표시에 활용합니다.
/// 예) MoveSpeed = 30 → 기본 이동속도의 +30% 보너스.
/// </summary>
[System.Serializable]
public class StatBonusPercent
{
    public float HP;
    public float AtkDamage;
    public float AtkSpeed;
    public float MoveSpeed;
    public float CritRate;
    public float CritDamage;

    public StatBonusPercent Clone() => new StatBonusPercent
    {
        HP         = HP,
        AtkDamage  = AtkDamage,
        AtkSpeed   = AtkSpeed,
        MoveSpeed  = MoveSpeed,
        CritRate   = CritRate,
        CritDamage = CritDamage,
    };
}